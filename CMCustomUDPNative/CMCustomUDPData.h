#pragma once
#include "CMCustomUDPDefines.h"
#include <cstdint>
#include <cstring>
#include <cmath>
#include <string>
#include <unordered_map>
#include <vector>
#include <array>
#include <stdexcept>
#include <type_traits>
#include "tinyxml2.h"

struct FourCC { char data[4]; };

class CMCUSTOMUDPNATIVE_API CMCustomUDPData
{
public:

    enum class VehicleType : uint32_t
    {
        Car,
        Bike,
        Aircraft,
        Boat,
        Pedestrian, 
        Helicopter
    };

    enum class DataKey : int
    {
        total_time,

        paused,
        vehicle_type,
        yaw,
        pitch,
        roll,
        yaw_velocity,
        pitch_velocity,
        roll_velocity,
        yaw_acceleration,
        pitch_acceleration,
        roll_acceleration,
        position_x,
        position_y,
        position_z,
        local_velocity_x,
        local_velocity_y,
        local_velocity_z,
        gforce_lateral,
        gforce_longitudinal,
        gforce_vertical,
        speed,
        suspension_position_bl,
        suspension_position_br,
        suspension_position_fl,
        suspension_position_fr,
        suspension_velocity_bl,
        suspension_velocity_br,
        suspension_velocity_fl,
        suspension_velocity_fr,
        suspension_acceleration_bl,
        suspension_acceleration_br,
        suspension_acceleration_fl,
        suspension_acceleration_fr,
        wheel_patch_speed_bl,
        wheel_patch_speed_br,
        wheel_patch_speed_fl,
        wheel_patch_speed_fr,
        throttle_input,
        steering_input,
        brake_input,
        clutch_input,
        gear,
        max_gears,
        engine_rate,
        race_position,
        race_sector,
        sector_time_1,
        sector_time_2,
        brake_temp_bl,
        brake_temp_br,
        brake_temp_fl,
        brake_temp_fr,
        tyre_pressure_bl,
        tyre_pressure_br,
        tyre_pressure_fl,
        tyre_pressure_fr,
        lap,
        laps_completed,
        total_laps,
        lap_time,
        lap_distance,
        track_length,
        last_lap_time,
        max_rpm,
        idle_rpm,

        // added for codemasters extradata=3
        total_distance,
        world_velocity_x,
        world_velocity_y,
        world_velocity_z,
        world_dir_rht_x,
        world_dir_rht_y,
        world_dir_rht_z,
        world_dir_fwd_x,
        world_dir_fwd_y,
        world_dir_fwd_z,
        sli_pro_native_support,
        kers_level,
        kers_max,
        drs,
        traction_control,
        anti_lock_brakes,
        fuel_in_tank,
        fuel_capacity,
        in_pits,
        team_info,
        session_type,
        drs_allowed,
        track_number,
        vehicle_fia_flags,
        engine_rate_div10,
        max_rpm_div10,
        idle_rpm_div10,
        slip_angle,
        slip_angle2,
        ffb_wheel_steer_constant,
        ffb_wheel_steer_damper,
        ffb_wheel_steer_vibration_gain,
        ffb_wheel_steer_vibration_freq,

        Max
    };

    // Public fields (kept to match C# surface API; numeric fields stored as float)
    float total_time;
    float vehicle_type;
    float paused;
    float yaw;
    float pitch;
    float roll;
    float yaw_velocity;
    float pitch_velocity;
    float roll_velocity;
    float yaw_acceleration;
    float pitch_acceleration;
    float roll_acceleration;
    float position_x;
    float position_y;
    float position_z;
    float local_velocity_x;
    float local_velocity_y;
    float local_velocity_z;
    float gforce_lateral;
    float gforce_longitudinal;
    float gforce_vertical;
    float speed;
    float suspension_position_bl;
    float suspension_position_br;
    float suspension_position_fl;
    float suspension_position_fr;
    float suspension_velocity_bl;
    float suspension_velocity_br;
    float suspension_velocity_fl;
    float suspension_velocity_fr;
    float suspension_acceleration_bl;
    float suspension_acceleration_br;
    float suspension_acceleration_fl;
    float suspension_acceleration_fr;
    float wheel_patch_speed_bl;
    float wheel_patch_speed_br;
    float wheel_patch_speed_fl;
    float wheel_patch_speed_fr;
    float throttle_input;
    float steering_input;
    float brake_input;
    float clutch_input;
    float gear;
    float max_gears;
    float engine_rate;
    float race_position;
    float race_sector;
    float sector_time_1;
    float sector_time_2;
    float brake_temp_bl;
    float brake_temp_br;
    float brake_temp_fl;
    float brake_temp_fr;
    float tyre_pressure_bl;
    float tyre_pressure_br;
    float tyre_pressure_fl;
    float tyre_pressure_fr;
    float lap;
    float laps_completed;
    float total_laps;
    float lap_time;
    float lap_distance;
    float track_length;
    float last_lap_time;
    float max_rpm;
    float idle_rpm;

