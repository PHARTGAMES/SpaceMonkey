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
using GTAVAPI;


namespace GenericTelemetryProvider
{
    class GTAVTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public GTAVUI ui;
        GTAVData gtaData;
        MemoryMappedFile gtaDataMMF;
        Mutex gtaDataMutex;
        float fixedDT = 0.01f;
        Vector3 gtaVel = Vector3.Zero;


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

            if (gtaDataMMF != null)
                gtaDataMMF.Dispose();
            gtaDataMMF = null;
        }

        void ReadTelemetry()
        {

            StartSending();

            //wait for telemetry
            while (true)
            {
                try
                {
                    gtaDataMMF = MemoryMappedFile.OpenExisting("GTADataMMF");

                    if (gtaDataMMF != null)
                        break;
                    else
                        Thread.Sleep(1000);
                }
                catch (FileNotFoundException)
                {
                    Thread.Sleep(1000);
                }
            }

            //wait for mutex
            while (true)
            {
                try
                {
                    gtaDataMutex = Mutex.OpenExisting("GTADataMMFMutex");

                    if (gtaDataMutex != null)
                        break;
                    else
                        Thread.Sleep(1000);
                }
                catch (Exception)
                {
                    Thread.Sleep(1000);
                }
            }

            //read and process
            while (!IsStopped)
            {
                try
                {
                    gtaDataMutex.WaitOne();
                    using (MemoryMappedViewStream stream = gtaDataMMF.CreateViewStream())
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        byte[] readBuffer = reader.ReadBytes((int)stream.Length);

                        var alloc = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                        gtaData = (GTAVData)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(GTAVData));
                        alloc.Free();
                    }
                    gtaDataMutex.ReleaseMutex();

                    if(!gtaData.paused)
                    {
                        ProcessGTAData();
                    }

                    using (var sleeper = new ManualResetEvent(false))
                    {
                        sleeper.WaitOne((int)(1000.0f * fixedDT));
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();
        }

            

        void ProcessGTAData()
        {
            if (gtaData == null)
                return;

            transform = new Matrix4x4();
            transform.M11 = gtaData.m11;
            transform.M12 = gtaData.m13;
            transform.M13 = gtaData.m12;
            transform.M14 = 0.0f;// gtaData.m14;
            transform.M21 = gtaData.m31;
            transform.M22 = gtaData.m33;
            transform.M23 = gtaData.m32;
            transform.M24 = 0.0f;// gtaData.m24;
            transform.M31 = gtaData.m21;
            transform.M32 = gtaData.m23;
            transform.M33 = gtaData.m22;
            transform.M34 = 0.0f;// gtaData.m34;
            transform.M41 = gtaData.m41;
            transform.M42 = gtaData.m43;
            transform.M43 = gtaData.m42;
            transform.M44 = 1.0f;// gtaData.m44;

            gtaVel = new Vector3(gtaData.velX, gtaData.velZ, gtaData.velY);

            ProcessTransform(transform, gtaData.dt);
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
            return base.ExtractFwdUpRht();

        }

        public override bool CheckLastFrameValid()
        {
            return base.CheckLastFrameValid();
        }

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.008f;
        }

        public override bool CalcPosition()
        {

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            lastRawPos = currRawPos;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            return true;
        }

        public override void CalcVelocity()
        {
            Vector3 worldVelocity = gtaVel;
            lastWorldVelocity = gtaVel;

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

            localVelocity.X = -localVelocity.X;

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;

            rawData.local_velocity_x = -(float)rawData.local_velocity_x;
        }

        public override void FilterVelocity()
        {
            //filter local velocity
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, velKeyMask, false);
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
            rawData.max_rpm = 6000;
            rawData.max_gears = gtaData.gears;
            rawData.gear = gtaData.gear;
            rawData.idle_rpm = 700;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

            filteredData.engine_rate = Math.Max(700, Math.Min(6000, 700 + (gtaData.engineRPM * (6000-700))));
        }


    }

}
