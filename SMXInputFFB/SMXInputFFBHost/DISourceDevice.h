#pragma once
#include "SMXInputFFBHostDefines.h"
#include <dinput.h>
#include <guiddef.h>
#include <string>
#include "DISourceEffect.h"  


enum class DIFfbType : int
{
    Constant = 0,
    Damper,
    Vibration,
    Friction
};

class SMXINPUTFFBHOST_API DISourceDevice
{
public:
    DISourceDevice(IDirectInput8* di, const DIDEVICEINSTANCE& inst);
    ~DISourceDevice();

    bool Initialize();
    void SetHWND(HWND hwnd);

    bool Acquire();
    void Unacquire();

    bool UpdateState();
    const DIJOYSTATE2& GetCachedState() const { return m_cachedState; }

    const GUID& GetInstanceGUID() const { return m_instanceGuid; }
    const GUID& GetProductGUID()  const { return m_productGuid; }
    const char* GetName() const { return m_name; }
    const std::string GetInstanceGUIDString() const { return m_instanceGuidString; }

    // Convenience setters (delegate to managers)
    bool SetConstantForce(DIAxis axis, LONG magnitude);
    bool SetDamper(DIAxis axis, LONG coeff, LONG saturation);
    bool SetVibration(DIAxis axis, LONG frequencyHz, LONG gain);
    bool SetFriction(DIAxis axis, LONG coeff, LONG saturation); 
    bool SetSpring(DIAxis axis, LONG magnitude);

    long  GetAxisValue(DIAxis axis);
    float GetAxisValueNorm(DIAxis axis);
    float GetButtonValueNorm(int button);


    // Fast arrays if you need to walk everything quickly
    IDirectInputEffect* const* ConstantArray() const { return m_constant.RawArray(); }
    IDirectInputEffect* const* DamperArray()   const { return m_damper.RawArray(); }
    IDirectInputEffect* const* VibrationArray()const { return m_vibration.RawArray(); }
    IDirectInputEffect* const* FrictionArray() const { return m_friction.RawArray(); }
    IDirectInputEffect* const* SpringArray() const { return m_spring.RawArray(); }

    void HandleFocusGain();

private:
    float NormalizeDIValue(long v);
    bool CreateDevice();
    bool SetupDataFormatAndRange();
    bool SetupCooperativeLevel();
    bool CreateAllAxisEffects();

private:
    IDirectInput8* m_di = nullptr;
    IDirectInputDevice8* m_dev = nullptr;
    GUID                    m_instanceGuid = {};
    GUID                    m_productGuid = {};
    std::string             m_instanceGuidString = {};
    char                    m_name[256];

    HWND                    m_hwnd = nullptr;
    bool                    m_ffbSupported = false;

    DIJOYSTATE2             m_cachedState{};
    bool                    m_hasState = false;

    DIConstantForceEffect   m_constant;
    DIDamperEffect          m_damper;
    DIVibrationEffect       m_vibration;
    DIFrictionEffect        m_friction; 
    DISpringEffect          m_spring;
};
