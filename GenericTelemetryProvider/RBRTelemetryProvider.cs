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
    class RBRTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public RBRUI ui;
        public int readPort;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        RBRAPI telemetryData;
        uint step = 0;

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

                    if (socket.Available == 0)
                    {
                        var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                        telemetryData = (RBRAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(RBRAPI));
                        alloc.Free();

                        if (telemetryData.totalSteps_ > step || ((long)telemetryData.totalSteps_-(long)step) < -100)
                        {
                            step = telemetryData.totalSteps_;
                            dt = (float)sw.Elapsed.TotalSeconds;
                            sw.Restart();
                            ProcessRBRAPI(dt);
                        }
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

        void ProcessRBRAPI(float dt)
        {
            if (telemetryData == null)
                return;

            transform = new Matrix4x4();

            ProcessTransform(transform, dt);
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
            return true;

        }

        public override bool CheckLastFrameValid()
        {
            return base.CheckLastFrameValid();
        }

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.01f;
        }

        public override bool CalcPosition()
        {
            
            rawData.position_x = telemetryData.car_.positionX_;
            rawData.position_y = telemetryData.car_.positionY_;
            rawData.position_z = telemetryData.car_.positionZ_;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            return true;
        }

        public override void CalcVelocity()
        {
            rawData.local_velocity_x = -telemetryData.car_.velocities_.sway_;
            rawData.local_velocity_y = telemetryData.car_.velocities_.heave_;
            rawData.local_velocity_z = telemetryData.car_.velocities_.surge_;
        }

        public override void FilterVelocity()
        {
            //filter local velocity
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, velKeyMask, false);
        }

        public override void CalcAcceleration()
        {

            rawData.gforce_lateral = -telemetryData.car_.accelerations_.sway_;
            rawData.gforce_vertical = telemetryData.car_.accelerations_.heave_;
            rawData.gforce_longitudinal = telemetryData.car_.accelerations_.surge_;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);
        }

        public override void CalcAngles()
        {
            rawData.pitch = Utils.LoopAngleRad(Utils.FlipAngleRad(-telemetryData.car_.pitch_ * ((float)Math.PI / 180.0f)), (float)Math.PI * 0.5f);
            rawData.yaw = telemetryData.car_.yaw_ * ((float)Math.PI / 180.0f);
            rawData.roll = Utils.LoopAngleRad(Utils.FlipAngleRad(-telemetryData.car_.roll_ * ((float)Math.PI / 180.0f)), (float)Math.PI * 0.5f);
        }

        public override void CalcAngularVelocityAndAccel()
        {
            rawData.yaw_velocity = telemetryData.car_.velocities_.yaw_ * ((float)Math.PI / 180.0f);
            rawData.pitch_velocity = -telemetryData.car_.velocities_.pitch_ * ((float)Math.PI / 180.0f);
            rawData.roll_velocity = -telemetryData.car_.velocities_.roll_ * ((float)Math.PI / 180.0f);

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, angVelKeyMask, false);

            rawData.yaw_acceleration = telemetryData.car_.accelerations_.yaw_ * ((float)Math.PI / 180.0f);
            rawData.pitch_acceleration = -telemetryData.car_.accelerations_.pitch_ * ((float)Math.PI / 180.0f);
            rawData.roll_acceleration = -telemetryData.car_.accelerations_.roll_ * ((float)Math.PI / 180.0f);
        }

        public override void SimulateEngine()
        {
            rawData.max_rpm = 6000;
            rawData.max_gears = 6;
            rawData.gear = telemetryData.control_.gear_;
            rawData.idle_rpm = 700;
            filteredData.engine_rate = telemetryData.car_.engine_.rpm_;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }


        public override void ProcessInputs()
        {
            filteredData.steering_input = telemetryData.control_.steering_;
            filteredData.throttle_input = telemetryData.control_.throttle_;
            filteredData.brake_input = telemetryData.control_.brake_;
            filteredData.clutch_input = telemetryData.control_.clutch_;
        }

    }

}
