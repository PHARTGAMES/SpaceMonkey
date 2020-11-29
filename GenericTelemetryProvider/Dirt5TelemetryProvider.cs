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

namespace GenericTelemetryProvider
{
    public class Dirt5TelemetryProvider : GenericProviderBase
    {

        Int64 memoryAddress;
        Thread t;
        Process mainProcess = null;

        public string vehicleString;
        GenericProviderData lastFilteredData;
        GenericProviderData filteredData;
        GenericProviderData rawData;
        Matrix4x4 lastTransform = Matrix4x4.Identity;
        bool lastFrameValid = false;
        Vector3 lastVelocity = Vector3.Zero;
        public Dirt5UI ui;
        public float lastWorldVelMag = 0.0f;
        private TelemetrySender telemetrySender = new TelemetrySender();
        SC4DR2CustomTelemetry lastUDPData = new SC4DR2CustomTelemetry();


        NestedSmooth dtFilter = new NestedSmooth(0, 60, 1000000.0f);

        //NestedSmooth velXFilter = new NestedSmooth(2, 100, 1000.0f);
        //NestedSmooth velYFilter = new NestedSmooth(2, 100, 1000.0f);
        //NestedSmooth velZFilter = new NestedSmooth(2, 100, 1000.0f);

        int posKeyMask = GenericProviderData.GetKeyMask(GenericProviderData.DataKey.PositionX, GenericProviderData.DataKey.PositionY, GenericProviderData.DataKey.PositionZ);
        int velKeyMask = GenericProviderData.GetKeyMask(GenericProviderData.DataKey.LocalVelX, GenericProviderData.DataKey.LocalVelY, GenericProviderData.DataKey.LocalVelZ);


        char[] versionString = new char[] { 'D', 'I', 'R', 'T', '5', '0', '0', '1' };

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


            lastTransform = Matrix4x4.Identity;
            lastFrameValid = false;
            lastVelocity = Vector3.Zero;

            filteredData = new GenericProviderData(versionString);
            rawData = new GenericProviderData(versionString);

            filteredData.version = versionString;
            rawData.version = versionString;

            telemetrySender.StartSending("127.0.0.1", 10001);

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

            telemetrySender.StopSending();

        }

        //        int worldPosFailCounter = 0;

        bool ProcessTransform(Matrix4x4 transform, float dt)
        {

            Vector3 rht = new Vector3(transform.M11, transform.M12, transform.M13);
            Vector3 up = new Vector3(transform.M21, transform.M22, transform.M23);
            Vector3 fwd = new Vector3(transform.M31, transform.M32, transform.M33);

            //            rht = Vector3.Normalize(rht);
            //            up = Vector3.Normalize(up);
            //            fwd = Vector3.Normalize(fwd);

            float rhtMag = rht.Length();
            float upMag = up.Length();
            float fwdMag = fwd.Length();

            //transform.M11 = rht.X;
            //transform.M12 = rht.Y;
            //transform.M13 = rht.Z;

            //transform.M21 = up.X;
            //transform.M22 = up.Y;
            //transform.M23 = up.Z;

            //transform.M31 = fwd.X;
            //transform.M32 = fwd.Y;
            //transform.M33 = fwd.Z;

            //reading garbage
            if (rhtMag < 0.9f || upMag < 0.9f || fwdMag < 0.9f)
            {
                return false;
            }

            if (!lastFrameValid)
            {
                lastFilteredData = new GenericProviderData(versionString);
                lastTransform = transform;
                lastFrameValid = true;
                lastVelocity = Vector3.Zero;
                lastWorldVelMag = 0.0f;
                return true;
            }

            dt = dtFilter.Filter(dt);

            if (dt <= 0)
                dt = 0.015f;
            /*
                        Vector3 currPos = Vector3.Lerp(lastTransform.Translation, transform.Translation, Math.Min(1.0f, dt * 5.0f));
            //            Vector3 currPos = Vector3.Lerp(lastTransform.Translation, transform.Translation, 0.333f);

                        //           Vector3 currPos = new Vector3(posXFilter.Filter(transPos.X), posYFilter.Filter(transPos.Y), posZFilter.Filter(transPos.Z));

                        Vector3 worldVelocity = (currPos - lastTransform.Translation)  / dt;
            //            Vector3 worldVelocity = (transform.Translation - lastTransform.Translation) / dt;
            */

            //                     Vector3 currPos = transform.Translation;

            rawData.position_x = transform.M41;
            rawData.position_y = transform.M42;
            rawData.position_z = transform.M43;

            //filter position
            FilterModule.Instance.Filter(rawData, ref filteredData, posKeyMask, true);




            //            Vector3 currPos = Vector3.Lerp(lastTransform.Translation, transform.Translation, Math.Min(1.0f, dt * 4.0f));

            //assign
            Vector3 worldPosition = new Vector3(filteredData.position_x, filteredData.position_y, filteredData.position_z);

            Vector3 worldVelocity = (worldPosition - lastTransform.Translation) / dt;

            transform.Translation = worldPosition;
            lastTransform = transform;

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
            FilterModule.Instance.Filter(rawData, ref filteredData, velKeyMask, false);

            //assign filtered local velocity
            localVelocity = new Vector3(filteredData.local_velocity_x, filteredData.local_velocity_y, filteredData.local_velocity_z);

            //calculate local acceleration
            Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt) * 0.10197162129779283f; //convert to g accel
            //            Vector3 localAcceleration = (localVelocity - lastVelocity) * 0.10197162129779283f; //convert to g accel
            //Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt);
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
            FilterModule.Instance.Filter(rawData, ref filteredData, int.MaxValue & ~(posKeyMask | velKeyMask), false);

            InputModule.Instance.Update();

            //            string debugString = "";
            //            debugString += "yaw: " + telemetryToSend.yaw + "\n";

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

            byte[] writeBuffer = filteredData.ToByteArray();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = filteredMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }

            writeBuffer = rawData.ToByteArray();
            using (MemoryMappedViewStream stream = rawMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(writeBuffer);
            }


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
            mutex.ReleaseMutex();

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

            lastFilteredData.Copy(filteredData);
            lastUDPData.Copy(udpData);

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
