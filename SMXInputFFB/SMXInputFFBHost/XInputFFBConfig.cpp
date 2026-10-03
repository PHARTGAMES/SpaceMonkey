#include "XInputFFBConfig.h"
#include <ShlObj.h>
#include <Ole2.h>
#include <fstream>
#include <Rpc.h>      // UuidToStringA / UuidFromStringA
#pragma comment(lib, "Rpcrt4.lib")

// ---------- GUID string helpers ----------
bool XInputFFBConfig::GuidToStringA(const GUID& g, std::string& out)
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
bool XInputFFBConfig::StringToGuidA(const char* s, GUID& out)
{
    if (!s || !*s) return false;
    return UuidFromStringA((RPC_CSTR)s, (UUID*)&out) == RPC_S_OK;
}

// ---------- DeviceConfig (de)serialization ----------
void XInputFFBDeviceConfig::ToJson(nlohmann::json& j) const
{
    using nlohmann::json;
    json ja = json::array();
    for (int i = 0; i < (int)XINPUT_INPUT_COUNT; ++i)
    {
        for (size_t k = 0; k < Axis[i].size(); ++k)
        {
            const AxisMapping& m = Axis[i][k];
            json jm;
            jm["mappingName"] = m.mappingName;  
            jm["deviceId"] = m.deviceId;  
            jm["xInputAxis"] = i;
            jm["diAxis"] = m.diAxis;
            jm["scale"] = m.scale;
            jm["deadzone"] = m.deadzone;
            jm["curve"] = m.curve;
            jm["invert"] = m.invert ? 1 : 0;
            jm["pedal"] = m.pedal ? 1 : 0;
            jm["vehicleTypeMask"] = m.vehicleTypeMask;
            jm["ffbEffectMask"] = m.ffbEffectMask;
            ja.push_back(jm);
        }
    }

    json jb = json::array();
    for (size_t i = 0; i < Buttons.size(); ++i)
    {
        const ButtonMapping& m = Buttons[i];
        json jm;
        jm["mappingName"] = m.mappingName;
        jm["xInputButton"] = (unsigned int)m.xinputBit;
        jm["deviceId"] = m.deviceId;  
        jm["diButton"] = m.diButtonIndex;
        jm["vehicleTypeMask"] = m.vehicleTypeMask;
        jb.push_back(jm);
    }

    j = nlohmann::json::object();
    j["axisMappings"] = ja;
    j["buttonMappings"] = jb;
}

void XInputFFBDeviceConfig::FromJson(const nlohmann::json& j)
{
    Clear();

    if (j.contains("axisMappings") && j["axisMappings"].is_array())
    {
        for (const auto& jm : j["axisMappings"])
        {
            AxisMapping m;
            int xi = jm.value("xInputAxis", 0);
            m.mappingName = jm.value("mappingName", std::string());
            m.deviceId = jm.value("deviceId", std::string());
            m.diAxis = jm.value("diAxis", 0);
            m.scale = jm.value("scale", 1.0f);
            m.deadzone = jm.value("deadzone", 0.0f);
            m.curve = jm.value("curve", 1.0f);
            m.invert = jm.value("invert", 0) != 0;
            m.pedal = jm.value("pedal", 0) != 0;
            m.vehicleTypeMask = jm.value("vehicleTypeMask", 0);
            m.ffbEffectMask = jm.value("ffbEffectMask", 0);
            if (xi >= 0 && xi < XINPUT_INPUT_COUNT)
                Axis[xi].push_back(m);
        }
    }

    if (j.contains("buttonMappings") && j["buttonMappings"].is_array())
    {
        for (const auto& jm : j["buttonMappings"])
        {
            ButtonMapping m;
            m.mappingName = jm.value("mappingName", std::string());
            m.deviceId = jm.value("deviceId", std::string());
            m.diButtonIndex = jm.value("diButton", 0);
            m.xinputBit = (WORD)jm.value("xInputButton", 0u);
            m.vehicleTypeMask = (uint32_t)jm.value("vehicleTypeMask", 0u);
            Buttons.push_back(m);
        }
    }
}

// ---------- XInputFFBConfig manager ----------
XInputFFBDeviceConfig& XInputFFBConfig::GetDeviceConfig(unsigned userIndex)
{
    if (userIndex >= XUSER_MAX_COUNT) userIndex = 0;
    return Devices[userIndex];
}
const XInputFFBDeviceConfig& XInputFFBConfig::GetDeviceConfig(unsigned userIndex) const
{
    if (userIndex >= XUSER_MAX_COUNT) userIndex = 0;
    return Devices[userIndex];
}

