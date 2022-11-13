using System;
using System.Runtime.InteropServices;

namespace WRCGenAPI
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class WRCGenData
    {
        public float m_time; // 0 Total Time(not reset after stage restart)
        public float m_lapTime; // 1 Current Lap/Stage Time(starts on Go!)
        public float m_lapDistance; //2 Current Lap/Stage Distance(meters)
        public float m_totalDistance; //3 ? (starts from 0) - if distance then not equal to above!
        public float m_x; // World space position 4 Position X
        public float m_y; // World space position 5 Position Y
        public float m_z; // World space position 6 Position Z
        public float m_speed; // 7 Velocity(Speed) [m/s]
        public float m_xv; // Velocity in world space 8 Velocity X
        public float m_yv; // Velocity in world space 9 Velocity Y
        public float m_zv; // Velocity in world space 10 Velocity Z
        public float m_xr; // World space right direction 11 Roll Vector X
        public float m_yr; // World space right direction 12 Roll Vector Y
        public float m_zr; // World space right direction 13 Roll Vector Z
        public float m_xd; // World space forward direction 14 Pitch Vector X
        public float m_yd; // World space forward direction 15 Pitch Vector Y
        public float m_zd; // World space forward direction 16 Pitch Vector Z
        public float m_susp_pos_bl; // 17 Position of Suspension Rear Left
        public float m_susp_pos_br; // 18 Position of Suspension Rear Right
        public float m_susp_pos_fl; // 19 Position of Suspension Front Left
        public float m_susp_pos_fr; // 20 Position of Suspension Front Right
        public float m_susp_vel_bl; // 21 Velocity of Suspension Rear Left
        public float m_susp_vel_br; // 22 Velocity of Suspension Rear Right
        public float m_susp_vel_fl; // 23 Velocity of Suspension Front Left
        public float m_susp_vel_fr; // 24 Velocity of Suspension Front Right
        public float m_wheel_speed_bl; // 25 Velocity of Wheel Rear Left
        public float m_wheel_speed_br; // 26 Velocity of Wheel Rear Right
        public float m_wheel_speed_fl; // 27 Velocity of Wheel Front Left
        public float m_wheel_speed_fr; // 28 Velocity of Wheel Front Right
        public float m_throttle; // 29 Position Throttle
        public float m_steer; // 30 Position Steer
        public float m_brake; // 31 Position Brake
        public float m_clutch; // 32 Position Clutch
        public float m_gear; // 33 Gear[0 = Neutral, 1 = 1, 2 = 2, ..., -1 = Reverse]
        public float m_gforce_lat; // 34 G-Force Lateral
        public float m_gforce_lon; // 35 G-Force Longitudinal
        public float m_lap; // 36 Current Lap(rx only)
        public float m_engineRate; // 37 Engine Speed[rpm / 10]

        public float m_sli_pro_native_support; // SLI Pro support Not used
        public float m_car_position; //car race position 39 Current Position(rx only)
        public float m_kers_level; // kers energy left 40 Not used
        public float m_kers_max_level; // kers maximum energy 41 Not used
        public float m_drs; // 0 = off, 1 = on Not used
        public float m_traction_control; // 0 (off) - 2 (high) 43 Not used
        public float m_anti_lock_brakes; // 0 (off) - 1 (on) 44 Not used
        public float m_fuel_in_tank; //current fuel mass 45 Not used
        public float m_fuel_capacity; //fuel capacity 46 Not used

        public float m_in_pits; // 0 = none, 1 = pitting, 2 = in pit area Not used

        public float m_sector; // 0 = sector1, 1 = sector2; 2 = sector3 48 Sector
        public float m_sector1_time; // time of sector1(or 0) 49 Sector 1 time
        public float m_sector2_time; // time of sector2(or 0) 50 Sector 2 time

        public float m_brakes_temp0; // brakes temperature (centigrade) 51 Temperature Brake in C
        public float m_brakes_temp1; // brakes temperature (centigrade) 51 Temperature Brake in C
        public float m_brakes_temp2; // brakes temperature (centigrade) 51 Temperature Brake in C
        public float m_brakes_temp3; // brakes temperature (centigrade) 51 Temperature Brake in C

        public float m_wheels_pressure0; // wheels pressure PSI 52 Wheel pressure
        public float m_wheels_pressure1; // wheels pressure PSI 52 Wheel pressure
        public float m_wheels_pressure2; // wheels pressure PSI 52 Wheel pressure
        public float m_wheels_pressure3; // wheels pressure PSI 52 Wheel pressure

        public float m_team_info; // team ID 53 Not used
        public float m_total_laps; // total number of laps in this race 54 Total laps of the race(if SSS)
        public float m_track_size; // 55 Not used  track size meters
        public float m_last_lap_time; // last lap time 56 Last lap time(if SSS)
        public float m_max_rpm; // cars max RPM, at which point the rev limiter will kick in 57 Max rpm
        public float m_idle_rpm; // cars idle RPM 58 Idle rpm
        public float m_max_gears; // maximum number of gears 59 Number of gears
        public float m_sessionType; // 0 = unknown, 1 = practice, 2 = qualifying, 3 = race 60 Not used
        public float m_drsAllowed; // 0 = not allowed, 1 = allowed, -1 = invalid / unknown 61 Not used
        public float m_track_number; // -1 for unknown, 0-21 for tracks 62 Not used
        public float m_vehicleFIAFlags; // -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red

        public byte[] ToByteArray()
        {
            WRCGenData packet = this;
            int num = Marshal.SizeOf<WRCGenData>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<WRCGenData>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
    }
}
