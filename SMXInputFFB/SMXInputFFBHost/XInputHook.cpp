#include "XInputHook.h"
#include <windows.h>
#include <Xinput.h>
#include <mutex>
#include <cstring>

#include "MinHook.h"


static const wchar_t* s_xInputDlls[] = {
    L"xinput1_4.dll",
    L"xinput1_3.dll",
    L"xinput9_1_0.dll",
    L"xinput1_2.dll",
    L"xinput1_1.dll"
};

XInputHook::PFN_XInputGetState XInputHook::s_OriginalXInputGetState = nullptr;
XInputHook* XInputHook::s_Instance = nullptr;

XInputHook::XInputHook()
    : m_xInputModule(nullptr)
{
    std::memset(m_gamePads, 0, sizeof(m_gamePads));
}

XInputHook::~XInputHook()
{
    Deinit();
}

void XInputHook::SetState(DWORD userIndex, const XINPUT_STATE& state, SetStateMode mode, bool enableOverride)
{
    if (userIndex >= XUSER_MAX_COUNT) return;
    std::lock_guard<std::mutex> lock(m_mutex);
    m_gamePads[userIndex].enabled = enableOverride;
    m_gamePads[userIndex].mode = mode;
    m_gamePads[userIndex].state = state;
}

static inline SHORT pick_greatest_axis(SHORT a, SHORT b)
{
    // Choose the value with the greatest magnitude; tie goes to 'a'
    return (std::abs((int)a) >= std::abs((int)b)) ? a : b;
}

static inline BYTE pick_greatest_trigger(BYTE a, BYTE b)
{
    return (a >= b) ? a : b;
}

DWORD WINAPI XInputHook::Hooked_XInputGetState(DWORD dwUserIndex, XINPUT_STATE* pState)
{
    // Call the real XInputGetState first
    DWORD hr = ERROR_DEVICE_NOT_CONNECTED;
    if (s_OriginalXInputGetState)
        hr = s_OriginalXInputGetState(dwUserIndex, pState);

    // If device not connected or invalid params, just return
    if (hr != ERROR_SUCCESS || pState == nullptr)
        return hr;

    XInputHook* self = s_Instance;
    if (!self) return hr;

    // Pull any configured override for this user
    XINPUT_STATE inject;
    bool enabled = false;
    SetStateMode mode = Greatest;

    {
        std::lock_guard<std::mutex> lock(self->m_mutex);
        if (dwUserIndex < XUSER_MAX_COUNT && self->m_gamePads[dwUserIndex].enabled)
        {
            inject = self->m_gamePads[dwUserIndex].state;
            mode = self->m_gamePads[dwUserIndex].mode;
            enabled = true;
        }
    }

    if (!enabled)
        return hr;

    // Apply according to mode
    if (mode == Override)
    {
        // Keep hardware dwPacketNumber semantics simple: bump it if we changed anything
        DWORD oldPacket = pState->dwPacketNumber;
        *pState = inject;
        pState->dwPacketNumber = oldPacket + 1;
    }
    else // Greatest
    {
        // Buttons: bitwise OR is a reasonable "greatest" aggregation
        WORD hwButtons = pState->Gamepad.wButtons;
        WORD inButtons = inject.Gamepad.wButtons;
        pState->Gamepad.wButtons = (WORD)(hwButtons | inButtons);

        // Triggers: higher value wins
        pState->Gamepad.bLeftTrigger = pick_greatest_trigger(pState->Gamepad.bLeftTrigger, inject.Gamepad.bLeftTrigger);
        pState->Gamepad.bRightTrigger = pick_greatest_trigger(pState->Gamepad.bRightTrigger, inject.Gamepad.bRightTrigger);

        // Thumbsticks: choose the greater magnitude per axis
        pState->Gamepad.sThumbLX = pick_greatest_axis(pState->Gamepad.sThumbLX, inject.Gamepad.sThumbLX);
        pState->Gamepad.sThumbLY = pick_greatest_axis(pState->Gamepad.sThumbLY, inject.Gamepad.sThumbLY);
        pState->Gamepad.sThumbRX = pick_greatest_axis(pState->Gamepad.sThumbRX, inject.Gamepad.sThumbRX);
        pState->Gamepad.sThumbRY = pick_greatest_axis(pState->Gamepad.sThumbRY, inject.Gamepad.sThumbRY);

        // Packet bump to signal a change
        pState->dwPacketNumber += 1;
    }

    return hr;
}


FARPROC XInputHook::ResolveXInputGetState()
{
    // Try common XInput dll names in order of modern to legacy
    const char* dlls[] = {
        "xinput1_4.dll",   // Win8+
        "xinput1_3.dll",   // DX SDK June 2010
        "xinput9_1_0.dll", // Vista/Win7 compat
        "xinput1_2.dll",
        "xinput1_1.dll"
    };

    for (size_t i = 0; i < sizeof(dlls) / sizeof(dlls[0]); ++i)
    {
        HMODULE mod = GetModuleHandleA(dlls[i]);
//        if (!mod) mod = LoadLibraryA(dlls[i]);
        if (!mod) continue;

        FARPROC proc = GetProcAddress(mod, "XInputGetState");
        if (proc)
            return proc;

        // If that dll did not have the export, keep trying others
    }
    return nullptr;
}

bool XInputHook::Init()
{
    if (s_Instance) return true; // already initialized for this process
    s_Instance = this;


    // Try each known XInput DLL, but only if it's already loaded.
    for (size_t i = 0; i < sizeof(s_xInputDlls) / sizeof(s_xInputDlls[0]); ++i)
    {
        HMODULE mod = GetModuleHandleW(s_xInputDlls[i]);
        if (!mod) continue; // not loaded -> skip

        // Create the hook against this specific module export.
        LPVOID target = nullptr;
        const MH_STATUS st = MH_CreateHookApiEx(
            s_xInputDlls[i],
            "XInputGetState",
            reinterpret_cast<LPVOID>(&XInputHook::Hooked_XInputGetState),
            reinterpret_cast<LPVOID*>(&s_OriginalXInputGetState),
            &target);

        if (st == MH_OK && target != nullptr)
        {
            if (MH_EnableHook(target) == MH_OK)
                return true;

            // Cleanup if enabling failed.
            MH_RemoveHook(target);
            s_OriginalXInputGetState = nullptr;
        }
    }


    //FARPROC target = ResolveXInputGetState();
    //if (!target)
    //{
    //    s_Instance = nullptr;
    //    return false;
    //}

    //// Create hook
    //if (MH_CreateHook((LPVOID)target, (LPVOID)&XInputHook::Hooked_XInputGetState,
    //    reinterpret_cast<LPVOID*>(&s_OriginalXInputGetState)) != MH_OK)
    //{
    //    s_Instance = nullptr;
    //    return false;
    //}

    //if (MH_EnableHook((LPVOID)target) != MH_OK)
    //{
    //    MH_RemoveHook((LPVOID)target);
    //    s_Instance = nullptr;
    //    return false;
    //}

    return true;
}

void XInputHook::Deinit()
{
    if (!s_Instance) return;

    FARPROC target = nullptr;

    // Try to resolve again to remove (safe even if already disabled)
    target = ResolveXInputGetState();
    if (target)
    {
        MH_DisableHook((LPVOID)target);
        MH_RemoveHook((LPVOID)target);
    }

    s_OriginalXInputGetState = nullptr;
    s_Instance = nullptr;
}