bool XInputFFBConfig::GetConfigPath(std::wstring& outJsonPath)
{
    PWSTR appdata = NULL;
    HRESULT hr = SHGetKnownFolderPath(FOLDERID_LocalAppDataLow, KF_FLAG_DEFAULT, NULL, &appdata);
    if (FAILED(hr) || appdata == NULL) return false;

    std::wstring base(appdata);
    CoTaskMemFree(appdata);

    std::wstring dir = base + L"\\PHARTGAMES\\SMXInputFFB";
    CreateDirectoryW((base + L"\\PHARTGAMES").c_str(), NULL);
    CreateDirectoryW(dir.c_str(), NULL);

    outJsonPath = dir + L"\\XInputFFBConfig.txt";
    return true;
}

bool XInputFFBConfig::Load()
{
    std::wstring path;
    if (!GetConfigPath(path)) return false;

    std::ifstream ifs(std::string(path.begin(), path.end()), std::ios::binary);
    if (!ifs.is_open()) return false;

    std::string data((std::istreambuf_iterator<char>(ifs)), std::istreambuf_iterator<char>());
    ifs.close();

    nlohmann::json root;
    try { root = nlohmann::json::parse(data); }
    catch (...) { return false; }

    if (!root.contains("devices") || !root["devices"].is_array()) return false;

    for (unsigned i = 0; i < XUSER_MAX_COUNT; ++i) Devices[i].Clear();

    unsigned idx = 0;
    for (const auto& jd : root["devices"])
    {
        if (idx >= XUSER_MAX_COUNT) break;
        Devices[idx].FromJson(jd);
        ++idx;
    }
    return true;
}

bool XInputFFBConfig::Save() const
{
    std::wstring path;
    if (!GetConfigPath(path)) return false;

    nlohmann::json root;
    root["devices"] = nlohmann::json::array();

    for (unsigned i = 0; i < XUSER_MAX_COUNT; ++i)
    {
        nlohmann::json jd;
        Devices[i].ToJson(jd);
        root["devices"].push_back(jd);
    }

    std::ofstream ofs(std::string(path.begin(), path.end()), std::ios::binary | std::ios::trunc);
    if (!ofs.is_open()) return false;

    std::string dumped = root.dump(2);
    ofs.write(dumped.data(), (std::streamsize)dumped.size());
    ofs.close();
    return true;
}


int XInputFFBConfig::AddAxisMapping(int deviceIndex, int axisIndex)
{
    if (deviceIndex >= XUSER_MAX_COUNT ||deviceIndex < 0)
        return -1;

    if (axisIndex >= XINPUT_INPUT_COUNT || axisIndex < 0)
        return -1;

    XInputFFBDeviceConfig& deviceConfig = Devices[deviceIndex];

    std::vector<AxisMapping>* axisBucket = deviceConfig.GetAxisBucket(axisIndex);

    AxisMapping newMapping{};
    newMapping.curve = 1.0f;
    newMapping.deadzone = 0.001f;
    newMapping.deviceId = "";
    newMapping.diAxis = 0;
    newMapping.ffbEffectMask = 0;
    newMapping.invert = false;
    newMapping.mappingName = "New Mapping";
    newMapping.pedal = false;
    newMapping.scale = 1.0f;
    newMapping.vehicleTypeMask = 0;
    axisBucket->push_back(newMapping);

    return axisBucket->size() - 1;

}

void XInputFFBConfig::DeleteAxisMapping(int deviceIndex, int axisIndex, int mappingIndex)
{
    if (deviceIndex >= XUSER_MAX_COUNT || deviceIndex < 0)
        return;

    if (axisIndex >= XINPUT_INPUT_COUNT || axisIndex < 0)
        return;

    XInputFFBDeviceConfig& deviceConfig = Devices[deviceIndex];

    std::vector<AxisMapping>* axisBucket = deviceConfig.GetAxisBucket(axisIndex);

    if (mappingIndex < 0 || mappingIndex >= axisBucket->size())
        return;

    if (axisBucket)
    {
        axisBucket->erase(axisBucket->begin() + mappingIndex);
    }
}

AxisMapping* XInputFFBConfig::GetAxisMapping(int deviceIndex, int axisIndex, int mappingIndex)
{
    if (deviceIndex >= XUSER_MAX_COUNT || deviceIndex < 0)
        return nullptr;

    if (axisIndex >= XINPUT_INPUT_COUNT || axisIndex < 0)
        return nullptr;

    XInputFFBDeviceConfig& deviceConfig = Devices[deviceIndex];

    std::vector<AxisMapping>* axisBucket = deviceConfig.GetAxisBucket(axisIndex);

    if (mappingIndex >= axisBucket->size())
    {
        return nullptr;
    }

    return &(*axisBucket)[mappingIndex];

}





