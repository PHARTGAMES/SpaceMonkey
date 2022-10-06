using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.Globalization;

namespace CMCustomUDP
{
    public class CMCustomUDPData
    {

        public static string formatFilename;

        Dictionary<DataKey, CMChannelMap> channels = new Dictionary<DataKey, CMChannelMap>();
        byte[] packet;

        public enum DataKey
        {
            total_time,
            paused,
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
            Max
        }

        public object total_time;
        public object paused;
        public object yaw;
        public object pitch;
        public object roll;
        public object yaw_velocity;
        public object pitch_velocity;
        public object roll_velocity;
        public object yaw_acceleration;
        public object pitch_acceleration;
        public object roll_acceleration;
        public object position_x;
        public object position_y;
        public object position_z;
        public object local_velocity_x;
        public object local_velocity_y;
        public object local_velocity_z;
        public object gforce_lateral;
        public object gforce_longitudinal;
        public object gforce_vertical;
        public object speed;
        public object suspension_position_bl;
        public object suspension_position_br;
        public object suspension_position_fl;
        public object suspension_position_fr;
        public object suspension_velocity_bl;
        public object suspension_velocity_br;
        public object suspension_velocity_fl;
        public object suspension_velocity_fr;
        public object suspension_acceleration_bl;
        public object suspension_acceleration_br;
        public object suspension_acceleration_fl;
        public object suspension_acceleration_fr;
        public object wheel_patch_speed_bl;
        public object wheel_patch_speed_br;
        public object wheel_patch_speed_fl;
        public object wheel_patch_speed_fr;
        public object throttle_input;
        public object steering_input;
        public object brake_input;
        public object clutch_input;
        public object gear;
        public object max_gears;
        public object engine_rate;
        public object race_position;
        public object race_sector;
        public object sector_time_1;
        public object sector_time_2;
        public object brake_temp_bl;
        public object brake_temp_br;
        public object brake_temp_fl;
        public object brake_temp_fr;
        public object tyre_pressure_bl;
        public object tyre_pressure_br;
        public object tyre_pressure_fl;
        public object tyre_pressure_fr;
        public object lap;
        public object laps_completed;
        public object total_laps;
        public object lap_time;
        public object lap_distance;
        public object track_length;
        public object last_lap_time;
        public object max_rpm;
        public object idle_rpm;

        public CMCustomUDPData()
        {
            total_time = 0.0f;
            paused = 0.0f;
            yaw = 0.0f;
            pitch = 0.0f;
            roll = 0.0f;
            yaw_velocity = 0.0f;
            pitch_velocity = 0.0f;
            roll_velocity = 0.0f;
            yaw_acceleration = 0.0f;
            pitch_acceleration = 0.0f;
            roll_acceleration = 0.0f;
            position_x = 0.0f;
            position_y = 0.0f;
            position_z = 0.0f;
            local_velocity_x = 0.0f;
            local_velocity_y = 0.0f;
            local_velocity_z = 0.0f;
            gforce_lateral = 0.0f;
            gforce_longitudinal = 0.0f;
            gforce_vertical = 0.0f;
            speed = 0.0f;
            suspension_position_bl = 0.0f;
            suspension_position_br = 0.0f;
            suspension_position_fl = 0.0f;
            suspension_position_fr = 0.0f;
            suspension_velocity_bl = 0.0f;
            suspension_velocity_br = 0.0f;
            suspension_velocity_fl = 0.0f;
            suspension_velocity_fr = 0.0f;
            suspension_acceleration_bl = 0.0f;
            suspension_acceleration_br = 0.0f;
            suspension_acceleration_fl = 0.0f;
            suspension_acceleration_fr = 0.0f;
            wheel_patch_speed_bl = 0.0f;
            wheel_patch_speed_br = 0.0f;
            wheel_patch_speed_fl = 0.0f;
            wheel_patch_speed_fr = 0.0f;
            throttle_input = 0.0f;
            steering_input = 0.0f;
            brake_input = 0.0f;
            clutch_input = 0.0f;
            gear = 0.0f;
            max_gears = 0.0f;
            engine_rate = 0.0f;
            race_position = 0.0f;
            race_sector = 0.0f;
            sector_time_1 = 0.0f;
            sector_time_2 = 0.0f;
            brake_temp_bl = 0.0f;
            brake_temp_br = 0.0f;
            brake_temp_fl = 0.0f;
            brake_temp_fr = 0.0f;
            tyre_pressure_bl = 0.0f;
            tyre_pressure_br = 0.0f;
            tyre_pressure_fl = 0.0f;
            tyre_pressure_fr = 0.0f;
            lap = 0.0f;
            laps_completed = 0.0f;
            total_laps = 0.0f;
            lap_time = 0.0f;
            lap_distance = 0.0f;
            track_length = 0.0f;
            last_lap_time = 0.0f;
            max_rpm = 0.0f;
            idle_rpm = 0.0f;
        }

