#include <windows.h>
#include "XInputFFBDevice.h"
#include "DISourceDevice.h"
#include "XInputFFBHost.h"
#include "XInputFFBConfig.h"
#include <string>
#include <algorithm>

// for parsing GUID strings
#include <Rpc.h>
#pragma comment(lib, "Rpcrt4.lib")



XInputFFBDevice::XInputFFBDevice() { std::memset(&m_state, 0, sizeof(m_state)); }
XInputFFBDevice::~XInputFFBDevice() {}

void XInputFFBDevice::SetHost(XInputFFBHost* host) { m_host = host; }
void XInputFFBDevice::SetConfig(XInputFFBConfig* c) { m_cfg = c; }
void XInputFFBDevice::SetUserIndex(unsigned idx) { m_user = (idx < XUSER_MAX_COUNT) ? idx : 0; }

float XInputFFBDevice::FilterAxis(float v, const AxisMapping& axisMapping)
{

    //invert
    if (axisMapping.invert)
    {
        v = -v;
    }

    //pedal
    if (axisMapping.pedal)
    {
        v = (v + 1) * 0.5f;
    }
    
    //deadzone
    float absv = v < 0.0f ? -v : v;
    if (absv < axisMapping.deadzone)
    {
        return 0.0f;
    }

    //deadzone
    if (axisMapping.deadzone > 0.0f && axisMapping.deadzone < 1.0f)
    {
        float sign = v < 0.0f ? -1.0f : 1.0f;
        float t = (absv - axisMapping.deadzone) / (1.0f - axisMapping.deadzone);
        v = sign * t;
    }

    //curve
    v = copysign(powf(fabs(v), axisMapping.curve), v);

    //scale
    v *= axisMapping.scale;

    //clamp
    if (v < -1.0f) 
        v = -1.0f; 
    if (v > 1.0f) 
        v = 1.0f;
    return v;
}
SHORT XInputFFBDevice::ToXInputStick(float v) { int sv = (int)(v * 32767.0f); if (sv < -32768) sv = -32768; if (sv > 32767) sv = 32767; return (SHORT)sv; }
BYTE  XInputFFBDevice::ToXInputTrigger(float v) { float t = v; if (t < 0.0f) t = 0.0f; if (t > 1.0f) t = 1.0f; int iv = (int)(t * 255.0f); if (iv < 0) iv = 0; if (iv > 255) iv = 255; return (BYTE)iv; }

DISourceDevice* XInputFFBDevice::FindDevice(const std::string &guid) const
{
    if (!m_host) return NULL;
    return m_host->GetDeviceByGUID(guid);
}
static bool GuidFromIdString(const std::string& id, GUID& out)
{
    if (id.empty()) return false;
    return XInputFFBConfig::StringToGuidA(id.c_str(), out);
}

float XInputFFBDevice::GetDIInputValue(DISourceDevice* dev, int diInput)
{
    float returnValue = 0;
    //axis
    if (diInput < DINPUT_AXIS_COUNT)
    {
        returnValue = dev->GetAxisValueNorm((DIAxis)diInput);
    }
    else //pov
    if (diInput < DINPUT_POV_COUNT)
    {

    }
    else //buttons
    if (diInput < DINPUT_BUTTON_COUNT)
    {
        returnValue = dev->GetButtonValueNorm(diInput - (DINPUT_AXIS_COUNT + DINPUT_POV_COUNT));
    }

    return returnValue;
}

const XINPUT_STATE& XInputFFBDevice::UpdateState(uint32_t vehicleTypeMask)
{
    std::memset(&m_state, 0, sizeof(m_state));
    if (!m_cfg) return m_state;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);

    //// Buttons
