using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;

namespace GenericTelemetryProvider
{
    public class CMCustomUDPData
    {

        List<CMChannelMap> channels = new List<CMChannelMap>();
        byte[] packet;

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

        public void Init(string packetFormatPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(packetFormatPath);

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

                channels.Add(new CMChannelMap(sysType, name, offset, scale));

                offset += 4;
            }

            packet = new byte[offset];

        }

        public byte[] GetBytes()
        {
            foreach(CMChannelMap map in channels)
            {
                if(map == null)
                {
                    byte[] valueBytes = map.GetBytes(this);

                    if(valueBytes != null)
                    {
                        valueBytes.CopyTo(packet, map.offset);
                    }
                }
            }

            return packet;
        }

    }





    public class CMChannelMap
    {

        public string channel;
        public float scale = 1.0f;
        public int offset = -1;
        public Type type; // the type to be returned as bytes

        MethodInfo getBytesMethod;
        FieldInfo fieldInfo;

        public CMChannelMap(Type _type, string _channel, int _offset, float _scale)
        {
            type = _type;
            channel = _channel;
            offset = _offset;
            scale = _scale;
            getBytesMethod = typeof(BitConverter).GetMethod("GetBytes", new[] { type });

            fieldInfo = typeof(CMCustomUDPData).GetField(channel);
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

    }


    public struct fourcc
    {
        public char[] data;
    }
}
