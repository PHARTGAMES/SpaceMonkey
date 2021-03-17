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

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Stage
    {
        // Token: 0x04000040 RID: 64
        public int index_;

        // Token: 0x04000041 RID: 65
        public float progress_;

        // Token: 0x04000042 RID: 66
        public float raceTime_;

        // Token: 0x04000043 RID: 67
        public float driveLineLocation_;

        // Token: 0x04000044 RID: 68
        public float distanceToEnd_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct InputControl
    {
        // Token: 0x04000038 RID: 56
        public float steering_;

        // Token: 0x04000039 RID: 57
        public float throttle_;

        // Token: 0x0400003A RID: 58
        public float brake_;

        // Token: 0x0400003B RID: 59
        public float handbrake_;

        // Token: 0x0400003C RID: 60
        public float clutch_;

        // Token: 0x0400003D RID: 61
        public int gear_;

        // Token: 0x0400003E RID: 62
        public float footbrakePressure_;

        // Token: 0x0400003F RID: 63
        public float handbrakePressure_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BrakeDisk
    {
        // Token: 0x04000010 RID: 16
        public float layerTemperature_;

        // Token: 0x04000011 RID: 17
        public float temperature_;

        // Token: 0x04000012 RID: 18
        public float wear_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Car
    {
        // Token: 0x04000029 RID: 41
        public int index_;

        // Token: 0x0400002A RID: 42
        public float speed_;

        // Token: 0x0400002B RID: 43
        public float positionX_;

        // Token: 0x0400002C RID: 44
        public float positionY_;

        // Token: 0x0400002D RID: 45
        public float positionZ_;

        // Token: 0x0400002E RID: 46
        public float roll_;

        // Token: 0x0400002F RID: 47
        public float pitch_;

        // Token: 0x04000030 RID: 48
        public float yaw_;

        // Token: 0x04000031 RID: 49
        public Motion velocities_;

        // Token: 0x04000032 RID: 50
        public Motion accelerations_;

        // Token: 0x04000033 RID: 51
        public Engine engine_;

        // Token: 0x04000034 RID: 52
        public Suspension suspensionLF_;

        // Token: 0x04000035 RID: 53
        public Suspension suspensionRF_;

        // Token: 0x04000036 RID: 54
        public Suspension suspensionLB_;

        // Token: 0x04000037 RID: 55
        public Suspension suspensionRB_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Damper
    {
        // Token: 0x04000015 RID: 21
        public float damage_;

        // Token: 0x04000016 RID: 22
        public float pistonVelocity_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Engine
    {
        // Token: 0x0400001F RID: 31
        public float rpm_;

        // Token: 0x04000020 RID: 32
        public float radiatorCoolantTemperature_;

        // Token: 0x04000021 RID: 33
        public float engineCoolantTemperature_;

        // Token: 0x04000022 RID: 34
        public float engineTemperature_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Motion
    {
        // Token: 0x04000023 RID: 35
        public float surge_;

        // Token: 0x04000024 RID: 36
        public float sway_;

        // Token: 0x04000025 RID: 37
        public float heave_;

        // Token: 0x04000026 RID: 38
        public float roll_;

        // Token: 0x04000027 RID: 39
        public float pitch_;

        // Token: 0x04000028 RID: 40
        public float yaw_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Suspension
    {
        // Token: 0x04000017 RID: 23
        public float springDeflection_;

        // Token: 0x04000018 RID: 24
        public float rollbarForce_;

        // Token: 0x04000019 RID: 25
        public float springForce_;

        // Token: 0x0400001A RID: 26
        public float damperForce_;

        // Token: 0x0400001B RID: 27
        public float strutForce_;

        // Token: 0x0400001C RID: 28
        public int helperSpringIsActive_;

        // Token: 0x0400001D RID: 29
        public Damper damper_;

        // Token: 0x0400001E RID: 30
        public Wheel wheel_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Tire
    {
        // Token: 0x04000003 RID: 3
        public float pressure_;

        // Token: 0x04000004 RID: 4
        public float temperature_;

        // Token: 0x04000005 RID: 5
        public float carcassTemperature_;

        // Token: 0x04000006 RID: 6
        public float treadTemperature_;

        // Token: 0x04000007 RID: 7
        public uint currentSegment_;

        // Token: 0x04000008 RID: 8
        public TireSegment segment1_;

        // Token: 0x04000009 RID: 9
        public TireSegment segment2_;

        // Token: 0x0400000A RID: 10
        public TireSegment segment3_;

        // Token: 0x0400000B RID: 11
        public TireSegment segment4_;

        // Token: 0x0400000C RID: 12
        public TireSegment segment5_;

        // Token: 0x0400000D RID: 13
        public TireSegment segment6_;

        // Token: 0x0400000E RID: 14
        public TireSegment segment7_;

        // Token: 0x0400000F RID: 15
        public TireSegment segment8_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TireSegment
    {
        // Token: 0x04000001 RID: 1
        public float temperature_;

        // Token: 0x04000002 RID: 2
        public float wear_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Wheel
    {
        // Token: 0x04000013 RID: 19
        public BrakeDisk brakeDisk_;

        // Token: 0x04000014 RID: 20
        public Tire tire_;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class RBRAPI
    {

        public uint totalSteps_;

        // Token: 0x04000046 RID: 70
        public Stage stage_;

        // Token: 0x04000047 RID: 71
        public InputControl control_;

        // Token: 0x04000048 RID: 72
        public Car car_;


        /*
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
            RBRAPI packet = this;
            int num = Marshal.SizeOf<RBRAPI>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<RBRAPI>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
        */
    }


}
