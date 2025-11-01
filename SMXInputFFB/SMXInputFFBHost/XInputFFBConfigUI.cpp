#include "XInputFFBConfigUI.h"
#include "XInputFFBHost.h"
#include "resource.h"
#include <commctrl.h>
#include <dbt.h>  // for WM_DEVICECHANGE, DBT_DEVICEARRIVAL, etc.
#include <initguid.h>
#include <hidclass.h>

#pragma comment(lib, "Comctl32.lib")

static XInputFFBConfigUI* g_instance = nullptr;
HMODULE XInputFFBConfigUI::s_dllHModule = nullptr;

static XInputFFBConfigUI g_ui;

#define AXIS_UPDATE_TIMER_ID  1
#define AXIS_UPDATE_INTERVAL  16  // ~60Hz

SMXINPUTFFBHOST_API bool StartInputConfigUI(UIChangeCallback callback, XInputFFBHost* host)
{
    return g_ui.Start(callback, host);
}

SMXINPUTFFBHOST_API void StopInputConfigUI()
{
    g_ui.Stop();
}

XInputFFBConfigUI::XInputFFBConfigUI()
    : m_callback(nullptr), m_hInstance(nullptr),
    m_hDialog(nullptr), m_hThread(nullptr), m_running(false)
{

}

XInputFFBConfigUI::~XInputFFBConfigUI()
{
    Stop();
}

bool XInputFFBConfigUI::Start(UIChangeCallback callback, XInputFFBHost* host)
{
    if (m_running)
        return false;

    m_callback = callback;
    m_host = host;
    m_hInstance = s_dllHModule;
    m_running = true;

    m_hThread = CreateThread(nullptr, 0, ThreadProc, this, 0, nullptr);
    return m_hThread != nullptr;
}

void XInputFFBConfigUI::Stop()
{
    if (!m_running) return;
    m_running = false;

    if (m_hDialog)
        PostMessage(m_hDialog, WM_CLOSE, 0, 0);

    if (m_hThread)
    {
        WaitForSingleObject(m_hThread, INFINITE);
        CloseHandle(m_hThread);
        m_hThread = nullptr;
    }
    m_hDialog = nullptr;
}

DWORD WINAPI XInputFFBConfigUI::ThreadProc(LPVOID param)
{
    XInputFFBConfigUI* self = static_cast<XInputFFBConfigUI*>(param);

    // Init common controls (trackbar etc.)
    INITCOMMONCONTROLSEX icc = { sizeof(icc), ICC_BAR_CLASSES | ICC_STANDARD_CLASSES };
    if (!InitCommonControlsEx(&icc)) {
        //DebugPrintFmt("[XInputUI] InitCommonControlsEx failed (GetLastError=%u)\n", GetLastError());
    }
    else {
        //DebugPrintFmt("[XInputUI] InitCommonControlsEx ok\n");
    }

    char path[MAX_PATH];
    GetModuleFileNameA((HMODULE)self->m_hInstance, path, MAX_PATH);

    // Verify the dialog resource is present in our module
    HRSRC hRes = FindResource(self->m_hInstance, MAKEINTRESOURCE(IDD_INPUTCONFIG_DIALOG), RT_DIALOG);
    if (!hRes) {
        //DebugPrintFmt("[XInputUI] FindResource RT_DIALOG IDD_INPUTCONFIG_DIALOG failed. Are resources linked into this module? GetLastError=%u\n", GetLastError());
        // If resource missing, don't call DialogBoxParam: will fail silently or return -1.
    }
    else {
        //DebugPrintFmt("[XInputUI] FindResource succeeded. resource handle=%p\n", hRes);
    }

    DWORD sessionId = 0;
    if (!ProcessIdToSessionId(GetCurrentProcessId(), &sessionId)) 
        sessionId = (DWORD)-1;

//    DebugPrintFmt("[XInputUI] Current PID=%u, SessionId=%u\n", GetCurrentProcessId(), sessionId);




    INT_PTR ret = DialogBoxParam(self->m_hInstance, MAKEINTRESOURCE(IDD_INPUTCONFIG_DIALOG), nullptr, StaticMainDlgProc, (LPARAM)self);

    if (ret == -1) {
        DWORD err = GetLastError();
        char* msg = nullptr;
        FormatMessageA(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
            NULL, err, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPSTR)&msg, 0, NULL);
//        DebugPrintFmt("[XInputUI] DialogBoxParam failed -> ret=%ld, GetLastError=%u, msg=%s\n", (long)ret, err, msg ? msg : "(null)");
        if (msg) LocalFree(msg);
    }
    else {
//        DebugPrintFmt("[XInputUI] DialogBoxParam returned %ld (dialog closed by user)\n", (long)ret);
    }

    return 0;
}


