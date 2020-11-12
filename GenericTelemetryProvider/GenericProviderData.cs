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
    struct GenericProviderData
    {
        public char[] version; // version id, always 8 characters

        //filtered
        public float yaw; //radians
        public float pitch; //radians
        public float roll; //radians
        public float position_x; //worldspace
        public float position_y; //worldspace
        public float position_z; //worldspace
        public float local_velocity_x; //metres per second
        public float local_velocity_y; //metres per second
        public float local_velocity_z; //metres per second
        public float gforce_lateral; //gs
        public float gforce_longitudinal; //gs
        public float gforce_vertical; //gs
        public float engine_rpm;

        //unfiltered
        public float yaw_raw; //radians
        public float pitch_raw; //radians
        public float roll_raw; //radians
        public float position_x_raw; //worldspace
        public float position_y_raw; //worldspace
        public float position_z_raw; //worldspace
        public float local_velocity_x_raw; //metres per second
        public float local_velocity_y_raw; //metres per second
        public float local_velocity_z_raw; //metres per second
        public float gforce_lateral_raw; //gs
        public float gforce_longitudinal_raw; //gs
        public float gforce_vertical_raw; //gs
        public float engine_rpm_raw;


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

            yaw_raw = other.yaw_raw;
            pitch_raw = other.pitch_raw;
            roll_raw = other.roll_raw;

            position_x_raw = other.position_x_raw;
            position_y_raw = other.position_y_raw;
            position_z_raw = other.position_z_raw;

            local_velocity_x_raw = other.local_velocity_x_raw;
            local_velocity_y_raw = other.local_velocity_y_raw;
            local_velocity_z_raw = other.local_velocity_z_raw;

            gforce_lateral_raw = other.gforce_lateral_raw;
            gforce_longitudinal_raw = other.gforce_longitudinal_raw;
            gforce_vertical_raw = other.gforce_vertical_raw;

            engine_rpm = other.engine_rpm_raw;

        }

        public void Reset()
        {
            version = new char[] { 'V','E','R','S','I','O','N','0'};

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

            yaw_raw = 0;
            pitch_raw = 0;
            roll_raw = 0;

            position_x_raw = 0;
            position_y_raw = 0;
            position_z_raw = 0;

            local_velocity_x_raw = 0;
            local_velocity_y_raw = 0;
            local_velocity_z_raw = 0;

            gforce_lateral_raw = 0;
            gforce_longitudinal_raw = 0;
            gforce_vertical_raw = 0;

            engine_rpm = engine_rpm_raw;

        }


    }
}
