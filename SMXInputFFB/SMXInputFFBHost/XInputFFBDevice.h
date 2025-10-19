#pragma once
#include "SMXInputFFBHostDefines.h"

#include <Xinput.h>
#include <guiddef.h>
#include <cstdint>
#include <string >

class DISourceDevice;
class XInputFFBHost;
class XInputFFBConfig;
class XInputFFBDeviceConfig;
struct AxisMapping;

enum class XInputAxis : int
{
    LX = 0, LY, RX, RY, LT, RT, COUNT
};

class SMXINPUTFFBHOST_API XInputFFBDevice
{
public:
    XInputFFBDevice();
    ~XInputFFBDevice();

    void SetHost(XInputFFBHost* host);
    void SetConfig(XInputFFBConfig* cfg);
    void SetUserIndex(unsigned userIndex); // 0..XUSER_MAX_COUNT-1

    // State population (reads cached DIJOYSTATE2 via host and current config)
    const XINPUT_STATE& UpdateState(uint32_t vehicleTypeMask);

    // Force feedback across all DI axes mapped to this XInput axis in the current user config
    bool SetAxisForce(XInputAxis axis, long magnitude);

    const XINPUT_STATE& GetCachedState() const { return m_state; }

private:
    // Helpers
    DISourceDevice* FindDevice(const std::string &guid) const;
    bool SampleAxis(DISourceDevice* dev, int diAxis, long& outValue) const;

    static float NormalizeDIValue(long v);
    static float FilterAxis(float v, const AxisMapping& axisMapping);
    static SHORT ToXInputStick(float v);
    static BYTE  ToXInputTrigger(float v);

private:
    XInputFFBHost* m_host = nullptr;
    XInputFFBConfig* m_cfg = nullptr;
    unsigned        m_user = 0; // which virtual controller (0..XUSER_MAX_COUNT-1)
    XINPUT_STATE    m_state;
};
