using System;
using System.Collections.Generic;
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
using System.Threading.Tasks;
using System.Linq;

namespace GenericTelemetryProvider
{
    class SquadronsTelemetryProvider : GenericProviderBase
    {
        Thread t;

        Int64 matrixGroupMemoryAddress;
        public SquadronsUI ui;
        Process mainProcess = null;
        IntPtr matrixAddress;


        public override void Run()
        {
            updateDelay = 16;
            base.Run();

            maxAccel2DMagSusp = 6.0f;
            telemetryPausedTime = 1.5f;

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("starwarssquadrons"))
                    mainProcess = process;
            }

            if (mainProcess == null) //no processes, better stop
            {
                ui.StatusTextChanged("starwarssquadrons.exe not running!");
                return;
            }

            /*
            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, 0,  140737488355327); //32gig //
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_PatternScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

            List<RegularMemoryScan.FloatPatternStep> pattern = new List<RegularMemoryScan.FloatPatternStep>();

//            pattern.Add(new RegularMemoryScan.FloatPatternStep(215.1884f, 215.1886f, 0));


            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 0, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 0.0f, 4));

            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 0.0f, 4));

            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 1.0f, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 0.0f, 4));

            pattern.Add(new RegularMemoryScan.FloatPatternStep(3.0f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(3.0f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(3.0f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
            pattern.Add(new RegularMemoryScan.FloatPatternStep(0.0f, 0.0f, 4));

            scan.StartScanForFloatPattern(pattern);

            //byte[] bytes = new byte[] { 0x48, 0x30, 0x57, 0x43 };
            //scan.StartScanForByteArray(bytes, -1);

            */

            FindMatrixPointerAddress();

            ui.InitButtonStatusChanged(true);
            ui.StatusTextChanged("If telemetry doesn't update press Initialize!");