        public void Init()
        {

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            XmlDocument doc = new XmlDocument();
            doc.Load(formatFilename);

            XmlNode root = doc.SelectSingleNode("custom_udp");

            int offset = 0;

            foreach(XmlNode channel in root.ChildNodes)
            {
                string type = channel.Name;
                string name = channel.Attributes["channel"]?.InnerText;
                float scale = float.Parse(channel.Attributes["scale"]?.InnerText);

                Type sysType = typeof(float);
                switch(type.ToLower())
                {
                    case "uint32":
                        {
                            sysType = typeof(uint);
                            break;
                        }
                    case "int32":
                        {
                            sysType = typeof(int);
                            break;
                        }
                    case "float":
                        {
                            sysType = typeof(float);
                            break;
                        }
                    case "fourcc":
                        {
                            sysType = typeof(fourcc);
                            break;
                        }
                }

                DataKey dataKey = (DataKey)Enum.Parse(typeof(DataKey), name, true);

                channels.Add(dataKey, new CMChannelMap(sysType, dataKey, name, offset, scale));

                offset += 4;
            }

            packet = new byte[offset];

        }

        public byte[] GetBytes()
        {
            foreach(KeyValuePair<DataKey, CMChannelMap> pair in channels)
            {
                CMChannelMap map = pair.Value;
                byte[] valueBytes = map.GetBytes(this);

                if(valueBytes != null)
                {
                    valueBytes.CopyTo(packet, map.offset);
                }
            }

            return packet;
        }


        public object GetValue(DataKey key)
        {
            return channels[key].GetValue(this);
        }

        public void SetValue(DataKey key, object value)
        {
            channels[key].SetValue(value, this);
        }

        public bool IsFloat(DataKey key)
        {
            return channels[key].IsFloat();
        }

        public bool IsValid(DataKey key)
        {
            return channels.ContainsKey(key);
        }

        public void Copy(CMCustomUDPData other)
        {
            total_time = other.total_time;
            paused = other.paused;
            yaw = other.yaw;
            pitch = other.pitch;
            roll = other.roll;
            yaw_velocity = other.yaw_velocity;
            pitch_velocity = other.pitch_velocity;
            roll_velocity = other.roll_velocity;
            yaw_acceleration = other.yaw_acceleration;
            pitch_acceleration = other.pitch_acceleration;
            roll_acceleration = other.roll_acceleration;
            position_x = other.position_x;
            position_y = other.position_y;
            position_z = other.position_z;
            local_velocity_x = other.local_velocity_x;
            local_velocity_y = other.local_velocity_y;
            local_velocity_z = other.local_velocity_z;
            gforce_lateral = other.gforce_lateral;
            gforce_longitudinal = other.gforce_longitudinal;
            gforce_vertical = other.gforce_vertical;
            speed = other.speed;
            suspension_position_bl = other.suspension_position_bl;
            suspension_position_br = other.suspension_position_br;
            suspension_position_fl = other.suspension_position_fl;
            suspension_position_fr = other.suspension_position_fr;
            suspension_velocity_bl = other.suspension_velocity_bl;
            suspension_velocity_br = other.suspension_velocity_br;
            suspension_velocity_fl = other.suspension_velocity_fl;
            suspension_velocity_fr = other.suspension_velocity_fr;
            suspension_acceleration_bl = other.suspension_acceleration_bl;
            suspension_acceleration_br = other.suspension_acceleration_br;
            suspension_acceleration_fl = other.suspension_acceleration_fl;
            suspension_acceleration_fr = other.suspension_acceleration_fr;
            wheel_patch_speed_bl = other.wheel_patch_speed_bl;
            wheel_patch_speed_br = other.wheel_patch_speed_br;
            wheel_patch_speed_fl = other.wheel_patch_speed_fl;
            wheel_patch_speed_fr = other.wheel_patch_speed_fr;
            throttle_input = other.throttle_input;
            steering_input = other.steering_input;
            brake_input = other.brake_input;
            clutch_input = other.clutch_input;
            gear = other.gear;
            max_gears = other.max_gears;
            engine_rate = other.engine_rate;
            race_position = other.race_position;
            race_sector = other.race_sector;
            sector_time_1 = other.sector_time_1;
            sector_time_2 = other.sector_time_2;
            brake_temp_bl = other.brake_temp_bl;
            brake_temp_br = other.brake_temp_br;
            brake_temp_fl = other.brake_temp_fl;
            brake_temp_fr = other.brake_temp_fr;
            tyre_pressure_bl = other.tyre_pressure_bl;
            tyre_pressure_br = other.tyre_pressure_br;
            tyre_pressure_fl = other.tyre_pressure_fl;
            tyre_pressure_fr = other.tyre_pressure_fr;
            lap = other.lap;
            laps_completed = other.laps_completed;
            total_laps = other.total_laps;
            lap_time = other.lap_time;
            lap_distance = other.lap_distance;
            track_length = other.track_length;
            last_lap_time = other.last_lap_time;
            max_rpm = other.max_rpm;
            idle_rpm = other.idle_rpm;

            channels = other.channels;
        }

