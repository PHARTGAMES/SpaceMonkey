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
    class IL2TelemetryProvider : GenericProviderBase
    {
        Thread t;

        public IL2UI ui;
        public int readPort;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        IL2API telemetryData;
        IL2API lastTelemetryData = new IL2API();

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

                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    telemetryData = (IL2API)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(IL2API));
                    alloc.Free();

                    if (socket.Available != 0)
                        continue;

                    if (telemetryData.packetID == 0x494C0100)
                    {
                        dt = (float)sw.Elapsed.TotalSeconds;
                        sw.Restart();
                        ProcessIL2API(dt);
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

        void ProcessIL2API(float dt)
        {
            if (telemetryData == null)
                return;

            transform = Matrix4x4.CreateFromYawPitchRoll(telemetryData.yaw, telemetryData.pitch, telemetryData.roll);
            transform.Translation = Vector3.Zero;

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



            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

            SendFilteredData();

            lastTelemetryData.CopyFields(telemetryData);
            return true;
        }

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;

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

            rawData.local_velocity_x = 0.0f;
            rawData.local_velocity_y = 0.0f;
            rawData.local_velocity_z = 0.0f;
        }

        public override void CalcAcceleration()
        {
            rawData.gforce_lateral = telemetryData.accX;
            rawData.gforce_vertical = telemetryData.accY;
            rawData.gforce_longitudinal = telemetryData.accZ;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);

        }

        public override void CalcAngles()
        {
         
            Quaternion quat = Quaternion.CreateFromRotationMatrix(transform);

            Vector3 pyr = Utils.GetPYRFromQuaternion(quat);

            rawData.pitch = Utils.LoopAngleRad(pyr.X, (float)Math.PI * 0.5f);
            rawData.yaw = pyr.Y;
            rawData.roll = Utils.LoopAngleRad(pyr.Z, (float)Math.PI * 0.5f);
        }

        public override void SimulateEngine()
        {
            base.SimulateEngine();

            filteredData.speed = 5;
        }

    }

}
