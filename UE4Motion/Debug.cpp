#include "Debug.h"

static std::mutex s_logFileMutex;
static int debugCTR = 0;
#define NO_LOGS

void Debug::Log(const char* szFormat, ...)
{
#ifndef NO_LOGS

    char szBuff[1024];
    va_list arg;
    va_start(arg, szFormat);
    _vsnprintf_s(szBuff, sizeof(szBuff), szFormat, arg);
    va_end(arg);

    OutputDebugStringA(szBuff);

    s_logFileMutex.lock();
    FILE* file;
    fopen_s(&file, "WWLOG.txt", debugCTR++ == 0 ? "w" : "a+");
    if (file != NULL)
    {
        fprintf(file, szBuff);
        fclose(file);
    }
    s_logFileMutex.unlock();
#endif
}