        public static int GetKeyMask(params DataKey[] list)
        {
            int rval = 0;

            foreach (DataKey key in list)
            {
                rval |= (1 << (int)key);
            }

            return rval;
        }

        public int GetSize()
        {
            return packet.Length;
        }

        public void FromBytes(byte[] bytes)
        {
            foreach(KeyValuePair<DataKey, CMChannelMap> pair in channels)
            {
                CMChannelMap map = pair.Value;

                map.SetValueFromBytes(bytes, this);
            }
        }

        public float pitchDeg
        {
            get
            {
                return (float)pitch * (180.0f / (float)Math.PI);
            }
        }

        public float yawDeg
        {
            get
            {
                return (float)yaw * (180.0f / (float)Math.PI);
            }
        }

        public float rollDeg
        {
            get
            {
                return (float)roll * (180.0f / (float)Math.PI);
            }
        }


        public void LerpAll(float lerp)
        {

            yaw = Lerp(0.0f, (float)yaw, lerp);
            pitch = Lerp(0.0f, (float)pitch, lerp);
            roll = Lerp(0.0f, (float)roll, lerp);
            yaw_velocity = Lerp(0.0f, (float)yaw_velocity, lerp);
            pitch_velocity = Lerp(0.0f, (float)pitch_velocity, lerp);
            roll_velocity = Lerp(0.0f, (float)roll_velocity, lerp);
            yaw_acceleration = Lerp(0.0f, (float)yaw_acceleration, lerp);
            pitch_acceleration = Lerp(0.0f, (float)pitch_acceleration, lerp);
            roll_acceleration = Lerp(0.0f, (float)roll_acceleration, lerp);
            local_velocity_x = Lerp(0.0f, (float)local_velocity_x, lerp);
            local_velocity_y = Lerp(0.0f, (float)local_velocity_y, lerp);
            local_velocity_z = Lerp(0.0f, (float)local_velocity_z, lerp);
            gforce_lateral = Lerp(0.0f, (float)gforce_lateral, lerp);
            gforce_longitudinal = Lerp(0.0f, (float)gforce_longitudinal, lerp);
            gforce_vertical = Lerp(0.0f, (float)gforce_vertical, lerp);
            suspension_position_bl = Lerp(0.0f, (float)suspension_position_bl, lerp);
            suspension_position_br = Lerp(0.0f, (float)suspension_position_br, lerp);
            suspension_position_fl = Lerp(0.0f, (float)suspension_position_fl, lerp);
            suspension_position_fr = Lerp(0.0f, (float)suspension_position_fr, lerp);
            suspension_velocity_bl = Lerp(0.0f, (float)suspension_velocity_bl, lerp);
            suspension_velocity_br = Lerp(0.0f, (float)suspension_velocity_br, lerp);
            suspension_velocity_fl = Lerp(0.0f, (float)suspension_velocity_fl, lerp);
            suspension_velocity_fr = Lerp(0.0f, (float)suspension_velocity_fr, lerp);
            suspension_acceleration_bl = Lerp(0.0f, (float)suspension_acceleration_bl, lerp);
            suspension_acceleration_br = Lerp(0.0f, (float)suspension_acceleration_br, lerp);
            suspension_acceleration_fl = Lerp(0.0f, (float)suspension_acceleration_fl, lerp);
            suspension_acceleration_fr = Lerp(0.0f, (float)suspension_acceleration_fr, lerp);
            wheel_patch_speed_bl = Lerp(0.0f, (float)wheel_patch_speed_bl, lerp);
            wheel_patch_speed_br = Lerp(0.0f, (float)wheel_patch_speed_br, lerp);
            wheel_patch_speed_fl = Lerp(0.0f, (float)wheel_patch_speed_fl, lerp);
            wheel_patch_speed_fr = Lerp(0.0f, (float)wheel_patch_speed_fr, lerp);
            throttle_input = Lerp(0.0f, (float)throttle_input, lerp);
            steering_input = Lerp(0.0f, (float)steering_input, lerp);
            brake_input = Lerp(0.0f, (float)brake_input, lerp);
            clutch_input = Lerp(0.0f, (float)clutch_input, lerp);
            tyre_pressure_bl = Lerp(0.0f, (float)tyre_pressure_bl, lerp);
            tyre_pressure_br = Lerp(0.0f, (float)tyre_pressure_br, lerp);
            tyre_pressure_fl = Lerp(0.0f, (float)tyre_pressure_fl, lerp);
            tyre_pressure_fr = Lerp(0.0f, (float)tyre_pressure_fr, lerp);
        }

