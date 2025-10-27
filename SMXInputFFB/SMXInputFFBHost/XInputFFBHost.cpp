#include "XInputFFBHost.h"
#include <dinput.h>
#include "XInputHook.h"
#include <tchar.h>
#include "XInputFFBConfig.h"
#include "XInputFFBDevice.h"
#include "SMXInputFFBClient.h"

void FFBClientRecieve(CMCustomUDPData *frameData, void* ctx)
{
    XInputFFBHost* host = static_cast<XInputFFBHost*>(ctx);

    host->ProcessFFBTelemetry(frameData);

}


void XInputFFBHost::ProcessFFBTelemetry(CMCustomUDPData* frameData)
{
    m_ffbFrameData.Copy(*frameData);

    for (size_t i = 0; i < m_xInputFFBDevices.size(); ++i)
    {
        XInputFFBDevice* device = m_xInputFFBDevices[i];

        device->SetAxisForce(XInputFFBEffectType::Steering, (long)m_ffbFrameData.ffb_wheel_steer_constant);
    }
}


XInputFFBHost::XInputFFBHost() 
{
    MH_Initialize();
    m_xInputHook = new XInputHook();
    m_xInputHook->Init();

    m_ffbClient = new SMXInputFFBClient();
    m_ffbClient->StartRecieving(&FFBClientRecieve, this);

}

XInputFFBHost::~XInputFFBHost()
{
    MH_Uninitialize();

    if (m_config != nullptr)
    {
        delete m_config;
        m_config = nullptr;
    }

    if (m_xInputHook != nullptr)
    {
        m_xInputHook->Deinit();
        delete m_xInputHook;
    }

    for (size_t i = 0; i < m_sourceDevices.size(); ++i)
    {
        delete m_sourceDevices[i];
    }
    m_sourceDevices.clear();

    DestroyXInputFFBDevices();

    if (m_di) { m_di->Release(); m_di = nullptr; }

    if (m_ffbClient != nullptr)
    {
        m_ffbClient->StopRecieving();
        delete m_ffbClient;

    }
}

bool XInputFFBHost::Initialize()
{
    if (m_di) 
        return true;
   
    HINSTANCE hInst = GetModuleHandleA(nullptr);
    HRESULT hr = DirectInput8Create(hInst, DIRECTINPUT_VERSION, IID_IDirectInput8, (void**)&m_di, nullptr);
    return SUCCEEDED(hr);
}

void XInputFFBHost::Deinit()
{

}

void XInputFFBHost::SetHWND(HWND hwnd)
{
    m_hwnd = hwnd;
    // forward HWND to devices (for cooperative level if already constructed)
    for (size_t i = 0; i < m_sourceDevices.size(); ++i)
    {
        m_sourceDevices[i]->SetHWND(hwnd);
    }
}

BOOL CALLBACK XInputFFBHost::EnumCb(const DIDEVICEINSTANCE* inst, VOID* ctx)
{
    XInputFFBHost* self = reinterpret_cast<XInputFFBHost*>(ctx);
    if (!self || !self->m_di)
    {
        return DIENUM_STOP;
    }

    DISourceDevice* dev = new DISourceDevice(self->m_di, *inst);
    if (self->m_hwnd)
    {
        dev->SetHWND(self->m_hwnd);
    }

    if (dev->Initialize())
    {
        self->m_sourceDevices.push_back(dev);
    }
    else
    {
        delete dev;
    }
    return DIENUM_CONTINUE;
}

void XInputFFBHost::EnumerateSourceDevices()
{
    if (!m_di && !Initialize())
    {
        return;
    }

    for (size_t i = 0; i < m_sourceDevices.size(); ++i)
    {
        delete m_sourceDevices[i];
    }
    m_sourceDevices.clear();

    m_di->EnumDevices(DI8DEVCLASS_GAMECTRL, EnumCb, this, DIEDFL_ATTACHEDONLY);
}

DISourceDevice* XInputFFBHost::GetDeviceByGUID(const std::string instanceGuid) const
{
    for (size_t i = 0; i < m_sourceDevices.size(); ++i)
    {
        if (instanceGuid == m_sourceDevices[i]->GetInstanceGUIDString())
            return m_sourceDevices[i];
    }
    return nullptr;
}


void XInputFFBHost::UpdateSourceDeviceState()
{
    for (DISourceDevice* device : m_sourceDevices)
    {
        device->UpdateState();
    }
}

void XInputFFBHost::UpdateXInputFFBDeviceState(uint32_t vehicleTypeMask)
{
    for (size_t i = 0; i < m_xInputFFBDevices.size(); ++i)
    {
        XInputFFBDevice* device = m_xInputFFBDevices[i];
        
        m_xInputHook->SetState(i, device->UpdateState(vehicleTypeMask), XInputHook::SetStateMode::Greatest, true);
    }
}


void XInputFFBHost::Update(float deltaTime, uint32_t vehicleTypeMask)
{
    UpdateSourceDeviceState();
    UpdateXInputFFBDeviceState(vehicleTypeMask);
}

const XINPUT_STATE& XInputFFBHost::GetXInputState(int deviceID)
{
    XInputFFBDevice* device = m_xInputFFBDevices[deviceID];

    return device->GetCachedState();
}




