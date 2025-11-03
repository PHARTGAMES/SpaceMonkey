#pragma once
#include "SMXInputFFBHostDefines.h"
#include "XInputFFBConfig.h"

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

    bool SetAxisConstantForce(XInputFFBEffectType effectType, long magnitude);
    bool SetAxisDamperForce(XInputFFBEffectType effectType, long magnitude);
    bool SetAxisVibration(XInputFFBEffectType effectType, long frequencyHz, long gain);

    const XINPUT_STATE& GetCachedState() const { return m_state; }

    float GetAxisValue(XInputAxis axis);
    float GetDIAxisValue(DISourceDevice* dev, int diAxis);

private:
    // Helpers
    DISourceDevice* FindDevice(const std::string &guid) const;

    static float NormalizeDIValue(long v);
    static float FilterAxis(float v, const AxisMapping& axisMapping);
    static SHORT ToXInputStick(float v);
    static BYTE  ToXInputTrigger(float v);

private:
    XInputFFBHost* m_host = nullptr;
    XInputFFBConfig* m_cfg = nullptr;
    unsigned        m_user = 0; // which virtual controller (0..XUSER_MAX_COUNT-1)
    XINPUT_STATE    m_state;
    float m_axisState[6];
};
