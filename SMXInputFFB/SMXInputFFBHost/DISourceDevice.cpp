#include "DISourceDevice.h"
#include <string>

static const long DI_AXIS_MIN = -10000;
static const long DI_AXIS_MAX = 10000;
static long ClampLong(long v, long lo, long hi) { if (v < lo) return lo; if (v > hi) return hi; return v; }



bool GuidToString(const GUID& g, std::string& out)
{
    RPC_CSTR s = NULL;
    if (UuidToStringA((UUID*)&g, &s) == RPC_S_OK && s)
    {
        out.assign((const char*)s);
        RpcStringFreeA(&s);
        return true;
    }
    return false;
}

static inline void SafeReleaseIUnknown(IUnknown*& p)
{
    if (p) { p->Release(); p = nullptr; }
}
DISourceDevice::DISourceDevice(IDirectInput8* di, const DIDEVICEINSTANCE& inst)
    : m_di(di)
{
    m_instanceGuid = inst.guidInstance;
    m_productGuid = inst.guidProduct;
    GuidToString(m_instanceGuid, m_instanceGuidString);

    std::memset(m_name, 0, sizeof(m_name));
    std::strncpy(m_name, inst.tszProductName ? inst.tszProductName : "", sizeof(m_name) - 1);

    std::memset(&m_cachedState, 0, sizeof(m_cachedState));
}

DISourceDevice::~DISourceDevice()
{
    // Managers release their effects in their own destructors
    if (m_dev) { m_dev->Unacquire(); }
    if (m_dev) { m_dev->Release(); m_dev = nullptr; }
}

void DISourceDevice::SetHWND(HWND hwnd)
{
    m_hwnd = hwnd;
    if (m_dev) SetupCooperativeLevel();
}

bool DISourceDevice::Initialize()
{
    if (!CreateDevice()) return false;
    if (!SetupDataFormatAndRange()) return false;
    SetupCooperativeLevel();

    DIDEVCAPS caps{};
    caps.dwSize = sizeof(caps);
    if (SUCCEEDED(m_dev->GetCapabilities(&caps)))
        m_ffbSupported = (caps.dwFlags & DIDC_FORCEFEEDBACK) != 0;

    if (m_ffbSupported) CreateAllAxisEffects(); // optional eager create
    return true;
}

bool DISourceDevice::CreateDevice()
{
    if (!m_di) return false;
    HRESULT hr = m_di->CreateDevice(m_instanceGuid, &m_dev, nullptr);
    return SUCCEEDED(hr) && m_dev != nullptr;
}

bool DISourceDevice::SetupCooperativeLevel()
{
    if (!m_dev) return false;

    // Try exclusive for reliable FFB if we have an HWND, fallback to non-exclusive
    if (m_hwnd)
    {
        HRESULT hr = m_dev->SetCooperativeLevel(m_hwnd, DISCL_EXCLUSIVE | DISCL_BACKGROUND);
        if (SUCCEEDED(hr)) return true;
        // Fallback
        hr = m_dev->SetCooperativeLevel(m_hwnd, DISCL_NONEXCLUSIVE | DISCL_BACKGROUND);
        return SUCCEEDED(hr);
    }
    // No HWND available yet; skip
    return true;
}

bool DISourceDevice::SetupDataFormatAndRange()
{
    if (!m_dev) return false;

    // Joystick2 data format (DIJOYSTATE2)
    HRESULT hr = m_dev->SetDataFormat(&c_dfDIJoystick2);
    if (FAILED(hr)) return false;

    // Suggested default ranges to -10000..10000 for all axes
    DIPROPRANGE range;
    std::memset(&range, 0, sizeof(range));
    range.diph.dwSize = sizeof(DIPROPRANGE);
    range.diph.dwHeaderSize = sizeof(DIPROPHEADER);
    range.diph.dwHow = DIPH_DEVICE;
    range.lMin = -10000;
    range.lMax = 10000;
    m_dev->SetProperty(DIPROP_RANGE, &range.diph);

    // Buffer size (small buffer; we poll anyway)
    DIPROPDWORD buffer;
    std::memset(&buffer, 0, sizeof(buffer));
    buffer.diph.dwSize = sizeof(DIPROPDWORD);
    buffer.diph.dwHeaderSize = sizeof(DIPROPHEADER);
    buffer.diph.dwHow = DIPH_DEVICE;
    buffer.dwData = 16;
    m_dev->SetProperty(DIPROP_BUFFERSIZE, &buffer.diph);

    return true;
}

bool DISourceDevice::Acquire()
{
    if (!m_dev) return false;
    HRESULT hr = m_dev->Acquire();
    if (hr == DIERR_INPUTLOST || hr == DIERR_NOTACQUIRED) hr = m_dev->Acquire();
    return SUCCEEDED(hr);
}

void DISourceDevice::Unacquire()
{
    if (m_dev) m_dev->Unacquire();
}

bool DISourceDevice::UpdateState()
{
    if (!m_dev) return false;

    HRESULT hr = m_dev->Poll();
    if (FAILED(hr))
    {
        // Try to reacquire if needed
        Acquire();
        m_dev->Poll();
    }

    hr = m_dev->GetDeviceState(sizeof(DIJOYSTATE2), &m_cachedState);
    m_hasState = SUCCEEDED(hr);
    return m_hasState;
}

