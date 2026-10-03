#pragma once

#include "SpaceMonkeyTelemetryAPI.h"
#include <array>
#include <atomic>
#include <thread>
#include <chrono>

class SMT_API SMMatrixReader
{
public:
    using Matrix = std::array<float, 16>;
    using Callback = void(*)(Matrix, double); // Matrix by value, timestamp in seconds

    // pollIntervalMs: how often to sample the matrix
    // stableDurationMs: how long the matrix must remain unchanged before callback
    explicit SMMatrixReader(
        unsigned int pollIntervalMs = 5,
        unsigned int stableDurationMs = 50);

    ~SMMatrixReader();

    // Thread-safe: can be called at any time to change the matrix address.
    // The address must point to at least 16 floats.
    void SetSourceAddress(const void* address);

    // Optional configuration (should be called before Start for simplicity).
    void SetPollIntervalMs(unsigned int milliseconds);
    void SetStableDurationMs(unsigned int milliseconds);

    // Starts the reader thread.
    // The callback is invoked from the reader thread whenever a change settles.
    // Returns false if already running or callback is null.
    bool Start(Callback callback);

    // Requests the reader thread to stop and waits for it to finish.
    void Stop();

private:
    void ThreadLoop();

    std::atomic<bool>         m_running;
    std::atomic<const float*> m_sourceAddress;
    std::atomic<Callback>     m_callback;

    std::thread               m_thread;
    double m_pollInterval;
    double m_stableDuration;
};