BOOL CALLBACK EnumWindowsCallbackHost(HWND hwnd, LPARAM lParam)
{
    DWORD processId;
    GetWindowThreadProcessId(hwnd, &processId);

    XInputFFBHost* host = reinterpret_cast<XInputFFBHost*>(lParam);

    if (!IsWindow(hwnd))
        return TRUE; //invalid window, boo

    // Check if the window belongs to the server
    if (processId == host->m_hostProcessID)
    {

        // Check if window is not a tool window
        LONG exStyle = GetWindowLongA(hwnd, GWL_EXSTYLE);

        // Check if the window is top-level and visible
//        if (GetWindow(hwnd, GW_OWNER) == NULL && IsWindowVisible(hwnd) && nameLength != 0 && !(exStyle & WS_EX_TOOLWINDOW))
        if (GetWindow(hwnd, GW_OWNER) == NULL && IsWindowVisible(hwnd) && !(exStyle & WS_EX_TOOLWINDOW))
        {
            // Retrieve the class name of the window
            TCHAR className[256];
            if (GetClassName(hwnd, className, _countof(className)))
            {

                // List of IME-related classes to skip
                static const TCHAR* IMEClasses[] = {
                    _T("MSCTFIME UI"),
                    _T("DIEmWin"),
                    _T("IME")
                };

                bool isIMEWindow = false;
                for (const auto& imeClass : IMEClasses)
                {
                    if (_tcsicmp(className, imeClass) == 0)
                    {
                        isIMEWindow = true;
                        break;
                    }
                }

                // If not an IME window, select it as the topmost window
                if (!isIMEWindow)
                {
                    host->SetHostHWND(hwnd);

                    return FALSE; // Stop enumeration
                }
            }
        }
    }

    return TRUE; // Continue enumeration
}

void XInputFFBHost::ResolveHostWindow()
{
    m_hostWindowHandle = NULL;

    m_hostProcessID = GetCurrentProcessId();

    EnumWindows(EnumWindowsCallbackHost, reinterpret_cast<LPARAM>(this));
}

void XInputFFBHost::SetHostHWND(HWND hwnd)
{
    m_hostWindowHandle = hwnd;
}

void XInputFFBHost::SetInputFocus(InputFocus inputFocus)
{
    switch (inputFocus)
    {
    case InputFocus::Host:
    {
        if (m_hostWindowHandle != 0)
        {
            SetHWND(m_hostWindowHandle);
        }
        break;
    }
    case InputFocus::ConfigUI:
    {
        break;
    }
    }
}

void XInputFFBHost::LoadConfig()
{
    if (m_config != nullptr)
    {
        delete m_config;
        m_config = nullptr;
    }

    m_config = new XInputFFBConfig();
    m_config->Load();
}

void XInputFFBHost::SaveConfig()
{
    if (m_config != nullptr)
    {
        m_config->Save();
    }
}

XInputFFBConfig* XInputFFBHost::GetConfig()
{
    return m_config;
}


void XInputFFBHost::CreateXInputFFBDevices()
{
    DestroyXInputFFBDevices();

    for (size_t i = 0; i < XUSER_MAX_COUNT; ++i)
    {
        XInputFFBDevice* newDevice = new XInputFFBDevice();
        newDevice->SetHost(this);
        newDevice->SetConfig(m_config);
        m_xInputFFBDevices.push_back(newDevice);
    }
}

void XInputFFBHost::DestroyXInputFFBDevices()
{
    for (size_t i = 0; i < m_xInputFFBDevices.size(); ++i)
    {
        delete m_xInputFFBDevices[i];
    }
    m_xInputFFBDevices.clear();

}

const std::vector<std::string>& XInputFFBHost::GetDIDeviceIdentifiers()
{
    m_diDeviceIdentifiers.clear();

    for (DISourceDevice *diDevice : m_sourceDevices)
    {
        m_diDeviceIdentifiers.push_back(std::string(diDevice->GetName()));
    }
    
    return m_diDeviceIdentifiers;
}


const std::vector<std::string>& XInputFFBHost::GetDIDeviceGUIDs()
{
    m_diDeviceGUIDs.clear();

    for (DISourceDevice *diDevice : m_sourceDevices)
    {
        m_diDeviceGUIDs.push_back(std::string(diDevice->GetInstanceGUIDString()));
    }
    
    return m_diDeviceGUIDs;
}



float XInputFFBHost::GetXInputAxisValueForEffectType(const XInputFFBEffectType& effectType)
{
    if (!m_config)
        return 0.0f;

    const uint32_t effectMask = static_cast<uint32_t>(effectType);
    float maxMagnitude = 0.0f;
    float returnValue = 0.0f;

    // Iterate through all virtual XInput devices
    for (size_t deviceIndex = 0; deviceIndex < m_xInputFFBDevices.size(); ++deviceIndex)
    {
        XInputFFBDevice* device = m_xInputFFBDevices[deviceIndex];
        if (!device)
            continue;

        XInputFFBDeviceConfig& deviceConfig = m_config->GetDeviceConfig((unsigned)deviceIndex);

        // For each XInput axis bucket (LX, LY, RX, RY, LT, RT)
        for (int axis = 0; axis < XInputFFBDeviceConfig::XInputAxisCount; ++axis)
        {
            auto axisMappings = deviceConfig.GetAxisBucket(axis);
            if (!axisMappings)
                continue;

            for (const AxisMapping& mapping : *axisMappings)
            {
                // Skip if this mapping doesn't include the effect type
                if ((mapping.ffbEffectMask & effectMask) == 0)
                    continue;

                // Retrieve cached axis value
                float val = device->GetAxisValue((XInputAxis)axis);

                float absVal = std::fabsf(val);

                // Track the maximum magnitude
                if (absVal > maxMagnitude)
                {
                    maxMagnitude = absVal;
                    returnValue = val;
                }
            }
        }
    }

    return returnValue;
}
