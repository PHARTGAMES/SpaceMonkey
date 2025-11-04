// FocusWinEventMonitor.cpp
#include "FocusWinEventMonitor.h"

FocusWinEventMonitor* FocusWinEventMonitor::s_instance = nullptr;

bool FocusWinEventMonitor::Start(void *context, const std::function<void(void*)>& onFocused)
{
    if (m_hook) return true;            // already started
    if (s_instance) return false;           // only one active instance supported

    m_callback = onFocused;
    s_instance = this;
    m_context = context;

    // Initialize cached state to current foreground ownership
    HWND fg = GetForegroundWindow();
    DWORD fgPid = 0;
    if (fg) GetWindowThreadProcessId(fg, &fgPid);
    InterlockedExchange(&m_isFocused, (fg && fgPid == GetCurrentProcessId()) ? 1 : 0);

    m_hook = SetWinEventHook(
        EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND,
        /* hmod */ NULL,
        /* pfn  */ &FocusWinEventMonitor::WinEventThunk,
        /* idProcess */ 0, /* idThread */ 0,
        /* flags */ WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNTHREAD
    );
    if (!m_hook) {
        DWORD err = GetLastError();
        // log err; common ones:
        // 87 (ERROR_INVALID_PARAMETER) -> using INCONTEXT without hmod/export
        // 5  (ERROR_ACCESS_DENIED)     -> UIPI/integrity issues when trying to go in-context
        s_instance = nullptr;
        m_callback = nullptr;
        InterlockedExchange(&m_isFocused, 0);
        return false;
    }
    return true;
}

void FocusWinEventMonitor::Stop()
{
    if (m_hook)
    {
        UnhookWinEvent(m_hook);
        m_hook = NULL;
    }
    m_callback = nullptr;
    s_instance = nullptr;
    InterlockedExchange(&m_isFocused, 0);
}

void CALLBACK FocusWinEventMonitor::WinEventThunk(HWINEVENTHOOK, DWORD event, HWND hwnd,
    LONG, LONG, DWORD, DWORD)
{
    if (event != EVENT_SYSTEM_FOREGROUND) return;
    if (!s_instance) return;
    s_instance->HandleForeground(hwnd);
}

void FocusWinEventMonitor::HandleForeground(HWND hwnd)
{
    HWND fg = GetForegroundWindow();
    if (!fg) 
        return;


    DWORD pid = 0;
    GetWindowThreadProcessId(fg, &pid);
    DWORD currentProcId = GetCurrentProcessId();
    bool nowFocused = (pid == currentProcId);

    // Atomically update state and detect transition 0 -> 1
    LONG prev = InterlockedExchange(&m_isFocused, nowFocused ? 1 : 0);
    if (!prev && nowFocused)
    {
        if (m_callback) m_callback(m_context); // fire only on unfocused->focused
    }
}
