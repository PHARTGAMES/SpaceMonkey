#pragma once
#include <dinput.h>
#include <cstdint>
#include <cstring>

enum class DIAxis : int
{
    X = 0, Y, Z, RX, RY, RZ, SLIDER0, SLIDER1, COUNT
};

// Base class managing one IDirectInputEffect* per axis
class DISourceEffectBase
{
public:
    DISourceEffectBase() { std::memset(m_fx, 0, sizeof(m_fx)); }
    virtual ~DISourceEffectBase() { DestroyAll(); }

    // Returns the per-axis effect (creates on-demand).
    IDirectInputEffect* Get(IDirectInputDevice8* dev, DIAxis axis)
    {
        const int idx = static_cast<int>(axis);
        if (!m_fx[idx]) CreateForAxis(dev, axis);
        return m_fx[idx];
    }

    // Stop and release all effects owned by this effect family.
    void DestroyAll()
    {
        for (int i = 0; i < (int)DIAxis::COUNT; ++i)
        {
            if (m_fx[i]) { m_fx[i]->Stop(); m_fx[i]->Unload(); m_fx[i]->Release(); m_fx[i] = nullptr; }
        }
    }

    // High-performance raw array access if the caller wants to walk all axes.
    IDirectInputEffect* const* RawArray() const { return m_fx; }

protected:
    static DWORD AxisToOffset(DIAxis a)
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

    static LONG Clamp10000(LONG v)
    {
        if (v < -10000) v = -10000;
        if (v > 10000)  v = 10000;
        return v;
    }

    virtual GUID EffectGuid() const = 0;                 // which effect family
    virtual void BuildTypeParamsDefaults(void* out, DWORD& bytes) = 0; // fill a default type-specific struct
    virtual DWORD BuildFlags() const { return DIEFF_CARTESIAN | DIEFF_OBJECTOFFSETS; }
    virtual void OnCreated(IDirectInputEffect* fx) { (void)fx; }

    // Called by Get() when m_fx[idx] is null
    void CreateForAxis(IDirectInputDevice8* dev, DIAxis axis)
    {
        if (!dev) return;

        // Make sure device is acquired (same pattern as your code)
        HRESULT hr = dev->Acquire();
        if (FAILED(hr)) return;

        const DWORD off = AxisToOffset(axis);
        DWORD rgdwAxes[1] = { off };
        LONG  rglDir[1] = { 0 };

        BYTE typeParams[64];
        DWORD typeBytes = 0;
        std::memset(typeParams, 0, sizeof(typeParams));
        BuildTypeParamsDefaults(typeParams, typeBytes);

        DIEFFECT eff;
        std::memset(&eff, 0, sizeof(eff));
        eff.dwSize = sizeof(eff);
        eff.dwFlags = BuildFlags();
        eff.dwDuration = INFINITE;
        eff.dwGain = DI_FFNOMINALMAX;
        eff.dwTriggerButton = DIEB_NOTRIGGER;
        eff.cAxes = 1;
        eff.rgdwAxes = rgdwAxes;
        eff.rglDirection = rglDir;
        eff.dwFlags = DIEFF_CARTESIAN | DIEFF_OBJECTOFFSETS;
        eff.cbTypeSpecificParams = typeBytes;
        eff.lpvTypeSpecificParams = typeParams;

        IDirectInputEffect* fx = nullptr;
        if (SUCCEEDED(dev->CreateEffect(EffectGuid(), &eff, &fx, nullptr)) && fx)
        {
            fx->Start(1, 0); // match your eager Start behavior
            OnCreated(fx);
            m_fx[(int)axis] = fx;
        }
    }

protected:
    IDirectInputEffect* m_fx[(int)DIAxis::COUNT];
};

// ---------------- Constant Force ----------------
class DIConstantForceEffect : public DISourceEffectBase
{
public:
    GUID EffectGuid() const override { return GUID_ConstantForce; }

