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
    public class Dirt5TelemetryProvider : GenericProviderBase
    {

        Int64 memoryAddress;
        Thread t;
        Process mainProcess = null;

        public string vehicleString;

        public Dirt5UI ui;


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

            /*
            string scanString = "";
            while (scanString.Length < 24)
                scanString += "\0";
            scanString += "A";

            if (vehicleString.Length < 20)
                scanString = scanString.Remove(20, 1).Insert(20, "\u0001");

            scanString = scanString.Remove(0, vehicleString.Length).Insert(0, vehicleString);
            */

            string scanString = "B`U>B`U>";


            scan.StartScanForString(scanString, 1);

        }

        void ScanComplete()
        { 
            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();

            Stopwatch sw = new Stopwatch();
            sw.Start();


            StartSending();
 
            while (!IsStopped)
            {
                try
                {

                    double frameDT = 0;
                    while (true)
                    {
                        frameDT = sw.Elapsed.TotalSeconds;
                        if (frameDT >= (updateDelay / 1000.0f))
                            break;
                    }
                    sw.Restart();

                    Int64 byteReadSize;
                    reader.ReadProcessMemory((IntPtr)memoryAddress, readSize, out byteReadSize, readBuffer);

                    if (byteReadSize == 0)
                    {
                        continue;
                    }

                    float[] floats = new float[4 * 4];

                    Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

                    Matrix4x4 transform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                , floats[4], floats[5], floats[6], floats[7]
                                , floats[8], floats[9], floats[10], floats[11]
                                , floats[12], floats[13], floats[14], floats[15]);

                    ProcessTransform(transform, (float)frameDT);

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
            try
            {
                base.ProcessTransform(newTransform, inDT);
            }
            catch (Exception e)
            {
                Console.WriteLine("ProcessTransform: " + e);
            }




            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);

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

            //            memoryAddress = e.MemoryAddresses[0] + 1208; //offset from found address to start of matrix
            memoryAddress = e.MemoryAddresses[0] - 152; //offset from found address to start of matrix

            ui.StatusTextChanged("Success");

            t = new Thread(ScanComplete);
            t.IsBackground = true;
            t.Start();
        }

        public override void StopAllThreads()
        {
            base.StopAllThreads();

            if (t != null)
                t.Join();

        }

  

        public override void FilterDT()
        {
            if (dt <= 0)
                dt = 0.015f;
        }


    }
}
