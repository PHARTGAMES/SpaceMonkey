#pragma once

#include <windows.h>
#include <Xinput.h>
#include <mutex>
#include <cstring>

#include "MinHook.h"

class XInputHook
{
public:
    enum SetStateMode
    {
        Greatest, // use the greatest magnitude values between our state and the hardware state
        Override  // replace the hardware state entirely with our state
    };

    XInputHook();
    ~XInputHook();

    // Create the hook on XInputGetState and enable it.
    // Returns true on success.
    bool Init();

    // Disable and remove the hook.
    void Deinit();

    // Set or clear the state to apply for a given user.
    // Pass mode for how to apply, and set enableOverride to false to clear.
    void SetState(DWORD userIndex, const XINPUT_STATE& state, SetStateMode mode, bool enableOverride);

private:
    // Original function pointer type for XInputGetState
    using PFN_XInputGetState = DWORD(WINAPI*)(DWORD dwUserIndex, XINPUT_STATE* pState);

    // Static trampoline storage and hook function
    static PFN_XInputGetState s_OriginalXInputGetState;
    static DWORD WINAPI Hooked_XInputGetState(DWORD dwUserIndex, XINPUT_STATE* pState);

    // Helper to find an available XInput dll and resolve the export
    static FARPROC ResolveXInputGetState();

    // Singleton-style access so the static hook can reach instance data
    static XInputHook* s_Instance;

    // State management
    struct GamePad
    {
        bool          enabled;
        SetStateMode  mode;
        XINPUT_STATE  state;
    };

    GamePad     m_gamePads[XUSER_MAX_COUNT];
    std::mutex  m_mutex;
    HMODULE     m_xInputModule;


};

