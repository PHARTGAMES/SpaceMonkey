using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Numerics;

namespace GenericTelemetryProvider
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public class GenericProviderData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public char[] version; // version id, always 8 characters

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        public float[] data; // always length of DataKey.Max

        public enum DataKey
        {
            Yaw,
            Pitch,
            Roll,
            PositionX,
            PositionY,
            PositionZ,
            LocalVelX,
            LocalVelY,
            LocalVelZ,
            GLat,
            GLong,
            GVert,
            EngineRPM,

            Max
        }

        public GenericProviderData()
        {
            version = null;
            data = new float[(int)DataKey.Max];
            Reset();
        }


        public GenericProviderData(char[] _version)
        {
            version = _version;
            data = new float[(int)DataKey.Max];
            Reset();
        }

        public float yaw //radians
        {
            get { return data[(int)DataKey.Yaw]; }
            set { data[(int)DataKey.Yaw] = value; }
        }

        public float pitch //radians
        {
            get { return data[(int)DataKey.Pitch]; }
            set { data[(int)DataKey.Pitch] = value; }
        }

        public float roll //radians
        {
            get { return data[(int)DataKey.Roll]; }
            set { data[(int)DataKey.Roll] = value; }
        }

        public float position_x //worldspace
        {
            get { return data[(int)DataKey.PositionX]; }
            set { data[(int)DataKey.PositionX] = value; }
        }

        public float position_y //worldspace
        {
            get { return data[(int)DataKey.PositionY]; }
            set { data[(int)DataKey.PositionY] = value; }
        }

        public float position_z //worldspace
        {
            get { return data[(int)DataKey.PositionZ]; }
            set { data[(int)DataKey.PositionZ] = value; }
        }

        public float local_velocity_x //metres per second
        {
            get { return data[(int)DataKey.LocalVelX]; }
            set { data[(int)DataKey.LocalVelX] = value; }
        }

        public float local_velocity_y //metres per second
        {
            get { return data[(int)DataKey.LocalVelY]; }
            set { data[(int)DataKey.LocalVelY] = value; }
        }

        public float local_velocity_z //metres per second
        {
            get { return data[(int)DataKey.LocalVelZ]; }
            set { data[(int)DataKey.LocalVelZ] = value; }
        }

        public float gforce_lateral //gs
        {
            get { return data[(int)DataKey.GLat]; }
            set { data[(int)DataKey.GLat] = value; }
        }

        public float gforce_longitudinal //gs
        {
            get { return data[(int)DataKey.GLong]; }
            set { data[(int)DataKey.GLong] = value; }
        }

        public float gforce_vertical //gs
        {
            get { return data[(int)DataKey.GVert]; }
            set { data[(int)DataKey.GVert] = value; }
        }

        public float engine_rpm //rpm
        {
            get { return data[(int)DataKey.EngineRPM]; }
            set { data[(int)DataKey.EngineRPM] = value; }
        }

        public int GetSize()
        {
            GenericProviderData packet = this;
            return Marshal.SizeOf<GenericProviderData>(packet);
        }

        public byte[] ToByteArray()
        {
            GenericProviderData packet = this;
            int num = Marshal.SizeOf<GenericProviderData>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<GenericProviderData>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

        public static GenericProviderData FromByteArray(byte[] bytes)
        {
            GCHandle pinnedPacket = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            GenericProviderData data = (GenericProviderData)Marshal.PtrToStructure(
                pinnedPacket.AddrOfPinnedObject(),
                typeof(GenericProviderData));
            pinnedPacket.Free();

            return data;
        }

        public void Copy(GenericProviderData other)
        {
            version = other.version;

            yaw = other.yaw;
            pitch = other.pitch;
            roll = other.roll;

            position_x = other.position_x;
            position_y = other.position_y;
            position_z = other.position_z;

            local_velocity_x = other.local_velocity_x;
            local_velocity_y = other.local_velocity_y;
            local_velocity_z = other.local_velocity_z;

            gforce_lateral = other.gforce_lateral;
            gforce_longitudinal = other.gforce_longitudinal;
            gforce_vertical = other.gforce_vertical;

            engine_rpm = other.engine_rpm;

        }

        public void Reset()
        {
            if(version == null)
                version = new char[] { 'V', 'E', 'R', 'S', 'I', 'O', 'N', '0' };

            yaw = 0;
            pitch = 0;
            roll = 0;

            position_x = 0;
            position_y = 0;
            position_z = 0;

            local_velocity_x = 0;
            local_velocity_y = 0;
            local_velocity_z = 0;

            gforce_lateral = 0;
            gforce_longitudinal = 0;
            gforce_vertical = 0;

            engine_rpm = 0;

        }


    }
}
