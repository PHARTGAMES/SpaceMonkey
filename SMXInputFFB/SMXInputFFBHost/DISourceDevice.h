#pragma once
#include "SMXInputFFBHostDefines.h"
#include <dinput.h>
#include <guiddef.h>
#include <string>

enum class DIAxis : int
{
    X = 0, Y, Z, RX, RY, RZ, SLIDER0, SLIDER1, COUNT
};

enum class DIFfbType : int
{
    Constant = 0,
    Damper,
    Collision
};

class SMXINPUTFFBHOST_API DISourceDevice
{
public:
    DISourceDevice(IDirectInput8* di, const DIDEVICEINSTANCE& inst);
    ~DISourceDevice();

    bool Initialize();            // create device, set format, enumerate caps/effects
    void SetHWND(HWND hwnd);      // set coop level target

    // Acquire / Unacquire
    bool Acquire();
    void Unacquire();

    // State
    bool UpdateState();           // polls device and caches DIJOYSTATE2
    const DIJOYSTATE2& GetCachedState() const { return m_cachedState; }

    // Identity
    const GUID& GetInstanceGUID() const { return m_instanceGuid; }
    const GUID& GetProductGUID()  const { return m_productGuid; }
    const char* GetName() const { return m_name; }
    const std::string GetInstanceGUIDString() const { return m_instanceGuidString; }

    // Effects API (create if needed, then retrieve)
    bool EnsureAxisEffect(DIFfbType type, DIAxis axis);
    IDirectInputEffect* GetAxisEffect(DIFfbType type, DIAxis axis);

    // Convenience setters
    bool SetConstantForce(DIAxis axis, LONG magnitude);          // 0..10000
    bool SetDamper(DIAxis axis, LONG coeff, LONG saturation);    // 0..10000 each
    bool FireCollisionPulse(DIAxis axis, LONG magnitude, DWORD durationMs);

private:
    bool CreateDevice();
    bool SetupDataFormatAndRange();
    bool SetupCooperativeLevel(); // requires HWND
    bool CreateAllAxisEffects();  // optional eager creation

    // helpers
    static DWORD AxisToOffset(DIAxis a);
    static LONG  Clamp10000(LONG v);

private:
    IDirectInput8* m_di = nullptr;
    IDirectInputDevice8* m_dev = nullptr;
    GUID                  m_instanceGuid = {};
    GUID                  m_productGuid = {};
    std::string           m_instanceGuidString = {};
    char                  m_name[256];

    HWND                  m_hwnd = nullptr;
    bool                  m_ffbSupported = false;

    // cached state
    DIJOYSTATE2           m_cachedState;
    bool                  m_hasState = false;

    // per-axis effects (Constant, Damper, Collision)
    IDirectInputEffect* m_constant[(int)DIAxis::COUNT];
    IDirectInputEffect* m_damper[(int)DIAxis::COUNT];
    IDirectInputEffect* m_collision[(int)DIAxis::COUNT];
};