// --------------------------------------
// Static wrapper: main dialog
// --------------------------------------
INT_PTR CALLBACK XInputFFBConfigUI::StaticMainDlgProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    XInputFFBConfigUI* self = reinterpret_cast<XInputFFBConfigUI*>(GetWindowLongPtr(hDlg, GWLP_USERDATA));

    if (msg == WM_INITDIALOG)
    {
        self = reinterpret_cast<XInputFFBConfigUI*>(lParam);
        SetWindowLongPtr(hDlg, GWLP_USERDATA, (LONG_PTR)self);
    }

    return self ? self->MainDlgProc(hDlg, msg, wParam, lParam) : FALSE;
}

// --------------------------------------
// Static wrapper: Axis tab
// --------------------------------------
INT_PTR CALLBACK XInputFFBConfigUI::StaticAxisTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    XInputFFBConfigUI* self = reinterpret_cast<XInputFFBConfigUI*>(GetWindowLongPtr(hDlg, GWLP_USERDATA));

    if (msg == WM_INITDIALOG)
    {
        self = reinterpret_cast<XInputFFBConfigUI*>(lParam);
        SetWindowLongPtr(hDlg, GWLP_USERDATA, (LONG_PTR)self);
    }

    return self ? self->AxisTabProc(hDlg, msg, wParam, lParam) : FALSE;
}

// --------------------------------------
// Static wrapper: Button tab
// --------------------------------------
INT_PTR CALLBACK XInputFFBConfigUI::StaticButtonTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    XInputFFBConfigUI* self = reinterpret_cast<XInputFFBConfigUI*>(GetWindowLongPtr(hDlg, GWLP_USERDATA));

    if (msg == WM_INITDIALOG)
    {
        self = reinterpret_cast<XInputFFBConfigUI*>(lParam);
        SetWindowLongPtr(hDlg, GWLP_USERDATA, (LONG_PTR)self);
    }

    return self ? self->ButtonTabProc(hDlg, msg, wParam, lParam) : FALSE;
}

INT_PTR XInputFFBConfigUI::MainDlgProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_INITDIALOG:
    {
        m_hDialog = hDlg;
        InitializeTabs(hDlg);

        // Register for HID device notifications
        DEV_BROADCAST_DEVICEINTERFACE NotificationFilter = {};
        NotificationFilter.dbcc_size = sizeof(NotificationFilter);
        NotificationFilter.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
        NotificationFilter.dbcc_classguid = GUID_DEVINTERFACE_HID;

        m_hDeviceNotify = RegisterDeviceNotification(
            hDlg, 
            &NotificationFilter,
            DEVICE_NOTIFY_WINDOW_HANDLE);

        return TRUE;
    }
    case WM_DEVICECHANGE:
        if (wParam == DBT_DEVICEARRIVAL || wParam == DBT_DEVICEREMOVECOMPLETE)
        {
            DEV_BROADCAST_HDR* hdr = (DEV_BROADCAST_HDR*)lParam;
            if (hdr && hdr->dbch_devicetype == DBT_DEVTYP_DEVICEINTERFACE)
            {
                DEV_BROADCAST_DEVICEINTERFACE* dev =
                    (DEV_BROADCAST_DEVICEINTERFACE*)hdr;

                if (IsEqualGUID(dev->dbcc_classguid, GUID_DEVINTERFACE_HID))
                {
                    m_host->EnumerateSourceDevices();
                    PopulateDIDeviceIDs();
                }
            }
        }
        return TRUE; // handled

    case WM_DESTROY:
        if (m_hDeviceNotify)
        {
            UnregisterDeviceNotification(m_hDeviceNotify);
            m_hDeviceNotify = nullptr;
        }
        break;

    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        //case IDC_SAVE_BUTTON:
        //    // handle save
        //    return TRUE;
        case IDC_CLOSE_BUTTON:
            EndDialog(hDlg, 0);
            return TRUE;
        }
        break;

    case WM_NOTIFY:
        OnTabNotify((LPNMHDR)lParam);
        break;

    case WM_CLOSE:
        EndDialog(hDlg, 0);
        return TRUE;
    }
    return FALSE;
}

// --------------------------------------

