#pragma once
#include "uevr/API.h"
#include <string>
#include <Windows.h>

#define PLUGIN_LOG_ONCE(...) {\
    static bool _logged_ = false; \
    if (!_logged_) { \
        _logged_ = true; \
        API::get()->log_info(__VA_ARGS__); \
    }}


inline std::wstring string_to_wstring(const std::string& str) 
{
    if (str.empty()) return std::wstring();

    // Determine the number of wide characters needed.
    int size_needed = MultiByteToWideChar(CP_UTF8, 0, str.c_str(), static_cast<int>(str.size()), nullptr, 0);
    if (size_needed == 0) 
    {
        // Handle error: call GetLastError() if needed.
        return std::wstring();
    }

    std::wstring wstr(size_needed, 0);
    MultiByteToWideChar(CP_UTF8, 0, str.c_str(), static_cast<int>(str.size()), &wstr[0], size_needed);
    return wstr;
}

