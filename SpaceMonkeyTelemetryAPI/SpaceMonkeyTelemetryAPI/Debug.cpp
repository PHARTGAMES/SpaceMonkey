
#include <thread>
#include <chrono>
#include "Debug.h"

#if defined( _WINDOWS )
#include <windows.h>
#endif
//#define NO_LOGS

HANDLE Debug::s_ipc_mutex = nullptr;

int Debug::s_debugCTR = 0;
int Debug::s_bufferCapacity = 2000;
std::vector<std::string> Debug::s_logBuffer;



// Internal function that actually writes the buffer to disk
void Debug::FlushBufferToFile()
{
    if (Debug::s_logBuffer.empty())
        return;

    FILE* file = nullptr;
    fopen_s(&file, "SMTAPILOG.txt", Debug::s_debugCTR++ == 0 ? "w" : "a+");
    if (file != nullptr)
    {
        // Write each buffered message
        for (const auto& msg : Debug::s_logBuffer)
        {
            // fprintf is safe for writing single lines
            // If you want to ensure each line ends with \n, 
            // you can do so in the message or here.
            fprintf(file, "%s", msg.c_str());
        }
        fprintf(file, "Write Log End\n");
        fclose(file);
    }
    Debug::s_logBuffer.clear();
}

void Debug::Log(const char* szFormat, ...)
{
#ifndef NO_LOGS
    // Create the mutex if it doesn't already exist.
    if (s_ipc_mutex == nullptr)
        s_ipc_mutex = CreateMutexA(NULL, FALSE, "SMTAPILOGMTX");

    // Build the formatted string
    char szBuff[1024];
    va_list arg;
    va_start(arg, szFormat);
    _vsnprintf_s(szBuff, sizeof(szBuff), szFormat, arg);
    va_end(arg);

    // Send to the Visual Studio Output window
    OutputDebugStringA(szBuff);

    // Lock the mutex before accessing the shared buffer
    WaitForSingleObject(s_ipc_mutex, INFINITE);

    // Add this message to our memory buffer
    s_logBuffer.emplace_back(szBuff);

    // If we reached the capacity, flush to file
    if (s_logBuffer.size() >= s_bufferCapacity)
    {
        Debug::FlushBufferToFile();
    }

    // Release the mutex
    ReleaseMutex(s_ipc_mutex);
#endif
}

// Optionally, if you want to manually flush remaining logs at shutdown:
void Debug::ShutdownLogs()
{
#ifndef NO_LOGS
    if (s_ipc_mutex)
    {
        WaitForSingleObject(s_ipc_mutex, INFINITE);
        Debug::FlushBufferToFile();
        ReleaseMutex(s_ipc_mutex);

        CloseHandle(s_ipc_mutex);
        Debug::s_ipc_mutex = nullptr;
    }
#endif
}
