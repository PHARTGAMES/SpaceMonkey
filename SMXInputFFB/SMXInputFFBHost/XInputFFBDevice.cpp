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

static const long DI_AXIS_MIN = -10000;
static const long DI_AXIS_MAX = 10000;

XInputFFBDevice::XInputFFBDevice() { std::memset(&m_state, 0, sizeof(m_state)); }
XInputFFBDevice::~XInputFFBDevice() {}

void XInputFFBDevice::SetHost(XInputFFBHost* host) { m_host = host; }
void XInputFFBDevice::SetConfig(XInputFFBConfig* c) { m_cfg = c; }
void XInputFFBDevice::SetUserIndex(unsigned idx) { m_user = (idx < XUSER_MAX_COUNT) ? idx : 0; }

static long ClampLong(long v, long lo, long hi) { if (v < lo) return lo; if (v > hi) return hi; return v; }
float XInputFFBDevice::NormalizeDIValue(long v) { v = ClampLong(v, DI_AXIS_MIN, DI_AXIS_MAX); return (float)v / 10000.0f; }
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

    //curve
    v = copysign(powf(fabs(v), axisMapping.curve), v);
    
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

bool XInputFFBDevice::SampleAxis(DISourceDevice* dev, int diAxis, long& outValue) const
{
    if (!dev) return false;
    const DIJOYSTATE2& js = dev->GetCachedState();
    switch (diAxis)
    {
    case 0: outValue = js.lX; break;
    case 1: outValue = js.lY; break;
    case 2: outValue = js.lZ; break;
    case 3: outValue = js.lRx; break;
    case 4: outValue = js.lRy; break;
    case 5: outValue = js.lRz; break;
    case 6: outValue = js.rglSlider[0]; break;
    case 7: outValue = js.rglSlider[1]; break;
    default: return false;
    }
    return true;
}

const XINPUT_STATE& XInputFFBDevice::UpdateState(uint32_t vehicleTypeMask)
{
    std::memset(&m_state, 0, sizeof(m_state));
    if (!m_cfg) return m_state;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);

    // Buttons
    WORD buttons = 0;
    const std::vector<ButtonMapping>& bmaps = cfg.GetButtons();
    for (size_t i = 0; i < bmaps.size(); ++i)
    {
        const ButtonMapping& bm = bmaps[i];

        if ((bm.vehicleTypeMask & vehicleTypeMask) == 0)
            continue;

        DISourceDevice* dev = FindDevice(bm.deviceId);
        if (!dev) continue;

        const DIJOYSTATE2& js = dev->GetCachedState();
        int idx = bm.diButtonIndex;
        if (idx >= 0 && idx < 128)
        {
            BYTE pressed = js.rgbButtons[idx];
            if (pressed & 0x80) buttons |= bm.xinputBit;
        }
    }

    // Axes
    float acc[6] = { 0 }; // LX,LY,RX,RY,LT,RT
    for (int a = 0; a < 6; ++a)
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

            long raw = 0;
            if (!SampleAxis(dev, am.diAxis, raw)) continue;
            float vn = NormalizeDIValue(raw);
            float vf = FilterAxis(vn, am);
            vsum += vf; any = true;
        }
        if (any)
        {
            if (vsum < -1.0f) vsum = -1.0f;
            if (vsum > 1.0f) vsum = 1.0f;
            acc[a] = vsum;
        }
    }

    XINPUT_GAMEPAD& gp = m_state.Gamepad;
    gp.wButtons = buttons;
    gp.sThumbLX = ToXInputStick(acc[0]);
    gp.sThumbLY = ToXInputStick(acc[1]);
    gp.sThumbRX = ToXInputStick(acc[2]);
    gp.sThumbRY = ToXInputStick(acc[3]);
    gp.bLeftTrigger = ToXInputTrigger(acc[4]);
    gp.bRightTrigger = ToXInputTrigger(acc[5]);

    ++m_state.dwPacketNumber;
    return m_state;
}

bool XInputFFBDevice::SetAxisForce(XInputAxis axis, long magnitude)
{
    if (!m_cfg) return false;

    XInputFFBDeviceConfig& cfg = m_cfg->GetDeviceConfig(m_user);
    int ai = (int)axis;
    std::vector<AxisMapping>* bucket = cfg.GetAxisBucket(ai);
    if (!bucket) return false;

    long mag = magnitude; if (mag < -10000) mag = -10000; if (mag > 10000) mag = 10000;

    bool ok = false;
    for (size_t i = 0; i < bucket->size(); ++i)
    {
        const AxisMapping& am = (*bucket)[i];
        DISourceDevice* dev = FindDevice(am.deviceId);
        if (!dev) continue;

        int diAxis = am.diAxis;
        if (diAxis < 0 || diAxis > 7) continue;

        ok |= dev->SetConstantForce((DIAxis)diAxis, mag);
    }
    return ok;
}
