#pragma once
#include "SMXInputFFBClientDefines.h"
#include <thread>
#include <atomic>
#include <mutex>
#include <chrono>
#include <string>
#include "CMCustomUDPData.h"
#include "SharedMemory.h"

#define SPACEMONKEY_FFB_TELEMETRY_FILENAME "SMT_FFB_FRAME"
#define SPACEMONKEY_FFB_TELEMETRY_MUTEX "SMT_FFB_FRAME_MUTEX"


class SMXINPUTFFBCLIENT_API SMXInputFFBClient
{
public:

    using RecieveCallback = void (*)(CMCustomUDPData* frameData, void *ctx);

    SMXInputFFBClient() = default;
    ~SMXInputFFBClient()
    {
        StopRecieving();
    }

    // Non-copyable, non-movable to avoid accidental double-joins
    SMXInputFFBClient(const SMXInputFFBClient&) = delete;
    SMXInputFFBClient& operator=(const SMXInputFFBClient&) = delete;
    SMXInputFFBClient(SMXInputFFBClient&&) = delete;
    SMXInputFFBClient& operator=(SMXInputFFBClient&&) = delete;

    // Starts the background thread (idempotent). Returns true if a new thread was started.
    bool StartRecieving(RecieveCallback callback, void *cbCtx);

    // Signals the thread to exit and joins it (idempotent).
    void StopRecieving();

private:
    // Runs on the background thread
    void ProcessFFBTelemetry();

private:
    std::thread m_thread;
    std::atomic<bool> m_running{ false };
    std::mutex m_stateMutex;

    std::string m_installDir;
    CMCustomUDPData m_frameData;
    float m_lastTime = 0.0f;
    SharedMemory* m_sharedMem = nullptr;
    uint8_t* m_packet = nullptr;
    int m_packetSize = 0;
    RecieveCallback m_recieveCallback = nullptr;
    void* m_recieveCallbackCtx = nullptr;

};
