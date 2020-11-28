using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GenericTelemetryProvider
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public class DirtRally2UDPTelemetry
    {
        public float total_time;
        public float lap_time;
        public float lap_distance;
        public float total_distance;
        public float position_x;
        public float position_y;
        public float position_z;
        public float speed;
        public float velocity_x;
        public float velocity_y;
        public float velocity_z;
        public float left_dir_x;
        public float left_dir_y;
        public float left_dir_z;
        public float forward_dir_x;
        public float forward_dir_y;
        public float forward_dir_z;
        public float suspension_position_bl;
        public float suspension_position_br;
        public float suspension_position_fl;
        public float suspension_position_fr;
        public float suspension_velocity_bl;
        public float suspension_velocity_br;
        public float suspension_velocity_fl;
        public float suspension_velocity_fr;
        public float wheel_patch_speed_bl;
        public float wheel_patch_speed_br;
        public float wheel_patch_speed_fl;
        public float wheel_patch_speed_fr;
        public float throttle_input;
        public float steering_input;
        public float brake_input;
        public float clutch_input;
        public float gear;
        public float gforce_lateral;
        public float gforce_longitudinal;
        public float lap;
        public float engine_rate;
        public float native_sli_support;
        public float race_position;
        public float kers_level;
        public float kers_level_max;
        public float drs;
        public float traction_control;
        public float abs;
        public float fuel_in_tank;
        public float fuel_capacity;
        public float in_pits;
        public float race_sector;
        public float sector_time_1;
        public float sector_time_2;
        public float brake_temp_bl;
        public float brake_temp_br;
        public float brake_temp_fl;
        public float brake_temp_fr;
        public float tyre_pressure_bl;
        public float tyre_pressure_br;
        public float tyre_pressure_fl;
        public float tyre_pressure_fr;
        public float laps_completed;
        public float total_laps;
        public float track_length;
        public float last_lap_time;
        public float max_rpm;
        public float idle_rpm;
        public float max_gears;

        public byte[] ToByteArray()
        {
            DirtRally2UDPTelemetry packet = this;
            int num = Marshal.SizeOf<DirtRally2UDPTelemetry>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<DirtRally2UDPTelemetry>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

    }
}