    // magnitude in [-10000, 10000]
    bool Set(IDirectInputDevice8* dev, DIAxis axis, LONG magnitude)
    {
        IDirectInputEffect* fx = Get(dev, axis);
        if (!fx) return false;

        DICONSTANTFORCE cf{};
        cf.lMagnitude = Clamp10000(magnitude);

        //DIENVELOPE env{};
        //env.dwSize = sizeof(env);
        //env.dwAttackLevel = 0;
        //env.dwAttackTime = 64;
        //env.dwFadeLevel = 0;
        //env.dwFadeTime = 0;

        DIEFFECT e{};
        e.dwSize = sizeof(e);
        e.cbTypeSpecificParams = sizeof(DICONSTANTFORCE);
        e.lpvTypeSpecificParams = &cf;
//        e.lpEnvelope = &env;

        return SUCCEEDED(fx->SetParameters(&e, DIEP_TYPESPECIFICPARAMS | DIEP_START));
    }

protected:
    void BuildTypeParamsDefaults(void* out, DWORD& bytes) override
    {
        auto* cf = reinterpret_cast<DICONSTANTFORCE*>(out);
        *cf = {};
        cf->lMagnitude = 0;
        bytes = sizeof(DICONSTANTFORCE);
    }
};

// ---------------- Spring Force ----------------
class DISpringEffect : public DISourceEffectBase
{
public:
    GUID EffectGuid() const override { return GUID_Spring; }

    bool Set(
        IDirectInputDevice8* dev,
        DIAxis axis,
        LONG coefficient,
        LONG offset = 0,
        LONG deadband = 0,
        DWORD saturation = 10000)
    {
        IDirectInputEffect* fx = Get(dev, axis);
        if (!fx) return false;

        DICONDITION cond{};
        cond.lOffset = offset;
        cond.lPositiveCoefficient = Clamp10000(coefficient);
        cond.lNegativeCoefficient = Clamp10000(coefficient);
        cond.dwPositiveSaturation = Clamp10000(saturation);
        cond.dwNegativeSaturation = Clamp10000(saturation);
        cond.lDeadBand = deadband;


        DIEFFECT e{};
        e.dwSize = sizeof(e);
        e.cbTypeSpecificParams = sizeof(DICONDITION);
        e.lpvTypeSpecificParams = &cond;

        return SUCCEEDED(fx->SetParameters(&e, DIEP_TYPESPECIFICPARAMS | DIEP_START));
    }

protected:
    void BuildTypeParamsDefaults(void* out, DWORD& bytes) override
    {
        DICONDITION* cond = reinterpret_cast<DICONDITION*>(out);
        *cond = {};
        cond->lOffset = 0;
        cond->lPositiveCoefficient = 0;
        cond->lNegativeCoefficient = 0;
        cond->dwPositiveSaturation = 10000;
        cond->dwNegativeSaturation = 10000;
        cond->lDeadBand = 0;
        bytes = sizeof(DICONDITION);
    }
};


// ---------------- Periodic Vibration (Sine) ----------------
class DIVibrationEffect : public DISourceEffectBase
{
public:
    GUID EffectGuid() const override { return GUID_Sine; }

    // frequencyHz >= 1, gain in [0,10000]
    bool Set(IDirectInputDevice8* dev, DIAxis axis, LONG frequencyHz, LONG gain)
    {
        if (frequencyHz <= 0) frequencyHz = 1;
        gain = Clamp10000(gain);

        IDirectInputEffect* fx = Get(dev, axis);
        if (!fx) return false;

        DIPERIODIC p{};
        p.dwMagnitude = DI_FFNOMINALMAX; // amplitude
        p.lOffset = 0;
        p.dwPhase = 0;
        p.dwPeriod = static_cast<DWORD>(1000000.0 / (double)frequencyHz); // microseconds

        DIEFFECT e{};
        e.dwSize = sizeof(e);
        e.dwGain = gain;
        e.dwDuration = INFINITE;
        e.cbTypeSpecificParams = sizeof(DIPERIODIC);
        e.lpvTypeSpecificParams = &p;

        return SUCCEEDED(fx->SetParameters(&e, DIEP_GAIN | DIEP_TYPESPECIFICPARAMS | DIEP_START));
    }

protected:
    void BuildTypeParamsDefaults(void* out, DWORD& bytes) override
    {
        auto* p = reinterpret_cast<DIPERIODIC*>(out);
        *p = {};
        p->dwMagnitude = 0;
        p->dwPeriod = 1000000; // 1 Hz default
        bytes = sizeof(DIPERIODIC);
    }
};

