﻿using System;
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
using WRCAPI;
using System.Threading.Tasks;

namespace GenericTelemetryProvider
{
    class WRCTelemetryProvider_v2 : GenericProviderBase
    {
        Thread t;

        Int64 vehicleMemoryAddress;
        Int64 wheelMemoryAddress;
        IntPtr wheelGroupMemoryAddress;
        public WRCUI ui;
        Process mainProcess = null;

        WRCWheelGroupData wheelsGroupData;

        double minUpdateDelay = 2;
        double maxUpdateDelay = 1000.0 / 50;


        public override void Run()
        {
            updateDelay = 1000.0 / 120;
            base.Run();

            maxAccel2DMagSusp = 6.0f;
            telemetryPausedTime = 1.5f;

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("WRC9"))
                    mainProcess = process;
            }

            if (mainProcess == null) //no processes, better stop
            {
                ui.StatusTextChanged("WRC9 exe not running!");
                return;
            }
            
            FindWheelGroupPointerAddress();

            ui.InitButtonStatusChanged(true);
            ui.StatusTextChanged("If telemetry doesn't update press Initialize!");

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
            
            /*
        
        RegularMemoryScan scan = new RegularMemoryScan(mainProcess, 0,  140737488355327); //32gig //
        scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
        scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_PatternScanCompleted);
        scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

        List<RegularMemoryScan.FloatPatternStep> pattern = new List<RegularMemoryScan.FloatPatternStep>();

        float minValue = 2500;
        float maxValue = 4000;
        pattern.Add(new RegularMemoryScan.FloatPatternStep(minValue, maxValue, 0));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 16));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.00005f, 0.00005f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(minValue, maxValue, 92));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 16));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.00005f, 0.00005f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(minValue, maxValue, 92));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 16));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.05f, 0.05f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-0.00005f, 0.00005f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(0.1f, float.MaxValue, 4, RegularMemoryScan.FloatPatternStep.Type.Absolute));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(minValue, maxValue, 92));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));
        pattern.Add(new RegularMemoryScan.FloatPatternStep(-1.0f, 1.0f, 4));

        scan.StartScanForFloatPattern(pattern);
        */
        /*

        RegularMemoryScan scan = new RegularMemoryScan(mainProcess, 0, 140737488355327); //32gig
        scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
        scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_VehicleScanCompleted);
        scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

        byte[] vehicleBytes = new byte[] { 0x57, 0x72, 0x63, 0x56, 0x65, 0x68, 0x69, 0x63, 0x6C, 0x65, 0x00 };
        scan.StartScanForByteArray(vehicleBytes);
        */
        }

        void FindWheelGroupPointerAddress()
        {
            Int64[] ptrOffs = new Int64[] { 0x390, 0x80, 0xDE8, 0x1A8, 0x18 };
            wheelGroupMemoryAddress = Utils.GetPointerAddress(mainProcess, mainProcess.MainModule.BaseAddress + 0x019FC238, ptrOffs);
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

            wheelGroupMemoryAddress = (IntPtr)e.MemoryAddresses[e.MemoryAddresses.Length - 1] - 100;

            //fixme: remove dis and look for vehicle, meby
            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();

        }
        /*
        void scan_VehicleScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            ui.InitButtonStatusChanged(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                ui.StatusTextChanged("Failed!");

                return;
            }

            vehicleMemoryAddress = e.MemoryAddresses[e.MemoryAddresses.Length - 1]; 

            ui.StatusTextChanged("Second Scan Started");

            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, 0, 140737488355327); //32gig
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_WheelScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

            byte[] wheelBytes = new byte[] { 0x57, 0x72, 0x63, 0x50, 0x68, 0x79, 0x73, 0x69, 0x63, 0x57, 0x68, 0x65, 0x65, 0x6C, 0x00, 0x00, 0x0E };
            scan.StartScanForByteArray(wheelBytes, 4);
        }


        void scan_WheelScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            ui.InitButtonStatusChanged(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                ui.StatusTextChanged("Failed!");

                return;
            }

            wheelMemoryAddress = e.MemoryAddresses[e.MemoryAddresses.Length - 1]; 

            ui.StatusTextChanged("Success");

            t = new Thread(ReadTelemetry);
            t.IsBackground = true;
            t.Start();
        }
        */

        public override void Stop()
        {
            base.Stop();


        }

        private byte[] ReadBuffer(Stream memoryMappedViewStream, int size)
        {
            using (var reader = new BinaryReader(memoryMappedViewStream))
            {
                return reader.ReadBytes(size);
            }
        }

        protected Object ReadSharedMemory(Type T, string sharedMemoryFile)
        {
            Object data;
            int sharedMemorySize = Marshal.SizeOf(T); ;
            using (var mappedFile = MemoryMappedFile.OpenExisting(sharedMemoryFile))
            {
                using (var memoryMappedViewStream = mappedFile.CreateViewStream())
                {
                    var buffer = ReadBuffer(memoryMappedViewStream, sharedMemorySize);
                    var alloc = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    data = Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), T);
                    memoryMappedViewStream.Close();
                    alloc.Free();
                }
            }
            return data;
        }

        void ReadTelemetry()
        {

            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 wheelGroupReadSize = (UInt64)Marshal.SizeOf(typeof(WRCWheelGroupData));
            byte[] wheelGroupReadBuffer = new byte[wheelGroupReadSize];
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
                    double frameDT = 0;
                    while(true)
                    {
                        frameDT = sw.Elapsed.TotalSeconds;
                        if (frameDT >= (updateDelay / 1000.0f))
                            break;
                    }
                    sw.Restart();

//                    mainProcess.Suspend();
                    if (droppedSampleCount >= 100)
                    {
                        FindWheelGroupPointerAddress();
                        droppedSampleCount = 0;
                        Debug.WriteLine("Finding Pointer");
                    }

                    Int64 byteReadSize;
                    reader.ReadProcessMemory((IntPtr)wheelGroupMemoryAddress, wheelGroupReadSize, out byteReadSize, wheelGroupReadBuffer);
//                    mainProcess.Resume();

                    if (byteReadSize == 0)
                    {
                        FindWheelGroupPointerAddress();
                        droppedSampleCount = 0;
                        Debug.WriteLine("Finding Pointer");
                        continue;
                    }

                    var alloc = GCHandle.Alloc(wheelGroupReadBuffer, GCHandleType.Pinned);
                    wheelsGroupData = (WRCWheelGroupData)Marshal.PtrToStructure(alloc.AddrOfPinnedObject(), typeof(WRCWheelGroupData));
                    alloc.Free();

                    ProcessWRCData((float)frameDT);

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



        void ProcessWRCData(float _dt)
        {
            if (wheelsGroupData == null)
                return;
            Debug.WriteLine(wheelsGroupData.wheelFL.pos1X);
            Debug.WriteLine(wheelsGroupData.wheelFL.pos1Y);
            Debug.WriteLine(wheelsGroupData.wheelFL.pos1Z);
            transform = Matrix4x4.Identity;

            Vector3 wPosFL = new Vector3(wheelsGroupData.wheelFL.pos1X, wheelsGroupData.wheelFL.pos1Z, wheelsGroupData.wheelFL.pos1Y);
            Vector3 wPosFR = new Vector3(wheelsGroupData.wheelFR.pos1X, wheelsGroupData.wheelFR.pos1Z, wheelsGroupData.wheelFR.pos1Y);
            Vector3 wPosRL = new Vector3(wheelsGroupData.wheelRL.pos1X, wheelsGroupData.wheelRL.pos1Z, wheelsGroupData.wheelRL.pos1Y);
            Vector3 wPosRR = new Vector3(wheelsGroupData.wheelRR.pos1X, wheelsGroupData.wheelRR.pos1Z, wheelsGroupData.wheelRR.pos1Y);

            fwd = Vector3.Normalize(wPosFL - wPosRL);
            rht = Vector3.Normalize(wPosRR - wPosRL);
            up = Vector3.Cross(fwd, rht);
            rht = Vector3.Normalize(Vector3.Cross(up, fwd));

            Vector3 pos = (wPosFL + wPosFR + wPosRL + wPosRR) * 0.25f;


            transform.M11 = rht.X;
            transform.M12 = rht.Y;
            transform.M13 = rht.Z;

            transform.M21 = up.X;
            transform.M22 = up.Y;
            transform.M23 = up.Z;

            transform.M31 = fwd.X;
            transform.M32 = fwd.Y;
            transform.M33 = fwd.Z;

            transform.M41 = pos.X;
            transform.M42 = pos.Y;
            transform.M43 = pos.Z;
            transform.M44 = 1.0f;

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

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;

            //fixed dt for wrc
//            dt = (float)updateDelay / 1000.0f;
        }

        Matrix4x4 LerpRotTransform(Matrix4x4 from, Matrix4x4 to, float lerp)
        {
            to.M11 = from.M11 + (to.M11 - from.M11) * lerp;
            to.M12 = from.M12 + (to.M12 - from.M12) * lerp;
            to.M13 = from.M13 + (to.M13 - from.M13) * lerp;

            to.M21 = from.M21 + (to.M21 - from.M21) * lerp;
            to.M22 = from.M22 + (to.M22 - from.M22) * lerp;
            to.M23 = from.M23 + (to.M23 - from.M23) * lerp;

            to.M31 = from.M31 + (to.M31 - from.M31) * lerp;
            to.M32 = from.M32 + (to.M32 - from.M32) * lerp;
            to.M33 = from.M33 + (to.M33 - from.M33) * lerp;

            return to;
        }

        int errorCount = 0;
        
        public override void CalcPosition()
        {

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            Vector3 vel = (currRawPos - lastRawPos);
            float velMag = vel.Length();

//            Debug.WriteLine("zpos: " + currRawPos.Z);
//            Debug.WriteLine("zmag: " + vel.Z);
            Debug.WriteLine("updateDelay: " + updateDelay);

            int maxErrors = 1000;

            double updateDelayOffs = 0.125;
            if (velMag == 0)
//            if(lastTransform == transform)
            {
                errorCount = Math.Min(maxErrors, errorCount + 1);
                updateDelay = Math.Max(minUpdateDelay, Math.Min(updateDelay + updateDelayOffs, maxUpdateDelay));
                throw new Exception("CalcPosition: VelMag == 0");
            }

            transform = LerpRotTransform(lastTransform, transform, Math.Min(1.0f, 3.0f * dt));

            if (errorCount > 0)
                updateDelay = Math.Max(minUpdateDelay, Math.Min(updateDelay - (updateDelayOffs * 0.75), maxUpdateDelay));
            errorCount = Math.Max(0, errorCount - 1);


            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

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

            rotInv = new Matrix4x4();
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

            rawData.max_rpm = 6000;
            rawData.max_gears = 6;
            rawData.gear = 1;
            rawData.idle_rpm = 700;


            Vector3 localVelocity = new Vector3((float)filteredData.local_velocity_x, (float)filteredData.local_velocity_y, (float)filteredData.local_velocity_z);

            filteredData.speed = localVelocity.Length();

        }

        public override void ProcessInputs()
        {
            base.ProcessInputs();

  //          filteredData.engine_rate = Math.Max(700, Math.Min(6000, 700 + (data.engine_rpm * (6000-700))));
        }
    }


    public static class ProcessExtension
    {
        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);

        public static void Suspend(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                SuspendThread(pOpenThread);
            }
        }
        public static void Resume(this Process process)
        {
            foreach (ProcessThread thread in process.Threads)
            {
                var pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)thread.Id);
                if (pOpenThread == IntPtr.Zero)
                {
                    break;
                }
                ResumeThread(pOpenThread);
            }
        }
    }
}
