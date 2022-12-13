//MIT License
//
//Copyright(c) 2019 PHARTGAMES
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
using System.Runtime.InteropServices;
using System.Numerics;

namespace GenericTelemetryProvider
{

    /// <summary>
    /// The data packet for sending over udp + some named properties 
    /// for human friendly mapping and stateless calculations
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class BNGAPI
    {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] magic;

        //World position of the car
        public float posX;
        public float posY;
        public float posZ;

        //Velocity of the car
        public float velX;
        public float velY;
        public float velZ;

        //Acceleration of the car, gravity not included
        public float accX;
        public float accY;
        public float accZ;

        //Vector components of a vector pointing "up" relative to the car
        public float upVecX;
        public float upVecY;
        public float upVecZ;

        //Roll, pitch and yaw positions of the car
        public float rollPos;
        public float pitchPos;
        public float yawPos;

        //Roll, pitch and yaw "velocities" of the car
        public float rollRate;
        public float pitchRate;
        public float yawRate;

        //Roll, pitch and yaw "accelerations" of the car
        public float rollAcc;
        public float pitchAcc;
        public float yawAcc;

        //engine
        public float engine_rate;
        public float idle_rpm;
        public float max_rpm;

        //gears
        public int gear;
        public int max_gears;

        //suspension 
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

        public float timeStamp;


        public void CopyFields(BNGAPI other)
        {
            posX = other.posX;
            posY = other.posY;
            posZ = other.posZ;

            velX = other.velX;
            velY = other.velY;
            velZ = other.velZ;

            accX = other.accX;
            accY = other.accY;
            accZ = other.accZ;

            upVecX = other.upVecX;
            upVecY = other.upVecY;
            upVecZ = other.upVecZ;

            rollPos = other.rollPos;
            pitchPos = other.pitchPos;
            yawPos = other.yawPos;

            rollRate = other.rollRate;
            pitchRate = other.pitchRate;
            yawRate = other.yawRate;

            rollAcc = other.rollAcc;
            pitchAcc = other.pitchAcc;
            yawAcc = other.yawAcc;


            engine_rate = other.engine_rate;
            idle_rpm = other.idle_rpm;
            max_rpm = other.max_rpm;

            gear = other.gear;
            max_gears = other.max_gears;

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

            timeStamp = other.timeStamp;

        }

        public void Reset()
        {
            posX = 0;
            posY = 0;
            posZ = 0;

            velX = 0;
            velY = 0;
            velZ = 0;

            accX = 0;
            accY = 0;
            accZ = 0;

            upVecX = 0;
            upVecY = 0;
            upVecZ = 0;

            rollPos = 0;
            pitchPos = 0;
            yawPos = 0;

            rollRate = 0;
            pitchRate = 0;
            yawRate = 0;

            rollAcc = 0;
            pitchAcc = 0;
            yawAcc = 0;


            engine_rate = 0;
            idle_rpm = 0;
            max_rpm = 0;

            gear = 0;
            max_gears = 0;

            suspension_position_bl = 0;
            suspension_position_br = 0;
            suspension_position_fl = 0;
            suspension_position_fr = 0;

            suspension_velocity_bl = 0;
            suspension_velocity_br = 0;
            suspension_velocity_fl = 0;
            suspension_velocity_fr = 0;

            suspension_acceleration_bl = 0;
            suspension_acceleration_br = 0;
            suspension_acceleration_fl = 0;
            suspension_acceleration_fr = 0;

            timeStamp = 0;

        }

        public float PitchAngle
        {
            get
            {
                return pitchPos * (180.0f / (float)Math.PI);
            }
        }
        public float RollAngle
        {
            get
            {
                return rollPos * (180.0f / (float)Math.PI);
            }
        }

        public float Heave
        {
            get
            {
                return accZ;
            }
        }

        public float Sway
        {
            get
            {
                return accX;
            }
        }

        public float Surge
        {
            get
            {
                return accY;
            }
        }


        public float CalculateSlipAngle()
        {
            Vector3 fwd = new Vector3(
            (float)Math.Sin(yawPos) * (float)Math.Cos(pitchPos),
            (float)Math.Cos(yawPos) * (float)Math.Cos(pitchPos),
            0.0f);

            fwd = Vector3.Normalize(fwd);

            float slipAngle = 0.0f;
            Vector3 velocity = new Vector3(velX, velY, 0.0f);
            float speedKPH = (float)velocity.Length() * 3.6f;
            if (speedKPH > 5)
            {
                Vector3 normVel = Vector3.Normalize(velocity);

                float angle = (float)Math.Acos(1.0f - Math.Max(0.0f, Vector3.Dot(fwd, normVel)));

                slipAngle = angle * yawRate;
            }

            return slipAngle;

        }
        public float SlipAngle
        {
            get
            {
                return yawAcc;
            }
        }

        public byte[] ToByteArray()
        {
            BNGAPI packet = this;
            int num = Marshal.SizeOf<BNGAPI>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<BNGAPI>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
    }


}
