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


            Stopwatch sw = new Stopwatch();
            sw.Start();

            //read and process
            while (!IsStopped)
            {
                try
                {
                    double timeNow = sw.Elapsed.TotalSeconds;

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

                            //while ((sw.Elapsed.TotalSeconds - lastSWTime) < ((1.0/60.0))) { }
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
            transform = Matrix4x4.CreateFromYawPitchRoll(frameData.m_rotYaw, frameData.m_rotPitch, frameData.m_rotRoll);

            //transform = Matrix4x4.CreateRotationY(frameData.m_rotYaw);
            //transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationX(frameData.m_rotPitch), transform);
            //transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationZ(frameData.m_rotRoll), transform);


            transform.Translation = new Vector3(frameData.m_posX, frameData.m_posY, frameData.m_posZ);

            ProcessTransform(transform, _dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n frametime: " + frameData.m_time + "\n Pitch: " + frameData.m_rotPitch + ", " + "\n Yaw: " + frameData.m_rotYaw + ", " + "\n Roll: " + frameData.m_rotRoll + ", " + "\n rht: " + rht.X + ", " + rht.Y + ", " + rht.Z + "\n up: " + up.X + ", " + up.Y + ", " + up.Z + "\n fwd: " + fwd.X + ", " + fwd.Y + ", " + fwd.Z);

            SendFilteredData();

            return true;
        }


        float Lerp(float from, float to, float lerp)
        {
            return from + ((to - from) * lerp);
        }


        public override void CalcPosition()
        {
            currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            //assign
            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);
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

            rawData.pitch = -pyr.X;
            rawData.yaw = -pyr.Y;
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

        public override void SimulateSuspension()
        {

        }


        public override void CalcAngularVelocityAndAccel()
        {

            //local non gimbal locked version
            Matrix4x4 lastTransformLocal = Matrix4x4.Multiply(lastTransform, rotInv);

            Vector3 lastRht = new Vector3(lastTransformLocal.M11, lastTransformLocal.M12, lastTransformLocal.M13);
            Vector3 lastUp = new Vector3(lastTransformLocal.M21, lastTransformLocal.M22, lastTransformLocal.M23);
            Vector3 lastFwd = new Vector3(lastTransformLocal.M31, lastTransformLocal.M32, lastTransformLocal.M33);

            Vector3 fwdProjX = Vector3.Normalize(new Vector3(0.0f, lastFwd.Y, lastFwd.Z));
            Vector3 fwdProjY = Vector3.Normalize(new Vector3(lastFwd.X, 0.0f, lastFwd.Z));
            Vector3 rhtProjZ = Vector3.Normalize(new Vector3(lastRht.X, lastRht.Y, 0.0f));

            Vector3 localRht = new Vector3(1.0f, 0.0f, 0.0f);
            Vector3 localUp = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 localFwd = new Vector3(0.0f, 0.0f, 1.0f);

            //angle * direction
            float yawVel = (float)Math.Acos((double)Vector3.Dot(fwdProjY, localFwd)) * Math.Sign(Vector3.Dot(lastFwd, localRht)) / dt;
            float pitchVel = (float)Math.Acos((double)Vector3.Dot(fwdProjX, localFwd)) * Math.Sign(Vector3.Dot(lastUp, localFwd)) / dt;
            float rollVel = (float)Math.Acos((double)Vector3.Dot(rhtProjZ, localRht)) * Math.Sign(Vector3.Dot(lastUp, localRht)) / dt;

            rawData.yaw_velocity = yawVel;
            rawData.pitch_velocity = pitchVel;
            rawData.roll_velocity = rollVel;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, angVelKeyMask, false);

            rawData.yaw_acceleration = ((float)filteredData.yaw_velocity - (float)lastFilteredData.yaw_velocity) / dt;
            rawData.pitch_acceleration = ((float)filteredData.pitch_velocity - (float)lastFilteredData.pitch_velocity) / dt;
            rawData.roll_acceleration = ((float)filteredData.roll_velocity - (float)lastFilteredData.roll_velocity) / dt;


            //world gimbal locked version
            /*
                        rawData.yaw_velocity = Utils.CalculateAngularChange((float) lastFilteredData.yaw, (float) filteredData.yaw) / dt;
                        rawData.pitch_velocity = Utils.CalculateAngularChange((float) lastFilteredData.pitch, (float) filteredData.pitch) / dt;
                        rawData.roll_velocity = Utils.CalculateAngularChange((float) lastFilteredData.roll, (float) filteredData.roll) / dt;

                        FilterModuleCustom.Instance.Filter(rawData, ref filteredData, angVelKeyMask, false);

                        rawData.yaw_acceleration = ((float) filteredData.yaw_velocity - (float) lastFilteredData.yaw_velocity) / dt;
                        rawData.pitch_acceleration = ((float) filteredData.pitch_velocity - (float) lastFilteredData.pitch_velocity) / dt;
                        rawData.roll_acceleration = ((float) filteredData.roll_velocity - (float) lastFilteredData.roll_velocity) / dt;
            */

        }


    }

}