    // added for codemasters extradata=3
    float total_distance;
    float world_velocity_x;
    float world_velocity_y;
    float world_velocity_z;
    float world_dir_rht_x;
    float world_dir_rht_y;
    float world_dir_rht_z;
    float world_dir_fwd_x;
    float world_dir_fwd_y;
    float world_dir_fwd_z;
    float sli_pro_native_support;
    float kers_level;
    float kers_max;
    float drs;
    float traction_control;
    float anti_lock_brakes;
    float fuel_in_tank;
    float fuel_capacity;
    float in_pits;
    float team_info;
    float session_type;
    float drs_allowed;
    float track_number;
    float vehicle_fia_flags;
    float engine_rate_div10;
    float max_rpm_div10;
    float idle_rpm_div10;
    float slip_angle;
    float slip_angle2;
    float ffb_wheel_steer_constant;
    float ffb_wheel_steer_damper;
    float ffb_wheel_steer_vibration_gain;
    float ffb_wheel_steer_vibration_freq;

    std::string formatFilename = "PacketFormats\\defaultPacketFormat.xml";

    CMCustomUDPData()
    {
        zeroAll();
    }

    void Init(std::string customFormatFilename)
    {
        if (!customFormatFilename.empty())
            formatFilename = customFormatFilename;

        tinyxml2::XMLDocument doc;
        auto loadRes = doc.LoadFile(formatFilename.c_str());
        if (loadRes != tinyxml2::XML_SUCCESS)
            throw std::runtime_error("Failed to load XML: " + formatFilename);

        const tinyxml2::XMLElement* root = doc.FirstChildElement("custom_udp");
        if (!root) throw std::runtime_error("Missing <custom_udp> root");

        channels.clear();
        size_t offset = 0;

        for (auto* ch = root->FirstChildElement(); ch; ch = ch->NextSiblingElement())
        {
            // skip comments implicitly (FirstChildElement only yields elements)
            const char* typeName = ch->Name(); // "uint32" / "int32" / "float" / "fourcc"
            const char* channel = ch->Attribute("channel");
            const char* scaleAttr = ch->Attribute("scale");
            if (!typeName || !channel || !scaleAttr)
                continue;

            float scale = static_cast<float>(std::atof(scaleAttr));

            ValueType vtype = parseValueType(typeName);
            DataKey key = parseDataKey(channel);

            CMChannelMap map;
            map.type = vtype;
            map.channel = channel;
            map.offsetInPacket = static_cast<int>(offset);
            map.scale = scale;
            map.dataKey = key;
            map.member = memberPtrFor(key); // float pointer-to-member for numeric keys

            channels.emplace(key, map);
            offset += 4; // each channel occupies 4 bytes
        }

        packet.resize(offset);
    }

    // Serialize all mapped channels to tightly packed 4-byte values
    const std::vector<uint8_t>& GetBytes()
    {
        for (const auto& kv : channels)
        {
            const CMChannelMap& m = kv.second;
            uint8_t* dst = packet.data() + m.offsetInPacket;

            switch (m.type)
            {
            case ValueType::Float:
            {
                float v = readFloat(m);
                std::memcpy(dst, &v, 4);
                break;
            }
            case ValueType::UInt32:
            {
                uint32_t v = static_cast<uint32_t>(readFloat(m));
                std::memcpy(dst, &v, 4);
                break;
            }
            case ValueType::Int32:
            {
                int32_t v = static_cast<int32_t>(readFloat(m));
                std::memcpy(dst, &v, 4);
                break;
            }
            case ValueType::FourCC:
            {
                FourCC v = readFourCC(m.dataKey);
                std::memcpy(dst, v.data, 4);
                break;
            }
            }
        }
        return packet;
    }

    // Deserialize from packed bytes into mapped fields
    void FromBytes(const uint8_t* bytes, size_t size)
    {
        if (size < packet.size())
            throw std::runtime_error("FromBytes: input too small");

        for (const auto& kv : channels)
        {
            const CMChannelMap& m = kv.second;
            const uint8_t* src = bytes + m.offsetInPacket;

            switch (m.type)
            {
            case ValueType::Float:
            {
                float v;
                std::memcpy(&v, src, 4);
                writeFloat(m, v);
                break;
            }
            case ValueType::UInt32:
            {
                uint32_t v;
                std::memcpy(&v, src, 4);
                writeFloat(m, static_cast<float>(v));
                break;
            }
            case ValueType::Int32:
            {
                int32_t v;
                std::memcpy(&v, src, 4);
                writeFloat(m, static_cast<float>(v));
                break;
            }
            case ValueType::FourCC:
            {
                FourCC v{};
                std::memcpy(v.data, src, 4);
                fourccValues[m.dataKey] = v;
                break;
            }
            }
        }
    }

    // Convenience overload
    void FromBytes(const std::vector<uint8_t>& bytes)
    {
        FromBytes(bytes.data(), bytes.size());
    }

    // Get/Set by key (numeric)
    float GetValueF(DataKey key) const
    {
        auto it = channels.find(key);
        if (it == channels.end()) throw std::out_of_range("key not mapped");
        return readFloat(it->second);
    }

    void SetValueF(DataKey key, float value)
    {
        auto it = channels.find(key);
        if (it == channels.end()) throw std::out_of_range("key not mapped");
        writeFloat(it->second, value);
    }

