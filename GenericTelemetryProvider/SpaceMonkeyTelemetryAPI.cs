using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GenericTelemetryProvider
{

    // Managed representation of the native SpaceMonkeyTelemetryFrameData structure.
//    [StructLayout(LayoutKind.Sequential)]
    [StructLayout(LayoutKind.Sequential, Pack = 0, CharSet = CharSet.Ansi)]
    public struct SpaceMonkeyTelemetryFrameData
    {
        public int m_version;         // Version number
        public double m_time;         // Absolute time (for calculating frame time delta)
        public double m_posX;         // World position X (meters)
        public double m_posY;         // World position Y (meters)
        public double m_posZ;         // World position Z (meters)
        public double m_fwdX;         // World forward direction X
        public double m_fwdY;         // World forward direction Y
        public double m_fwdZ;         // World forward direction Z
        public double m_upX;          // World up direction X
        public double m_upY;          // World up direction Y
        public double m_upZ;          // World up direction Z
        public float m_idleRPM;       // Engine idle RPM
        public float m_maxRPM;        // Engine max RPM
        public float m_rpm;           // Engine RPM
        public float m_gear;          // Gear
        public float m_throttleInput; // Throttle input (0 to 1)
        public float m_brakeInput;    // Brake input (0 to 1)
        public float m_steeringInput; // Steering input (-1 to 1)
        public float m_clutchInput;   // Clutch input (0 to 1)
    }

    // Managed wrapper for the native SpaceMonkeyTelemetryAPI.
    public class SpaceMonkeyTelemetryAPI : IDisposable
    {
        // Pointer to the native instance.
        private IntPtr nativeHandle;

        // Import the native functions from the DLL.
        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SpaceMonkeyTelemetryAPI_Create();

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_Destroy(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_InitSendSharedMemory(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_SendFrame(IntPtr instance, ref SpaceMonkeyTelemetryFrameData frame);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_RecieveFrame(IntPtr instance, ref SpaceMonkeyTelemetryFrameData frame);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_Deinit(IntPtr instance);



        // Constructor: Creates the native instance.
        public SpaceMonkeyTelemetryAPI()
        {
            nativeHandle = SpaceMonkeyTelemetryAPI_Create();
            if (nativeHandle == IntPtr.Zero)
            {
                throw new Exception("Failed to create native SpaceMonkeyTelemetryAPI instance.");
            }
        }

        // Initialize the shared memory for sending.
        public void InitSendSharedMemory()
        {
            SpaceMonkeyTelemetryAPI_InitSendSharedMemory(nativeHandle);
        }

        // Initialize the shared memory for receiving.
        public void InitRecieveSharedMemory()
        {
            SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(nativeHandle);
        }

        // Send a telemetry frame.
        public void SendFrame(ref SpaceMonkeyTelemetryFrameData frame)
        {
            SpaceMonkeyTelemetryAPI_SendFrame(nativeHandle, ref frame);
        }

        // Receive a telemetry frame.
        public void RecieveFrame(ref SpaceMonkeyTelemetryFrameData frame)
        {
            SpaceMonkeyTelemetryAPI_RecieveFrame(nativeHandle, ref frame);
        }

        // Deinitialize the API.
        public void Deinit()
        {
            SpaceMonkeyTelemetryAPI_Deinit(nativeHandle);
        }

        // Dispose pattern to free native resources.
        public void Dispose()
        {
            if (nativeHandle != IntPtr.Zero)
            {
                SpaceMonkeyTelemetryAPI_Destroy(nativeHandle);
                nativeHandle = IntPtr.Zero;
            }
            GC.SuppressFinalize(this);
        }

        ~SpaceMonkeyTelemetryAPI()
        {
            Dispose();
        }
    }

}