//    WORD buttons = 0;
    //const std::vector<ButtonMapping>& bmaps = cfg.GetButtons();
    //for (size_t i = 0; i < bmaps.size(); ++i)
    //{
    //    const ButtonMapping& bm = bmaps[i];

    //    if ((bm.vehicleTypeMask & vehicleTypeMask) == 0)
    //        continue;

    //    DISourceDevice* dev = FindDevice(bm.deviceId);
    //    if (!dev) continue;

    //    const DIJOYSTATE2& js = dev->GetCachedState();
    //    int idx = bm.diButtonIndex;
    //    if (idx >= 0 && idx < 128)
    //    {
    //        BYTE pressed = js.rgbButtons[idx];
    //        if (pressed & 0x80) buttons |= bm.xinputBit;
    //    }
    //}

    // Axes
    // LX,LY,RX,RY,LT,RT
    memset(m_axisState, 0, sizeof(float) * XINPUT_INPUT_COUNT);

    for (int a = 0; a < XINPUT_INPUT_COUNT; ++a)
    {
        const std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(a);
        if (!bucket) continue;

        float vsum = 0.0f; bool any = false;
        for (size_t k = 0; k < bucket->size(); ++k)
        {
            const AxisMapping& am = (*bucket)[k];

            if ((am.vehicleTypeMask & vehicleTypeMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev) continue;

            float vn = GetDIInputValue(dev, am.diAxis);
            float vf = FilterAxis(vn, am);
            vsum += vf; any = true;
        }
        if (any)
        {
            if (vsum < -1.0f) vsum = -1.0f;
            if (vsum > 1.0f) vsum = 1.0f;
            m_axisState[a] = vsum;
        }
    }

    //process buttons
    const WORD XINPUT_Buttons[] =
    {
        XINPUT_GAMEPAD_DPAD_UP,
        XINPUT_GAMEPAD_DPAD_DOWN,
        XINPUT_GAMEPAD_DPAD_LEFT,
        XINPUT_GAMEPAD_DPAD_RIGHT,
        XINPUT_GAMEPAD_START,
        XINPUT_GAMEPAD_BACK,
        XINPUT_GAMEPAD_LEFT_THUMB,
        XINPUT_GAMEPAD_RIGHT_THUMB,
        XINPUT_GAMEPAD_LEFT_SHOULDER,
        XINPUT_GAMEPAD_RIGHT_SHOULDER,
        XINPUT_GAMEPAD_A,
        XINPUT_GAMEPAD_B,
        XINPUT_GAMEPAD_X,
        XINPUT_GAMEPAD_Y
    };

    m_buttonState = 0;
    for (int b = XINPUT_AXIS_COUNT; b < XINPUT_INPUT_COUNT; ++b)
    {
        int btnIdx = b - XINPUT_AXIS_COUNT;

        if (m_axisState[b] > 0.5f)
        {
            m_buttonState |= XINPUT_Buttons[btnIdx];
        }
    }

    XINPUT_GAMEPAD& gp = m_state.Gamepad;
    gp.wButtons = m_buttonState;
    gp.sThumbLX = ToXInputStick(m_axisState[0]);
    gp.sThumbLY = ToXInputStick(m_axisState[1]);
    gp.sThumbRX = ToXInputStick(m_axisState[2]);
    gp.sThumbRY = ToXInputStick(m_axisState[3]);
    gp.bLeftTrigger = ToXInputTrigger(m_axisState[4]);
    gp.bRightTrigger = ToXInputTrigger(m_axisState[5]);

    ++m_state.dwPacketNumber;
    return m_state;
}


