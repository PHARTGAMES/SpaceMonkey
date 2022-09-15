#include <cstdarg>
#include <stdio.h>
#include <wtypes.h>
#include <mutex>

#pragma once

class __declspec(dllexport) Debug
{
public:
    static void Log(const char* szFormat, ...);
};