    // Get/Set FourCC
    FourCC GetValue4CC(DataKey key) const
    {
        auto it = fourccValues.find(key);
        if (it == fourccValues.end()) return FourCC{ {0,0,0,0} };
        return it->second;
    }

    void SetValue4CC(DataKey key, const FourCC& v)
    {
        fourccValues[key] = v;
    }

    bool IsFloat(DataKey key) const
    {
        auto it = channels.find(key);
        return it != channels.end() && it->second.type == ValueType::Float;
    }

    bool IsValid(DataKey key) const
    {
        return channels.find(key) != channels.end();
    }

    // Copy all public fields; if copyChannels is true, copy channel map too
    void Copy(const CMCustomUDPData& other, bool copyChannels = true)
    {
        std::memcpy(this, &other, sizeofPublicNumeric());
        fourccValues = other.fourccValues;
        if (copyChannels) channels = other.channels;
        packet = other.packet;
        formatFilename = other.formatFilename;
    }

    static int GetKeyMask(std::initializer_list<DataKey> list)
    {
        int rval = 0;
        for (auto k : list) rval |= (1 << static_cast<int>(k));
        return rval;
    }

    int GetSize() const { return static_cast<int>(packet.size()); }

    // Degree helpers
    float pitchDeg() const { return pitch * (180.0f / 3.14159265358979323846f); }
    float yawDeg()   const { return yaw * (180.0f / 3.14159265358979323846f); }
    float rollDeg()  const { return roll * (180.0f / 3.14159265358979323846f); }

    // Lerp helpers
    void LerpAll(float t)
    {
        // Only the fields present in the original LerpAll are blended:
        yaw = Lerp(0.0f, yaw, t);
        pitch = Lerp(0.0f, pitch, t);
        roll = Lerp(0.0f, roll, t);
        yaw_velocity = Lerp(0.0f, yaw_velocity, t);
        pitch_velocity = Lerp(0.0f, pitch_velocity, t);
        roll_velocity = Lerp(0.0f, roll_velocity, t);
        yaw_acceleration = Lerp(0.0f, yaw_acceleration, t);
        pitch_acceleration = Lerp(0.0f, pitch_acceleration, t);
        roll_acceleration = Lerp(0.0f, roll_acceleration, t);
        local_velocity_x = Lerp(0.0f, local_velocity_x, t);
        local_velocity_y = Lerp(0.0f, local_velocity_y, t);
        local_velocity_z = Lerp(0.0f, local_velocity_z, t);
        gforce_lateral = Lerp(0.0f, gforce_lateral, t);
        gforce_longitudinal = Lerp(0.0f, gforce_longitudinal, t);
        gforce_vertical = Lerp(0.0f, gforce_vertical, t);
        suspension_position_bl = Lerp(0.0f, suspension_position_bl, t);
        suspension_position_br = Lerp(0.0f, suspension_position_br, t);
        suspension_position_fl = Lerp(0.0f, suspension_position_fl, t);
        suspension_position_fr = Lerp(0.0f, suspension_position_fr, t);
        suspension_velocity_bl = Lerp(0.0f, suspension_velocity_bl, t);
        suspension_velocity_br = Lerp(0.0f, suspension_velocity_br, t);
        suspension_velocity_fl = Lerp(0.0f, suspension_velocity_fl, t);
        suspension_velocity_fr = Lerp(0.0f, suspension_velocity_fr, t);
        suspension_acceleration_bl = Lerp(0.0f, suspension_acceleration_bl, t);
        suspension_acceleration_br = Lerp(0.0f, suspension_acceleration_br, t);
        suspension_acceleration_fl = Lerp(0.0f, suspension_acceleration_fl, t);
        suspension_acceleration_fr = Lerp(0.0f, suspension_acceleration_fr, t);
        wheel_patch_speed_bl = Lerp(0.0f, wheel_patch_speed_bl, t);
        wheel_patch_speed_br = Lerp(0.0f, wheel_patch_speed_br, t);
        wheel_patch_speed_fl = Lerp(0.0f, wheel_patch_speed_fl, t);
        wheel_patch_speed_fr = Lerp(0.0f, wheel_patch_speed_fr, t);
        throttle_input = Lerp(0.0f, throttle_input, t);
        steering_input = Lerp(0.0f, steering_input, t);
        brake_input = Lerp(0.0f, brake_input, t);
        clutch_input = Lerp(0.0f, clutch_input, t);
        tyre_pressure_bl = Lerp(0.0f, tyre_pressure_bl, t);
        tyre_pressure_br = Lerp(0.0f, tyre_pressure_br, t);
        tyre_pressure_fl = Lerp(0.0f, tyre_pressure_fl, t);
        tyre_pressure_fr = Lerp(0.0f, tyre_pressure_fr, t);

        total_distance = Lerp(0.0f, total_distance, t);
        world_velocity_x = Lerp(0.0f, world_velocity_x, t);
        world_velocity_y = Lerp(0.0f, world_velocity_y, t);
        world_velocity_z = Lerp(0.0f, world_velocity_z, t);
        world_dir_rht_x = Lerp(0.0f, world_dir_rht_x, t);
        world_dir_rht_y = Lerp(0.0f, world_dir_rht_y, t);
        world_dir_rht_z = Lerp(0.0f, world_dir_rht_z, t);
        world_dir_fwd_x = Lerp(0.0f, world_dir_fwd_x, t);
        world_dir_fwd_y = Lerp(0.0f, world_dir_fwd_y, t);
        world_dir_fwd_z = Lerp(0.0f, world_dir_fwd_z, t);
        sli_pro_native_support = Lerp(0.0f, sli_pro_native_support, t);
        kers_level = Lerp(0.0f, kers_level, t);
        kers_max = Lerp(0.0f, kers_max, t);
        drs = Lerp(0.0f, drs, t);
        traction_control = Lerp(0.0f, traction_control, t);
        anti_lock_brakes = Lerp(0.0f, anti_lock_brakes, t);
        fuel_in_tank = Lerp(0.0f, fuel_in_tank, t);
        fuel_capacity = Lerp(0.0f, fuel_capacity, t);
        in_pits = Lerp(0.0f, in_pits, t);
        team_info = Lerp(0.0f, team_info, t);
        session_type = Lerp(0.0f, session_type, t);
        drs_allowed = Lerp(0.0f, drs_allowed, t);
        track_number = Lerp(0.0f, track_number, t);
        vehicle_fia_flags = Lerp(0.0f, vehicle_fia_flags, t);
        engine_rate_div10 = Lerp(0.0f, engine_rate_div10, t);
        max_rpm_div10 = Lerp(0.0f, max_rpm_div10, t);
        idle_rpm_div10 = Lerp(0.0f, idle_rpm_div10, t);
        slip_angle = Lerp(0.0f, slip_angle, t);
        slip_angle2 = Lerp(0.0f, slip_angle2, t);
        ffb_wheel_steer_constant = Lerp(0.0f, ffb_wheel_steer_constant, t);
        ffb_wheel_steer_damper = Lerp(0.0f, ffb_wheel_steer_damper, t);
        ffb_wheel_steer_vibration_gain = Lerp(0.0f, ffb_wheel_steer_vibration_gain, t);
        ffb_wheel_steer_vibration_freq = Lerp(0.0f, ffb_wheel_steer_vibration_freq, t);
    }

