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

        public override void Run()
        {
            base.Run();

            t = new Thread(ReadTelemetry);
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

                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    telemetryData = (BNGAPI)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(BNGAPI));
                    alloc.Free();

                    if (telemetryData.magic[0] == 'B'
                        && telemetryData.magic[1] == 'N'
                        && telemetryData.magic[2] == 'G'
                        && telemetryData.magic[3] == '1')
                    {
                        dt = (float)sw.ElapsedMilliseconds / 1000.0f;
                        sw.Restart();
                        ProcessBNGAPI(dt);
                    }

                    using (var sleeper = new ManualResetEvent(false))
                    {
                        sleeper.WaitOne((int)(1000.0f * 0.01f));
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            StopSending();
            socket.Close();
        }

        void ProcessBNGAPI(float dt)
        {
            if (telemetryData == null)
                return;

            //Matrix4x4 rollRot = Matrix4x4.CreateRotationZ(telemetryData.rollPos);
            //Matrix4x4 pitchRot = Matrix4x4.CreateRotationX(telemetryData.pitchPos);
            //Matrix4x4 yawRot = Matrix4x4.CreateRotationY(telemetryData.yawPos);
            //transform = Matrix4x4.Multiply(rollRot, pitchRot);
            //transform = Matrix4x4.Multiply(transform, yawRot);

            transform = Matrix4x4.CreateFromYawPitchRoll(telemetryData.yawPos, telemetryData.pitchPos, telemetryData.rollPos);
            transform.Translation = new Vector3(telemetryData.posX, telemetryData.posZ, telemetryData.posY);

            ProcessTransform(transform, dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;


            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n yaw: " + telemetryData.yawPos + "\n pitch: " + telemetryData.pitchPos + "\n roll: " + telemetryData.rollPos + "\n rht.x: " + rht.X + "\n rht.y: " + rht.Y + "\n rht.z: " + rht.Z + "\n rht.mag: " + rht.Length());

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
            base.FilterDT();
        }

        public override bool CalcPosition()
        {
            return base.CalcPosition();
        }

        public override void CalcVelocity()
        {
            base.CalcVelocity();

            rawData.local_velocity_x = -(float)rawData.local_velocity_x;
        }

        public override void FilterVelocity()
        {
            base.FilterVelocity();
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
            rawData.yaw = pyr.Y;
            rawData.roll = Utils.LoopAngleRad(-pyr.Z, (float)Math.PI * 0.5f);
        }

    }

}
