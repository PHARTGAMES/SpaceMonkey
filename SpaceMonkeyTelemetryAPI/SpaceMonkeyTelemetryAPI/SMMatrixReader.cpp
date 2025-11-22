#include "SMMatrixReader.h"
#ifdef _WIN32
#include <Windows.h>
#endif

#include "systemtime.h"
#include <cstring>   // std::memcpy, std::memcmp
#include <chrono>

SMMatrixReader::SMMatrixReader(
    unsigned int pollIntervalMs,
    unsigned int stableDurationMs)
    : m_running(false)
    , m_sourceAddress(nullptr)
    , m_callback(nullptr)
    , m_pollInterval(pollIntervalMs)
    , m_stableDuration(stableDurationMs)
{
}

SMMatrixReader::~SMMatrixReader()
{
    Stop();
}

void SMMatrixReader::SetSourceAddress(const void* address)
{
    const float* addr = static_cast<const float*>(address);
    m_sourceAddress.store(addr, std::memory_order_release);
}

void SMMatrixReader::SetPollIntervalMs(unsigned int milliseconds)
{
    m_pollInterval = milliseconds;
}

void SMMatrixReader::SetStableDurationMs(unsigned int milliseconds)
{
    m_stableDuration = milliseconds;
}

bool SMMatrixReader::Start(Callback callback)
{
    if (!callback)
        return false;

    bool expected = false;
    if (!m_running.compare_exchange_strong(expected, true, std::memory_order_acq_rel))
    {
        // Already running
        return false;
    }

    m_callback.store(callback, std::memory_order_release);

    m_thread = std::thread(&SMMatrixReader::ThreadLoop, this);

    return true;
}

void SMMatrixReader::Stop()
{
    bool expected = true;
    if (m_running.compare_exchange_strong(expected, false, std::memory_order_acq_rel))
    {
        if (m_thread.joinable())
        {
            m_thread.join();
        }
    }
}

void SMMatrixReader::ThreadLoop()
{

#ifdef _WIN32
    SetThreadPriority(GetCurrentThread(), THREAD_PRIORITY_HIGHEST);
#endif


    Matrix prevSample = {};
    bool hasPrevSample = false;

    bool inChange = false;

    // Time when we first detected a change (steady clock, for internal use)
    double changeStartSteady = SystemTime::GetInMicroseconds();

    // Absolute timestamp (seconds since epoch) corresponding to changeStartSteady
    double changeStartAbsoluteSec = 0.0;

    // Time since the matrix last appeared to be "stable" (unchanged)
    double stableSince = SystemTime::GetInMicroseconds();

    while (m_running.load(std::memory_order_acquire))
    {
        const float* addr = m_sourceAddress.load(std::memory_order_acquire);

        if (addr != nullptr)
        {
            Matrix currentSample = {};
            // Read 16 floats from the provided address
            std::memcpy(currentSample.data(), addr, sizeof(float) * 16);

            if (!hasPrevSample)
            {
                prevSample = currentSample;
                hasPrevSample = true;
                inChange = false;
            }
            else
            {
                // Compare current and previous samples
                if (std::memcmp(prevSample.data(), currentSample.data(),
                    sizeof(float) * 16) != 0)
                {
                    // Matrix contents changed
                    prevSample = currentSample;

                    double timeNow = SystemTime::GetInMicroseconds();

                    if (!inChange)
                    {
                        // This is the first detection of a new change sequence
                        inChange = true;
                        changeStartSteady = timeNow;
                        stableSince = timeNow;

                        // Record absolute timestamp in seconds (system time)
                        changeStartAbsoluteSec = SystemTime::GetInSeconds();
                    }
                    else
                    {
                        // Already in a change sequence; still changing, so reset stableSince
                        stableSince = timeNow;
                        changeStartAbsoluteSec = SystemTime::GetInSeconds();
                    }
                }
                else
                {
                    // Matrix is the same as previous sample
                    if (inChange)
                    {
                        //SteadyClock::time_point nowSteady = SteadyClock::now();
                        //auto stableDuration =
                        //    std::chrono::duration_cast<std::chrono::milliseconds>(
                        //        nowSteady - stableSince);

                        double timeNow = SystemTime::GetInMicroseconds();
                        double stableDuration = (timeNow - stableSince) / 1000.0;

                        if (stableDuration >= m_stableDuration)
                        {
                            // The matrix has remained unchanged long enough
                            Callback cb = m_callback.load(std::memory_order_acquire);
                            if (cb)
                            {
                                // Pass matrix by value and the absolute timestamp
                                cb(currentSample, changeStartAbsoluteSec);
//                                cb(currentSample, SystemTime::GetInSeconds());
                            }

                            // Reset change tracking
                            inChange = false;
                        }
                    }
                }
            }
        }

        // Sleep for the configured poll interval
//        std::this_thread::sleep_for(m_pollInterval);
    }
}
