using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GenericTelemetryProvider
{
    public class SpaceMonkeyTelemetryAPI : IDisposable
    {
        // Pointer to the native instance.
        private IntPtr nativeHandle;

        // Import the native functions from the DLL.
        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SpaceMonkeyTelemetryAPI_Create();

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_Destroy(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr SpaceMonkeyTelemetryAPI_InitSendSharedMemory(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string packetFormatPath);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(IntPtr instance, [MarshalAs(UnmanagedType.LPStr)] string packetFormatPath);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_SendFrame(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_RecieveFrame(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_Deinit(IntPtr instance);

        [DllImport("SpaceMonkeyTelemetryAPI.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SpaceMonkeyTelemetryAPI_SetPacket(IntPtr instance, IntPtr packet);

        



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
        public void InitSendSharedMemory(string packetFormatPath)
        {
            SpaceMonkeyTelemetryAPI_InitSendSharedMemory(nativeHandle, packetFormatPath);
        }

        // Initialize the shared memory for receiving.
        public void InitRecieveSharedMemory(string packetFormatPath)
        {
            SpaceMonkeyTelemetryAPI_InitRecieveSharedMemory(nativeHandle, packetFormatPath);
        }

        public void SetPacket(IntPtr packet)
        {
            SpaceMonkeyTelemetryAPI_SetPacket(nativeHandle, packet);
        }

        // Send a telemetry frame.
        public void SendFrame()
        {
            SpaceMonkeyTelemetryAPI_SendFrame(nativeHandle);
        }

        // Receive a telemetry frame.
        public void RecieveFrame()
        {
            SpaceMonkeyTelemetryAPI_RecieveFrame(nativeHandle);
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
