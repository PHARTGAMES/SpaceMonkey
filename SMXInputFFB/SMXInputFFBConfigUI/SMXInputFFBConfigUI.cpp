// SMXInputFFBConfigUI.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Windows.h>
#include "XInputFFBConfigUI.h"
#include "XInputFFBHost.h"

static XInputFFBHost* s_xInputFFBHost = nullptr;

void __stdcall OnUIChange(const char* msg)
{
    OutputDebugStringA(msg);
    OutputDebugStringA("\n");

}

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR, int)
{
    //// Load your UI DLL
    //HMODULE hMod = LoadLibraryA("XInputFFBConfigUI.dll");
    //if (!hMod)
    //{
    //    MessageBoxA(NULL, "Failed to load XInputFFBConfigUI.dll", "Error", MB_ICONERROR);
    //    return 1;
    //}

    //using StartFunc = bool(__stdcall*)(UIChangeCallback);
    //using StopFunc = void(__stdcall*)();

    //StartFunc StartInputConfigUI = (StartFunc)GetProcAddress(hMod, "StartInputConfigUI");
    //StopFunc StopInputConfigUI = (StopFunc)GetProcAddress(hMod, "StopInputConfigUI");

    //if (!StartInputConfigUI || !StopInputConfigUI)
    //{
    //    MessageBoxA(NULL, "Missing exports in UI DLL", "Error", MB_ICONERROR);
    //    return 1;
    //}

    s_xInputFFBHost = new XInputFFBHost();
    s_xInputFFBHost->Initialize();
    s_xInputFFBHost->LoadConfig();
    s_xInputFFBHost->CreateXInputFFBDevices();
    s_xInputFFBHost->ResolveHostWindow();
    s_xInputFFBHost->SetInputFocus(XInputFFBHost::InputFocus::Host);
    s_xInputFFBHost->EnumerateSourceDevices();


    // Start UI
    StartInputConfigUI(OnUIChange, s_xInputFFBHost);

    // Wait until the user closes the dialog
    MSG msg;
    while (GetMessage(&msg, nullptr, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    StopInputConfigUI();
    //FreeLibrary(hMod);
    return 0;
}