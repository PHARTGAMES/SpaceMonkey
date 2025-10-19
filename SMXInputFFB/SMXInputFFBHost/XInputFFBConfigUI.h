#pragma once
#include <windows.h>
#include "SMXInputFFBHostDefines.h"
#include <string>
#include <thread>
#include <functional>
#include "XInputFFBConfig.h"

class XInputFFBHost;

// Delegate signature
typedef void(__stdcall* UIChangeCallback)(const char* changeString);

// Manages the lifetime of the dialog thread
class SMXINPUTFFBHOST_API XInputFFBConfigUI
{
public:
    XInputFFBConfigUI();
    ~XInputFFBConfigUI();

    bool Start(UIChangeCallback callback, XInputFFBHost *host);
    void Stop();

    static HMODULE s_dllHModule;

    int m_selectedXInputDevice = 0;
    int m_selectedXInputAxis = 0;
    int m_selectedXInputButton = -1;

    int m_selectedDIDevice = 0;
    int m_selectedDIAxis = -1;
    int m_selectedDIButton = -1;
    int m_selectedXInputMapping = 0;

private:
    static DWORD WINAPI ThreadProc(LPVOID param);

    // Static window proc glue functions
    static INT_PTR CALLBACK StaticMainDlgProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);
    static INT_PTR CALLBACK StaticAxisTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);
    static INT_PTR CALLBACK StaticButtonTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);

    // Instance versions
    INT_PTR MainDlgProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);
    INT_PTR AxisTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);
    INT_PTR ButtonTabProc(HWND hDlg, UINT msg, WPARAM wParam, LPARAM lParam);


    void NotifyChange(const std::string& id);
    void InitializeTabs(HWND hDlg);
    void OnTabNotify(LPNMHDR hdr);
    // --- Combo box population utilities ---
    void PopulateComboBoxXInputAxes(HWND hDlg, int comboID);
    void PopulateComboBoxXInputButtons(HWND hDlg, int comboID);
    void PopulateComboBoxDirectInputAxes(HWND hDlg, int comboID);
    void PopulateComboBoxDirectInputButtons(HWND hDlg, int comboID);
    void PopulateComboBoxFromVector(HWND hDlg, int comboID, const std::vector<std::string>& items);
    void PopulateXInputAxisMappingUI();
    void PopulateXInputButtonMappingUI();
    void PopulateXInputMappingNames();

    void AddComboItem(HWND hCombo, const char* text, int value = -1);

    void PopulateDIDeviceIDs();

    void InitAxisTab();
    void InitButtonTab();

    void ChangeXInputDevice(int newIndex);
    void ChangeDInputDevice(int newIndex);
    void ChangeXInputAxis(int newIndex);
    void ChangeDInputAxis(int newIndex);
    void ChangeXInputButton(int newIndex);
    void ChangeDInputButton(int newIndex);
    void ChangeXInputMapping(int newIndex);

    float GetFloatFromEdit(HWND hDlg, int editControlID, float defaultValue);
    void SetFloatToEdit(HWND hDlg, int editControlID, float value, int precision = 2);

    std::string GetStringFromEdit(HWND hDlg, int editControlID);
    void SetStringToEdit(HWND hDlg, int editControlID, const std::string& value);

    void SetCheckBox(HWND hDlg, int checkBoxID, bool checked);
    bool GetCheckBox(HWND hDlg, int checkBoxID);

    UIChangeCallback m_callback;
    HINSTANCE m_hInstance;
    HWND m_hDialog;
    HANDLE m_hThread;
    bool m_running;


    HWND m_hTab;
    HWND m_hAxisTab;
    HWND m_hButtonTab;
    XInputFFBHost* m_host;
};

// DLL exports
SMXINPUTFFBHOST_API bool StartInputConfigUI(UIChangeCallback callback, XInputFFBHost *host);
SMXINPUTFFBHOST_API void StopInputConfigUI();
