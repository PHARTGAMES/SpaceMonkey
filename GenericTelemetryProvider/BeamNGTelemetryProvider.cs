using System;
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
        BNGAPI lastTelemetryData = new BNGAPI();
        UdpClient socket;


        public override void Run()
        {
            base.Run();

            maxPosVelocityDelta = 100.0f;
            maxRotVelocityDelta = 15.0f;

            socket = new UdpClient();
            socket.ExclusiveAddressUse = false;

            t = new Thread(MonitorThread);
            t.IsBackground = true;
            t.Start();
        }

        void MonitorThread()
        {
            StartSending();

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, readPort);
            socket.Client.Bind(remoteEP);
            socket.BeginReceive(new AsyncCallback(ReceiveCallback), remoteEP);

            while (!IsStopped)
            {
                Thread.Sleep(1000);
            }

            StopSending();
            socket.Close();

            Thread.CurrentThread.Join();
        }

        void ReceiveCallback(IAsyncResult ar)
        {
            if (IsStopped)
                return;

            IPEndPoint remoteEP = (IPEndPoint)ar.AsyncState;
            try
            {
                byte[] received = socket.EndReceive(ar, ref remoteEP);

                var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                telemetryData = (BNGAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(BNGAPI));
                alloc.Free();

                if (telemetryData.magic[0] == 'B'
                    && telemetryData.magic[1] == 'N'
                    && telemetryData.magic[2] == 'G'
                    && telemetryData.magic[3] == '2')
                {
                    dt = telemetryData.timeStamp - lastTelemetryData.timeStamp;
//                    Console.WriteLine($"dt: {dt}");
                    ProcessBNGAPI(dt);
                    lastTelemetryData.CopyFields(telemetryData);
                }

                socket.BeginReceive(new AsyncCallback(ReceiveCallback), remoteEP);
            }
            catch (Exception e)
            {
                socket.BeginReceive(new AsyncCallback(ReceiveCallback), remoteEP);
                Thread.Sleep(1000);
            }

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


        public override void SimulateEngine()
        {
            rawData.max_rpm = telemetryData.max_rpm;
            rawData.max_gears = (float)telemetryData.max_gears;
            rawData.gear = (float)telemetryData.gear;
            rawData.idle_rpm = telemetryData.idle_rpm;
            rawData.engine_rate = telemetryData.engine_rate;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);
            rawData.speed = localVelocity.Length();
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

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, suspVelKeyMask, false, dt);

            rawData.suspension_acceleration_bl = telemetryData.suspension_acceleration_bl;
            rawData.suspension_acceleration_br = telemetryData.suspension_acceleration_br;
            rawData.suspension_acceleration_fl = telemetryData.suspension_acceleration_fl;
            rawData.suspension_acceleration_fr = telemetryData.suspension_acceleration_fr;

            rawData.wheel_patch_speed_bl = telemetryData.wheel_speed_bl;
            rawData.wheel_patch_speed_br = telemetryData.wheel_speed_br;
            rawData.wheel_patch_speed_fl = telemetryData.wheel_speed_fl;
            rawData.wheel_patch_speed_fr = telemetryData.wheel_speed_fr;
        }

        public override void ProcessInputs()
        {
//            base.ProcessInputs();
        }
        /*


        public override void CalcPosition()
        {

            currRawPos = new Vector3(telemetryData.posX, telemetryData.posZ, telemetryData.posY);

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
            rawData.local_velocity_x = telemetryData.velX;
            rawData.local_velocity_y = telemetryData.velZ;
            rawData.local_velocity_z = telemetryData.velY;
        }

        public override void CalcAcceleration()
        {

            rawData.gforce_lateral = telemetryData.accX;
            rawData.gforce_vertical = telemetryData.accZ;
            rawData.gforce_longitudinal = telemetryData.accY;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);

        }


        public override void CalcAngles()
        {
            rawData.pitch = -telemetryData.pitchPos;
            rawData.yaw = -telemetryData.yawPos;
            rawData.roll = Utils.LoopAngleRad(-telemetryData.rollPos, (float)Math.PI * 0.5f);
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
        */
    }

}