INT_PTR XInputFFBConfigUI::AxisTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_INITDIALOG:
        InitAxisTab();
        return TRUE;

    case WM_DESTROY:
        DeinitAxisTab();
        return TRUE;

    case WM_TIMER:
        if (wParam == AXIS_UPDATE_TIMER_ID)
        {
            HWND hForeground = GetForegroundWindow();
            if (hForeground == m_hDialog)
            {
                uint32_t vehicleTypeMask = 0;

                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        vehicleTypeMask = axisMapping->vehicleTypeMask;
                    }
                }

                m_host->Update(1.0f / 60.0f, vehicleTypeMask);

                float diAxisValue = m_host->GetDInputAxisValue(m_selectedDIDevice, m_selectedDIAxis);
                SetProgressBarValue(hDlg, IDC_AT_DI_PROGRESS, diAxisValue);
                SetStaticTextFromFloat(hDlg, IDC_AT_DI_VALUE_LBL, diAxisValue);

                float xiAxisValue = m_host->GetXInputAxisValue(m_selectedXInputDevice, m_selectedXInputAxis);
                SetProgressBarValue(hDlg, IDC_AT_XI_AXIS_PROGRESS, xiAxisValue);
                SetStaticTextFromFloat(hDlg, IDC_AT_XI_VALUE_LBL, xiAxisValue);
            }
        }
        return TRUE;

    case WM_COMMAND:
    {
        const int ctrlId = LOWORD(wParam);
        const int notif = HIWORD(wParam);

        switch (ctrlId)
        {
        case IDC_AT_XINPUT_DEVICE_COMBO:
            if (notif == CBN_SELCHANGE)
            {
                HWND hCombo = (HWND)lParam;
                int selIndex = (int)SendMessage(hCombo, CB_GETCURSEL, 0, 0);
                if (selIndex != CB_ERR)
                {
                    ChangeXInputDevice(selIndex);
                }
            }
            break;
        case IDC_AT_XINPUT_AXIS_COMBO:
            if (notif == CBN_SELCHANGE)
            {
                HWND hCombo = (HWND)lParam;
                int selIndex = (int)SendMessage(hCombo, CB_GETCURSEL, 0, 0);
                if (selIndex != CB_ERR)
                {
                    ChangeXInputAxis(selIndex);
                }
            }
            break;

        case IDC_AT_XINPUT_MAPPING_COMBO:
            if (notif == CBN_SELCHANGE)
            {
                HWND hCombo = (HWND)lParam;
                int selIndex = (int)SendMessage(hCombo, CB_GETCURSEL, 0, 0);
                if (selIndex != CB_ERR)
                {
                    ChangeXInputMapping(selIndex);
                    SendMessage(hCombo, CB_SETCURSEL, selIndex, 0);

                }
            }
            break;


        case IDC_AT_DINPUT_DEVICE_COMBO:
            if (notif == CBN_SELCHANGE)
            {
                HWND hCombo = (HWND)lParam;
                int selIndex = (int)SendMessage(hCombo, CB_GETCURSEL, 0, 0);
                if (selIndex != CB_ERR)
                {
                    ChangeDInputDevice(selIndex);
                }
            }
            break;

        case IDC_AT_DINPUT_AXIS_COMBO:
            if (notif == CBN_SELCHANGE)
            {
                HWND hCombo = (HWND)lParam;
                int selIndex = (int)SendMessage(hCombo, CB_GETCURSEL, 0, 0);
                if (selIndex != CB_ERR)
                {
                    ChangeDInputAxis(selIndex);
                }
            }
            break;


        case IDC_AT_MAPPING_NAME_EDIT:
            if (notif == EN_CHANGE)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        std::string mappingName = GetStringFromEdit(hDlg, IDC_AT_MAPPING_NAME_EDIT);

                        axisMapping->mappingName = mappingName;

                        PopulateXInputMappingNames(false);

                        config->Save();
                    }
                }
            }
            break;

        case IDC_AT_CURVE_EDIT:
            if (notif == EN_CHANGE)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        axisMapping->curve = GetFloatFromEdit(hDlg, IDC_AT_CURVE_EDIT, axisMapping->curve);

                        config->Save();

                    }
                }
            }
            break;

        case IDC_AT_DEADZONE_EDIT:
            if (notif == EN_CHANGE)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        axisMapping->deadzone = GetFloatFromEdit(hDlg, IDC_AT_DEADZONE_EDIT, axisMapping->deadzone);

                        config->Save();

                    }
                }
            }
            break;

        case IDC_AT_SCALE_EDIT:
            if (notif == EN_CHANGE)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {

                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        axisMapping->scale = GetFloatFromEdit(hDlg, IDC_AT_SCALE_EDIT, axisMapping->scale);

                        config->Save();

                    }
                }
            }
            break;

            

        case IDC_AT_INVERT_CHECK:
            if (notif == BN_CLICKED)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        bool checked = GetCheckBox(m_hAxisTab, IDC_AT_INVERT_CHECK);

                        axisMapping->invert = checked;

                        config->Save();
                    }
                }

            }
            break;

        case IDC_AT_PEDAL_CHECK:
            if (notif == BN_CLICKED)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    if (axisMapping != nullptr)
                    {
                        bool checked = GetCheckBox(m_hAxisTab, IDC_AT_PEDAL_CHECK);

                        axisMapping->pedal = checked;

                        config->Save();
                    }
                }
            }
            break;


        case IDC_AT_EFFECT_STEERING_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Steering, IDC_AT_EFFECT_STEERING_CHECK);
            }
            break;
        case IDC_AT_EFFECT_AILERON_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Aileron, IDC_AT_EFFECT_AILERON_CHECK);
            }
            break;
        case IDC_AT_EFFECT_ELEVATOR_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Elevator, IDC_AT_EFFECT_ELEVATOR_CHECK);
            }
            break;
        case IDC_AT_EFFECT_RUDDER_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Rudder, IDC_AT_EFFECT_RUDDER_CHECK);
            }
            break;
        case IDC_AT_EFFECT_BRAKE_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Brake, IDC_AT_EFFECT_BRAKE_CHECK);
            }
            break;
        case IDC_AT_EFFECT_CLUTCH_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyEffectCheckToCurrentMapping(XInputFFBEffectType::Clutch, IDC_AT_EFFECT_CLUTCH_CHECK);
            }
            break;


        case IDC_AT_VEHICLE_CAR_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Car, IDC_AT_VEHICLE_CAR_CHECK);
            }
            break;
        case IDC_AT_VEHICLE_BIKE_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Bike, IDC_AT_VEHICLE_BIKE_CHECK);
            }
            break;
        case IDC_AT_VEHICLE_AIRCRAFT_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Aircraft, IDC_AT_VEHICLE_AIRCRAFT_CHECK);
            }
            break;
        case IDC_AT_VEHICLE_BOAT_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Boat, IDC_AT_VEHICLE_BOAT_CHECK);
            }
            break;
        case IDC_AT_VEHICLE_PEDESTRIAN_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Pedestrian, IDC_AT_VEHICLE_PEDESTRIAN_CHECK);
            }
            break;
        case IDC_AT_VEHICLE_HELICOPTER_CHECK:
            if (notif == BN_CLICKED)
            {
                ApplyVehicleCheckToCurrentMapping(XInputFFBVehicleType::Helicopter, IDC_AT_VEHICLE_HELICOPTER_CHECK);
            }
            break;



        case IDC_AT_ADD_MAPPING_BTN:
            if (notif == BN_CLICKED)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    int selectedMappingIndex = m_selectedXInputMapping;
                    m_selectedXInputMapping = config->AddAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis);

                    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);
                    AxisMapping* lastAxisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, selectedMappingIndex);

                    if (axisMapping != nullptr)
                    {
                        axisMapping->mappingName = std::string("New Mapping");
                        if (lastAxisMapping)
                        {
                            axisMapping->deviceId = lastAxisMapping->deviceId;
                            axisMapping->diAxis = 0;
                        }

                        PopulateXInputMappingNames();

                        ChangeXInputMapping(m_selectedXInputMapping);
                    }
                }
            }
            break;

        case IDC_AT_DELETE_MAPPING_BTN:
            if (notif == BN_CLICKED)
            {
                XInputFFBConfig* config = m_host->GetConfig();

                if (config != nullptr)
                {
                    config->DeleteAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

                    m_selectedXInputMapping = 0;

                    PopulateXInputMappingNames();

                    ChangeXInputMapping(0);
                }
            }
            break;

            // other controls in Axis tab:
        case IDC_SAVE_BUTTON:
            // ...
            break;
        }

        return TRUE;
    }
    }

    return FALSE;
}