        public void LerpAllFrom(CMCustomUDPData from, float lerp)
        {
            yaw = Lerp((float)from.yaw, (float)yaw, lerp);
            pitch = Lerp((float)from.pitch, (float)pitch, lerp);
            roll = Lerp((float)from.roll, (float)roll, lerp);
            yaw_velocity = Lerp((float)from.yaw_velocity, (float)yaw_velocity, lerp);
            pitch_velocity = Lerp((float)from.pitch_velocity, (float)pitch_velocity, lerp);
            roll_velocity = Lerp((float)from.roll_velocity, (float)roll_velocity, lerp);
            yaw_acceleration = Lerp((float)from.yaw_acceleration, (float)yaw_acceleration, lerp);
            pitch_acceleration = Lerp((float)from.pitch_acceleration, (float)pitch_acceleration, lerp);
            roll_acceleration = Lerp((float)from.roll_acceleration, (float)roll_acceleration, lerp);
            local_velocity_x = Lerp((float)from.local_velocity_x, (float)local_velocity_x, lerp);
            local_velocity_y = Lerp((float)from.local_velocity_y, (float)local_velocity_y, lerp);
            local_velocity_z = Lerp((float)from.local_velocity_z, (float)local_velocity_z, lerp);
            gforce_lateral = Lerp((float)from.gforce_lateral, (float)gforce_lateral, lerp);
            gforce_longitudinal = Lerp((float)from.gforce_longitudinal, (float)gforce_longitudinal, lerp);
            gforce_vertical = Lerp((float)from.gforce_vertical, (float)gforce_vertical, lerp);
            suspension_position_bl = Lerp((float)from.suspension_position_bl, (float)suspension_position_bl, lerp);
            suspension_position_br = Lerp((float)from.suspension_position_br, (float)suspension_position_br, lerp);
            suspension_position_fl = Lerp((float)from.suspension_position_fl, (float)suspension_position_fl, lerp);
            suspension_position_fr = Lerp((float)from.suspension_position_fr, (float)suspension_position_fr, lerp);
            suspension_velocity_bl = Lerp((float)from.suspension_velocity_bl, (float)suspension_velocity_bl, lerp);
            suspension_velocity_br = Lerp((float)from.suspension_velocity_br, (float)suspension_velocity_br, lerp);
            suspension_velocity_fl = Lerp((float)from.suspension_velocity_fl, (float)suspension_velocity_fl, lerp);
            suspension_velocity_fr = Lerp((float)from.suspension_velocity_fr, (float)suspension_velocity_fr, lerp);
            suspension_acceleration_bl = Lerp((float)from.suspension_acceleration_bl, (float)suspension_acceleration_bl, lerp);
            suspension_acceleration_br = Lerp((float)from.suspension_acceleration_br, (float)suspension_acceleration_br, lerp);
            suspension_acceleration_fl = Lerp((float)from.suspension_acceleration_fl, (float)suspension_acceleration_fl, lerp);
            suspension_acceleration_fr = Lerp((float)from.suspension_acceleration_fr, (float)suspension_acceleration_fr, lerp);
            wheel_patch_speed_bl = Lerp((float)from.wheel_patch_speed_bl, (float)wheel_patch_speed_bl, lerp);
            wheel_patch_speed_br = Lerp((float)from.wheel_patch_speed_br, (float)wheel_patch_speed_br, lerp);
            wheel_patch_speed_fl = Lerp((float)from.wheel_patch_speed_fl, (float)wheel_patch_speed_fl, lerp);
            wheel_patch_speed_fr = Lerp((float)from.wheel_patch_speed_fr, (float)wheel_patch_speed_fr, lerp);
            throttle_input = Lerp((float)from.throttle_input, (float)throttle_input, lerp);
            steering_input = Lerp((float)from.steering_input, (float)steering_input, lerp);
            brake_input = Lerp((float)from.brake_input, (float)brake_input, lerp);
            clutch_input = Lerp((float)from.clutch_input, (float)clutch_input, lerp);
            tyre_pressure_bl = Lerp((float)from.tyre_pressure_bl, (float)tyre_pressure_bl, lerp);
            tyre_pressure_br = Lerp((float)from.tyre_pressure_br, (float)tyre_pressure_br, lerp);
            tyre_pressure_fl = Lerp((float)from.tyre_pressure_fl, (float)tyre_pressure_fl, lerp);
            tyre_pressure_fr = Lerp((float)from.tyre_pressure_fr, (float)tyre_pressure_fr, lerp);
        }


