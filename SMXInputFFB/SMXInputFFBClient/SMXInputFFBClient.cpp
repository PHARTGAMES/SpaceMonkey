#include "pch.h"
#include "SMXInputFFBClient.h"
#include "SpaceMonkeyTelemetryAPI.h"


// Starts the background thread (idempotent). Returns true if a new thread was started.
bool SMXInputFFBClient::StartRecieving(RecieveCallback callback, void* cbCtx)
{
    m_recieveCallback = callback;
    m_recieveCallbackCtx = cbCtx;

    std::lock_guard<std::mutex> lock(m_stateMutex);
    if (m_running.load(std::memory_order_acquire))
        return false; // already running

    m_installDir = SpaceMonkeyTelemetryAPI::GetInstallDir();

    m_frameData.Init(m_installDir + std::string("\\PacketFormats\\ffbPacketFormat.xml"));
    m_packetSize = m_frameData.GetSize();
    m_packet = (uint8_t*)malloc(m_packetSize);

    if (m_sharedMem == NULL)
    {
        m_sharedMem = new SharedMemory(SPACEMONKEY_FFB_TELEMETRY_FILENAME, SPACEMONKEY_FFB_TELEMETRY_MUTEX, SharedMemType::SharedMem_Read, (void*)m_packet, m_packetSize);
    }


    m_running.store(true, std::memory_order_release);
    m_thread = std::thread(&SMXInputFFBClient::ProcessFFBTelemetry, this);
    return true;
}

// Signals the thread to exit and joins it (idempotent).
void SMXInputFFBClient::StopRecieving()
{
    std::thread localThread;
    {
        std::lock_guard<std::mutex> lock(m_stateMutex);
        if (!m_running.load(std::memory_order_acquire))
            return; // not running

        m_running.store(false, std::memory_order_release);
        // Move out to join without holding the lock (avoid deadlocks)
        localThread = std::move(m_thread);
    }

    if (localThread.joinable())
        localThread.join();
}

void SMXInputFFBClient::ProcessFFBTelemetry()
{
    // Loop until StopRecieving requests exit
    while (m_running.load(std::memory_order_acquire))
    {
        if (m_sharedMem == nullptr)
            return;

        m_sharedMem->Read();

        m_frameData.FromBytes(m_packet, m_packetSize);

        if (m_recieveCallback)
        {
            m_recieveCallback(&m_frameData, m_recieveCallbackCtx);
        }

        //fixme:
        std::this_thread::sleep_for(std::chrono::milliseconds((long)((1.0f / 60.0f) * 1000.0f)));
    }

}