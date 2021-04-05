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
using MonsterGamesAPI;
using GenericTelemetryProvider.Properties;
using System.Threading.Tasks;

namespace GenericTelemetryProvider
{
    class MonsterGamesTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public MonsterGamesUI ui;
        MonsterGamesData data;
        int readPort = 13371;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        uint lastPacketId = 0;
        float worldScale = 0.1f;

        public override void Run()
        {
            base.Run();

            updateDelay = 18;
            maxAccel2DMagSusp = 6.0f;
            telemetryPausedTime = 1.5f;

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
        }

        public override void Stop()
        {
            base.Stop();


        }

        void PerformInjection()
        {
            AutoResetEvent injectionEvent = new AutoResetEvent(false);
            string[] processNames = new string[] { "NascarHeat5", "NascarHeat4", "AllAmericanRacing", "SprintCarRacing" };
                
            foreach(string processName in processNames)
            {
                //processName
                Task.Run(delegate ()
                {
                    InjectionManager.Monitor(processName, Resources.MonsterGamesTelemetry, injectionEvent, "MonsterGamesTelemetry");
                });
            }

            while (true)
            {
                injectionEvent.WaitOne();
                ui.StatusTextChanged(InjectionManager.GetStatus());

                if (InjectionManager.GetState() == InjectionManager.State.Failed || InjectionManager.GetState() == InjectionManager.State.Success)
                    break;
            }
        }

        void ReadTelemetry()
        {
            PerformInjection();

            if (InjectionManager.GetState() == InjectionManager.State.Failed)
                return;

            UdpClient socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
            socket.Client.Bind(new IPEndPoint(IPAddress.Any, readPort));

            StartSending();


             //read and process
            while (!IsStopped)
            {
                try
                {
                    //wait for telemetry
                    if (socket.Available == 0)
                    {
                        using (var sleeper = new ManualResetEvent(false))
                        {
                            sleeper.WaitOne(1);
                        }
                        continue;
                    }

                    Byte[] received = socket.Receive(ref senderIP);


                    if (socket.Available == 0)
                    {
                        data = JsonConvert.DeserializeObject<MonsterGamesData>(System.Text.Encoding.UTF8.GetString(received));

                        if (data.packetId < lastPacketId && Math.Abs((long)data.packetId - (long)lastPacketId) < 1000)
                        {
                            continue;
                        }

                        lastPacketId = data.packetId;

                        if (!data.paused)
                        {
                            ProcessMonsterGamesData(data.dt);
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



        void ProcessMonsterGamesData(float _dt)
        {
            if (data == null)
                return;

            transform = new Matrix4x4();

            ProcessTransform(transform, _dt);// data.dt);
        }

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged("dt: " + inDT + "\n" + JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

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
            rawData.position_x = data.posX * worldScale;
            rawData.position_y = data.posY * worldScale;
            rawData.position_z = data.posZ * worldScale;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            return true;
        }

        public override void CalcVelocity()
        {
            rawData.local_velocity_x = data.velX * worldScale;
            rawData.local_velocity_y = data.velY * worldScale;
            rawData.local_velocity_z = data.velZ * worldScale;
        }

        public override void FilterVelocity()
        {
            //filter local velocity
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, velKeyMask, false);
        }

        public override void CalcAcceleration()
        {

            rawData.gforce_lateral = data.accelX * worldScale;
            rawData.gforce_vertical = data.accelY * worldScale;
            rawData.gforce_longitudinal = data.accelZ * worldScale;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, accelKeyMask, false);
        }

        public override void CalcAngles()
        {
            rawData.pitch = Utils.LoopAngleRad(Utils.FlipAngleRad(data.pitch), (float)Math.PI * 0.5f);
            rawData.yaw = data.yaw;
            rawData.roll = Utils.LoopAngleRad(Utils.FlipAngleRad(data.roll), (float)Math.PI * 0.5f);
        }

        public override void CalcAngularVelocityAndAccel()
        {
            rawData.yaw_velocity = data.yawVel;
            rawData.pitch_velocity = data.pitchVel;
            rawData.roll_velocity = data.rollVel;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, angVelKeyMask, false);

            rawData.yaw_acceleration = data.yawAccel;
            rawData.pitch_acceleration = data.pitchAccel;
            rawData.roll_acceleration = data.rollAccel;
        }

        public override void SimulateEngine()
        {
            rawData.max_rpm = 6000;
            rawData.max_gears = data.gears;
            rawData.gear = data.gear;
            rawData.idle_rpm = 700;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

            filteredData.engine_rate = Math.Max(700, Math.Min(6000, 700 + (data.engineRPM * (6000-700))));
        }
    }
}
