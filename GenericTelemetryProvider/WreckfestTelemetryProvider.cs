﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using System.IO;
using System.IO.MemoryMappedFiles;
using NoiseFilters;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using CMCustomUDP;

namespace GenericTelemetryProvider
{
    class WreckfestTelemetryProvider : GenericProviderBase
    {
        Int64 memoryAddress;
        Thread t;
        Process mainProcess = null;

        public string vehicleString;

        public WreckfestUI ui;


        public override void Run()
        {
            base.Run();


            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Wreckfest"))
                    mainProcess = process;
            }

            if (mainProcess == null) //no processes, better stop
            {

                ui.StatusTextChanged("Wreckfest_x64.exe exe not running!");
                return;
            }


            //For current WF builds we can start at //1400000000 safely. 
            long lStart = 1400000000;
            lStart -= 1000000;//skip a meg back
            if (lStart < 0) lStart = 0;

            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, lStart, 140737488355327); //32gig            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_ScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);


            string scanString = "carRootNode" + vehicleString;
            scan.StartScanForString(scanString, 1);

        }

        void ScanComplete()
        {
            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            byte[] lastReadBuffer = new byte[readSize];
            reader.OpenProcess();

            float frameRateSecs = 1.0f / 60.0f;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            StartSending();

            while (!IsStopped)
            {
                try
                {
                    Matrix4x4 transform = Matrix4x4.Identity;
                    Int64 byteReadSize;
                    bool different = false;
                    do
                    {
                        //read
                        reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out byteReadSize, readBuffer);

                        if (byteReadSize == 0)
                        {
                            continue;
                        }

                        //check if different
                        for (int i = 0; i < (int)readSize; ++i)
                        {
                            if (readBuffer[i] != lastReadBuffer[i])
                            {
                                different = true;
                                break;
                            }
                        }

                        //sleep until the end of the frame
                        if(different)
                            Thread.Sleep(1);

                    } while (!different);

                    
                    //read transform
                    reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out byteReadSize, readBuffer);

                    if (byteReadSize == 0)
                    {
                        Console.WriteLine("REEAAALLY DONT WANT THIS TO HAPPEN");
                        continue;
                    }

                    Buffer.BlockCopy(readBuffer, 0, lastReadBuffer, 0, readBuffer.Length);

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

                    Matrix4x4 newTransform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                , floats[4], floats[5], floats[6], floats[7]
                                , floats[8], floats[9], floats[10], floats[11]
                                , floats[12], floats[13], floats[14], floats[15]);

                    ProcessTransform(newTransform, frameRateSecs);

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

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger + ", " + "\n rht: " + rht.X + ", " + rht.Y + ", " + rht.Z + "\n up: " + up.X + ", " + up.Y + ", " + up.Z + "\n fwd: " + fwd.X + ", " + fwd.Y + ", " + fwd.Z);

            SendFilteredData();

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

//            memoryAddress = e.MemoryAddresses[0] - ((4 * 4 * 4) + 4); //offset backwards from found address to start of matrix
//            memoryAddress = e.MemoryAddresses[0] - ((4 * 4 * 4) + 8); //offset backwards from found address to start of matrix
            memoryAddress = e.MemoryAddresses[0] - (((4 * 4 * 4) * 2) + 8); //offset backwards from found address to start of matrix

            ui.StatusTextChanged("Success");

            t = new Thread(ScanComplete);
            t.IsBackground = true;
            t.Start();
        }

        //public override void CalcAngles()
        //{
        //    base.CalcAngles();

        //    rawData.roll = -(float)rawData.roll;
        //    rawData.yaw = -(float)rawData.yaw;
        //}


        //public override void CalcVelocity()
        //{
        //    base.CalcVelocity();

        //    rawData.local_velocity_x = -(float)rawData.local_velocity_x;
        //}

        public override void StopAllThreads()
        {
            base.StopAllThreads();

            if (t != null)
                t.Join();

        }


    }

}