void XInputFFBConfigUI::ApplyEffectCheckToCurrentMapping(const XInputFFBEffectType &effectType, int checkboxId)
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config != nullptr)
    {
        AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

        if (axisMapping != nullptr)
        {
            bool checked = GetCheckBox(m_hAxisTab, checkboxId);

            if (checked)
            {
                axisMapping->ffbEffectMask |= (uint32_t)effectType;
            }
            else
            {
                axisMapping->ffbEffectMask &= ~(uint32_t)effectType;
            }

            config->Save();
        }
    }

}


void XInputFFBConfigUI::ApplyVehicleCheckToCurrentMapping(const XInputFFBVehicleType& vehicleType, int checkboxId)
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config != nullptr)
    {
        AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

        if (axisMapping != nullptr)
        {
            bool checked = GetCheckBox(m_hAxisTab, checkboxId);

            if (checked)
            {
                axisMapping->vehicleTypeMask |= (uint32_t)vehicleType;
            }
            else
            {
                axisMapping->vehicleTypeMask &= ~(uint32_t)vehicleType;
            }

            config->Save();
        }
    }

}



INT_PTR XInputFFBConfigUI::ButtonTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch (msg)
    {
    case WM_INITDIALOG:
        InitButtonTab();
        return TRUE;

    case WM_COMMAND:
        // handle button mapping clicks
        break;
    }
    return FALSE;
}

