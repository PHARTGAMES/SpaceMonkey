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

        public override bool ExtractFwdUpRht()
        {
            //            return base.ExtractFwdUpRht();

            return true;
        }

        public override bool CheckLastFrameValid()
        {
            return base.CheckLastFrameValid();
        }

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;
        }

        public override bool CalcPosition()
        {
            /*
            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            lastRawPos = currRawPos;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);
            */
            return true;
        }

        public override void CalcVelocity()
        {
            /*
            Vector3 worldVelocity = (worldPosition - lastPosition) / dt;
            lastWorldVelocity = worldVelocity;

            lastPosition = transform.Translation = worldPosition;

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            Matrix4x4 rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            //transform world velocity to local space
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

//            localVelocity.X = -localVelocity.X;

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;
*/
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
            rawData.max_rpm = data.engine_max_rpm;
            rawData.max_gears = 6;
            rawData.gear = data.gear;
            rawData.idle_rpm = data.engine_idle_rpm;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

            filteredData.engine_rate = Math.Max(700, Math.Min(6000, 700 + (data.engine_rpm * (6000-700))));
        }
    }
}