            t = new Thread(ReadTelemetry);
            t.Start();

        }

        void FindMatrixPointerAddress()
        {
            Int64[] matrixPtrOffs = new Int64[] { 0x40, 0xC0, 0x88, 0x68, 0x3E0 };
            matrixAddress = Utils.GetPointerAddress(mainProcess, mainProcess.MainModule.BaseAddress + 0x03E9FA18, matrixPtrOffs);
        }


        void scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
            ui.ProgressBarChanged(e.Progress);
        }

        void scan_ScanCanceled(object sender, ScanCanceledEventArgs e)
        {
            ui.InitButtonStatusChanged(true);
        }

        void scan_PatternScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            ui.InitButtonStatusChanged(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                ui.StatusTextChanged("Failed!");

                return;
            }

            List<long> narrowedList = NarrowMemoryAddresses(e.MemoryAddresses);

            matrixGroupMemoryAddress = e.MemoryAddresses[e.MemoryAddresses.Length - 1];


            t = new Thread(ReadTelemetry);
            t.Start();
        }

       
        List<long> NarrowMemoryAddresses(long[] addresses)
        {
            if (addresses == null || addresses.Length == 0)
                return null;

            List<long> returnList = new List<long>();

            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;

            UInt64 readSize = ((4 * 4 * 4) * 3) + ((4 * 4 * 6) * 2);
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();
            float[] floats = new float[4 * 4];


            foreach (long address in addresses)
            {
                Int64 byteReadSize;
                reader.ReadProcessMemory((IntPtr)address, readSize, out byteReadSize, readBuffer);

                if (byteReadSize == 0)
                {
                    continue;
                }

                Buffer.BlockCopy(readBuffer, 0, floats, 0, (4 * 4 * 4));

                Vector3 r = new Vector3(floats[0], floats[1], floats[2]);
                Vector3 u = new Vector3(floats[4], floats[5], floats[6]);
                Vector3 f = new Vector3(floats[8], floats[9], floats[10]);

                float rl = r.Length();
                float ul = u.Length();
                float fl = f.Length();

                float minVal = 0.9f;
                float maxVal = 1.1f;
                if (rl >= minVal && rl <= maxVal && ul >= minVal && ul <= maxVal && fl >= minVal && fl <= maxVal)
                {

                    byte[] a = readBuffer.Take((4 * 4 * 4)).ToArray();

                    byte[] b = readBuffer.Skip((4 * 4 * 4) + (4 * 4 * 6)).Take((4 * 4 * 4)).ToArray();

                    byte[] c = readBuffer.Skip(((4 * 4 * 4) + (4 * 4 * 6)) * 2).Take((4 * 4 * 4)).ToArray();

                    if (ArraysEqual(a, b))
                    {
                        if(ArraysEqual(a, c))
                            returnList.Add(address);
                    }
                }
            }

            reader.CloseHandle();

            return returnList;

        }




        public static bool ArraysEqual(byte[] array1, byte[] array2, int bytesToCompare = 0)
        {
            if (array1.Length != array2.Length) return false;

            var length = (bytesToCompare == 0) ? array1.Length : bytesToCompare;
            var tailIdx = length - length % sizeof(Int64);

            //check in 8 byte chunks
            for (var i = 0; i < tailIdx; i += sizeof(Int64))
            {
                if (BitConverter.ToInt64(array1, i) != BitConverter.ToInt64(array2, i)) return false;
            }

            //check the remainder of the array, always shorter than 8 bytes
            for (var i = tailIdx; i < length; i++)
            {
                if (array1[i] != array2[i]) return false;
            }

            return true;
        }

        public override void Stop()
        {
            base.Stop();

        }

        void ReadTelemetry()
        {

            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();


            StartSending();

            Stopwatch sw = new Stopwatch();
            sw.Start();


            //read and process
            while (!IsStopped)
            {
                try
                {
                    //dis is super accurate for timing
                    float frameDT = 0;
                    while(true)
                    {
                        frameDT = (float)sw.Elapsed.TotalSeconds;
                        if (frameDT >= (updateDelay / 1000.0f))
                            break;
                    }
                    sw.Restart();

                    Int64 byteReadSize;
                    reader.ReadProcessMemory((IntPtr)matrixAddress, readSize, out byteReadSize, readBuffer);

                    if (byteReadSize == 0)
                    {
                        FindMatrixPointerAddress();
                        continue;
                    }

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);



                    Matrix4x4 trans = new Matrix4x4(floats[0], floats[1], floats[2], 0.0f
                                                , floats[4], floats[5], floats[6], 0.0f
                                                , floats[8], floats[9], floats[10], 0.0f
                                                , floats[12], floats[13], floats[14], 1.0f);

                    ProcessSquadronsData(trans, frameDT);

                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }

            reader.CloseHandle();

            StopSending();
            Thread.CurrentThread.Join();

        }



        void ProcessSquadronsData(Matrix4x4 _transform, float _dt)
        {
            if (_transform == null)
                return;

            transform = _transform;

            ProcessTransform(transform, _dt);
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
            return base.ExtractFwdUpRht();
        }

        public override bool CheckLastFrameValid()
        {
            return base.CheckLastFrameValid();
        }

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;

            //fixed dt for Squadrons
            dt = updateDelay / 1000.0f;
        }

        
        public override bool CalcPosition()
        {

            if (transform == lastTransform)
            {
                return false;
            }

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            lastRawPos = currRawPos;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            return true;
        }
        

        public override void CalcVelocity()
        {
            Vector3 worldVelocity = (worldPosition - lastPosition) / dt;
            lastWorldVelocity = worldVelocity;

            //Debug.WriteLine("Velocity: " + worldVelocity.Length());


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

//            localVelocity = worldVelocity;
//            localVelocity.X = -localVelocity.X;

            rawData.local_velocity_x = localVelocity.X;
            rawData.local_velocity_y = localVelocity.Y;
            rawData.local_velocity_z = localVelocity.Z;

        }


        public override void CalcAngles()
        {
            Quaternion quat = Quaternion.CreateFromRotationMatrix(transform);

            Vector3 pyr = Utils.GetPYRFromQuaternion(quat);

            rawData.pitch = pyr.X;
            rawData.yaw = pyr.Y;
            rawData.roll = Utils.LoopAngleRad(pyr.Z, (float)Math.PI * 0.5f);
        }

        public override void SimulateEngine()
        {
/*
            rawData.max_rpm = data.engine_max_rpm;
            rawData.max_gears = 6;
            rawData.gear = data.gear;
            rawData.idle_rpm = data.engine_idle_rpm;
*/
            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();

        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

  //          filteredData.engine_rate = Math.Max(700, Math.Min(6000, 700 + (data.engine_rpm * (6000-700))));
        }

        /*
       List<long> NarrowMemoryAddresses(long[] addresses)
       {
           if (addresses == null || addresses.Length == 0)
               return null;

           List<long> returnList = new List<long>();

           ProcessMemoryReader reader = new ProcessMemoryReader();

           reader.ReadProcess = mainProcess;

           UInt64 readSize = (4 * 4 * 4) * 2;
           byte[] readBuffer = new byte[readSize];
           reader.OpenProcess();
           int halfReadSize = (int)(readSize / 2);

           foreach (long address in addresses)
           {
               Int64 byteReadSize;
               reader.ReadProcessMemory((IntPtr)address, readSize, out byteReadSize, readBuffer);

               if (byteReadSize == 0)
               {
                   continue;
               }

               byte[] a = readBuffer.Take(halfReadSize).ToArray();
               byte[] b = readBuffer.Skip(halfReadSize).Take(halfReadSize).ToArray();

               if(ArraysEqual(a,b))
               {
                   returnList.Add(address);
               }

           }

           reader.CloseHandle();

           return returnList;

       }
       */
    }
}