void XInputFFBConfigUI::NotifyChange(const std::string& id)
{
    if (m_callback)
        m_callback(id.c_str());
}

void XInputFFBConfigUI::InitializeTabs(HWND hDlg)
{
    m_hTab = GetDlgItem(hDlg, IDC_TAB_MAIN);

    TCITEM tie{};
    tie.mask = TCIF_TEXT;
    tie.pszText = (LPSTR)"Axis Mappings";
    TabCtrl_InsertItem(m_hTab, 0, &tie);

    tie.pszText = (LPSTR)"Button Mappings";
    TabCtrl_InsertItem(m_hTab, 1, &tie);

    // Create the child dialogs (each designed in RC)
    m_hAxisTab = CreateDialogParam(m_hInstance,
        MAKEINTRESOURCE(IDD_AXIS_TAB), hDlg, StaticAxisTabProc, (LPARAM)this);

    m_hButtonTab = CreateDialogParam(m_hInstance,
        MAKEINTRESOURCE(IDD_BUTTON_TAB), hDlg, StaticButtonTabProc, (LPARAM)this);

    // Position them inside tab client area
    RECT rc; GetClientRect(m_hTab, &rc);
    TabCtrl_AdjustRect(m_hTab, FALSE, &rc);
    MapWindowPoints(m_hTab, hDlg, (LPPOINT)&rc, 2);

    SetWindowPos(m_hAxisTab, nullptr, rc.left, rc.top,
        rc.right - rc.left, rc.bottom - rc.top, SWP_SHOWWINDOW);
    SetWindowPos(m_hButtonTab, nullptr, rc.left, rc.top,
        rc.right - rc.left, rc.bottom - rc.top, SWP_HIDEWINDOW);

    InitAxisTab();
    InitButtonTab();
}

void XInputFFBConfigUI::InitAxisTab()
{
    std::vector<std::string> xInputDevices = { "0", "1", "2", "3"};
    PopulateComboBoxFromVector(m_hAxisTab, IDC_AT_XINPUT_DEVICE_COMBO, xInputDevices);
    PopulateComboBoxXInputAxes(m_hAxisTab, IDC_AT_XINPUT_AXIS_COMBO);

    PopulateComboBoxDirectInputAxes(m_hAxisTab, IDC_AT_DINPUT_AXIS_COMBO);

    PopulateDIDeviceIDs();

    PopulateXInputMappingNames();

    ChangeXInputMapping(0);

    SetTimer(m_hAxisTab, AXIS_UPDATE_TIMER_ID, AXIS_UPDATE_INTERVAL, nullptr);

}

void XInputFFBConfigUI::DeinitAxisTab()
{
    KillTimer(m_hAxisTab, AXIS_UPDATE_TIMER_ID);
}

void XInputFFBConfigUI::InitButtonTab()
{


}

void XInputFFBConfigUI::PopulateDIDeviceIDs()
{
    const std::vector<std::string>& deviceIDs = m_host->GetDIDeviceIdentifiers();
    PopulateComboBoxFromVector(m_hAxisTab, IDC_AT_DINPUT_DEVICE_COMBO, deviceIDs);
}

void XInputFFBConfigUI::OnTabNotify(LPNMHDR hdr)
{
    if (hdr->idFrom != IDC_TAB_MAIN || hdr->code != TCN_SELCHANGE)
        return;

    int sel = TabCtrl_GetCurSel(m_hTab);
    ShowWindow(m_hAxisTab, sel == 0 ? SW_SHOW : SW_HIDE);
    ShowWindow(m_hButtonTab, sel == 1 ? SW_SHOW : SW_HIDE);
}

