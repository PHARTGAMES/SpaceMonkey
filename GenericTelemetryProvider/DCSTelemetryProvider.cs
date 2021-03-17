using System;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text;



namespace GenericTelemetryProvider
{

    class DCSTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public DCSUI ui;
        public int readPort;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        DCSData telemetryData;

        public override void Run()
        {
            base.Run();

            telemetryData = new DCSData();

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

            Stopwatch processSW = new Stopwatch();

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

                    telemetryData.FromString(Encoding.UTF8.GetString(received));
                    dt = (float)sw.ElapsedMilliseconds / 1000.0f;
                    sw.Restart();

                    ProcessData(dt);

                    if (socket.Available == 0)
                    {
                        using (var sleeper = new ManualResetEvent(false))
                        {
                            int processTime = (int)processSW.ElapsedMilliseconds;
                            sleeper.WaitOne(Math.Max(0, updateDelay - processTime));
                        }
                        processSW.Restart();
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

        void ProcessData(float dt)
        {
            if (telemetryData == null)
                return;

//            transform = Matrix4x4.CreateRotationY(telemetryData.yaw);
//            transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationZ(telemetryData.pitch), transform);
//            transform = Matrix4x4.Multiply(Matrix4x4.CreateRotationX(telemetryData.roll), transform);

            transform = Matrix4x4.CreateFromYawPitchRoll(telemetryData.yaw, telemetryData.pitch, telemetryData.roll);
            transform.Translation = Vector3.Zero;

            ProcessTransform(transform, dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;


            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n yaw: " + telemetryData.yaw + "\n pitch: " + telemetryData.pitch + "\n roll: " + telemetryData.roll + "\n rht.x: " + rht.X + "\n rht.y: " + rht.Y + "\n rht.z: " + rht.Z + "\n rht.mag: " + rht.Length());

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
            //            base.FilterDT();
            if (dt <= 0)
                dt = 0.015f;

        }

        public override bool CalcPosition()
        {
            //            return base.CalcPosition();

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
            //            base.CalcVelocity();
            
            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            Matrix4x4 rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            Vector3 worldVelocity = new Vector3(telemetryData.velZ, telemetryData.velY, telemetryData.velX);


            //transform world velocity to local space
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;
            
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
            rawData.yaw = -pyr.Y;
            rawData.roll = Utils.LoopAngleRad(-pyr.Z, (float)Math.PI * 0.5f);
        }

       

    }

    public class DCSData
    {

        public float time;      // sec
        public float pitch;     // rad
        public float roll;      // rad
        public float yaw;       // rad
        public float velX;      // m/s
        public float velY;      // m/s
        public float velZ;      // m/s


        public void FromString(string str)
        {
            string[] tokens = str.Split(';');
            if (tokens.Length == 7)
            {
                time = float.Parse(tokens[0], CultureInfo.InvariantCulture);

                pitch = float.Parse(tokens[1], CultureInfo.InvariantCulture);
                yaw = float.Parse(tokens[2], CultureInfo.InvariantCulture);
                roll = float.Parse(tokens[3], CultureInfo.InvariantCulture);

                velX = float.Parse(tokens[4], CultureInfo.InvariantCulture);
                velY = float.Parse(tokens[5], CultureInfo.InvariantCulture);
                velZ = float.Parse(tokens[6], CultureInfo.InvariantCulture);
            }

        }
    }

}
