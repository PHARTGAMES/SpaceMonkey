#include "XInputFFBConfigUI.h"
#include "XInputFFBHost.h"
#include "resource.h"
#include <commctrl.h>

#pragma comment(lib, "Comctl32.lib")

static XInputFFBConfigUI* g_instance = nullptr;
HMODULE XInputFFBConfigUI::s_dllHModule = nullptr;

// ---------- DLL entrypoints ----------
static XInputFFBConfigUI g_ui;

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
        m_hDialog = hDlg;
        InitializeTabs(hDlg);
        return TRUE;

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

// --------------------------------------

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

    PopulateXInputAxisMappingUI();

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

}

void XInputFFBConfigUI::ChangeXInputAxis(int newIndex)
{
    m_selectedXInputAxis = newIndex;
    ChangeXInputMapping(0);

}

void XInputFFBConfigUI::ChangeDInputAxis(int newIndex)
{

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
    }

}

void XInputFFBConfigUI::PopulateXInputMappingNames()
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

    if (names.size() > 0)
    {
        SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, names[m_selectedXInputMapping]);
    }
    else
    {
        SetStringToEdit(m_hAxisTab, IDC_AT_MAPPING_NAME_EDIT, "");
    }

}


void XInputFFBConfigUI::PopulateXInputButtonMappingUI()
{


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

// -----------------------------------------------------------
// Get string from edit control
// -----------------------------------------------------------
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

// -----------------------------------------------------------
// Set string to edit control
// -----------------------------------------------------------
void XInputFFBConfigUI::SetStringToEdit(HWND hDlg, int editControlID, const std::string& value)
{
    HWND hEdit = GetDlgItem(hDlg, editControlID);
    if (!hEdit)
        return;

    SetWindowTextA(hEdit, value.c_str());
}