bool XInputFFBDevice::SetAxisConstantForce(XInputFFBEffectType effectType, long magnitude)
{
    if (!m_cfg)
        return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    const uint32_t effectMask = static_cast<uint32_t>(effectType);

    // Clamp magnitude to DirectInput constant force limits
    long mag = magnitude;
    if (mag < -10000) mag = -10000;
    if (mag > 10000)  mag = 10000;

    bool ok = false;

    // Iterate all XInput axis buckets (LX, LY, RX, RY, LT, RT, etc.)
    for (int axisIndex = 0; axisIndex < XINPUT_AXIS_COUNT; ++axisIndex)
    {
        std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(axisIndex);
        if (!bucket)
            continue;

        for (size_t i = 0; i < bucket->size(); ++i)
        {
            const AxisMapping& am = (*bucket)[i];

            // Skip mappings that don't include this effect type
            if ((am.ffbEffectMask & effectMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev)
                continue;

            const int diAxis = am.diAxis;
            if (diAxis < 0 || diAxis > 7)
                continue;

            // Apply the force
            ok |= dev->SetConstantForce((DIAxis)diAxis, mag);
        }
    }

    return ok;
}


bool XInputFFBDevice::SetAxisDamperForce(XInputFFBEffectType effectType, long magnitude)
{
    if (!m_cfg)
        return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    const uint32_t effectMask = static_cast<uint32_t>(effectType);

    // Clamp coefficient and saturation values to valid DirectInput range
    long coeff = magnitude;
    if (coeff < -10000) coeff = -10000;
    if (coeff > 10000)  coeff = 10000;

    // Use saturation equal to |magnitude|, fully clamped
    long saturation = std::abs(coeff);
    if (saturation > 10000)
        saturation = 10000;

    bool ok = false;

    // Iterate all XInput axis buckets (LX, LY, RX, RY, LT, RT, etc.)
    for (int axisIndex = 0; axisIndex < XINPUT_AXIS_COUNT; ++axisIndex)
    {
        std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(axisIndex);
        if (!bucket)
            continue;

        for (size_t i = 0; i < bucket->size(); ++i)
        {
            const AxisMapping& am = (*bucket)[i];

            // Skip mappings that don't include this effect type
            if ((am.ffbEffectMask & effectMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev)
                continue;

            const int diAxis = am.diAxis;
            if (diAxis < 0 || diAxis > 7)
                continue;

            // Apply damper force via DirectInput device
            ok |= dev->SetDamper((DIAxis)diAxis, coeff, saturation);
        }
    }

    return ok;
}

bool XInputFFBDevice::SetAxisVibration(XInputFFBEffectType effectType, long frequencyHz, long gain)
{
    if (!m_cfg)
        return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    const uint32_t effectMask = static_cast<uint32_t>(effectType);

    // Clamp frequency and gain to valid ranges
    if (frequencyHz < 1) frequencyHz = 1;
    if (frequencyHz > 1000) frequencyHz = 1000; // upper bound for safety
    if (gain < 0) gain = 0;
    if (gain > 10000) gain = 10000;

    bool ok = false;

    // Iterate all XInput axis buckets (LX, LY, RX, RY, LT, RT, etc.)
    for (int axisIndex = 0; axisIndex < XINPUT_AXIS_COUNT; ++axisIndex)
    {
        std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(axisIndex);
        if (!bucket)
            continue;

        for (size_t i = 0; i < bucket->size(); ++i)
        {
            const AxisMapping& am = (*bucket)[i];

            // Skip mappings that don't include this effect type
            if ((am.ffbEffectMask & effectMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev)
                continue;

            const int diAxis = am.diAxis;
            if (diAxis < 0 || diAxis > 7)
                continue;

            // Apply vibration effect on this mapped DirectInput axis
            ok |= dev->SetVibration((DIAxis)diAxis, frequencyHz, gain);
        }
    }

    return ok;
}

bool XInputFFBDevice::SetAxisSpring(XInputFFBEffectType effectType, long magnitude)
{
    if (!m_cfg)
        return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    const uint32_t effectMask = static_cast<uint32_t>(effectType);

    // Clamp magnitude to DirectInput spring force limits
    long mag = magnitude;
    if (mag < -10000) mag = -10000;
    if (mag > 10000)  mag = 10000;

    bool ok = false;

    // Iterate all XInput axis buckets (LX, LY, RX, RY, LT, RT, etc.)
    for (int axisIndex = 0; axisIndex < XINPUT_AXIS_COUNT; ++axisIndex)
    {
        std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(axisIndex);
        if (!bucket)
            continue;

        for (size_t i = 0; i < bucket->size(); ++i)
        {
            const AxisMapping& am = (*bucket)[i];

            // Skip mappings that don't include this effect type
            if ((am.ffbEffectMask & effectMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev)
                continue;

            const int diAxis = am.diAxis;
            if (diAxis < 0 || diAxis > 7)
                continue;

            // Apply the force
            ok |= dev->SetSpring((DIAxis)diAxis, mag);
        }
    }

    return ok;
}

bool XInputFFBDevice::SetAxisFriction(XInputFFBEffectType effectType, long magnitude)
{
    if (!m_cfg)
        return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    const uint32_t effectMask = static_cast<uint32_t>(effectType);

    // Clamp magnitude to DirectInput friction force limits
    long mag = magnitude;
    if (mag < -10000) mag = -10000;
    if (mag > 10000)  mag = 10000;

    bool ok = false;

    // Iterate all XInput axis buckets (LX, LY, RX, RY, LT, RT, etc.)
    for (int axisIndex = 0; axisIndex < XINPUT_AXIS_COUNT; ++axisIndex)
    {
        std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(axisIndex);
        if (!bucket)
            continue;

        for (size_t i = 0; i < bucket->size(); ++i)
        {
            const AxisMapping& am = (*bucket)[i];

            // Skip mappings that don't include this effect type
            if ((am.ffbEffectMask & effectMask) == 0)
                continue;

            DISourceDevice* dev = FindDevice(am.deviceId);
            if (!dev)
                continue;

            const int diAxis = am.diAxis;
            if (diAxis < 0 || diAxis > 7)
                continue;

            // Apply the force
            ok |= dev->SetFriction((DIAxis)diAxis, mag, 10000UL);
        }
    }

    return ok;
}


float XInputFFBDevice::GetAxisValue(int axis)
{
    if (axis >= XINPUT_INPUT_COUNT || (int)axis < 0)
        return 0;

    return m_axisState[axis];
}

