﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;



namespace GenericTelemetryProvider
{
    class BeamNGTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public BeamNGUI ui;
        public int readPort;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        BNGAPI telemetryData;

        public override void Run()
        {
            base.Run();

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
        }

        void ReadTelemetry()
        {

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, readPort));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            StartSending();


            while (!IsStopped)
            {
                try
                {

                    //wait for telemetry
                    if (socket.Available == 0)
                    {
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            Thread.Sleep(1000);
                        }
                        continue;
                    }

                    Byte[] received = socket.Receive(ref senderIP);

                    if (socket.Available != 0)
                        continue;

                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    telemetryData = (BNGAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(BNGAPI));
                    alloc.Free();

                    if (telemetryData.magic[0] == 'B'
                        && telemetryData.magic[1] == 'N'
                        && telemetryData.magic[2] == 'G'
                        && telemetryData.magic[3] == '2')
                    {
                        dt = (float)sw.Elapsed.TotalSeconds;
                        sw.Restart();
                        ProcessBNGAPI(dt);
                    }

                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();
            socket.Close();

            Thread.CurrentThread.Join();

        }

        void ProcessBNGAPI(float dt)
        {
            if (telemetryData == null)
                return;

            transform = Matrix4x4.CreateFromYawPitchRoll(telemetryData.yawPos, telemetryData.pitchPos, telemetryData.rollPos);
            transform.Translation = new Vector3(telemetryData.posX, telemetryData.posZ, telemetryData.posY);

            ProcessTransform(transform, dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            try
            {
                base.ProcessTransform(newTransform, inDT);
            }
            catch (Exception e)
            {
                Console.WriteLine("ProcessTransform: " + e);
            }




            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n yaw: " + telemetryData.yawPos + "\n pitch: " + telemetryData.pitchPos + "\n roll: " + telemetryData.rollPos + "\n rht.x: " + rht.X + "\n rht.y: " + rht.Y + "\n rht.z: " + rht.Z + "\n rht.mag: " + rht.Length());

            SendFilteredData();

            return true;
        }

        public override void FilterDT()
        {
            //            base.FilterDT();
            if (dt <= 0)
                dt = 0.015f;

        }

        public override void CalcPosition()
        {
            //            return base.CalcPosition();

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

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
            //            base.CalcVelocity();

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            Vector3 worldVelocity = new Vector3(telemetryData.velX, telemetryData.velZ, telemetryData.velY);
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;

            //            rawData.local_velocity_x = -(float)rawData.local_velocity_x;
        }
        /*
        public override void CalcAcceleration()
        {
            Vector3 worldAcc = new Vector3(telemetryData.accX, telemetryData.accZ, telemetryData.accY);
            Vector3 localAcc = Vector3.Transform(worldAcc, rotInv);

            rawData.gforce_lateral = localAcc.X;
            rawData.gforce_vertical = localAcc.Y;
            rawData.gforce_longitudinal = localAcc.Z;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);
        }
        */
        public override void CalcAngles()
        {
          
            Quaternion quat = Quaternion.CreateFromRotationMatrix(transform);

            Vector3 pyr = Utils.GetPYRFromQuaternion(quat);

            rawData.pitch = -pyr.X;
            rawData.yaw = -pyr.Y;
            rawData.roll = Utils.LoopAngleRad(-pyr.Z, (float)Math.PI * 0.5f);
        }


        public override void CalcAngularVelocityAndAccel()
        {
            rawData.yaw_velocity = telemetryData.yawRate;
            rawData.pitch_velocity = telemetryData.pitchRate;
            rawData.roll_velocity = telemetryData.rollRate;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, angVelKeyMask, false);

            rawData.yaw_acceleration = telemetryData.yawAcc;
            rawData.pitch_acceleration = telemetryData.pitchAcc;
            rawData.roll_acceleration = telemetryData.rollAcc;

        }

        public override void SimulateEngine()
        {
            rawData.max_rpm = telemetryData.max_rpm;
            rawData.max_gears = telemetryData.max_gears;
            rawData.gear = telemetryData.gear;
            rawData.idle_rpm = telemetryData.idle_rpm;
            rawData.engine_rate = telemetryData.engine_rate;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);
            rawData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
        }

        public override void SimulateSuspension()
        {

            rawData.suspension_position_bl = telemetryData.suspension_position_bl;
            rawData.suspension_position_br = telemetryData.suspension_position_br;
            rawData.suspension_position_fl = telemetryData.suspension_position_fl;
            rawData.suspension_position_fr = telemetryData.suspension_position_fr;

            rawData.suspension_velocity_bl = telemetryData.suspension_velocity_bl;
            rawData.suspension_velocity_br = telemetryData.suspension_velocity_br;
            rawData.suspension_velocity_fl = telemetryData.suspension_velocity_fl;
            rawData.suspension_velocity_fr = telemetryData.suspension_velocity_fr;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, suspVelKeyMask, false);

            rawData.suspension_acceleration_bl = telemetryData.suspension_acceleration_bl;
            rawData.suspension_acceleration_br = telemetryData.suspension_acceleration_br;
            rawData.suspension_acceleration_fl = telemetryData.suspension_acceleration_fl;
            rawData.suspension_acceleration_fr = telemetryData.suspension_acceleration_fr;

            //calc wheel patch speed.
            rawData.wheel_patch_speed_bl = filteredData.local_velocity_z;
            rawData.wheel_patch_speed_br = filteredData.local_velocity_z;
            rawData.wheel_patch_speed_fl = filteredData.local_velocity_z;
            rawData.wheel_patch_speed_fr = filteredData.local_velocity_z;
        }

    }

}
