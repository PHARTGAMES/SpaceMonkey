using System;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.MemoryMappedFiles;
using WRCAPI;
using GenericTelemetryProvider.Properties;
using System.Threading.Tasks;

namespace GenericTelemetryProvider
{
    class WRCTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public WRCUI ui;
        WRCData data;
         

        public override void Run()
        {
            base.Run();

            maxAccel2DMagSusp = 6.0f;
            telemetryPausedTime = 1.5f;

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
        }

        public override void Stop()
        {
            base.Stop();


        }

        private byte[] ReadBuffer(Stream memoryMappedViewStream, int size)
        {
            using (var reader = new BinaryReader(memoryMappedViewStream))
            {
                return reader.ReadBytes(size);
            }
        }

        protected Object ReadSharedMemory(Type T, string sharedMemoryFile)
        {
            Object data = null;
            int sharedMemorySize = Marshal.SizeOf(T);
            try
            {
                using (var mappedFile = MemoryMappedFile.OpenExisting(sharedMemoryFile))
                {
                    if (mappedFile == null)
                        return null;

                    using (var memoryMappedViewStream = mappedFile.CreateViewStream())
                    {
                        if (memoryMappedViewStream == null)
                            return null;

                        var buffer = ReadBuffer(memoryMappedViewStream, sharedMemorySize);
                        var alloc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                        data = Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), T);
                        memoryMappedViewStream.Close();
                        alloc.Free();
                    }
                }
            }
            catch
            {

            }
            return data;
        }

        void ReadTelemetry()
        {

            StartSending();

            Stopwatch sw = new Stopwatch();
            sw.Start();

             //read and process
            while (!IsStopped)
            {
                try
                {
                    double frameDT = 0;
                    while (true)
                    {
                        frameDT = sw.Elapsed.TotalSeconds;
                        if (frameDT >= (updateDelay / 1000.0f))
                            break;
                    }
                    sw.Restart();

                    data = (WRCData)ReadSharedMemory(typeof(WRCData), "Local\\WRC-8wSotWzFKAhBlbW10ZJBKaWMdWszbBXg");

                    if (data == null)
                        continue;

                    ProcessWRCData((float)frameDT);
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();
            Thread.CurrentThread.Join();

        }



        void ProcessWRCData(float _dt)
        {
            if (data == null)
                return;

            transform = Matrix4x4.Identity;


            ProcessTransform(transform, _dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

            SendFilteredData();

            return true;
        }


        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;
        }


        public override void CalcVelocity()
        {
            rawData.local_velocity_x = data.velocityX;
            rawData.local_velocity_y = data.velocityY;
            rawData.local_velocity_z = data.velocityZ;
        }

        public override void FilterVelocity()
        {
            //filter local velocity
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, velKeyMask, true);
        }

        public override void CalcAcceleration()
        {
            base.CalcAcceleration();
        }

        public override void CalcAngles()
        {
            Quaternion quat = Quaternion.CreateFromRotationMatrix(transform);

            Vector3 pyr = Utils.GetPYRFromQuaternion(quat);

            rawData.pitch = pyr.X;
            rawData.yaw = pyr.Y;
            rawData.roll = Utils.LoopAngleRad(pyr.Z, (float)Math.PI * 0.5f);
        }

        public override void SimulateEngine()
        {
            rawData.max_rpm = (float)data.engine_max_rpm;
            rawData.max_gears = 6.0f;
            rawData.gear = (float)data.gear;
            rawData.idle_rpm = (float)data.engine_idle_rpm;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

            filteredData.engine_rate = (float)Math.Max(700, Math.Min(6000, 700 + (data.engine_rpm * (6000-700))));
        }
    }
}
