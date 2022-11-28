using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace TCATelemetry
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Ansi)]
    public class TCAData
    {
        public uint packetId;

        public float posX;
        public float posY;
        public float posZ;

        public float pitch;
        public float yaw;
        public float roll;

        public float velX;
        public float velY;
        public float velZ;

        public float accelX;
        public float accelY;
        public float accelZ;

        public float pitchVel;
        public float yawVel;
        public float rollVel;

        public float pitchAccel;
        public float yawAccel;
        public float rollAccel;

        public bool paused;
        public float dt;

        public float engineRPM;


        public byte[] ToByteArray()
        {
            TCAData packet = this;
            int num = Marshal.SizeOf<TCAData>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<TCAData>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;

        }
    }
}