    void LerpAllFrom(const CMCustomUDPData& from, float t)
    {
        auto L = [t](float a, float b) { return a + (b - a) * t; };

        yaw = L(from.yaw, yaw);
        pitch = L(from.pitch, pitch);
        roll = L(from.roll, roll);
        yaw_velocity = L(from.yaw_velocity, yaw_velocity);
        pitch_velocity = L(from.pitch_velocity, pitch_velocity);
        roll_velocity = L(from.roll_velocity, roll_velocity);
        yaw_acceleration = L(from.yaw_acceleration, yaw_acceleration);
        pitch_acceleration = L(from.pitch_acceleration, pitch_acceleration);
        roll_acceleration = L(from.roll_acceleration, roll_acceleration);
        local_velocity_x = L(from.local_velocity_x, local_velocity_x);
        local_velocity_y = L(from.local_velocity_y, local_velocity_y);
        local_velocity_z = L(from.local_velocity_z, local_velocity_z);
        gforce_lateral = L(from.gforce_lateral, gforce_lateral);
        gforce_longitudinal = L(from.gforce_longitudinal, gforce_longitudinal);
        gforce_vertical = L(from.gforce_vertical, gforce_vertical);
        suspension_position_bl = L(from.suspension_position_bl, suspension_position_bl);
        suspension_position_br = L(from.suspension_position_br, suspension_position_br);
        suspension_position_fl = L(from.suspension_position_fl, suspension_position_fl);
        suspension_position_fr = L(from.suspension_position_fr, suspension_position_fr);
        suspension_velocity_bl = L(from.suspension_velocity_bl, suspension_velocity_bl);
        suspension_velocity_br = L(from.suspension_velocity_br, suspension_velocity_br);
        suspension_velocity_fl = L(from.suspension_velocity_fl, suspension_velocity_fl);
        suspension_velocity_fr = L(from.suspension_velocity_fr, suspension_velocity_fr);
        suspension_acceleration_bl = L(from.suspension_acceleration_bl, suspension_acceleration_bl);
        suspension_acceleration_br = L(from.suspension_acceleration_br, suspension_acceleration_br);
        suspension_acceleration_fl = L(from.suspension_acceleration_fl, suspension_acceleration_fl);
        suspension_acceleration_fr = L(from.suspension_acceleration_fr, suspension_acceleration_fr);
        wheel_patch_speed_bl = L(from.wheel_patch_speed_bl, wheel_patch_speed_bl);
        wheel_patch_speed_br = L(from.wheel_patch_speed_br, wheel_patch_speed_br);
        wheel_patch_speed_fl = L(from.wheel_patch_speed_fl, wheel_patch_speed_fl);
        wheel_patch_speed_fr = L(from.wheel_patch_speed_fr, wheel_patch_speed_fr);
        throttle_input = L(from.throttle_input, throttle_input);
        steering_input = L(from.steering_input, steering_input);
        brake_input = L(from.brake_input, brake_input);
        clutch_input = L(from.clutch_input, clutch_input);
        tyre_pressure_bl = L(from.tyre_pressure_bl, tyre_pressure_bl);
        tyre_pressure_br = L(from.tyre_pressure_br, tyre_pressure_br);
        tyre_pressure_fl = L(from.tyre_pressure_fl, tyre_pressure_fl);
        tyre_pressure_fr = L(from.tyre_pressure_fr, tyre_pressure_fr);

        total_distance = L(from.total_distance, total_distance);
        world_velocity_x = L(from.world_velocity_x, world_velocity_x);
        world_velocity_y = L(from.world_velocity_y, world_velocity_y);
        world_velocity_z = L(from.world_velocity_z, world_velocity_z);
        world_dir_rht_x = L(from.world_dir_rht_x, world_dir_rht_x);
        world_dir_rht_y = L(from.world_dir_rht_y, world_dir_rht_y);
        world_dir_rht_z = L(from.world_dir_rht_z, world_dir_rht_z);
        world_dir_fwd_x = L(from.world_dir_fwd_x, world_dir_fwd_x);
        world_dir_fwd_y = L(from.world_dir_fwd_y, world_dir_fwd_y);
        world_dir_fwd_z = L(from.world_dir_fwd_z, world_dir_fwd_z);
        sli_pro_native_support = L(from.sli_pro_native_support, sli_pro_native_support);
        kers_level = L(from.kers_level, kers_level);
        kers_max = L(from.kers_max, kers_max);
        drs = L(from.drs, drs);
        traction_control = L(from.traction_control, traction_control);
        anti_lock_brakes = L(from.anti_lock_brakes, anti_lock_brakes);
        fuel_in_tank = L(from.fuel_in_tank, fuel_in_tank);
        fuel_capacity = L(from.fuel_capacity, fuel_capacity);
        in_pits = L(from.in_pits, in_pits);
        team_info = L(from.team_info, team_info);
        session_type = L(from.session_type, session_type);
        drs_allowed = L(from.drs_allowed, drs_allowed);
        track_number = L(from.track_number, track_number);
        vehicle_fia_flags = L(from.vehicle_fia_flags, vehicle_fia_flags);
        engine_rate_div10 = L(from.engine_rate_div10, engine_rate_div10);
        max_rpm_div10 = L(from.max_rpm_div10, max_rpm_div10);
        idle_rpm_div10 = L(from.idle_rpm_div10, idle_rpm_div10);
        slip_angle = L(from.slip_angle, slip_angle);
        slip_angle2 = L(from.slip_angle2, slip_angle2);
        ffb_wheel_steer_constant = L(from.ffb_wheel_steer_constant, ffb_wheel_steer_constant);
        ffb_wheel_steer_damper = L(from.ffb_wheel_steer_damper, ffb_wheel_steer_damper);
        ffb_wheel_steer_vibration_gain = L(from.ffb_wheel_steer_vibration_gain, ffb_wheel_steer_vibration_gain);
        ffb_wheel_steer_vibration_freq = L(from.ffb_wheel_steer_vibration_freq, ffb_wheel_steer_vibration_freq);
    }

private:
    enum class ValueType { Float, UInt32, Int32, FourCC };

