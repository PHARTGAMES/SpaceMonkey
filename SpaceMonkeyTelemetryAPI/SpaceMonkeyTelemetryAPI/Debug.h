#include <cstdarg>
#include <stdio.h>
#include <wtypes.h>
#include <mutex>
#include "SpaceMonkeyTelemetryAPI.h"
#include <vector>

#pragma once

class SMT_API Debug
{
public:

    // Buffer for storing log lines before flushing
    static std::vector<std::string> s_logBuffer;

    // Decide how many messages to keep in memory before writing to file
    static int s_bufferCapacity;

    // Keep track of how many times we've opened the file to decide whether 
    // to use "w" (once) or "a+" (append) mode.
    static int s_debugCTR;

    static HANDLE s_ipc_mutex;

    static void Log(const char* szFormat, ...);
    static void FlushBufferToFile();
    static void ShutdownLogs();



};
