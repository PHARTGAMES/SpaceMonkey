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
using OpenMotion;


namespace GenericTelemetryProvider
{
    class OpenMotionTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public OpenMotionUI ui;
        OpenMotionAPI frameData;
        MemoryMappedFile dataMMF;
        Mutex dataMutex;
        float lastFrameTime = 0.0f;


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

            if (dataMMF != null)
                dataMMF.Dispose();
            dataMMF = null;
        }

        void ReadTelemetry()
        {

            StartSending();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            Stopwatch processSW = new Stopwatch();


            //wait for telemetry
            while (true)
            {
                try
                {
                    dataMMF = MemoryMappedFile.OpenExisting("OM_FRAME");

                    if (dataMMF != null)
                    {
                        break;
                    }
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
                    dataMutex = Mutex.OpenExisting("OM_FRAME_MUTEX");

                    if (dataMutex != null)
                    {
                        ui.StatusTextChanged("Ready");
                        break;
                    }
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
                    processSW.Restart();
                    dataMutex.WaitOne();
                    using (MemoryMappedViewStream stream = dataMMF.CreateViewStream())
                    {
                        BinaryReader reader = new BinaryReader(stream);
                        byte[] readBuffer = reader.ReadBytes((int)stream.Length);

                        var alloc = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                        frameData = (OpenMotionAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(OpenMotionAPI));
                        alloc.Free();
                    }
                    dataMutex.ReleaseMutex();

                    sw.Restart();

                    if (frameData != null)
                    {
                        if (lastFrameTime == 0.0f && frameData.m_time != 0.0f)
                        {
                            lastFrameTime = frameData.m_time;
                            continue;
                        }

                        float calcDT = frameData.m_time - lastFrameTime;

                        if (calcDT != 0)
                        {
                            lastFrameTime = frameData.m_time;

                            ProcessFrameData(calcDT);

                            using (var sleeper = new ManualResetEvent(false))
                            {
                                int processTime = (int)processSW.ElapsedMilliseconds;
                                //sleeper.WaitOne(Math.Max(0, (int)updateDelay - processTime));
                                sleeper.WaitOne(1);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();

            Thread.CurrentThread.Join();
        }

            

        void ProcessFrameData(float _dt)
        {
            if (frameData == null)
                return;
            
            transform = new Matrix4x4();
            //transform = Matrix4x4.CreateFromYawPitchRoll(frameData.m_rotYaw, frameData.m_rotPitch, frameData.m_rotRoll);

            transform = Matrix4x4.CreateRotationZ(frameData.m_rotPitch);
            transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationX(frameData.m_rotRoll), transform);
            transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationY(frameData.m_rotYaw), transform);

            transform.Translation = new Vector3(frameData.m_posX, frameData.m_posY, frameData.m_posZ);

            ProcessTransform(transform, _dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n frametime: " + frameData.m_time + "\n rht: " + rht.X + ", " + rht.Y + ", " + rht.Z + "\n up: " + up.X + ", " + up.Y + ", " + up.Z + "\n fwd: " + fwd.X + ", " + fwd.Y + ", " + fwd.Z);

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
            //if (dt <= 0)
            //    dt = 0.015f;
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

            //assign
            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            return true;
        }

        public override void CalcVelocity()
        {
            Vector3 worldVelocity = (worldPosition - lastPosition) / dt;
            lastWorldVelocity = worldVelocity;

            lastPosition = transform.Translation = worldPosition;

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            //transform world velocity to local space
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;
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
            rawData.roll = Utils.LoopAngleRad(-pyr.Z, (float)Math.PI * 0.5f);
        }

        public override void SimulateEngine()
        {
            rawData.max_rpm = 6000;
            rawData.max_gears = 6;
            rawData.gear = 1;
            rawData.idle_rpm = 700;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

            filteredData.engine_rate = 700;// Math.Max(700, Math.Min(6000, 700 + (frameData.engineRPM * (6000-700))));
        }




    }

}