bool DISourceDevice::CreateAllAxisEffects()
{
    if (!m_dev) return false;

    // match your previous global device props
    DIPROPDWORD ac = { { sizeof(DIPROPDWORD), sizeof(DIPROPHEADER), 0, DIPH_DEVICE }, 0 };
    m_dev->SetProperty(DIPROP_AUTOCENTER, &ac.diph);

    DIPROPDWORD gain = { { sizeof(DIPROPDWORD), sizeof(DIPROPHEADER), 0, DIPH_DEVICE }, 10000 };
    m_dev->SetProperty(DIPROP_FFGAIN, &gain.diph);

    bool ok = true;
    for (int i = 0; i < (int)DIAxis::COUNT; ++i)
    {
        // Touch Get() to lazily create and start each axis effect
        ok = (m_constant.Get(m_dev, (DIAxis)i) != nullptr) && ok;
        ok = (m_damper.Get(m_dev, (DIAxis)i) != nullptr) && ok;
        ok = (m_vibration.Get(m_dev, (DIAxis)i) != nullptr) && ok;
        ok = (m_friction.Get(m_dev, (DIAxis)i) != nullptr) && ok; // NEW
    }
    return ok;
}

// ------------ Convenience setters ------------
bool DISourceDevice::SetConstantForce(DIAxis axis, LONG magnitude)
{
    if (!m_ffbSupported || !m_dev) return false;
    return m_constant.Set(m_dev, axis, magnitude);
}

bool DISourceDevice::SetDamper(DIAxis axis, LONG coeff, LONG saturation)
{
    if (!m_ffbSupported || !m_dev) return false;
    return m_damper.Set(m_dev, axis, coeff, saturation);
}

bool DISourceDevice::SetVibration(DIAxis axis, LONG frequencyHz, LONG gain)
{
    if (!m_ffbSupported || !m_dev) return false;
    return m_vibration.Set(m_dev, axis, frequencyHz, gain);
}

bool DISourceDevice::SetFriction(DIAxis axis, LONG coeff, LONG saturation)
{
    if (!m_ffbSupported || !m_dev) return false;
    return m_friction.Set(m_dev, axis, coeff, saturation);
}

long DISourceDevice::GetAxisValue(DIAxis diAxis)
{
    long outValue = 0;

    const DIJOYSTATE2& js = GetCachedState();
    switch (diAxis)
    {
    case DIAxis::X: outValue = js.lX; break;
    case DIAxis::Y: outValue = js.lY; break;
    case DIAxis::Z: outValue = js.lZ; break;
    case DIAxis::RX: outValue = js.lRx; break;
    case DIAxis::RY: outValue = js.lRy; break;
    case DIAxis::RZ: outValue = js.lRz; break;
    case DIAxis::SLIDER0: outValue = js.rglSlider[0]; break;
    case DIAxis::SLIDER1: outValue = js.rglSlider[1]; break;
    }
    return outValue;

}

float DISourceDevice::NormalizeDIValue(long v)
{
    v = ClampLong(v, DI_AXIS_MIN, DI_AXIS_MAX);
    return (float)v / 10000.0f;
}


float DISourceDevice::GetAxisValueNorm(DIAxis axis)
{
    return NormalizeDIValue(GetAxisValue(axis));
}

float DISourceDevice::GetButtonValueNorm(int button)
{
    if (button < 0 || button >= 128)
        return 0.0f;

    const DIJOYSTATE2& js = GetCachedState();

    return (float)js.rgbButtons[button] / 255.0f;

}


void DISourceDevice::HandleFocusGain()
{
    if (!m_dev)
        return;

    // Best-effort: stop anything currently running (ignore errors if unacquired)
    m_dev->SendForceFeedbackCommand(DISFFC_STOPALL);

    // Release our effect objects so they will be recreated cleanly on next Set()
    m_constant.DestroyAll();
    m_damper.DestroyAll();
    m_vibration.DestroyAll();
    m_friction.DestroyAll();

    // Fully unacquire before re-acquiring
    m_dev->Unacquire();

    // Reacquire the device (standard DI reacquire pattern)
    Acquire();

    // Drivers often flip these back on focus changes — reapply every time.
    DIPROPDWORD ac = { { sizeof(DIPROPDWORD), sizeof(DIPROPHEADER), 0, DIPH_DEVICE }, 0 /* OFF */ };
    m_dev->SetProperty(DIPROP_AUTOCENTER, &ac.diph);

    DIPROPDWORD gain = { { sizeof(DIPROPDWORD), sizeof(DIPROPHEADER), 0, DIPH_DEVICE }, 10000 /* 0..10000 */ };
    m_dev->SetProperty(DIPROP_FFGAIN, &gain.diph);

    // Do NOT eagerly recreate effects here; they are created lazily in the Set* calls
    // via the effect managers' Get(...). If you want eager creation, uncomment:
    // if (m_ffbSupported) { CreateAllAxisEffects(); }
}
