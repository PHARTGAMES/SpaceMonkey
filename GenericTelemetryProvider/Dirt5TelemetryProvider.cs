using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Sojaner.MemoryScanner;
using System.Numerics;
using System.IO;
using System.IO.MemoryMappedFiles;
using NoiseFilters;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using CMCustomUDP;

namespace GenericTelemetryProvider
{
    public class Dirt5TelemetryProvider : GenericProviderBase
    {

        Int64 memoryAddress;
        Thread t;
        Process mainProcess = null;

        public string vehicleString;
        CMCustomUDPData lastFilteredData;
        CMCustomUDPData filteredData;
        CMCustomUDPData rawData;
        bool lastFrameValid = false;
        Vector3 lastVelocity = Vector3.Zero;
        Vector3 lastPosition = Vector3.Zero;
        Vector3 lastWorldVelocity = Vector3.Zero;
        Vector3 lastRawPos = Vector3.Zero;

        public Dirt5UI ui;
        public float lastWorldVelMag = 0.0f;
        private TelemetrySender telemetrySender = new TelemetrySender();
        SC4DR2CustomTelemetry lastUDPData = new SC4DR2CustomTelemetry();
        bool sendUDP = false;
        bool fillMMF = false;


        NestedSmooth dtFilter = new NestedSmooth(0, 60, 100000.0f);
//        KalmanFilter dtFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);

        //NestedSmooth velXFilter = new NestedSmooth(2, 100, 1000.0f);
        //NestedSmooth velYFilter = new NestedSmooth(2, 100, 1000.0f);
        //NestedSmooth velZFilter = new NestedSmooth(2, 100, 1000.0f);

        int posKeyMask = CMCustomUDPData.GetKeyMask(CMCustomUDPData.DataKey.position_x, CMCustomUDPData.DataKey.position_y, CMCustomUDPData.DataKey.position_z);
        int velKeyMask = CMCustomUDPData.GetKeyMask(CMCustomUDPData.DataKey.local_velocity_x, CMCustomUDPData.DataKey.local_velocity_y, CMCustomUDPData.DataKey.local_velocity_z);


        public override void Run()
        {
            base.Run();


            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("DIRT5"))
                    mainProcess = process;
            }

