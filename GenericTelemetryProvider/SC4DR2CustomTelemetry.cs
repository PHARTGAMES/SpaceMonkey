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
    public class SC4DR2CustomTelemetry
    {
        public float total_time;
        public float paused;
        public float yaw;
        public float pitch;
        public float roll;
        public float yaw_velocity;
        public float pitch_velocity;
        public float roll_velocity;
        public float yaw_acceleration;
        public float pitch_acceleration;
        public float roll_acceleration;
        public float position_x;
        public float position_y;
        public float position_z;
        public float local_velocity_x;
        public float local_velocity_y;
        public float local_velocity_z;
        public float gforce_lateral;
        public float gforce_longitudinal;
        public float gforce_vertical;
        public float speed;
        public float suspension_position_bl;
        public float suspension_position_br;
        public float suspension_position_fl;
        public float suspension_position_fr;
        public float suspension_velocity_bl;
        public float suspension_velocity_br;
        public float suspension_velocity_fl;
        public float suspension_velocity_fr;
        public float suspension_acceleration_bl;
        public float suspension_acceleration_br;
        public float suspension_acceleration_fl;
        public float suspension_acceleration_fr;
        public float wheel_patch_speed_bl;
        public float wheel_patch_speed_br;
        public float wheel_patch_speed_fl;
        public float wheel_patch_speed_fr;
        public float throttle_input;
        public float steering_input;
        public float brake_input;
        public float clutch_input;
        public float gear;
        public float max_gears;
        public float engine_rate;
        public float race_position;
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
        public float lap;
        public float laps_completed;
        public float total_laps;
        public float lap_time;
        public float lap_distance;
        public float track_length;
        public float last_lap_time;
        public float max_rpm;
        public float idle_rpm;

        /*
          <float scale="1.0" channel="total_time" />
          <float scale="1.0" channel="paused" />
          <float scale="1.0" channel="yaw" />
          <float scale="1.0" channel="pitch" />
          <float scale="1.0" channel="roll" />
          <float scale="1.0" channel="yaw_velocity" />
          <float scale="1.0" channel="pitch_velocity" />
          <float scale="1.0" channel="roll_velocity" />
          <float scale="1.0" channel="yaw_acceleration" />
          <float scale="1.0" channel="pitch_acceleration" />
          <float scale="1.0" channel="roll_acceleration" />
          <float scale="1.0" channel="position_x" />
          <float scale="1.0" channel="position_y" />
          <float scale="1.0" channel="position_z" />  
          <float scale="1.0" channel="local_velocity_x" />
          <float scale="1.0" channel="local_velocity_y" />
          <float scale="1.0" channel="local_velocity_z" />
          <float scale="1.0" channel="gforce_lateral" />
          <float scale="1.0" channel="gforce_longitudinal" />
          <float scale="1.0" channel="gforce_vertical" />
          <float scale="1.0" channel="speed" />
          <float scale="1000.0" channel="suspension_position_bl" />
          <float scale="1000.0" channel="suspension_position_br" />
          <float scale="1000.0" channel="suspension_position_fl" />
          <float scale="1000.0" channel="suspension_position_fr" />
          <float scale="1000.0" channel="suspension_velocity_bl" />
          <float scale="1000.0" channel="suspension_velocity_br" />
          <float scale="1000.0" channel="suspension_velocity_fl" />
          <float scale="1000.0" channel="suspension_velocity_fr" />
          <float scale="1000.0" channel="suspension_acceleration_bl" />
          <float scale="1000.0" channel="suspension_acceleration_br" />
          <float scale="1000.0" channel="suspension_acceleration_fl" />
          <float scale="1000.0" channel="suspension_acceleration_fr" />
          <float scale="1.0" channel="wheel_patch_speed_bl" />
          <float scale="1.0" channel="wheel_patch_speed_br" />
          <float scale="1.0" channel="wheel_patch_speed_fl" />
          <float scale="1.0" channel="wheel_patch_speed_fr" />
          <float scale="1.0" channel="throttle_input" />
          <float scale="1.0" channel="steering_input" />
          <float scale="1.0" channel="brake_input" />
          <float scale="1.0" channel="clutch_input" />
          <float scale="1.0" channel="gear" />
          <float scale="1.0" channel="max_gears" />
          <float scale="1.0" channel="engine_rate" />
          <float scale="1.0" channel="race_position" />
          <float scale="1.0" channel="race_sector" />
          <float scale="1.0" channel="sector_time_1" />
          <float scale="1.0" channel="sector_time_2" />
          <float scale="1.0" channel="brake_temp_bl" />
          <float scale="1.0" channel="brake_temp_br" />
          <float scale="1.0" channel="brake_temp_fl" />
          <float scale="1.0" channel="brake_temp_fr" />
          <float scale="1.0" channel="tyre_pressure_bl" />
          <float scale="1.0" channel="tyre_pressure_br" />
          <float scale="1.0" channel="tyre_pressure_fl" />
          <float scale="1.0" channel="tyre_pressure_fr" />
          <float scale="1.0" channel="lap" />
          <float scale="1.0" channel="laps_completed" />
          <float scale="1.0" channel="total_laps" />
          <float scale="1.0" channel="lap_time" />
          <float scale="1.0" channel="lap_distance" />  
          <float scale="1.0" channel="track_length" />
          <float scale="1.0" channel="last_lap_time" />
          <float scale="1.0" channel="max_rpm" />
          <float scale="1.0" channel="idle_rpm" /> 

         */
        public byte[] ToByteArray()
        {
            SC4DR2CustomTelemetry packet = this;
            int num = Marshal.SizeOf<SC4DR2CustomTelemetry>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<SC4DR2CustomTelemetry>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }

        public void Copy(SC4DR2CustomTelemetry other)
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

    }

    }
}