    struct CMChannelMap
    {
        ValueType type = ValueType::Float;
        std::string channel;
        int offsetInPacket = -1;
        float scale = 1.0f;
        DataKey dataKey = DataKey::total_time;
        // numeric members stored as float pointer-to-member for speed
        float CMCustomUDPData::* member = nullptr;
    };

    std::unordered_map<DataKey, CMChannelMap> channels;
    std::unordered_map<DataKey, FourCC> fourccValues;
    std::vector<uint8_t> packet;

    // Member pointer directory for fast lookup (numeric)
    static float CMCustomUDPData::* memberPtrFor(DataKey k)
    {
        using DK = DataKey;
        switch (k)
        {
        case DK::total_time: return &CMCustomUDPData::total_time;
        case DK::paused: return &CMCustomUDPData::paused;
        case DK::vehicle_type: return &CMCustomUDPData::vehicle_type;
        case DK::yaw: return &CMCustomUDPData::yaw;
        case DK::pitch: return &CMCustomUDPData::pitch;
        case DK::roll: return &CMCustomUDPData::roll;
        case DK::yaw_velocity: return &CMCustomUDPData::yaw_velocity;
        case DK::pitch_velocity: return &CMCustomUDPData::pitch_velocity;
        case DK::roll_velocity: return &CMCustomUDPData::roll_velocity;
        case DK::yaw_acceleration: return &CMCustomUDPData::yaw_acceleration;
        case DK::pitch_acceleration: return &CMCustomUDPData::pitch_acceleration;
        case DK::roll_acceleration: return &CMCustomUDPData::roll_acceleration;
        case DK::position_x: return &CMCustomUDPData::position_x;
        case DK::position_y: return &CMCustomUDPData::position_y;
        case DK::position_z: return &CMCustomUDPData::position_z;
        case DK::local_velocity_x: return &CMCustomUDPData::local_velocity_x;
        case DK::local_velocity_y: return &CMCustomUDPData::local_velocity_y;
        case DK::local_velocity_z: return &CMCustomUDPData::local_velocity_z;
        case DK::gforce_lateral: return &CMCustomUDPData::gforce_lateral;
        case DK::gforce_longitudinal: return &CMCustomUDPData::gforce_longitudinal;
        case DK::gforce_vertical: return &CMCustomUDPData::gforce_vertical;
        case DK::speed: return &CMCustomUDPData::speed;
        case DK::suspension_position_bl: return &CMCustomUDPData::suspension_position_bl;
        case DK::suspension_position_br: return &CMCustomUDPData::suspension_position_br;
        case DK::suspension_position_fl: return &CMCustomUDPData::suspension_position_fl;
        case DK::suspension_position_fr: return &CMCustomUDPData::suspension_position_fr;
        case DK::suspension_velocity_bl: return &CMCustomUDPData::suspension_velocity_bl;
        case DK::suspension_velocity_br: return &CMCustomUDPData::suspension_velocity_br;
        case DK::suspension_velocity_fl: return &CMCustomUDPData::suspension_velocity_fl;
        case DK::suspension_velocity_fr: return &CMCustomUDPData::suspension_velocity_fr;
        case DK::suspension_acceleration_bl: return &CMCustomUDPData::suspension_acceleration_bl;
        case DK::suspension_acceleration_br: return &CMCustomUDPData::suspension_acceleration_br;
        case DK::suspension_acceleration_fl: return &CMCustomUDPData::suspension_acceleration_fl;
        case DK::suspension_acceleration_fr: return &CMCustomUDPData::suspension_acceleration_fr;
        case DK::wheel_patch_speed_bl: return &CMCustomUDPData::wheel_patch_speed_bl;
        case DK::wheel_patch_speed_br: return &CMCustomUDPData::wheel_patch_speed_br;
        case DK::wheel_patch_speed_fl: return &CMCustomUDPData::wheel_patch_speed_fl;
        case DK::wheel_patch_speed_fr: return &CMCustomUDPData::wheel_patch_speed_fr;
        case DK::throttle_input: return &CMCustomUDPData::throttle_input;
        case DK::steering_input: return &CMCustomUDPData::steering_input;
        case DK::brake_input: return &CMCustomUDPData::brake_input;
        case DK::clutch_input: return &CMCustomUDPData::clutch_input;
        case DK::gear: return &CMCustomUDPData::gear;
        case DK::max_gears: return &CMCustomUDPData::max_gears;
        case DK::engine_rate: return &CMCustomUDPData::engine_rate;
        case DK::race_position: return &CMCustomUDPData::race_position;
        case DK::race_sector: return &CMCustomUDPData::race_sector;
        case DK::sector_time_1: return &CMCustomUDPData::sector_time_1;
        case DK::sector_time_2: return &CMCustomUDPData::sector_time_2;
        case DK::brake_temp_bl: return &CMCustomUDPData::brake_temp_bl;
        case DK::brake_temp_br: return &CMCustomUDPData::brake_temp_br;
        case DK::brake_temp_fl: return &CMCustomUDPData::brake_temp_fl;
        case DK::brake_temp_fr: return &CMCustomUDPData::brake_temp_fr;
        case DK::tyre_pressure_bl: return &CMCustomUDPData::tyre_pressure_bl;
        case DK::tyre_pressure_br: return &CMCustomUDPData::tyre_pressure_br;
        case DK::tyre_pressure_fl: return &CMCustomUDPData::tyre_pressure_fl;
        case DK::tyre_pressure_fr: return &CMCustomUDPData::tyre_pressure_fr;
        case DK::lap: return &CMCustomUDPData::lap;
        case DK::laps_completed: return &CMCustomUDPData::laps_completed;
        case DK::total_laps: return &CMCustomUDPData::total_laps;
        case DK::lap_time: return &CMCustomUDPData::lap_time;
        case DK::lap_distance: return &CMCustomUDPData::lap_distance;
        case DK::track_length: return &CMCustomUDPData::track_length;
        case DK::last_lap_time: return &CMCustomUDPData::last_lap_time;
        case DK::max_rpm: return &CMCustomUDPData::max_rpm;
        case DK::idle_rpm: return &CMCustomUDPData::idle_rpm;

        case DK::total_distance: return &CMCustomUDPData::total_distance;
        case DK::world_velocity_x: return &CMCustomUDPData::world_velocity_x;
        case DK::world_velocity_y: return &CMCustomUDPData::world_velocity_y;
        case DK::world_velocity_z: return &CMCustomUDPData::world_velocity_z;
        case DK::world_dir_rht_x: return &CMCustomUDPData::world_dir_rht_x;
        case DK::world_dir_rht_y: return &CMCustomUDPData::world_dir_rht_y;
        case DK::world_dir_rht_z: return &CMCustomUDPData::world_dir_rht_z;
        case DK::world_dir_fwd_x: return &CMCustomUDPData::world_dir_fwd_x;
        case DK::world_dir_fwd_y: return &CMCustomUDPData::world_dir_fwd_y;
        case DK::world_dir_fwd_z: return &CMCustomUDPData::world_dir_fwd_z;
        case DK::sli_pro_native_support: return &CMCustomUDPData::sli_pro_native_support;
        case DK::kers_level: return &CMCustomUDPData::kers_level;
        case DK::kers_max: return &CMCustomUDPData::kers_max;
        case DK::drs: return &CMCustomUDPData::drs;
        case DK::traction_control: return &CMCustomUDPData::traction_control;
        case DK::anti_lock_brakes: return &CMCustomUDPData::anti_lock_brakes;
        case DK::fuel_in_tank: return &CMCustomUDPData::fuel_in_tank;
        case DK::fuel_capacity: return &CMCustomUDPData::fuel_capacity;
        case DK::in_pits: return &CMCustomUDPData::in_pits;
        case DK::team_info: return &CMCustomUDPData::team_info;
        case DK::session_type: return &CMCustomUDPData::session_type;
        case DK::drs_allowed: return &CMCustomUDPData::drs_allowed;
        case DK::track_number: return &CMCustomUDPData::track_number;
        case DK::vehicle_fia_flags: return &CMCustomUDPData::vehicle_fia_flags;
        case DK::engine_rate_div10: return &CMCustomUDPData::engine_rate_div10;
        case DK::max_rpm_div10: return &CMCustomUDPData::max_rpm_div10;
        case DK::idle_rpm_div10: return &CMCustomUDPData::idle_rpm_div10;
        case DK::slip_angle: return &CMCustomUDPData::slip_angle;
        case DK::slip_angle2: return &CMCustomUDPData::slip_angle2;
        case DK::ffb_wheel_steer_constant: return &CMCustomUDPData::ffb_wheel_steer_constant;
        case DK::ffb_wheel_steer_damper: return &CMCustomUDPData::ffb_wheel_steer_damper;
        case DK::ffb_wheel_steer_vibration_gain: return &CMCustomUDPData::ffb_wheel_steer_vibration_gain;
        case DK::ffb_wheel_steer_vibration_freq: return &CMCustomUDPData::ffb_wheel_steer_vibration_freq;

        case DK::Max: default: return nullptr;
        }
    }

