using System;
using System.Runtime.InteropServices;

namespace GTAVAPI
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class GTAVData
    {
        public float m11;
        public float m12;
        public float m13;
        public float m14;

        public float m21;
        public float m22;
        public float m23;
        public float m24;

        public float m31;
        public float m32;
        public float m33;
        public float m34;

        public float m41;
        public float m42;
        public float m43;
        public float m44;

        public bool paused;
        public bool inVehicle;
        public float dt;

        public float velX;
        public float velY;
        public float velZ;

        public int gear;
        public int gears;
        public float engineRPM;

        public byte[] ToByteArray()
        {
            GTAVData packet = this;
            int num = Marshal.SizeOf<GTAVData>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<GTAVData>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }


    }



}
