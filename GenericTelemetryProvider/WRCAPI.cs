using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace WRCAPI
{
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