    static ValueType parseValueType(const std::string& s)
    {
        if (ieq(s, "float"))  return ValueType::Float;
        if (ieq(s, "uint32")) return ValueType::UInt32;
        if (ieq(s, "int32"))  return ValueType::Int32;
        if (ieq(s, "fourcc")) return ValueType::FourCC;
        throw std::runtime_error("Unknown type: " + s);
    }

    static bool ieq(const std::string& a, const std::string& b)
    {
        if (a.size() != b.size()) return false;
        for (size_t i = 0; i < a.size(); ++i)
        {
            char ca = a[i], cb = b[i];
            if (ca >= 'A' && ca <= 'Z') ca = static_cast<char>(ca - 'A' + 'a');
            if (cb >= 'A' && cb <= 'Z') cb = static_cast<char>(cb - 'A' + 'a');
            if (ca != cb) return false;
        }
        return true;
    }

    static DataKey parseDataKey(const std::string& name)
    {
        static const std::unordered_map<std::string, DataKey> map = buildNameToKey();
        auto it = map.find(name);
        if (it == map.end()) throw std::runtime_error("Unknown DataKey: " + name);
        return it->second;
    }

    static std::unordered_map<std::string, DataKey> buildNameToKey()
    {
        std::unordered_map<std::string, DataKey> m;
        auto add = [&m](const char* n, DataKey k) { m.emplace(n, k); };

        // fill all names exactly as enum identifiers
#define ADD(name) add(#name, DataKey::name)
        ADD(total_time);
        ADD(paused);
        ADD(vehicle_type);
        ADD(yaw);
        ADD(pitch);
        ADD(roll);
        ADD(yaw_velocity);
        ADD(pitch_velocity);
        ADD(roll_velocity);
        ADD(yaw_acceleration);
        ADD(pitch_acceleration);
        ADD(roll_acceleration);
        ADD(position_x);
        ADD(position_y);
        ADD(position_z);
        ADD(local_velocity_x);
        ADD(local_velocity_y);
        ADD(local_velocity_z);
        ADD(gforce_lateral);
        ADD(gforce_longitudinal);
        ADD(gforce_vertical);
        ADD(speed);
        ADD(suspension_position_bl);
        ADD(suspension_position_br);
        ADD(suspension_position_fl);
        ADD(suspension_position_fr);
        ADD(suspension_velocity_bl);
        ADD(suspension_velocity_br);
        ADD(suspension_velocity_fl);
        ADD(suspension_velocity_fr);
        ADD(suspension_acceleration_bl);
        ADD(suspension_acceleration_br);
        ADD(suspension_acceleration_fl);
        ADD(suspension_acceleration_fr);
        ADD(wheel_patch_speed_bl);
        ADD(wheel_patch_speed_br);
        ADD(wheel_patch_speed_fl);
        ADD(wheel_patch_speed_fr);
        ADD(throttle_input);
        ADD(steering_input);
        ADD(brake_input);
        ADD(clutch_input);
        ADD(gear);
        ADD(max_gears);
        ADD(engine_rate);
        ADD(race_position);
        ADD(race_sector);
        ADD(sector_time_1);
        ADD(sector_time_2);
        ADD(brake_temp_bl);
        ADD(brake_temp_br);
        ADD(brake_temp_fl);
        ADD(brake_temp_fr);
        ADD(tyre_pressure_bl);
        ADD(tyre_pressure_br);
        ADD(tyre_pressure_fl);
        ADD(tyre_pressure_fr);
        ADD(lap);
        ADD(laps_completed);
        ADD(total_laps);
        ADD(lap_time);
        ADD(lap_distance);
        ADD(track_length);
        ADD(last_lap_time);
        ADD(max_rpm);
        ADD(idle_rpm);
        ADD(total_distance);
        ADD(world_velocity_x);
        ADD(world_velocity_y);
        ADD(world_velocity_z);
        ADD(world_dir_rht_x);
        ADD(world_dir_rht_y);
        ADD(world_dir_rht_z);
        ADD(world_dir_fwd_x);
        ADD(world_dir_fwd_y);
        ADD(world_dir_fwd_z);
        ADD(sli_pro_native_support);
        ADD(kers_level);
        ADD(kers_max);
        ADD(drs);
        ADD(traction_control);
        ADD(anti_lock_brakes);
        ADD(fuel_in_tank);
        ADD(fuel_capacity);
        ADD(in_pits);
        ADD(team_info);
        ADD(session_type);
        ADD(drs_allowed);
        ADD(track_number);
        ADD(vehicle_fia_flags);
        ADD(engine_rate_div10);
        ADD(max_rpm_div10);
        ADD(idle_rpm_div10);
        ADD(slip_angle);
        ADD(slip_angle2);
        ADD(ffb_wheel_steer_constant);
        ADD(ffb_wheel_steer_damper);
        ADD(ffb_wheel_steer_vibration_gain);
        ADD(ffb_wheel_steer_vibration_freq);
#undef ADD
        return m;
    }

