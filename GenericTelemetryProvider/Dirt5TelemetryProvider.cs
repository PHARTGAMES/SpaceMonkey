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
        GenericProviderData lastTelemetryData;
        Matrix4x4 lastTransform = Matrix4x4.Identity;
        bool lastFrameValid = false;
        Vector3 lastVelocity = Vector3.Zero;

        NestedSmooth accXSmooth = new NestedSmooth(3, 6, 0.5f);
        NestedSmooth accYSmooth = new NestedSmooth(3, 6, 0.5f);
        NestedSmooth accZSmooth = new NestedSmooth(3, 6, 0.5f);

        KalmanFilter velXFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);
        KalmanFilter velYFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);
        KalmanFilter velZFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.02f, 0.0f);

        NoiseFilter velXSmooth = new NoiseFilter(6, 0.5f);
        NoiseFilter velYSmooth = new NoiseFilter(6, 0.5f);
        NoiseFilter velZSmooth = new NoiseFilter(6, 0.5f);

        NoiseFilter pitchFilter = new NoiseFilter(3);
        NoiseFilter rollFilter = new NoiseFilter(3);
        NoiseFilter yawFilter = new NoiseFilter(3);

        KalmanFilter posXFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);
        KalmanFilter posYFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);
        KalmanFilter posZFilter = new KalmanFilter(1, 1, 0.02f, 1, 0.1f, 0.0f);

        NoiseFilter posXSmooth = new NoiseFilter(6, 0.5f);
        NoiseFilter posYSmooth = new NoiseFilter(6, 0.5f);
        NoiseFilter posZSmooth = new NoiseFilter(6, 0.5f);

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
                statusChangedCallback?.Invoke("DIRT5 exe not running!");
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

            Mutex mutex = new Mutex(false, "Dirt5MatrixProviderMutex");


            Stopwatch sw = new Stopwatch();
            sw.Start();


            lastTelemetryData = new GenericProviderData();
            lastTelemetryData.Reset();
            lastTransform = Matrix4x4.Identity;
            lastFrameValid = false;
            lastVelocity = Vector3.Zero;


            float dt = 0.0f;
 
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew("Dirt5MatrixProvider", 10000))
            {

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

                        ProcessTransform(transform, dt);

                        sw.Restart();

                        Thread.Sleep(1000 / 100);
                    }
                    catch (Exception e)
                    {
                        Thread.Sleep(1000);
                    }

                }
            }

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
                lastTransform = transform;
                lastFrameValid = true;
                lastVelocity = Vector3.Zero;
                return true;
            }

            GenericProviderData telemetryData = new GenericProviderData();
            telemetryData.version = versionString;

            if (dt <= 0)
                dt = 1.0f;


            Vector3 worldVelocity = (transform.Translation - lastTransform.Translation) / dt;
            lastTransform = transform;

            Matrix4x4 rotation = new Matrix4x4();
            rotation = transform;
            rotation.M41 = 0.0f;
            rotation.M42 = 0.0f;
            rotation.M43 = 0.0f;
            rotation.M44 = 1.0f;

            Matrix4x4 rotInv = new Matrix4x4();
            Matrix4x4.Invert(rotation, out rotInv);

            Vector3 localVelocity = Vector3.Transform(worldVelocity, rotInv);

            Vector3 localAcceleration = ((localVelocity - lastVelocity) / dt) * 0.10197162129779283f; //convert to g accel
            lastVelocity = localVelocity;


            float pitch = (float)Math.Asin(-fwd.Y);
            float yaw = (float)Math.Atan2(fwd.X, fwd.Z);

            float roll = 0.0f;
            Vector3 rhtPlane = rht;
            rhtPlane.Y = 0;
            rhtPlane = Vector3.Normalize(rhtPlane);
            if (rhtPlane.Length() <= float.Epsilon)
            {
                roll = -(float)(Math.Sign(rht.Y) * Math.PI * 0.5f);
                //                        Debug.WriteLine( "---Roll = " + roll + " " + Math.Sign( rht.Y ) );
            }
            else
            {
                roll = -(float)Math.Asin(Vector3.Dot(up, rhtPlane));
                //                        Debug.WriteLine( "Roll = " + roll + " " + Math.Sign(rht.Y) );
            }
            //                  Debug.WriteLine( "" );

            telemetryData.pitch_raw = pitch;
            telemetryData.yaw_raw = yaw;
            telemetryData.roll_raw = roll;

            telemetryData.position_x_raw = transform.M41;
            telemetryData.position_y_raw = transform.M42;
            telemetryData.position_z_raw = transform.M43;

            telemetryData.local_velocity_x_raw = localVelocity.X;
            telemetryData.local_velocity_y_raw = localVelocity.Y;
            telemetryData.local_velocity_z_raw = localVelocity.Z;

            telemetryData.gforce_lateral_raw = localAcceleration.X;
            telemetryData.gforce_vertical_raw = localAcceleration.Y;
            telemetryData.gforce_longitudinal_raw = localAcceleration.Z;

            GenericProviderData telemetryToSend = new GenericProviderData();
            telemetryToSend.Reset();

            telemetryToSend.Copy(telemetryData);

            telemetryToSend.pitch = pitchFilter.Filter(telemetryData.pitch_raw);
            telemetryToSend.roll = rollFilter.Filter(telemetryData.roll_raw);
            telemetryToSend.yaw = yawFilter.Filter(telemetryData.yaw_raw);

            telemetryToSend.position_x = posXSmooth.Filter(posXFilter.Filter(telemetryData.position_x_raw));
            telemetryToSend.position_y = posYSmooth.Filter(posYFilter.Filter(telemetryData.position_y_raw));
            telemetryToSend.position_z = posZSmooth.Filter(posZFilter.Filter(telemetryData.position_z_raw));

            telemetryToSend.gforce_lateral = accXSmooth.Filter(telemetryData.gforce_lateral_raw);
            telemetryToSend.gforce_vertical = accYSmooth.Filter(telemetryData.gforce_vertical_raw);
            telemetryToSend.gforce_longitudinal = accZSmooth.Filter(telemetryData.gforce_longitudinal_raw);

            telemetryToSend.local_velocity_x = velXSmooth.Filter(velXFilter.Filter(telemetryData.local_velocity_x_raw));
            telemetryToSend.local_velocity_y = velYSmooth.Filter(velYFilter.Filter(telemetryData.local_velocity_y_raw));
            telemetryToSend.local_velocity_z = velZSmooth.Filter(velZFilter.Filter(telemetryData.local_velocity_z_raw));

            lastTelemetryData = telemetryToSend;


//            string debugString = "";
//            debugString += "yaw: " + telemetryToSend.yaw + "\n";

            debugChangedCallback?.Invoke(JsonConvert.SerializeObject(telemetryToSend, Formatting.Indented));


            byte[] writeBuffer = telemetryToSend.ToByteArray();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = mmf.CreateViewStream())
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

            return true;
        }

        void scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            progressBarChangedCallback?.Invoke(e.Progress);
        }

        void scan_ScanCanceled(object sender, ScanCanceledEventArgs e)
        {
            initBtnStatusChangedCallback?.Invoke(true);
        }

        void scan_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            initBtnStatusChangedCallback?.Invoke(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                statusChangedCallback?.Invoke("Failed!");

                return;
            }

            memoryAddress = e.MemoryAddresses[0] + 541; //offset from found address to start of matrix

            statusChangedCallback?.Invoke("Success");

            t = new Thread(ScanComplete);
            t.Start();
        }


    }
}