        float Lerp(float from, float to, float lerp)
        {
            return from + ((to-from) * lerp);
        }
    }


    public class CMChannelMap
    {

        public string channel;
        public float scale = 1.0f;
        public int offset = -1;
        public Type type; // the type to be returned as bytes
        public CMCustomUDPData.DataKey dataKey;

        MethodInfo getBytesMethod;
        FieldInfo fieldInfo;

        public CMChannelMap(Type _type, CMCustomUDPData.DataKey _dataKey, string _channel, int _offset, float _scale)
        {
            type = _type;
            channel = _channel;
            offset = _offset;
            scale = _scale;
            dataKey = _dataKey;
            getBytesMethod = typeof(BitConverter).GetMethod("GetBytes", new[] { type });

            fieldInfo = typeof(CMCustomUDPData).GetField(channel);
        }

        public object GetValue(CMCustomUDPData data)
        {
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(data);
            }

            return 0.0f;
        }

        public void SetValue(object value, CMCustomUDPData data)
        {
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(data, value);
            }
        }

        public void SetValueFromBytes(byte[] bytes, CMCustomUDPData data)
        {
            if(type == typeof(float))
            {
                SetValue(BitConverter.ToSingle(bytes, offset), data);
            }
            else
            if (type == typeof(UInt32))
            {
                SetValue(BitConverter.ToUInt32(bytes, offset), data);
            }
            else
            if (type == typeof(int))
            {
                SetValue(BitConverter.ToInt32(bytes, offset), data);
            }
            else
            if (type == typeof(fourcc))
            {
                fourcc val = new fourcc();
                val.data = Encoding.UTF8.GetString(bytes, offset, 4).ToCharArray();

                SetValue(val, data);
            }
        }


        public byte[] GetBytes(CMCustomUDPData data)
        {

            object value = 0;

            if(fieldInfo != null)
            {
                value = fieldInfo.GetValue(data);
            }

            if (getBytesMethod != null)
            {
                return (byte[])getBytesMethod.Invoke(null, new object[] { value });
            }
            else
            if(type == typeof(fourcc))
            {
                return Encoding.UTF8.GetBytes(((fourcc)value).data);
            }
            return null;
        }

        public bool IsFloat()
        {
            return type == typeof(float);
        }

    }


    public struct fourcc
    {
        public char[] data;
    }

 }
