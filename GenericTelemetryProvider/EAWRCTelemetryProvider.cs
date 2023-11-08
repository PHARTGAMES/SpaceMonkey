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
    class EAWRCTelemetryProvider : GenericProviderBase
    {
        Thread t;

        public EAWRCUI ui;
        public int readPort;
        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread
        EAWRCCustomUDPData session_updateData;
        UdpClient socket;

        public override void Run()
        {
            base.Run();

            EAWRCCustomUDPData.LoadConfig();

            string structure = "wrc";
            string packet = "session_update";

            session_updateData = EAWRCCustomUDPData.GetPacket(structure, packet);

            t = new Thread(MonitorThread);
            t.IsBackground = true;
            t.Start();

            socket = new UdpClient();
            socket.ExclusiveAddressUse = false;
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

                //put recieved into session_updateData
                if (session_updateData.FromBytes(received))
                {

                    Vector3 fwd = new Vector3((float)session_updateData.vehicle_forward_direction_x, (float)session_updateData.vehicle_forward_direction_y, (float)session_updateData.vehicle_forward_direction_z);
                    Vector3 rht = -new Vector3((float)session_updateData.vehicle_left_direction_x, (float)session_updateData.vehicle_left_direction_y, (float)session_updateData.vehicle_left_direction_z);
                    Vector3 up = Vector3.Cross(fwd, rht);
                    Vector3 pos = new Vector3(-(float)session_updateData.vehicle_position_x, (float)session_updateData.vehicle_position_y, (float)session_updateData.vehicle_position_z);

                    transform = new Matrix4x4(rht.X, rht.Y, rht.Z, 0.0f,
                                            up.X, up.Y, up.Z, 0.0f,
                                            fwd.X, fwd.Y, fwd.Z, 0.0f,
                                            pos.X, pos.Y, pos.Z, 1.0f);



                    ProcessTransform(transform, (float)session_updateData.game_delta_time);
                }

                socket.BeginReceive(new AsyncCallback(ReceiveCallback), remoteEP);
            }
            catch (Exception e)
            {
                socket.BeginReceive(new AsyncCallback(ReceiveCallback), remoteEP);
                Thread.Sleep(1000);
            }

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


            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + "\n rht.x: " + rht.X + "\n rht.y: " + rht.Y + "\n rht.z: " + rht.Z + "\n rht.mag: " + rht.Length());

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
            rawData.max_rpm = session_updateData.vehicle_engine_rpm_max;
            rawData.max_gears = session_updateData.vehicle_gear_maximum;
            rawData.gear = session_updateData.vehicle_gear_index;
            rawData.idle_rpm = session_updateData.vehicle_engine_rpm_idle;
            rawData.engine_rate = session_updateData.vehicle_engine_rpm_current;

            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);
            rawData.speed = localVelocity.Length();
        }

        public override void ProcessInputs()
        {
            rawData.steering_input = session_updateData.vehicle_steering;
            rawData.brake_input = session_updateData.vehicle_brake;
            rawData.throttle_input = session_updateData.vehicle_throttle;
        }

        public override void SimulateSuspension()
        {
            rawData.suspension_position_bl = session_updateData.vehicle_hub_position_bl;
            rawData.suspension_position_br = session_updateData.vehicle_hub_position_br;
            rawData.suspension_position_fl = session_updateData.vehicle_hub_position_fl;
            rawData.suspension_position_fr = session_updateData.vehicle_hub_position_fr;

            rawData.suspension_velocity_bl = session_updateData.vehicle_hub_velocity_bl;
            rawData.suspension_velocity_br = session_updateData.vehicle_hub_velocity_br;
            rawData.suspension_velocity_fl = session_updateData.vehicle_hub_velocity_fl;
            rawData.suspension_velocity_fr = session_updateData.vehicle_hub_velocity_fr;

            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, suspVelKeyMask, false);

            //rawData.suspension_acceleration_bl = session_updateData.suspension_acceleration_bl;
            //rawData.suspension_acceleration_br = session_updateData.suspension_acceleration_br;
            //rawData.suspension_acceleration_fl = session_updateData.suspension_acceleration_fl;
            //rawData.suspension_acceleration_fr = session_updateData.suspension_acceleration_fr;

            rawData.wheel_patch_speed_bl = session_updateData.vehicle_cp_forward_speed_bl;
            rawData.wheel_patch_speed_br = session_updateData.vehicle_cp_forward_speed_br;
            rawData.wheel_patch_speed_fl = session_updateData.vehicle_cp_forward_speed_fl;
            rawData.wheel_patch_speed_fr = session_updateData.vehicle_cp_forward_speed_fr;
        }

    }

}
