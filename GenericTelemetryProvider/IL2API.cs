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
    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Unicode)]
    public class IL2API
    {

        public uint packetID;
        public uint tick;
        public float yaw;
        public float pitch;
        public float roll;
        public float spinX;
        public float spinY;
        public float spinZ;
        public float accX;
        public float accY;
        public float accZ;

        public void CopyFields(IL2API other)
        {
            packetID = other.packetID;
            tick = other.tick;
            yaw = other.yaw;
            pitch = other.pitch;
            roll = other.roll;
            spinX = other.spinX;
            spinY = other.spinY;
            spinZ = other.spinZ;
            accX = other.accX;
            accY = other.accY;
            accZ = other.accZ;
        }

        public void Reset()
        {
            packetID = 0;
            tick = 0;
            yaw = 0;
            pitch = 0;
            roll = 0;
            spinX = 0;
            spinY = 0;
            spinZ = 0;
            accX = 0;
            accY = 0;
            accZ = 0;
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
                return accY;
            }
        }

        public float Surge
        {
            get
            {
                return accX;
            }
        }

        public byte[] ToByteArray()
        {
            IL2API packet = this;
            int num = Marshal.SizeOf<IL2API>(packet);
            byte[] array = new byte[num];
            IntPtr intPtr = Marshal.AllocHGlobal(num);
            Marshal.StructureToPtr<IL2API>(packet, intPtr, false);
            Marshal.Copy(intPtr, array, 0, num);
            Marshal.FreeHGlobal(intPtr);
            return array;
        }
    }


}
