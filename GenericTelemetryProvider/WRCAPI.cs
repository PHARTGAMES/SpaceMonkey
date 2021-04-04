using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace WRCAPI
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct WRCWheelData
    {
        public float pos1X; 
        public float pos1Y;
        public float pos1Z; //higher than pos2, Z is UP!
        public float f1;
        public float f2;
        public float f3;
        public float f4;
        public float f5;
        public float f6;
        public float f7;
        public float f8;
        public float f9;
        public float f10;
        public float f11;
        public float f12;
        public float pos2X;
        public float pos2Y;
        public float pos2Z;
        public float f13;
        public float f14;
        public float f15;
        public float f16;
        public float f17;
        public float f18;
        public float f19;
        public float fBigValue; //3310 etc???
        public float orientA; // could be a quat but there's 5 values here....
        public float orientB;
        public float orientC;
        public float orientD;
        public float orientE; // last one is something else?
        public float f25;
        public float f26;
        public float f27;
        public float f28;
        public float f29;
        public float wheelSpeed;
        public float f31;
        public float f32;
        public float f33;


    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public class WRCWheelGroupData
    {
        public WRCWheelData wheelFL;
        public WRCWheelData wheelFR;
        public WRCWheelData wheelRL;
        public WRCWheelData wheelRR;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class WRCData
    {
        public UInt32 sequence_number; // Odd while game is updating shared memory
        public UInt32 version; // Version of this struct

        // Version 1:
        public int gear; // Neutral = 1, First = 2, ...
        public float velocityX; // Left, up, forward [m/s]
        public float velocityY; // Left, up, forward [m/s]
        public float velocityZ; // Left, up, forward [m/s]
        public float accelerationX; // Left, up, forward [m/s^2]
        public float accelerationY; // Left, up, forward [m/s^2]
        public float accelerationZ; // Left, up, forward [m/s^2]
        public int engine_idle_rpm; // [rpm/10]
        public int engine_max_rpm; // [rpm/10]
        public int engine_rpm; // [rpm/10]
        public float suspension_travelLF; // It can move this much (LF,LR,RR,RF) [m]
        public float suspension_travelLR; // It can move this much (LF,LR,RR,RF) [m]
        public float suspension_travelRR; // It can move this much (LF,LR,RR,RF) [m]
        public float suspension_travelRF; // It can move this much (LF,LR,RR,RF) [m]
        public float suspension_positionLF; // From zero to `suspension_travel` [m]
        public float suspension_positionLR; // From zero to `suspension_travel` [m]
        public float suspension_positionRR; // From zero to `suspension_travel` [m]
        public float suspension_positionRF; // From zero to `suspension_travel` [m]
        public float unknown1; // (LF,LR,RR,RF) [?]
        public float unknown2; // (LF,LR,RR,RF) [?]
        public float unknown3; // (LF,LR,RR,RF) [?]
        public float unknown4; // (LF,LR,RR,RF) [?]

        public byte[] ToByteArray()
        {
            WRCData packet = this;
            int num = Marshal.SizeOf<WRCData>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<WRCData>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
    }
}
