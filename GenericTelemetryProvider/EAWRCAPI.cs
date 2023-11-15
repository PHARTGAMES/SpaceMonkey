//MIT License
//
//Copyright(c) 2023 PHARTGAMES
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace GenericTelemetryProvider
{

    [System.Serializable]
    public class EAWRCVersionsData
    {
        public int schema;
        public int data;
    }

    [System.Serializable]
    public class EAWRCChannelData
    {
        public string id;
        public string type;
        public string units;
        public string description;
    }


    [System.Serializable]
    public class EAWRCChannelsData
    {
        public EAWRCVersionsData versions;
        public EAWRCChannelData[] channels;

    }

    [System.Serializable]
    public class EAWRCPacketData
    {
        public string id;
        public string fourCC;
    }


    [System.Serializable]
    public class EAWRCPacketsData
    {
        public EAWRCVersionsData versions;
        public EAWRCPacketData[] packets;
    }


    [System.Serializable]
    public class EAWRCPacketStructureHeaderData
    {
        public string[] channels;
    }

    [System.Serializable]
    public class EAWRCPacketStructurePacketData
    {
        public string id;
        public string[] channels;
    }

    [System.Serializable]
    public class EAWRCPacketStructureData
    {
        public EAWRCVersionsData versions;
        public string id;
        public EAWRCPacketStructureHeaderData header;
        public EAWRCPacketStructurePacketData[] packets;
    }


    [System.Serializable]
    public class EAWRCConfigPacketData
    {
        public string structure;
        public string packet;
        public string ip;
        public int port;
        public int frequencyHz;
        public bool bEnabled;
    }

    [System.Serializable]
    public class EAWRCConfigUDPData
    {
        public EAWRCConfigPacketData[] packets;

    }

    [System.Serializable]
    public class EAWRCConfigLCDData
    {
        public bool bDisplayGears;

    }

    [System.Serializable]
    public class EAWRCConfigDBoxData
    {
        public bool bEnabled;
        public bool bLegacyMode;
    }


    [System.Serializable]
    public class EAWRCConfigData
    {
        public string schema;
        public EAWRCConfigUDPData udp;
        public EAWRCConfigLCDData lcd;
        public EAWRCConfigDBoxData dBox;
    }



    public class EAWRCPacketStructureMap
    {
        public string structure;
        public EAWRCPacketStructureData packetStructureData;

    }


    public class EAWRCCustomUDPData
    {

        public static Dictionary<string, EAWRCPacketStructureMap> packetStructureMapping = new Dictionary<string, EAWRCPacketStructureMap>();

        public static EAWRCChannelsData channelsData;
        public static EAWRCPacketsData packetsData;

        Dictionary<DataKey, EAWRCChannelMap> channels = new Dictionary<DataKey, EAWRCChannelMap>();
        byte[] packet;

        public static EAWRCConfigData configData;

        public fourcc fourCC; // the fourCC id for this packet

        public enum DataKey
        {
            packet_4cc,
            packet_uid,
			game_total_time,
			game_delta_time,
			game_frame_count,
			shiftlights_fraction,
			shiftlights_rpm_start,
			shiftlights_rpm_end,
			shiftlights_rpm_valid,
			vehicle_gear_index,
			vehicle_gear_index_neutral,
			vehicle_gear_index_reverse,
			vehicle_gear_maximum,
			vehicle_speed,
			vehicle_transmission_speed,
			vehicle_position_x,
			vehicle_position_y,
			vehicle_position_z,
			vehicle_velocity_x,
			vehicle_velocity_y,
			vehicle_velocity_z,
			vehicle_acceleration_x,
			vehicle_acceleration_y,
			vehicle_acceleration_z,
			vehicle_left_direction_x,
			vehicle_left_direction_y,
			vehicle_left_direction_z,
			vehicle_forward_direction_x,
			vehicle_forward_direction_y,
			vehicle_forward_direction_z,
			vehicle_up_direction_x,
			vehicle_up_direction_y,
			vehicle_up_direction_z,
			vehicle_hub_position_bl,
			vehicle_hub_position_br,
			vehicle_hub_position_fl,
			vehicle_hub_position_fr,
			vehicle_hub_velocity_bl,
			vehicle_hub_velocity_br,
			vehicle_hub_velocity_fl,
			vehicle_hub_velocity_fr,
			vehicle_cp_forward_speed_bl,
			vehicle_cp_forward_speed_br,
			vehicle_cp_forward_speed_fl,
			vehicle_cp_forward_speed_fr,
			vehicle_brake_temperature_bl,
			vehicle_brake_temperature_br,
			vehicle_brake_temperature_fl,
			vehicle_brake_temperature_fr,
			vehicle_engine_rpm_max,
			vehicle_engine_rpm_idle,
			vehicle_engine_rpm_current,
			vehicle_throttle,
			vehicle_brake,
			vehicle_clutch,
			vehicle_steering,
			vehicle_handbrake,
			stage_current_time,
			stage_current_distance,
			stage_length,

            Max
        }


        public object packet_4cc;
        public object packet_uid;
        public object game_total_time;
        public object game_delta_time;
        public object game_frame_count;
        public object shiftlights_fraction;
        public object shiftlights_rpm_start;
        public object shiftlights_rpm_end;
        public object shiftlights_rpm_valid;
        public object vehicle_gear_index;
        public object vehicle_gear_index_neutral;
        public object vehicle_gear_index_reverse;
        public object vehicle_gear_maximum;
        public object vehicle_speed;
        public object vehicle_transmission_speed;
        public object vehicle_position_x;
        public object vehicle_position_y;
        public object vehicle_position_z;
        public object vehicle_velocity_x;
        public object vehicle_velocity_y;
        public object vehicle_velocity_z;
        public object vehicle_acceleration_x;
        public object vehicle_acceleration_y;
        public object vehicle_acceleration_z;
        public object vehicle_left_direction_x;
        public object vehicle_left_direction_y;
        public object vehicle_left_direction_z;
        public object vehicle_forward_direction_x;
        public object vehicle_forward_direction_y;
        public object vehicle_forward_direction_z;
        public object vehicle_up_direction_x;
        public object vehicle_up_direction_y;
        public object vehicle_up_direction_z;
        public object vehicle_hub_position_bl;
        public object vehicle_hub_position_br;
        public object vehicle_hub_position_fl;
        public object vehicle_hub_position_fr;
        public object vehicle_hub_velocity_bl;
        public object vehicle_hub_velocity_br;
        public object vehicle_hub_velocity_fl;
        public object vehicle_hub_velocity_fr;
        public object vehicle_cp_forward_speed_bl;
        public object vehicle_cp_forward_speed_br;
        public object vehicle_cp_forward_speed_fl;
        public object vehicle_cp_forward_speed_fr;
        public object vehicle_brake_temperature_bl;
        public object vehicle_brake_temperature_br;
        public object vehicle_brake_temperature_fl;
        public object vehicle_brake_temperature_fr;
        public object vehicle_engine_rpm_max;
        public object vehicle_engine_rpm_idle;
        public object vehicle_engine_rpm_current;
        public object vehicle_throttle;
        public object vehicle_brake;
        public object vehicle_clutch;
        public object vehicle_steering;
        public object vehicle_handbrake;
        public object stage_current_time;
        public object stage_current_distance;
        public object stage_length;

        public EAWRCCustomUDPData()
        {
            packet_4cc = "";
            packet_uid = 0;
            game_total_time = 0.0f;
            game_delta_time = 0.0f;
            game_frame_count = 0;
            shiftlights_fraction = 0.0f;
            shiftlights_rpm_start = 0.0f;
            shiftlights_rpm_end = 0.0f;
            shiftlights_rpm_valid = false;
            vehicle_gear_index = 0;
            vehicle_gear_index_neutral = 0;
            vehicle_gear_index_reverse = 0;
            vehicle_gear_maximum = 0;
            vehicle_speed = 0.0f;
            vehicle_transmission_speed = 0.0f;
            vehicle_position_x = 0.0f;
            vehicle_position_y = 0.0f;
            vehicle_position_z = 0.0f;
            vehicle_velocity_x = 0.0f;
            vehicle_velocity_y = 0.0f;
            vehicle_velocity_z = 0.0f;
            vehicle_acceleration_x = 0.0f;
            vehicle_acceleration_y = 0.0f;
            vehicle_acceleration_z = 0.0f;
            vehicle_left_direction_x = 0.0f;
            vehicle_left_direction_y = 0.0f;
            vehicle_left_direction_z = 0.0f;
            vehicle_forward_direction_x = 0.0f;
            vehicle_forward_direction_y = 0.0f;
            vehicle_forward_direction_z = 0.0f;
            vehicle_up_direction_x = 0.0f;
            vehicle_up_direction_y = 0.0f;
            vehicle_up_direction_z = 0.0f;
            vehicle_hub_position_bl = 0.0f;
            vehicle_hub_position_br = 0.0f;
            vehicle_hub_position_fl = 0.0f;
            vehicle_hub_position_fr = 0.0f;
            vehicle_hub_velocity_bl = 0.0f;
            vehicle_hub_velocity_br = 0.0f;
            vehicle_hub_velocity_fl = 0.0f;
            vehicle_hub_velocity_fr = 0.0f;
            vehicle_cp_forward_speed_bl = 0.0f;
            vehicle_cp_forward_speed_br = 0.0f;
            vehicle_cp_forward_speed_fl = 0.0f;
            vehicle_cp_forward_speed_fr = 0.0f;
            vehicle_brake_temperature_bl = 0.0f;
            vehicle_brake_temperature_br = 0.0f;
            vehicle_brake_temperature_fl = 0.0f;
            vehicle_brake_temperature_fr = 0.0f;
            vehicle_engine_rpm_max = 0.0f;
            vehicle_engine_rpm_idle = 0.0f;
            vehicle_engine_rpm_current = 0.0f;
            vehicle_throttle = 0.0f;
            vehicle_brake = 0.0f;
            vehicle_clutch = 0.0f;
            vehicle_steering = 0.0f;
            vehicle_handbrake = 0.0f;
            stage_current_time = 0.0f;
            stage_current_distance = 0.0f;
            stage_length = 0.0f;
        }


        public void CopyFields(EAWRCCustomUDPData other)
        {

        }

        public static void LoadConfig()
        {
            try 
            {
                string documentsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "WRC", "telemetry");
                string configFilePath = documentsPath + "/config.json";
                if (File.Exists(configFilePath))
                {
                    string text = File.ReadAllText(configFilePath);

                    configData = JsonConvert.DeserializeObject<EAWRCConfigData>(text);
                }

                if (configData == null)
                    return;

                //load structures
                foreach (EAWRCConfigPacketData packetConfig in configData.udp.packets)
                {
                    if (!packetConfig.bEnabled)
                        continue;

                    //load structure file
                    string structureFilePath = documentsPath + "/readme/udp/" + packetConfig.structure + ".json";

                    if (File.Exists(structureFilePath))
                    {
                        string text = File.ReadAllText(structureFilePath);

                        EAWRCPacketStructureData packetStructureData = JsonConvert.DeserializeObject<EAWRCPacketStructureData>(text);

                        if (packetStructureData == null)
                            continue;

                        EAWRCPacketStructureMap newPacketStructureMap = new EAWRCPacketStructureMap();

                        newPacketStructureMap.packetStructureData = packetStructureData;
                        newPacketStructureMap.structure = packetConfig.structure;

                        packetStructureMapping.Add(newPacketStructureMap.structure, newPacketStructureMap);

                    }
                }

                //load channels.json
                string channelsFilePath = documentsPath + "/readme/channels.json";
                if (File.Exists(channelsFilePath))
                {
                    string text = File.ReadAllText(channelsFilePath);

                    channelsData = JsonConvert.DeserializeObject<EAWRCChannelsData>(text);
                }

                //load packets.json
                string packetsFilePath = documentsPath + "/readme/packets.json";
                if (File.Exists(channelsFilePath))
                {
                    string text = File.ReadAllText(packetsFilePath);

                    packetsData = JsonConvert.DeserializeObject<EAWRCPacketsData>(text);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("EAWRCAPI::Exception: " + e.Message);
            }

        }

        public static EAWRCConfigPacketData GetConfigPacketData(string structure, string packet)
        {
            foreach (EAWRCConfigPacketData packetConfig in configData.udp.packets)
            {
                if (packetConfig.structure == structure && packetConfig.packet == packet)
                    return packetConfig;
            }
            return null;
        }

 

        public static EAWRCCustomUDPData GetPacket(string structure, string packet)
        {
            if(!packetStructureMapping.TryGetValue(structure, out EAWRCPacketStructureMap structureMap))
            {
                return null;
            }

            if (structureMap == null)
                return null;


            foreach(EAWRCPacketStructurePacketData packetData in structureMap.packetStructureData.packets)
            {
                if(packetData.id == packet)
                {
                    EAWRCCustomUDPData newPacket = new EAWRCCustomUDPData();

                    newPacket.InitPacket(structureMap.packetStructureData.header, packetData);

                    return newPacket;
                }
            }

            return null;


        }

        public void InitPacket(EAWRCPacketStructureHeaderData headerData, EAWRCPacketStructurePacketData packetData)
        {
            int offset = 0;
            bool foundFourCC = false;

            foreach (string channelId in headerData.channels)
            {
                foreach (EAWRCChannelData channelData in channelsData.channels)
                {
                    if (channelData.id == channelId)
                    {
                        DataKey dataKey = (DataKey)Enum.Parse(typeof(DataKey), channelId, true);

                        Type sysType;
                        GetTypeDataForName(channelData.type, out sysType, out int size);

                        channels.Add(dataKey, new EAWRCChannelMap(sysType, dataKey, channelId, offset, 1.0f));

                        offset += size;

                        if(channelId == "packet_4cc")
                        {
                            foundFourCC = true;
                        }
                        break;
                    }
                }
            }

            ////add packet_4cc at the start for packets without packet_4cc in the header definition
            //if (!foundFourCC)
            //{
            //    Type sysType;
            //    GetTypeDataForName("fourcc", out sysType, out int size);

            //    channels.Add(DataKey.packet_4cc, new EAWRCChannelMap(sysType, DataKey.packet_4cc, "packet_4cc", offset, 1.0f));

            //    offset += size;
            //}


            foreach (string channelId in packetData.channels)
            {
                foreach (EAWRCChannelData channelData in channelsData.channels)
                {
                    if(channelData.id == channelId)
                    {
                        DataKey dataKey = (DataKey)Enum.Parse(typeof(DataKey), channelId, true);

                        Type sysType;
                        GetTypeDataForName(channelData.type, out sysType, out int size);

                        channels.Add(dataKey, new EAWRCChannelMap(sysType, dataKey, channelId, offset, 1.0f));

                        offset += size;
                        break;
                    }
                }
            }

            //set the fourcc for this packet
            foreach(EAWRCPacketData pd in packetsData.packets)
            {
                if(pd.id == packetData.id)
                {
                    fourCC = new fourcc();
                    fourCC.data = pd.id.ToCharArray();
                    break;
                }
            }
           
        }

        public void GetTypeDataForName(string typeName, out Type sysType, out int size)
        {
            sysType = typeof(float);
            size = 4;

            switch (typeName)
            {
                case "fourcc":
                    {
                        sysType = typeof(fourcc);
                        size = 4;
                        break;
                    }
                case "float32":
                    {
                        sysType = typeof(float);
                        size = 4;
                        break;
                    }
                case "float64":
                    {
                        sysType = typeof(double);
                        size = 8;
                        break;
                    }
                case "int8":
                    {
                        sysType = typeof(sbyte);
                        size = 1;
                        break;
                    }
                case "uint8":
                    {
                        sysType = typeof(byte);
                        size = 1;
                        break;
                    }
                case "int16":
                    {
                        sysType = typeof(short);
                        size = 2;
                        break;
                    }
                case "uint16":
                    {
                        sysType = typeof(ushort);
                        size = 2;
                        break;
                    }
                case "int32":
                    {
                        sysType = typeof(Int32);
                        size = 4;
                        break;
                    }
                case "uint32":
                    {
                        sysType = typeof(UInt32);
                        size = 4;
                        break;
                    }
                case "uint64":
                    {
                        sysType = typeof(UInt64);
                        size = 8;
                        break;
                    }
                case "int64":
                    {
                        sysType = typeof(Int64);
                        size = 8;
                        break;
                    }
                case "boolean":
                    {
                        sysType = typeof(bool);
                        size = 1;
                        break;
                    }
                default:
                    {
                        Console.WriteLine("EAWRCAPI ERROR: unknown typeName " + typeName);
                        break;
                    }

            };

        }

        public byte[] GetBytes()
        {
            foreach (KeyValuePair<DataKey, EAWRCChannelMap> pair in channels)
            {
                EAWRCChannelMap map = pair.Value;
                byte[] valueBytes = map.GetBytes(this);

                if (valueBytes != null)
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

        public void Copy(EAWRCCustomUDPData other, bool copyChannels = true)
        {


            if (copyChannels)
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

        public bool FromBytes(byte[] bytes)
        {
            //check fourCC
            //if (fourCC.data[0] != bytes[0] || fourCC.data[1] != bytes[1] || fourCC.data[2] != bytes[2] || fourCC.data[3] != bytes[3])
            //    return false;

            //read into structure
            foreach (KeyValuePair<DataKey, EAWRCChannelMap> pair in channels)
            {
                EAWRCChannelMap map = pair.Value;

                map.SetValueFromBytes(bytes, this);
            }

            return true;
        }


        public void LerpAll(float lerp)
        {

            //yaw = Lerp(0.0f, (float)yaw, lerp);
        }

        public void LerpAllFrom(EAWRCCustomUDPData from, float lerp)
        {
            //yaw = Lerp((float)from.yaw, (float)yaw, lerp);
        }


        float Lerp(float from, float to, float lerp)
        {
            return from + ((to - from) * lerp);
        }
    }


    public class EAWRCChannelMap
    {

        public string channel;
        public float scale = 1.0f;
        public int offset = -1;
        public Type type; // the type to be returned as bytes
        public EAWRCCustomUDPData.DataKey dataKey;

        MethodInfo getBytesMethod;
        FieldInfo fieldInfo;

        public EAWRCChannelMap(Type _type, EAWRCCustomUDPData.DataKey _dataKey, string _channel, int _offset, float _scale)
        {
            type = _type;
            channel = _channel;
            offset = _offset;
            scale = _scale;
            dataKey = _dataKey;
            try 
            {
                getBytesMethod = typeof(BitConverter).GetMethod("GetBytes", new[] { type });
            }
            catch(Exception e)
            {
                Console.WriteLine($"No getBytesMethod for channel {_channel} of type {type.ToString()}");
            }

            fieldInfo = typeof(EAWRCCustomUDPData).GetField(channel);
        }

        public object GetValue(EAWRCCustomUDPData data)
        {
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(data);
            }

            return 0.0f;
        }

        public void SetValue(object value, EAWRCCustomUDPData data)
        {
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(data, value);
            }
        }

        public void SetValueFromBytes(byte[] bytes, EAWRCCustomUDPData data)
        {
           
            if (type == typeof(byte))
            {
                SetValue(bytes[offset], data);
            }
            else if (type == typeof(sbyte))
            {
                SetValue(bytes[offset], data);
            }
            else if (type == typeof(UInt16))
            {
                SetValue(BitConverter.ToUInt16(bytes, offset), data);
            }
            else if (type == typeof(Int16))
            {
                SetValue(BitConverter.ToInt16(bytes, offset), data);
            }
            else if (type == typeof(bool))
            {
                SetValue(BitConverter.ToBoolean(bytes, offset), data);
            }
            else if (type == typeof(float))
            {
                SetValue(BitConverter.ToSingle(bytes, offset), data);
            }
            else if (type == typeof(UInt32))
            {
                SetValue(BitConverter.ToUInt32(bytes, offset), data);
            }
            else if (type == typeof(int))
            {
                SetValue(BitConverter.ToInt32(bytes, offset), data);
            }
            else if (type == typeof(UInt64))
            {
                SetValue(BitConverter.ToUInt64(bytes, offset), data);
            }
            else if (type == typeof(Int64))
            {
                SetValue(BitConverter.ToInt64(bytes, offset), data);
            }
            else if (type == typeof(double))
            {
                SetValue(BitConverter.ToDouble(bytes, offset), data);
            }
            else if (type == typeof(fourcc))
            {
                fourcc val = new fourcc();
                val.data = Encoding.UTF8.GetString(bytes, offset, 4).ToCharArray();

                SetValue(val, data);
            }
        }


        public byte[] GetBytes(EAWRCCustomUDPData data)
        {

            object value = 0;

            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(data);
            }

            if (getBytesMethod != null)
            {
                return (byte[])getBytesMethod.Invoke(null, new object[] { value });
            }
            else
            if(type == typeof(bool) || type == typeof(byte) || type == typeof(sbyte))
            {
                return new byte[] { (byte)value };
            }
            else
            if (type == typeof(fourcc))
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