// --------------------------------------
// Helper to add one item to a combo box
// --------------------------------------
void XInputFFBConfigUI::AddComboItem(HWND hCombo, const char* text, int value)
{
    int idx = static_cast<int>(SendMessageA(hCombo, CB_ADDSTRING, 0, (LPARAM)text));
    if (value >= 0)
        SendMessageA(hCombo, CB_SETITEMDATA, idx, (LPARAM)value);
}

// --------------------------------------
//  XInput Axes
// --------------------------------------
void XInputFFBConfigUI::PopulateComboBoxXInputAxes(HWND hDlg, int comboID)
{
    HWND hCombo = GetDlgItem(hDlg, comboID);
    if (!hCombo) return;

    SendMessage(hCombo, CB_RESETCONTENT, 0, 0);

    const char* axes[] =
    {
        "Left Stick X-Axis",
        "Left Stick Y-Axis",
        "Right Stick X-Axis",
        "Right Stick Y-Axis",
        "Left Trigger",
        "Right Trigger"
    };

    for (int i = 0; i < _countof(axes); ++i)
        AddComboItem(hCombo, axes[i], i);

    SendMessage(hCombo, CB_SETCURSEL, 0, 0);

}

// --------------------------------------
//  XInput Buttons
// --------------------------------------
void XInputFFBConfigUI::PopulateComboBoxXInputButtons(HWND hDlg, int comboID)
{
    HWND hCombo = GetDlgItem(hDlg, comboID);
    if (!hCombo) return;

    SendMessage(hCombo, CB_RESETCONTENT, 0, 0);

    const char* buttons[] =
    {
        "A Button",
        "B Button",
        "X Button",
        "Y Button",
        "Left Shoulder",
        "Right Shoulder",
        "Back",
        "Start",
        "Left Thumb",
        "Right Thumb",
        "D-Pad Up",
        "D-Pad Down",
        "D-Pad Left",
        "D-Pad Right"
    };

    for (int i = 0; i < _countof(buttons); ++i)
        AddComboItem(hCombo, buttons[i], i);

    if (_countof(buttons) > 0)
        SendMessage(hCombo, CB_SETCURSEL, 0, 0);
}

// --------------------------------------
// DirectInput Axes
// --------------------------------------
void XInputFFBConfigUI::PopulateComboBoxDirectInputAxes(HWND hDlg, int comboID)
{
    HWND hCombo = GetDlgItem(hDlg, comboID);
    if (!hCombo) return;

    SendMessage(hCombo, CB_RESETCONTENT, 0, 0);

    const char* axes[] =
    {
        "X Axis",
        "Y Axis",
        "Z Axis",
        "RX Axis",
        "RY Axis",
        "RZ Axis",
        "Slider 1",
        "Slider 2"
    };

    for (int i = 0; i < _countof(axes); ++i)
        AddComboItem(hCombo, axes[i], i);

    if (_countof(axes) > 0)
        SendMessage(hCombo, CB_SETCURSEL, 0, 0);
}

// --------------------------------------
//  DirectInput Buttons
// --------------------------------------
void XInputFFBConfigUI::PopulateComboBoxDirectInputButtons(HWND hDlg, int comboID)
{
    HWND hCombo = GetDlgItem(hDlg, comboID);
    if (!hCombo) return;

    SendMessage(hCombo, CB_RESETCONTENT, 0, 0);

    // Add up to 32 buttons (typical DirectInput limit)
    for (int i = 0; i < 32; ++i)
    {
        char buf[32];
        sprintf_s(buf, "Button %d", i + 1);
        AddComboItem(hCombo, buf, i);
    }

    SendMessage(hCombo, CB_SETCURSEL, 0, 0);
}

// --------------------------------------
//  Populate from std::vector<std::string>
// --------------------------------------
void XInputFFBConfigUI::PopulateComboBoxFromVector(HWND hDlg, int comboID, const std::vector<std::string>& items)
{
    HWND hCombo = GetDlgItem(hDlg, comboID);
    if (!hCombo) return;

    SendMessage(hCombo, CB_RESETCONTENT, 0, 0);

    for (size_t i = 0; i < items.size(); ++i)
        AddComboItem(hCombo, items[i].c_str(), (int)i);

    SendMessage(hCombo, CB_SETCURSEL, 0, 0);
}

void XInputFFBConfigUI::ChangeXInputDevice(int newIndex)
{
    m_selectedXInputDevice = newIndex;
    ChangeXInputMapping(0);
}

void XInputFFBConfigUI::ChangeDInputDevice(int newIndex)
{
    m_selectedDIDevice = newIndex;

    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        const std::vector<std::string>& deviceIDs = m_host->GetDIDeviceGUIDs();

        axisMapping->deviceId = deviceIDs[newIndex];

        config->Save();
    }
}

