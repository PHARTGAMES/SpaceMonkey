#pragma once
#include <Windows.h>
#include "SMXInputFFBHostDefines.h"
#include "DISourceDevice.h"
#include <vector>
#include <guiddef.h>
#include "CMCustomUDPData.h"
#include "XInput.h"
#include "XInputFFBConfig.h"

class XInputHook;
class XInputFFBDevice;
class SMXInputFFBClient;



#pragma warning(push)
#pragma warning(disable:4251)
class SMXINPUTFFBHOST_API XInputFFBHost
{
public:

    enum InputFocus
    {
        Host,
        ConfigUI
    };

    XInputFFBHost();
    ~XInputFFBHost();

    // Initialize DirectInput and enumerate devices
    bool Initialize();
    void Deinit();
    void EnumerateSourceDevices();

    // Optional: needed for exclusive FFB cooperative level
    void SetHWND(HWND hwnd);

    void SetHostHWND(HWND hwnd);

    // Lookup by Instance GUID
    DISourceDevice* GetDeviceByGUID(const std::string instanceGuid) const;

    // Poll/refresh all cached states
    void UpdateSourceDeviceState();
    void UpdateXInputFFBDeviceState(uint32_t vehicleTypeMask);

    void Update(float deltaTime, uint32_t vehicleTypeMask);

    void ResolveHostWindow();

    void SetInputFocus(InputFocus inputFocus);

    HWND m_hostWindowHandle = nullptr;
    unsigned int m_hostProcessID = 0;

    void LoadConfig();
    void SaveConfig();

    void CreateXInputFFBDevices();
    void DestroyXInputFFBDevices();

    void ProcessFFBTelemetry(CMCustomUDPData *frameData);

    const XINPUT_STATE& GetXInputState(int deviceID);

    const std::vector<std::string>& GetDIDeviceIdentifiers();
    const std::vector<std::string>& GetDIDeviceGUIDs();

    XInputFFBConfig* GetConfig();

    float GetXInputAxisValueForEffectType(const XInputFFBEffectType& effectType);


private:
    static BOOL CALLBACK EnumCb(const DIDEVICEINSTANCE* inst, VOID* ctx);

    IDirectInput8* m_di = nullptr;
    HWND           m_hwnd = nullptr;
    std::vector<DISourceDevice*> m_sourceDevices;
    XInputHook* m_xInputHook = nullptr;
    XInputFFBConfig *m_config = nullptr;

    std::vector<XInputFFBDevice*> m_xInputFFBDevices;

    SMXInputFFBClient* m_ffbClient;
    CMCustomUDPData m_ffbFrameData;

    std::vector<std::string> m_diDeviceIdentifiers;
    std::vector<std::string> m_diDeviceGUIDs;




};

#pragma warning(pop)