            if (mainProcess == null) //no processes, better stop
            {

                ui.StatusTextChanged("DIRT5 exe not running!");
                return;
            }


            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, 0, 34359720776);// 140737488355327); //32gig
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_ScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);


            //            string scanString = "(\0\0\0\0skoda_fabia_r5";
            string scanString = "(\0\0\0\0" + vehicleString;
            scan.StartScanForString(scanString);

        }

        void ScanComplete()
        { 
            bool isStopped = false;
            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();


            Stopwatch sw = new Stopwatch();
            sw.Start();


            lastPosition = Vector3.Zero;
            lastFrameValid = false;
            lastVelocity = Vector3.Zero;
            lastWorldVelocity = Vector3.Zero;
            lastRawPos = Vector3.Zero;

            filteredData = new CMCustomUDPData();
            filteredData.Init();
            rawData = new CMCustomUDPData();
            rawData.Init();

            fillMMF = MainConfig.Instance.configData.fillMMF;
            sendUDP = MainConfig.Instance.configData.sendUDP;

            if (sendUDP)
            {
                telemetrySender.StartSending(MainConfig.Instance.configData.udpIP, MainConfig.Instance.configData.udpPort);
            }
           

            float dt = 0.0f;
 
            while (!isStopped)
            {
                try
                {

                    Int64 byteReadSize;
                    reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out byteReadSize, readBuffer);

                    if (byteReadSize == 0)
                    {
                        continue;
                    }

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

//                        debugChangedCallback?.Invoke("" + floats[0] + " " + floats[1] + " " + floats[2] + " " + floats[3] + "\n" + floats[4] + " " + floats[5] + " " + floats[6] + " " + floats[7] + "\n" + floats[8] + " " + floats[9] + " " + floats[10] + " " + floats[11] + "\n" + floats[12] + " " + floats[13] + " " + floats[14] + " " + floats[15]);
                    //                        SetRichTextBoxThreadSafe(matrixBox, "" + floats[0] + " " + floats[1] + " " + floats[2] + " " + floats[3] + "\n" + floats[4] + " " + floats[5] + " " + floats[6] + " " + floats[7] + "\n" + floats[8] + " " + floats[9] + " " + floats[10] + " " + floats[11] + "\n" + floats[12] + " " + floats[13] + " " + floats[14] + " " + floats[15]);

                    Matrix4x4 transform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                , floats[4], floats[5], floats[6], floats[7]
                                , floats[8], floats[9], floats[10], floats[11]
                                , floats[12], floats[13], floats[14], floats[15]);

                    dt = (float)sw.ElapsedMilliseconds / 1000.0f;
                    sw.Restart();
                    ProcessTransform(transform, dt);

  //                  if (ProcessTransform(transform, dt))
  //                      sw.Restart();


                    Thread.Sleep(1000 / 100);
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            if(sendUDP)
                telemetrySender.StopSending();

        }

        bool ProcessTransform(Matrix4x4 transform, float dt)
        {

            Vector3 rht = new Vector3(transform.M11, transform.M12, transform.M13);
            Vector3 up = new Vector3(transform.M21, transform.M22, transform.M23);
            Vector3 fwd = new Vector3(transform.M31, transform.M32, transform.M33);

            float rhtMag = rht.Length();
            float upMag = up.Length();
            float fwdMag = fwd.Length();

            //reading garbage
            if (rhtMag < 0.9f || upMag < 0.9f || fwdMag < 0.9f)
            {
                return false;
            }

            if (!lastFrameValid)
            {
                lastFilteredData = new CMCustomUDPData();
                lastPosition = transform.Translation;
                lastFrameValid = true;
                lastVelocity = Vector3.Zero;
                lastWorldVelMag = 0.0f;
                lastWorldVelocity = Vector3.Zero;
                lastRawPos = Vector3.Zero;
                return true;
            }

            dt = dtFilter.Filter(dt);

            if (dt <= 0)
                dt = 0.015f;

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            Vector3 rawVel = (currRawPos - lastRawPos) / dt;
            float rawVelMag = rawVel.Length();
            float lastVelMag = lastWorldVelocity.Length();

            if (rawVelMag <= lastVelMag * 0.25f && (lastVelMag < rawVelMag * 10000.0f || rawVelMag <= float.Epsilon))
            { 
                return true;
            }

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            lastRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            //assign
            Vector3 worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            Vector3 worldVelocity = (worldPosition - lastPosition) / dt;
            lastWorldVelocity = worldVelocity;

            lastPosition = transform.Translation = worldPosition;

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            Matrix4x4 rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            //transform world velocity to local space
            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            localVelocity.X = -localVelocity.X;

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;

            //filter local velocity
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, velKeyMask, false);

            //assign filtered local velocity
            localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            //calculate local acceleration
            Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt) * 0.10197162129779283f; //convert to g accel
            lastVelocity = localVelocity;

            //calculate pitch yaw roll
            float pitch = (float)Math.Asin(-fwd.Y);
            float yaw = (float)Math.Atan2(fwd.X, fwd.Z);

            float roll = 0.0f;
            Vector3 rhtPlane = rht;
            rhtPlane.Y = 0;
            rhtPlane = Vector3.Normalize(rhtPlane);
            if (rhtPlane.Length() <= float.Epsilon)
            {
                roll = (float)(Math.Sign(rht.Y) * Math.PI * 0.5f);
                //                        Debug.WriteLine( "---Roll = " + roll + " " + Math.Sign( rht.Y ) );
            }
            else
            {
                roll = (float)Math.Asin(Vector3.Dot(up, rhtPlane));
                //                        Debug.WriteLine( "Roll = " + roll + " " + Math.Sign(rht.Y) );
            }
            //                  Debug.WriteLine( "" );

            rawData.pitch = pitch;
            rawData.yaw = yaw;
            rawData.roll = roll;

            rawData.gforce_lateral = localAcceleration.X;
            rawData.gforce_vertical = localAcceleration.Y;
            rawData.gforce_longitudinal = localAcceleration.Z;

            //finally filter everything else
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, int.MaxValue & ~(posKeyMask | velKeyMask), false);

            InputModule.Instance.Update();

            //            string debugString = "";
            //            debugString += "yaw: " + telemetryToSend.yaw + "\n";



            /*
             //debug
            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
            {
                BinaryReader reader = new BinaryReader(stream);
                byte[] readBuffer = reader.ReadBytes((int)stream.Length);

                var alloc = GCHandle.Alloc(readBuffer, GCHandleType.Pinned);
                GenericProviderData readTelemetry = (GenericProviderData)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(GenericProviderData));
            }
            */

            /*
                        DirtRally2UDPTelemetry udpData = new DirtRally2UDPTelemetry();

                        udpData.position_x = filteredData.position_x;
                        udpData.position_y = filteredData.position_y;
                        udpData.position_z = filteredData.position_z;

                        udpData.velocity_x = filteredData.local_velocity_x;
                        udpData.velocity_y = filteredData.local_velocity_y;
                        udpData.velocity_z = filteredData.local_velocity_z;

                        udpData.left_dir_x = -rht.X;
                        udpData.left_dir_y = -rht.Y;
                        udpData.left_dir_z = -rht.Z;

                        udpData.forward_dir_x = fwd.X;
                        udpData.forward_dir_y = fwd.Y;
                        udpData.forward_dir_z = fwd.Z;

                        udpData.gforce_lateral = filteredData.gforce_lateral;
                        udpData.gforce_longitudinal = filteredData.gforce_longitudinal;

                        udpData.engine_rate = filteredData.engine_rpm;

                        byte[] bytes = udpData.ToByteArray();
                        telemetrySender.SendAsync(bytes);
            */
            /*

                        SC4DR2CustomTelemetry udpData = new SC4DR2CustomTelemetry();

                        udpData.paused = 0;

                        udpData.position_x = filteredData.position_x;
                        udpData.position_y = filteredData.position_y;
                        udpData.position_z = filteredData.position_z;

                        udpData.local_velocity_x = filteredData.local_velocity_x;
                        udpData.local_velocity_y = filteredData.local_velocity_y;
                        udpData.local_velocity_z = filteredData.local_velocity_z;

                        udpData.gforce_lateral = filteredData.gforce_lateral;
                        udpData.gforce_longitudinal = filteredData.gforce_longitudinal;
                        udpData.gforce_vertical = filteredData.gforce_vertical;

                        udpData.yaw = filteredData.yaw;
                        udpData.pitch = filteredData.pitch;
                        udpData.roll = filteredData.roll;

                        udpData.yaw_velocity = Utils.CalculateAngularChange(lastFilteredData.yaw, filteredData.yaw) / dt;
                        udpData.pitch_velocity = Utils.CalculateAngularChange(lastFilteredData.pitch, filteredData.pitch) / dt;
                        udpData.roll_velocity = Utils.CalculateAngularChange(lastFilteredData.roll, filteredData.roll) / dt;

                        udpData.yaw_acceleration = (udpData.yaw_velocity - lastUDPData.yaw_velocity) / dt;
                        udpData.pitch_acceleration = (udpData.pitch_velocity - lastUDPData.pitch_acceleration) / dt;
                        udpData.roll_acceleration = (udpData.roll_velocity - lastUDPData.roll_velocity) / dt;


                        //calc wheel patch speed.
                        udpData.wheel_patch_speed_bl = udpData.local_velocity_z;
                        udpData.wheel_patch_speed_br = udpData.local_velocity_z;
                        udpData.wheel_patch_speed_fl = udpData.local_velocity_z;
                        udpData.wheel_patch_speed_fr = udpData.local_velocity_z;

                        Vector2 accel2D = new Vector2(udpData.gforce_lateral, udpData.gforce_longitudinal) / 0.10197162129779283f;
                        float accel2DMag = accel2D.Length();
                        Vector2 accel2DNorm = Vector2.Normalize(accel2D);


                        Vector2[] suspensionOffsets = new Vector2[] { new Vector2(-0.5f,-1.0f), //bl
                                                                        new Vector2(0.5f,-1.0f), //br
                                                                        new Vector2(-0.5f,1.0f), //fl
                                                                        new Vector2(0.5f,1.0f)}; //fr


                        Vector2[] suspensionVectors = new Vector2[4];

                        //calc suspension vectors
                        Vector2 centerOfGravity = new Vector2(0.0f, 0.0f);
                        for(int i = 0; i < 4; ++i)
                        {
                            suspensionVectors[i] = Vector2.Normalize(suspensionOffsets[i] - centerOfGravity);
                        }

                        //suspension travel at rest = -18 to -20
                        //rear gets to 7 at maximum acceleration
                        //front gets to -75 at max acceleration
                        //front gets to 7 at max braking
                        //rear gets to -80 at max braking
                        float travelCenter = -20.0f;
                        float travelMax = 8 - travelCenter;
                        float travelMin = -80 - travelCenter;
                        float scaledAccelMag = Math.Min(accel2DMag, 3.0f) / 3.0f;
                        for (int i = 0; i < 4; ++i)
                        {
                            float dot = Vector2.Dot(accel2DNorm, suspensionVectors[i]);
                            float travel = travelCenter;
                            float travelMag = 0.0f;
                            if (dot > 0.0f)
                            {
                                travelMag = travelMax;
                            }
                            else
                            if (dot < 0.0f)
                            {
                                travelMag = travelMin;
                            }

                            travel += travelMag * Math.Abs(dot) * scaledAccelMag;

                            switch (i)
                            {
                                case 0:
                                    {
                                        udpData.suspension_position_bl = travel;
                                        break;
                                    }
                                case 1:
                                    {
                                        udpData.suspension_position_br = travel;
                                        break;
                                    }
                                case 2:
                                    {
                                        udpData.suspension_position_fl = travel;
                                        break;
                                    }
                                case 3:
                                    {
                                        udpData.suspension_position_fr = travel;
                                        break;
                                    }
                            }
                        }


                        udpData.suspension_velocity_bl = (udpData.suspension_position_bl - lastUDPData.suspension_position_bl) / dt;
                        udpData.suspension_velocity_br = (udpData.suspension_position_br - lastUDPData.suspension_position_br) / dt;
                        udpData.suspension_velocity_fl = (udpData.suspension_position_fl - lastUDPData.suspension_position_fl) / dt;
                        udpData.suspension_velocity_fr = (udpData.suspension_position_fr - lastUDPData.suspension_position_fr) / dt;


                        udpData.suspension_acceleration_bl = (udpData.suspension_velocity_bl - lastUDPData.suspension_velocity_bl) / dt;
                        udpData.suspension_acceleration_br = (udpData.suspension_velocity_br - lastUDPData.suspension_velocity_br) / dt;
                        udpData.suspension_acceleration_fl = (udpData.suspension_velocity_fl - lastUDPData.suspension_velocity_fl) / dt;
                        udpData.suspension_acceleration_fr = (udpData.suspension_velocity_fr - lastUDPData.suspension_velocity_fr) / dt;

                        udpData.max_rpm = 6000;
                        udpData.max_gears = 6;
                        udpData.gear = 1;
                        udpData.idle_rpm = 700;

                        udpData.speed = localVelocity.Length();

                        udpData.engine_rate = (InputModule.Instance.controller.rightTrigger * 5500) + 700;
                        udpData.steering_input = InputModule.Instance.controller.leftThumb.X;
                        udpData.throttle_input = InputModule.Instance.controller.rightTrigger;
                        udpData.brake_input = InputModule.Instance.controller.leftTrigger;

                        byte[] bytes = udpData.ToByteArray();
                        telemetrySender.SendAsync(bytes);

                        lastUDPData.Copy(udpData);*/


            filteredData.paused = rawData.paused = 0;

            filteredData.yaw_velocity = Utils.CalculateAngularChange((float)lastFilteredData.yaw, (float)filteredData.yaw) / dt;
            filteredData.pitch_velocity = Utils.CalculateAngularChange((float)lastFilteredData.pitch, (float)filteredData.pitch) / dt;
            filteredData.roll_velocity = Utils.CalculateAngularChange((float)lastFilteredData.roll, (float)filteredData.roll) / dt;

            filteredData.yaw_acceleration = ((float)filteredData.yaw_velocity - (float)lastFilteredData.yaw_velocity) / dt;
            filteredData.pitch_acceleration = ((float)filteredData.pitch_velocity - (float)lastFilteredData.pitch_acceleration) / dt;
            filteredData.roll_acceleration = ((float)filteredData.roll_velocity - (float)lastFilteredData.roll_velocity) / dt;


            //calc wheel patch speed.
            filteredData.wheel_patch_speed_bl = filteredData.local_velocity_z;
            filteredData.wheel_patch_speed_br = filteredData.local_velocity_z;
            filteredData.wheel_patch_speed_fl = filteredData.local_velocity_z;
            filteredData.wheel_patch_speed_fr = filteredData.local_velocity_z;

            Vector2 accel2D = new Vector2((float)filteredData.gforce_lateral, (float)filteredData.gforce_longitudinal) / 0.10197162129779283f;
            float accel2DMag = accel2D.Length();
            Vector2 accel2DNorm = Vector2.Normalize(accel2D);


            Vector2[] suspensionOffsets = new Vector2[] { new Vector2(-0.5f,-1.0f), //bl
                                                            new Vector2(0.5f,-1.0f), //br
                                                            new Vector2(-0.5f,1.0f), //fl
                                                            new Vector2(0.5f,1.0f)}; //fr


            Vector2[] suspensionVectors = new Vector2[4];

            //calc suspension vectors
            Vector2 centerOfGravity = new Vector2(0.0f, 0.0f);
            for (int i = 0; i < 4; ++i)
            {
                suspensionVectors[i] = Vector2.Normalize(suspensionOffsets[i] - centerOfGravity);
            }

            //suspension travel at rest = -18 to -20
            //rear gets to 7 at maximum acceleration
            //front gets to -75 at max acceleration
            //front gets to 7 at max braking
            //rear gets to -80 at max braking
            float travelCenter = -20.0f;
            float travelMax = 8 - travelCenter;
            float travelMin = -80 - travelCenter;
            float scaledAccelMag = Math.Min(accel2DMag, 3.0f) / 3.0f;
            for (int i = 0; i < 4; ++i)
            {
                float dot = Vector2.Dot(accel2DNorm, suspensionVectors[i]);
                float travel = travelCenter;
                float travelMag = 0.0f;
                if (dot > 0.0f)
                {
                    travelMag = travelMax;
                }
                else
                if (dot < 0.0f)
                {
                    travelMag = travelMin;
                }

                travel += travelMag * Math.Abs(dot) * scaledAccelMag;

                switch (i)
                {
                    case 0:
                        {
                            filteredData.suspension_position_bl = travel;
                            break;
                        }
                    case 1:
                        {
                            filteredData.suspension_position_br = travel;
                            break;
                        }
                    case 2:
                        {
                            filteredData.suspension_position_fl = travel;
                            break;
                        }
                    case 3:
                        {
                            filteredData.suspension_position_fr = travel;
                            break;
                        }
                }
            }


            filteredData.suspension_velocity_bl = ((float)filteredData.suspension_position_bl - (float)lastFilteredData.suspension_position_bl) / dt;
            filteredData.suspension_velocity_br = ((float)filteredData.suspension_position_br - (float)lastFilteredData.suspension_position_br) / dt;
            filteredData.suspension_velocity_fl = ((float)filteredData.suspension_position_fl - (float)lastFilteredData.suspension_position_fl) / dt;
            filteredData.suspension_velocity_fr = ((float)filteredData.suspension_position_fr - (float)lastFilteredData.suspension_position_fr) / dt;


            filteredData.suspension_acceleration_bl = ((float)filteredData.suspension_velocity_bl - (float)lastFilteredData.suspension_velocity_bl) / dt;
            filteredData.suspension_acceleration_br = ((float)filteredData.suspension_velocity_br - (float)lastFilteredData.suspension_velocity_br) / dt;
            filteredData.suspension_acceleration_fl = ((float)filteredData.suspension_velocity_fl - (float)lastFilteredData.suspension_velocity_fl) / dt;
            filteredData.suspension_acceleration_fr = ((float)filteredData.suspension_velocity_fr - (float)lastFilteredData.suspension_velocity_fr) / dt;

            filteredData.max_rpm = 6000;
            filteredData.max_gears = 6;
            filteredData.gear = 1;
            filteredData.idle_rpm = 700;

            filteredData.speed = localVelocity.Length();

            filteredData.engine_rate = (InputModule.Instance.controller.rightTrigger * 5500) + 700;
            filteredData.steering_input = InputModule.Instance.controller.leftThumb.X;
            filteredData.throttle_input = InputModule.Instance.controller.rightTrigger;
            filteredData.brake_input = InputModule.Instance.controller.leftTrigger;



            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

            byte[] bytes = filteredData.GetBytes();

            mutex.WaitOne();

            if (fillMMF)
            {

                using (MemoryMappedViewStream stream = filteredMMF.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(bytes);
                }
            }

            if(sendUDP)
                telemetrySender.SendAsync(bytes);

            if (fillMMF)
            {
                bytes = rawData.GetBytes();
                using (MemoryMappedViewStream stream = rawMMF.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(bytes);
                }
            }


            lastFilteredData.Copy(filteredData);

            mutex.ReleaseMutex();

            return true;
        }

        void scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            ui.ProgressBarChanged(e.Progress);
        }

        void scan_ScanCanceled(object sender, ScanCanceledEventArgs e)
        {
            ui.InitButtonStatusChanged(true);
        }

        void scan_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            ui.InitButtonStatusChanged(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                ui.StatusTextChanged("Failed!");

                return;
            }

            memoryAddress = e.MemoryAddresses[0] + 541; //offset from found address to start of matrix

            ui.StatusTextChanged("Success");

            t = new Thread(ScanComplete);
            t.Start();
        }
    }
}
