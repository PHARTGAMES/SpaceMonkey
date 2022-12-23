using System;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using WRCGenAPI;



namespace GenericTelemetryProvider
{
    class WRCGenTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public WRCGenUI ui;
        public int readPort = 20777;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        WRCGenData telemetryData;
        float lastTime = 0.0f;
        public float updateRate = 1.0f / 60.0f;
        int droppedFrameCounter = 0;
        float extraTime = 0.0f;

        public override void Run()
        {
            base.Run();

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
        }

        void ReadTelemetry()
        {
            UdpClient socket = null;
            try

            {

                socket = new UdpClient();
                socket.ExclusiveAddressUse = false;
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, readPort));


                //                socket.Connect(new IPEndPoint(IPAddress.Parse("192.168.50.194"), readPort));

                //socket = new UdpClient(readPort);
                //socket.ExclusiveAddressUse = false;



                //socket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), readPort));
                //socket.Client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), readPort));
                //                socket.Client.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), readPort));

//                senderIP = new IPEndPoint(IPAddress.Any, readPort);
            }
            catch (SocketException e)
            {

            }

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
//                        Thread.Sleep(0);
                        if (sw.ElapsedMilliseconds > 500)
                        {
                            Thread.Sleep(1000);
                        }
                        continue;
                    }

                    Byte[] received = socket.Receive(ref senderIP);

                    if (socket.Available != 0)
                    {
                        Console.WriteLine("-------------------------------ExtraRead--------------------------------");
                        continue;
                    }

                    var alloc = GCHandle.Alloc(received, GCHandleType.Pinned);
                    telemetryData = (WRCGenData)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(WRCGenData));
                    alloc.Free();

                    if(telemetryData.m_lapTime > 0)
                    {
                        float finalDT = updateRate;
                        float calcDT = telemetryData.m_time - lastTime;

                        if (calcDT < 0.01f)
                        {
//                            finalDT = calcDT;
//                            lastTime = telemetryData.m_time;
                            Console.WriteLine("short frame: " + calcDT);
                            continue;
                        }

                        if(calcDT > 0.02f)
                        {
                            Console.WriteLine("ExtraTime: " + calcDT);
                            finalDT = updateRate * 2;
                        }


                        ProcessTelemetryData(finalDT);
                        extraTime = 0.0f;
                        lastTime = telemetryData.m_time;
                        droppedFrameCounter = 0;

                        sw.Restart();
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

        void ProcessTelemetryData(float _dt)
        {
            if (telemetryData == null)
                return;

            dt = _dt;
            //dt = 1.0f / 60.0f;

            Vector3 fwd = -new Vector3(telemetryData.m_xd, telemetryData.m_yd, telemetryData.m_zd);
            Vector3 rht = -new Vector3(telemetryData.m_xr, telemetryData.m_yr, telemetryData.m_zr);
            Vector3 up = Vector3.Cross(rht, fwd);
            Vector3 pos = new Vector3(telemetryData.m_x, telemetryData.m_y, telemetryData.m_z);

            transform = new Matrix4x4(rht.X, rht.Z, rht.Y, 0.0f,
                                    up.X, up.Z, up.Y, 0.0f,
                                    fwd.X, fwd.Z, fwd.Y, 0.0f,
                                    pos.X, pos.Z, pos.Y, 1.0f);

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

            //            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n xr: " + telemetryData.m_xr + "\n yr: " + telemetryData.m_yr + "\n zr: " + telemetryData.m_zr + "\n xd: " + telemetryData.m_xd + "\n yd: " + telemetryData.m_yd + "\n zd: " + telemetryData.m_zd);
            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n frametime: " + ", " + "\n rht: " + rht.X + ", " + rht.Y + ", " + rht.Z + "\n up: " + up.X + ", " + up.Y + ", " + up.Z + "\n fwd: " + fwd.X + ", " + fwd.Y + ", " + fwd.Z);

            SendFilteredData();

            return true;
        }



    }

}