void XInputFFBConfigUI::ChangeXInputAxis(int newIndex)
{
    m_selectedXInputAxis = newIndex;
    ChangeXInputMapping(0);

}

void XInputFFBConfigUI::ChangeDInputAxis(int newIndex)
{
    m_selectedDIAxis = newIndex;

    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        axisMapping->diAxis = newIndex;

        config->Save();
    }

}

void XInputFFBConfigUI::ChangeXInputButton(int newIndex)
{

}

void XInputFFBConfigUI::ChangeDInputButton(int newIndex)
{

}

void XInputFFBConfigUI::ChangeXInputMapping(int newIndex)
{
    m_selectedXInputMapping = newIndex;


    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        m_selectedDIDevice = m_host->GetDeviceIndexByGUID(axisMapping->deviceId);
        m_selectedDIAxis = axisMapping->diAxis;
    }

    PopulateXInputAxisMappingUI();
}

void XInputFFBConfigUI::PopulateXInputAxisMappingUI()
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, axisMapping->mappingName);

        SetCheckBox(m_hAxisTab, IDC_AT_INVERT_CHECK, axisMapping->invert);
        SetCheckBox(m_hAxisTab, IDC_AT_PEDAL_CHECK, axisMapping->pedal);

        SetFloatToEdit(m_hAxisTab, IDC_AT_CURVE_EDIT, axisMapping->curve);
        SetFloatToEdit(m_hAxisTab, IDC_AT_DEADZONE_EDIT, axisMapping->deadzone);
        SetFloatToEdit(m_hAxisTab, IDC_AT_SCALE_EDIT, axisMapping->scale);

        SetComboBoxSelection(m_hAxisTab, IDC_AT_DINPUT_AXIS_COMBO, axisMapping->diAxis);

    }
    else
    {
        SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, std::string(""));

        SetCheckBox(m_hAxisTab, IDC_AT_INVERT_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_PEDAL_CHECK, false);

        SetFloatToEdit(m_hAxisTab, IDC_AT_CURVE_EDIT, 1.0f);
        SetFloatToEdit(m_hAxisTab, IDC_AT_DEADZONE_EDIT, 0.01f);
        SetFloatToEdit(m_hAxisTab, IDC_AT_SCALE_EDIT, 1.0f);
    }

    PopulateXInputMappingContext();
    PopulateXInputFFBEffects();

}

void XInputFFBConfigUI::PopulateXInputMappingNames(bool updateSelectedNameEdit)
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }


    if (m_selectedXInputDevice == -1 || m_selectedXInputAxis == -1)
    {
        return;
    }

    XInputFFBDeviceConfig& deviceConfig = config->GetDeviceConfig(m_selectedXInputDevice);

    std::vector<std::string> names;

    std::vector<AxisMapping>* axisBucket = deviceConfig.GetAxisBucket(m_selectedXInputAxis);

    if (axisBucket == nullptr)
        return;

    for (const AxisMapping& axisMapping : *axisBucket)
    {
        names.push_back(axisMapping.mappingName);
    }

    PopulateComboBoxFromVector(m_hAxisTab, IDC_AT_XINPUT_MAPPING_COMBO, names);

    if (updateSelectedNameEdit)
    {
        if (names.size() > 0)
        {
            SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, names[m_selectedXInputMapping]);
        }
        else
        {
            SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, "");
        }
    }

}

void XInputFFBConfigUI::PopulateXInputButtonMappingUI()
{


}

void XInputFFBConfigUI::PopulateXInputMappingContext()
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_CAR_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Car);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_BIKE_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Bike);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_AIRCRAFT_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Aircraft);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_BOAT_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Boat);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_PEDESTRIAN_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Pedestrian);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_HELICOPTER_CHECK, axisMapping->vehicleTypeMask & (uint32_t)XInputFFBVehicleType::Helicopter);
    }
    else
    {
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_CAR_CHECK, true);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_BIKE_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_AIRCRAFT_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_BOAT_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_PEDESTRIAN_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_VEHICLE_HELICOPTER_CHECK, false);
    }
}

