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
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    class SMTTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public SMTUI ui;
        MemoryMappedFile dataMMF;
        Mutex dataMutex;
        double lastFrameTime = 0.0f;
        SpaceMonkeyTelemetryAPI SMTAPI;
        CMCustomUDPData frameData = new CMCustomUDPData();
        string packetFormatPath = "PacketFormats\\SpaceMonkeyTelemetryDefault.xml";
        Vector3 worldVelocity;


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

            if(SMTAPI != null)
            {
                SMTAPI.Deinit();
                SMTAPI = null;
            }
        }

        void ReadTelemetry()
        {

            StartSending();

            frameData.Init(packetFormatPath);
            
            SMTAPI = new SpaceMonkeyTelemetryAPI();
            SMTAPI.InitRecieveSharedMemory(packetFormatPath);
            var handle = GCHandle.Alloc(frameData.packet, GCHandleType.Pinned);
            SMTAPI.SetPacket(handle.AddrOfPinnedObject());

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //read and process
            while (!IsStopped)
            {
                try
                {
                    double timeNow = sw.Elapsed.TotalSeconds;

                    SMTAPI.RecieveFrame();
                    frameData.FromBytes(frameData.packet);

                    if ((lastFrameTime == 0.0f && (float)frameData.total_time != 0.0f) || (float)frameData.total_time < lastFrameTime)
                    {
                        lastFrameTime = (float)frameData.total_time;
                        continue;
                    }

                    double calcDT = (float)frameData.total_time - lastFrameTime;

                    if (calcDT > 0)
                    {
                        lastFrameTime = (float)frameData.total_time;

                        ProcessFrameData((float)calcDT);

//                        Debug.WriteLine($"calcDT = {calcDT}");
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();

            handle.Free();

            Thread.CurrentThread.Join();
        }

            

        void ProcessFrameData(float _dt)
        {
            fwd = new Vector3((float)frameData.world_dir_fwd_x, (float)frameData.world_dir_fwd_y, (float)frameData.world_dir_fwd_z);
            rht = new Vector3((float)frameData.world_dir_rht_x, (float)frameData.world_dir_rht_y, (float)frameData.world_dir_rht_z);
            up = Vector3.Cross(fwd, rht);

            transform = new Matrix4x4();
            transform.M11 = rht.X;
            transform.M12 = rht.Y;
            transform.M13 = rht.Z;
            transform.M14 = 0.0f;
            transform.M21 = up.X;
            transform.M22 = up.Y;
            transform.M23 = up.Z;
            transform.M24 = 0.0f;
            transform.M31 = fwd.X;
            transform.M32 = fwd.Y;
            transform.M33 = fwd.Z;
            transform.M34 = 0.0f;
            transform.M41 = (float)frameData.position_x;
            transform.M42 = (float)frameData.position_y;
            transform.M43 = (float)frameData.position_z;
            transform.M44 = 1.0f;

            ProcessTransform(transform, _dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n frametime: " + frameData.total_time + "\n Pitch: " + filteredData.pitch + ", " + "\n Yaw: " + filteredData.yaw + ", " + "\n Roll: " + filteredData.roll + ", " + "\n rht: " + rht.X + ", " + rht.Y + ", " + rht.Z + "\n up: " + up.X + ", " + up.Y + ", " + up.Z + "\n fwd: " + fwd.X + ", " + fwd.Y + ", " + fwd.Z);

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
            //worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);            ////worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);
            worldPosition = new Vector3((float)rawData.position_x, (float)rawData.position_y, (float)rawData.position_z);            ////worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);
        }

        public override void CalcVelocity()
        {
            worldVelocity = (worldPosition - lastPosition) / dt;
//            worldVelocity = (worldPosition - lastPosition) / (1.0f/60.0f);

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
            //   base.CalcAcceleration();

//            Vector3 worldAcceleration = ((worldVelocity - lastWorldVelocity) / dt) * 0.10197162129779283f; //convert to g accel

            lastWorldVelocity = worldVelocity;



            //assign filtered local velocity
            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);
            //Vector3 localVelocity = new Vector3((float)rawData.local_velocity_x, (float)rawData.local_velocity_y, (float)rawData.local_velocity_z);

            //calculate local acceleration
            //Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt) * 0.10197162129779283f; //convert to g accel
            Vector3 localAcceleration = ((localVelocity - lastVelocity) / systemDT) * 0.10197162129779283f; //convert to g accel
//            Vector3 localAcceleration = ((localVelocity - lastVelocity) / (1.0f/60.0f)) * 0.10197162129779283f; //convert to g accel

            lastVelocity = localVelocity;

            //            Vector3 localAcceleration = Vector3.Transform(worldAcceleration, rotInv);

            rawData.gforce_lateral = localAcceleration.X;
            rawData.gforce_vertical = localAcceleration.Y;
            rawData.gforce_longitudinal = localAcceleration.Z;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);


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
            rawData.max_rpm = 6000.0f;
            rawData.max_gears = 6.0f;
            rawData.gear = 1.0f;
            rawData.idle_rpm = 700.0f;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            filteredData.steering_input = rawData.steering_input = frameData.steering_input;
            filteredData.brake_input = rawData.brake_input = frameData.brake_input;
            filteredData.clutch_input = rawData.clutch_input = frameData.clutch_input;

//            base.ProcessInputs();

//            filteredData.engine_rate = 700;// Math.Max(700, Math.Min(6000, 700 + (frameData.engineRPM * (6000-700))));
        }

        public override void SimulateSuspension()
        {
            rawData.suspension_position_bl = frameData.suspension_position_bl;
            rawData.suspension_position_br = frameData.suspension_position_br;
            rawData.suspension_position_fl = frameData.suspension_position_fl;
            rawData.suspension_position_fr = frameData.suspension_position_fr;

            rawData.suspension_velocity_bl = frameData.suspension_velocity_bl;
            rawData.suspension_velocity_br = frameData.suspension_velocity_br;
            rawData.suspension_velocity_fl = frameData.suspension_velocity_fl;
            rawData.suspension_velocity_fr = frameData.suspension_velocity_fr;

            rawData.suspension_acceleration_bl = frameData.suspension_acceleration_bl;
            rawData.suspension_acceleration_br = frameData.suspension_acceleration_br;
            rawData.suspension_acceleration_fl = frameData.suspension_acceleration_fl;
            rawData.suspension_acceleration_fr = frameData.suspension_acceleration_fr;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, suspVelKeyMask, false);

        }

        public override void CalcFFBData()
        {
            filteredData.vehicle_type = rawData.vehicle_type = frameData.vehicle_type;
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
