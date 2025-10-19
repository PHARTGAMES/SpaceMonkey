#include "DISourceDevice.h"
#include <string>


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

    for (int i = 0; i < (int)DIAxis::COUNT; ++i)
    {
        m_constant[i] = nullptr;
        m_damper[i] = nullptr;
        m_collision[i] = nullptr;
    }
    std::memset(&m_cachedState, 0, sizeof(m_cachedState));
}

DISourceDevice::~DISourceDevice()
{
    for (int i = 0; i < (int)DIAxis::COUNT; ++i)
    {
        if (m_constant[i]) { m_constant[i]->Release();  m_constant[i] = nullptr; }
        if (m_damper[i]) { m_damper[i]->Release();    m_damper[i] = nullptr; }
        if (m_collision[i]) { m_collision[i]->Release(); m_collision[i] = nullptr; }
    }
    SafeReleaseIUnknown((IUnknown*&)m_dev);
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
    SetupCooperativeLevel(); // best-effort (exclusive for FFB if hwnd present)

    // Check FFB support
    DIDEVCAPS caps;
    std::memset(&caps, 0, sizeof(caps));
    caps.dwSize = sizeof(caps);
    if (SUCCEEDED(m_dev->GetCapabilities(&caps)))
    {
        m_ffbSupported = (caps.dwFlags & DIDC_FORCEFEEDBACK) != 0;
    }

    // Optional: eager create baseline effects (lazy creation also supported)
    if (m_ffbSupported) CreateAllAxisEffects();

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

DWORD DISourceDevice::AxisToOffset(DIAxis a)
{
    switch (a)
    {
    case DIAxis::X:       return DIJOFS_X;
    case DIAxis::Y:       return DIJOFS_Y;
    case DIAxis::Z:       return DIJOFS_Z;
    case DIAxis::RX:      return DIJOFS_RX;
    case DIAxis::RY:      return DIJOFS_RY;
    case DIAxis::RZ:      return DIJOFS_RZ;
    case DIAxis::SLIDER0: return DIJOFS_SLIDER(0);
    case DIAxis::SLIDER1: return DIJOFS_SLIDER(1);
    default:              return DIJOFS_X;
    }
}

LONG DISourceDevice::Clamp10000(LONG v)
{
    if (v < -10000) v = -10000;
    if (v > 10000) v = 10000;
    return v;
}

bool DISourceDevice::EnsureAxisEffect(DIFfbType type, DIAxis axis)
{
    if (!m_dev || !m_ffbSupported) return false;

    IDirectInputEffect** slot = nullptr;
    if (type == DIFfbType::Constant) slot = &m_constant[(int)axis];
    else if (type == DIFfbType::Damper) slot = &m_damper[(int)axis];
    else slot = &m_collision[(int)axis];

    if (*slot) return true; // already exists

    DWORD axisOffset = AxisToOffset(axis);
    DWORD rgdwAxes[1] = { axisOffset };
    LONG  rglDirection[1] = { 0 }; // 0 = along axis

    if (type == DIFfbType::Constant)
    {
        DICONSTANTFORCE cf;
        std::memset(&cf, 0, sizeof(cf));
        cf.lMagnitude = 0;

        DIEFFECT eff;
        std::memset(&eff, 0, sizeof(eff));
        eff.dwSize = sizeof(eff);
        eff.dwFlags = DIEFF_CARTESIAN | DIEFF_OBJECTOFFSETS;
        eff.dwDuration = INFINITE;
        eff.dwGain = 10000;
        eff.dwTriggerButton = DIEB_NOTRIGGER;
        eff.cAxes = 1;
        eff.rgdwAxes = rgdwAxes;
        eff.rglDirection = rglDirection;
        eff.cbTypeSpecificParams = sizeof(DICONSTANTFORCE);
        eff.lpvTypeSpecificParams = &cf;

        return SUCCEEDED(m_dev->CreateEffect(GUID_ConstantForce, &eff, slot, nullptr));
    }
    else if (type == DIFfbType::Damper)
    {
        DICONDITION cond;
        std::memset(&cond, 0, sizeof(cond));
        cond.lPositiveCoefficient = 0;
        cond.lNegativeCoefficient = 0;
        cond.dwPositiveSaturation = DI_FFNOMINALMAX;
        cond.dwNegativeSaturation = DI_FFNOMINALMAX;
        cond.lDeadBand = 0;
        cond.lOffset = 0;

        DIEFFECT eff;
        std::memset(&eff, 0, sizeof(eff));
        eff.dwSize = sizeof(eff);
        eff.dwFlags = DIEFF_CARTESIAN | DIEFF_OBJECTOFFSETS;
        eff.dwDuration = INFINITE;
        eff.dwGain = 10000;
        eff.dwTriggerButton = DIEB_NOTRIGGER;
        eff.cAxes = 1;
        eff.rgdwAxes = rgdwAxes;
        eff.rglDirection = rglDirection;
        eff.cbTypeSpecificParams = sizeof(DICONDITION);
        eff.lpvTypeSpecificParams = &cond;

        return SUCCEEDED(m_dev->CreateEffect(GUID_Damper, &eff, slot, nullptr));
    }
    else // Collision pulse as short constant-force burst
    {
        DICONSTANTFORCE cf;
        std::memset(&cf, 0, sizeof(cf));
        cf.lMagnitude = 0;

        DIEFFECT eff;
        std::memset(&eff, 0, sizeof(eff));
        eff.dwSize = sizeof(eff);
        eff.dwFlags = DIEFF_CARTESIAN | DIEFF_OBJECTOFFSETS;
        eff.dwDuration = 100000; // 100ms (unit is 100ns)
        eff.dwGain = 10000;
        eff.dwTriggerButton = DIEB_NOTRIGGER;
        eff.cAxes = 1;
        eff.rgdwAxes = rgdwAxes;
        eff.rglDirection = rglDirection;
        eff.cbTypeSpecificParams = sizeof(DICONSTANTFORCE);
        eff.lpvTypeSpecificParams = &cf;

        return SUCCEEDED(m_dev->CreateEffect(GUID_ConstantForce, &eff, slot, nullptr));
    }
}

IDirectInputEffect* DISourceDevice::GetAxisEffect(DIFfbType type, DIAxis axis)
{
    if (!EnsureAxisEffect(type, axis)) return nullptr;
    if (type == DIFfbType::Constant) return m_constant[(int)axis];
    if (type == DIFfbType::Damper)   return m_damper[(int)axis];
    return m_collision[(int)axis];
}

bool DISourceDevice::SetConstantForce(DIAxis axis, LONG magnitude)
{
    IDirectInputEffect* fx = GetAxisEffect(DIFfbType::Constant, axis);
    if (!fx) return false;

    DICONSTANTFORCE cf;
    std::memset(&cf, 0, sizeof(cf));
    cf.lMagnitude = Clamp10000(magnitude);

    DIEFFECT eff;
    std::memset(&eff, 0, sizeof(eff));
    eff.dwSize = sizeof(eff);
    eff.cbTypeSpecificParams = sizeof(DICONSTANTFORCE);
    eff.lpvTypeSpecificParams = &cf;

    if (FAILED(fx->SetParameters(&eff, DIEP_TYPESPECIFICPARAMS))) return false;
    fx->Start(1, 0);
    return true;
}

bool DISourceDevice::SetDamper(DIAxis axis, LONG coeff, LONG saturation)
{
    IDirectInputEffect* fx = GetAxisEffect(DIFfbType::Damper, axis);
    if (!fx) return false;

    DICONDITION cond;
    std::memset(&cond, 0, sizeof(cond));
    LONG c = Clamp10000(coeff);
    LONG s = Clamp10000(saturation);
    cond.lPositiveCoefficient = c;
    cond.lNegativeCoefficient = c;
    //cond.lPositiveSaturation = s;
    //cond.lNegativeSaturation = s;
    cond.dwPositiveSaturation = DI_FFNOMINALMAX;
    cond.dwNegativeSaturation = DI_FFNOMINALMAX;
    cond.lDeadBand = 0;
    cond.lOffset = 0;

    DIEFFECT eff;
    std::memset(&eff, 0, sizeof(eff));
    eff.dwSize = sizeof(eff);
    eff.cbTypeSpecificParams = sizeof(DICONDITION);
    eff.lpvTypeSpecificParams = &cond;

    if (FAILED(fx->SetParameters(&eff, DIEP_TYPESPECIFICPARAMS))) return false;
    fx->Start(1, 0);
    return true;
}

bool DISourceDevice::FireCollisionPulse(DIAxis axis, LONG magnitude, DWORD durationMs)
{
    IDirectInputEffect* fx = GetAxisEffect(DIFfbType::Collision, axis);
    if (!fx) return false;

    DICONSTANTFORCE cf;
    std::memset(&cf, 0, sizeof(cf));
    cf.lMagnitude = Clamp10000(magnitude);

    DIEFFECT eff;
    std::memset(&eff, 0, sizeof(eff));
    eff.dwSize = sizeof(eff);
    eff.dwDuration = (DWORD)durationMs * 10000U; // ms -> 100ns units
    eff.cbTypeSpecificParams = sizeof(DICONSTANTFORCE);
    eff.lpvTypeSpecificParams = &cf;

    if (FAILED(fx->SetParameters(&eff, DIEP_DURATION | DIEP_TYPESPECIFICPARAMS))) return false;
    fx->Stop(); // ensure restart
    return SUCCEEDED(fx->Start(1, 0));
}

bool DISourceDevice::CreateAllAxisEffects()
{
    bool ok = true;
    for (int i = 0; i < (int)DIAxis::COUNT; ++i)
    {
        ok = EnsureAxisEffect(DIFfbType::Constant, (DIAxis)i) && ok;
        ok = EnsureAxisEffect(DIFfbType::Damper, (DIAxis)i) && ok;
        ok = EnsureAxisEffect(DIFfbType::Collision, (DIAxis)i) && ok;
    }
    return ok;
}