void XInputFFBConfigUI::PopulateXInputFFBEffects()
{
    XInputFFBConfig* config = m_host->GetConfig();

    if (config == nullptr)
    {
        return;
    }

    AxisMapping* axisMapping = config->GetAxisMapping(m_selectedXInputDevice, m_selectedXInputAxis, m_selectedXInputMapping);

    if (axisMapping != nullptr)
    {
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_STEERING_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Steering);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_AILERON_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Aileron);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_ELEVATOR_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Elevator);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_RUDDER_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Rudder);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_BRAKE_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Brake);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_CLUTCH_CHECK, axisMapping->ffbEffectMask & (uint32_t)XInputFFBEffectType::Clutch);
    }
    else
    {
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_STEERING_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_AILERON_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_ELEVATOR_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_RUDDER_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_BRAKE_CHECK, false);
        SetCheckBox(m_hAxisTab, IDC_AT_EFFECT_CLUTCH_CHECK, false);
    }
}

float XInputFFBConfigUI::GetFloatFromEdit(HWND hDlg, int editControlID, float defaultValue)
{
    HWND hEdit = GetDlgItem(hDlg, editControlID);
    if (!hEdit)
        return defaultValue;

    char buffer[64] = {};
    GetWindowTextA(hEdit, buffer, sizeof(buffer));

    // Trim leading/trailing whitespace
    char* start = buffer;
    while (*start == ' ' || *start == '\t') ++start;
    char* end = start + strlen(start);
    while (end > start && (end[-1] == ' ' || end[-1] == '\t' || end[-1] == '\n' || end[-1] == '\r')) --end;
    *end = '\0';

    if (*start == '\0')
        return defaultValue;

    // Try to parse as float
    char* parseEnd = nullptr;
    float value = strtof(start, &parseEnd);

    // If no valid conversion or extra junk after the number, fallback to default
    if (parseEnd == start || *parseEnd != '\0')
        return defaultValue;

    return value;
}

void XInputFFBConfigUI::SetFloatToEdit(HWND hDlg, int editControlID, float value, int precision)
{
    HWND hEdit = GetDlgItem(hDlg, editControlID);
    if (!hEdit)
        return;

    char buffer[64];
    char format[16];
    sprintf_s(format, "%%.%df", precision);
    sprintf_s(buffer, format, value);

    SetWindowTextA(hEdit, buffer);
}

void XInputFFBConfigUI::SetCheckBox(HWND hDlg, int checkBoxID, bool checked)
{
    HWND hCheck = GetDlgItem(hDlg, checkBoxID);
    if (!hCheck)
        return;

    SendMessage(hCheck, BM_SETCHECK, checked ? BST_CHECKED : BST_UNCHECKED, 0);
}

bool XInputFFBConfigUI::GetCheckBox(HWND hDlg, int checkBoxID)
{
    HWND hCheck = GetDlgItem(hDlg, checkBoxID);
    if (!hCheck)
        return false;

    return (SendMessage(hCheck, BM_GETCHECK, 0, 0) == BST_CHECKED);
}

std::string XInputFFBConfigUI::GetStringFromEdit(HWND hDlg, int editControlID)
{
    HWND hEdit = GetDlgItem(hDlg, editControlID);
    if (!hEdit)
        return {};

    int length = GetWindowTextLengthA(hEdit);
    if (length <= 0)
        return {};

    std::string result;
    result.resize(static_cast<size_t>(length));

    GetWindowTextA(hEdit, &result[0], length + 1);
    return result;
}

void XInputFFBConfigUI::SetStringToEdit(HWND hDlg, int editControlID, const std::string& value)
{
    HWND hEdit = GetDlgItem(hDlg, editControlID);
    if (!hEdit)
        return;

    SetWindowTextA(hEdit, value.c_str());
}


void XInputFFBConfigUI::SetProgressBarValue(HWND hDlg, int controlId, float normalizedValue)
{
    HWND hBar = GetDlgItem(hDlg, controlId);
    if (!hBar) return;

    int pos = static_cast<int>((normalizedValue * 0.5f + 0.5f) * 100.0f);
    SendMessage(hBar, PBM_SETPOS, pos, 0);
}


void XInputFFBConfigUI::SetComboBoxSelection(HWND hDlg, int comboId, int selectionIndex)
{
    HWND hCombo = GetDlgItem(hDlg, comboId);
    if (!hCombo) return;

    SendMessage(hCombo, CB_SETCURSEL, selectionIndex, 0);
}


void XInputFFBConfigUI::SetStaticTextFromFloat(HWND hParent, int controlId, float value, int precision)
{
    // Validate the handle and control
    if (!hParent)
        return;

    HWND hCtrl = GetDlgItem(hParent, controlId);
    if (!hCtrl)
        return;

    // Format the float value to string
    char buffer[64];
    sprintf_s(buffer, "%.*f", precision, value);

    // Set the text to the static control
    SetWindowTextA(hCtrl, buffer);
}