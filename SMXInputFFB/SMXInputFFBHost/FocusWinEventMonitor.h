#pragma once
#include <windows.h>
#include <functional>

class FocusWinEventMonitor
{
public:
    // Start listening. The callback is invoked only on unfocused->focused transitions.
    // Returns false if the hook could not be installed.
    bool Start(void *context, const std::function<void(void*)>& onFocused);

    // Stop listening and clear the hook.
    void Stop();

    // Current cached state (best-effort).
    bool IsFocused() const { return InterlockedCompareExchange(const_cast<LONG*>(&m_isFocused), 0, 0) != 0; }

private:
    static void CALLBACK WinEventThunk(HWINEVENTHOOK, DWORD event, HWND hwnd,
        LONG idObject, LONG idChild, DWORD tid, DWORD ms);

    void HandleForeground(HWND hwnd);

    // Only one instance should be active; we keep a static back-pointer for the thunk.
    static FocusWinEventMonitor* s_instance;

    HWINEVENTHOOK m_hook = nullptr;
    std::function<void(void*)> m_callback;
    volatile LONG m_isFocused = 0; // 0 = not focused, 1 = focused
    void* m_context = nullptr;
};
#pragma once
