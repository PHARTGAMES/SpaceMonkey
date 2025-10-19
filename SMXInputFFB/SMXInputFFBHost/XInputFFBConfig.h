#pragma once
#include <windows.h>
#include <Xinput.h>
#include <guiddef.h>
#include <vector>
#include <string>
#include <cstdint>

#include "nlohmann/json.hpp"

enum class XInputFFBVehicleType : uint32_t
{
    Car = 1u,
    Bike = 1u << 1,
    Aircraft = 1u << 2,
    Boat = 1u << 3,
    Pedestrian = 1u << 4,
    Helicopter = 1u << 5
};

inline XInputFFBVehicleType VehicleIndexToFlag(unsigned index)
{
    // Optionally guard against out-of-range values
    if (index >= 32) index = 31;
    return static_cast<XInputFFBVehicleType>(1u << index);
}

struct AxisMapping
{
    std::string mappingName; //name of the mapping
    std::string deviceId; // e.g. "{3F2504E0-4F89-11D3-9A0C-0305E82C3301}" or any string you choose
    int diAxis;           // 0..7 : X,Y,Z,RX,RY,RZ,SLIDER0,SLIDER1
    float scale;           // e.g. 1.0
    float deadzone;        // 0..1
    float curve;            //exponent
    bool invert;          // flip sign
    bool pedal;            // pedal 
    uint32_t vehicleTypeMask; //mask of vehicle types that enable this binding
};

struct ButtonMapping
{
    std::string mappingName; //name of the mapping
    std::string deviceId;   // simple string ID
    int  diButtonIndex;      // 0..127 into DIJOYSTATE2::rgbButtons
    WORD xinputBit;          // XINPUT_GAMEPAD_* bit
    uint32_t vehicleTypeMask; //mask of vehicle types that enable this binding
};


// Per-XInput user config (one virtual controller)
class XInputFFBDeviceConfig
{
public:
    enum { LX = 0, LY, RX, RY, LT, RT, XInputAxisCount };

    void Clear()
    {
        for (int i = 0; i < (int)XInputAxisCount; ++i) Axis[i].clear();
        Buttons.clear();
    }

    // Read
    std::vector<AxisMapping>* GetAxisBucket(int xinputAxis)
    {
        return (xinputAxis >= 0 && xinputAxis < (int)XInputAxisCount) ? &Axis[xinputAxis] : nullptr;
    }
    std::vector<ButtonMapping>& GetButtons() { return Buttons; }

    // Serialization
    void ToJson(nlohmann::json& j) const;
    void FromJson(const nlohmann::json& j);

private:
    std::vector<AxisMapping>   Axis[XInputAxisCount];
    std::vector<ButtonMapping> Buttons;
};

// Whole app config: up to XUSER_MAX_COUNT devices in one file
class XInputFFBConfig
{
public:
    XInputFFBConfig() = default;

    // Access
    XInputFFBDeviceConfig& GetDeviceConfig(unsigned userIndex);
    const XInputFFBDeviceConfig& GetDeviceConfig(unsigned userIndex) const;

    // Persistence (%APPDATA%\PHARTGAMES\SMXInputFFB\XInputFFBConfig.txt)
    bool Load();
    bool Save() const;

    static bool GetConfigPath(std::wstring& outJsonPath);

    // Helpers for GUID<->string when you want to use standard GUID strings as IDs
    static bool GuidToStringA(const GUID& g, std::string& out);
    static bool StringToGuidA(const char* s, GUID& out);

    int AddAxisMapping(int deviceIndex, int axisIndex);
    void DeleteAxisMapping(int deviceIndex, int axisIndex);
    AxisMapping* GetAxisMapping(int deviceIndex, int axisIndex, int mappingIndex);


private:
    XInputFFBDeviceConfig Devices[XUSER_MAX_COUNT];
};