// ---------------- Damper (Condition) ----------------
class DIDamperEffect : public DISourceEffectBase
{
public:
    GUID EffectGuid() const override { return GUID_Damper; }

    // coeff, sat in [-10000,10000] and [0,10000] respectively
    bool Set(IDirectInputDevice8* dev, DIAxis axis, LONG coeff, LONG saturation)
    {
        IDirectInputEffect* fx = Get(dev, axis);
        if (!fx) return false;

        LONG c = Clamp10000(coeff);
        LONG s = Clamp10000(saturation);

        DICONDITION cond{};
        cond.lPositiveCoefficient = c;
        cond.lNegativeCoefficient = c;
        cond.dwPositiveSaturation = s > 0 ? (DWORD)s : (DWORD)DI_FFNOMINALMAX;
        cond.dwNegativeSaturation = s > 0 ? (DWORD)s : (DWORD)DI_FFNOMINALMAX;
        cond.lDeadBand = 0;
        cond.lOffset = 0;

        DIEFFECT e{};
        e.dwSize = sizeof(e);
        e.cbTypeSpecificParams = sizeof(DICONDITION);
        e.lpvTypeSpecificParams = &cond;

        return SUCCEEDED(fx->SetParameters(&e, DIEP_TYPESPECIFICPARAMS | DIEP_START));
    }

protected:
    void BuildTypeParamsDefaults(void* out, DWORD& bytes) override
    {
        auto* c = reinterpret_cast<DICONDITION*>(out);
        *c = {};
        c->dwPositiveSaturation = DI_FFNOMINALMAX;
        c->dwNegativeSaturation = DI_FFNOMINALMAX;
        bytes = sizeof(DICONDITION);
    }
};

// ---------------- Friction (Condition) ----------------
class DIFrictionEffect : public DISourceEffectBase
{
public:
    GUID EffectGuid() const override { return GUID_Friction; }

    // coeff, sat in [-10000,10000] and [0,10000] respectively
    bool Set(IDirectInputDevice8* dev, DIAxis axis, LONG coeff, LONG saturation)
    {
        IDirectInputEffect* fx = Get(dev, axis);
        if (!fx) return false;

        LONG c = Clamp10000(coeff);
        LONG s = Clamp10000(saturation);

        // For friction, coefficients represent Coulomb friction level.
        DICONDITION cond{};
        cond.lPositiveCoefficient = c;
        cond.lNegativeCoefficient = c;
        cond.dwPositiveSaturation = s > 0 ? (DWORD)s : (DWORD)DI_FFNOMINALMAX;
        cond.dwNegativeSaturation = s > 0 ? (DWORD)s : (DWORD)DI_FFNOMINALMAX;
        cond.lDeadBand = 0;
        cond.lOffset = 0;

        DIEFFECT e{};
        e.dwSize = sizeof(e);
        e.cbTypeSpecificParams = sizeof(DICONDITION);
        e.lpvTypeSpecificParams = &cond;

        return SUCCEEDED(fx->SetParameters(&e, DIEP_TYPESPECIFICPARAMS | DIEP_START));
    }

protected:
    void BuildTypeParamsDefaults(void* out, DWORD& bytes) override
    {
        auto* c = reinterpret_cast<DICONDITION*>(out);
        *c = {};
        c->dwPositiveSaturation = DI_FFNOMINALMAX;
        c->dwNegativeSaturation = DI_FFNOMINALMAX;
        bytes = sizeof(DICONDITION);
    }
};
