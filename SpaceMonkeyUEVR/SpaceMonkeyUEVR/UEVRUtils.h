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

inline std::string wstring_to_string(const std::wstring& wstr)
{
    if (wstr.empty()) return std::string();

    // Determine the number of bytes needed.
    int size_needed = WideCharToMultiByte(CP_UTF8, 0, wstr.c_str(),
        static_cast<int>(wstr.size()),
        nullptr, 0, nullptr, nullptr);
    if (size_needed == 0)
    {
        // Handle error: call GetLastError() if needed.
        return std::string();
    }

    std::string str(size_needed, 0);
    WideCharToMultiByte(CP_UTF8, 0, wstr.c_str(),
        static_cast<int>(wstr.size()),
        &str[0], size_needed, nullptr, nullptr);
    return str;
}