    // Helpers
    static float Lerp(float a, float b, float t) { return a + (b - a) * t; }

    float readFloat(const CMChannelMap& m) const
    {
        if (!m.member) return 0.0f;
        return this->*m.member;
    }

    void writeFloat(const CMChannelMap& m, float v)
    {
        if (m.member) this->*m.member = v;
    }

    FourCC readFourCC(DataKey key) const
    {
        auto it = fourccValues.find(key);
        if (it == fourccValues.end()) return FourCC{ {0,0,0,0} };
        return it->second;
    }

    void zeroAll()
    {
        // zero all public numeric members
        std::memset(reinterpret_cast<void*>(this), 0, sizeofPublicNumeric());
    }

    static size_t sizeofPublicNumeric()
    {
        // Everything up to (and including) ffb_wheel_steer_vibration_freq is float fields.
        // Return the offset past the last float field.
        struct LayoutProbe
        {
            // mirror the public float region in the same order
#define FLD float
            FLD total_time, paused, vehicle_type;
            FLD yaw, pitch, roll, yaw_velocity, pitch_velocity, roll_velocity;
            FLD yaw_acceleration, pitch_acceleration, roll_acceleration;
            FLD position_x, position_y, position_z;
            FLD local_velocity_x, local_velocity_y, local_velocity_z;
            FLD gforce_lateral, gforce_longitudinal, gforce_vertical, speed;
            FLD suspension_position_bl, suspension_position_br, suspension_position_fl, suspension_position_fr;
            FLD suspension_velocity_bl, suspension_velocity_br, suspension_velocity_fl, suspension_velocity_fr;
            FLD suspension_acceleration_bl, suspension_acceleration_br, suspension_acceleration_fl, suspension_acceleration_fr;
            FLD wheel_patch_speed_bl, wheel_patch_speed_br, wheel_patch_speed_fl, wheel_patch_speed_fr;
            FLD throttle_input, steering_input, brake_input, clutch_input, gear, max_gears, engine_rate;
            FLD race_position, race_sector, sector_time_1, sector_time_2;
            FLD brake_temp_bl, brake_temp_br, brake_temp_fl, brake_temp_fr;
            FLD tyre_pressure_bl, tyre_pressure_br, tyre_pressure_fl, tyre_pressure_fr;
            FLD lap, laps_completed, total_laps, lap_time, lap_distance, track_length, last_lap_time, max_rpm, idle_rpm;
            FLD total_distance;
            FLD world_velocity_x, world_velocity_y, world_velocity_z;
            FLD world_dir_rht_x, world_dir_rht_y, world_dir_rht_z;
            FLD world_dir_fwd_x, world_dir_fwd_y, world_dir_fwd_z;
            FLD sli_pro_native_support, kers_level, kers_max, drs, traction_control, anti_lock_brakes;
            FLD fuel_in_tank, fuel_capacity, in_pits, team_info, session_type, drs_allowed, track_number, vehicle_fia_flags;
            FLD engine_rate_div10, max_rpm_div10, idle_rpm_div10;
            FLD slip_angle, slip_angle2;
            FLD ffb_wheel_steer_constant, ffb_wheel_steer_damper, ffb_wheel_steer_vibration_gain, ffb_wheel_steer_vibration_freq;
#undef FLD
        };
        return sizeof(LayoutProbe);
    }
};


