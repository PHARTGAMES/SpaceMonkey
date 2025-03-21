using System;
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
    public class WF2CarAddress
    {
        public string id;
        public Int64 baseAddress;
        public Int64 transformAddress;
    }

    class Wreckfest2TelemetryProvider : GenericProviderBase
    {
        WF2CarAddress selectedCarAddress = null;
        Thread t;
        Process mainProcess = null;

        public string vehicleString;

        public Wreckfest2UI ui;

        public List<WF2CarAddress> carAddresses = new List<WF2CarAddress>();

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

                ui.StatusTextChanged("Wreckfest2.exe exe not running!");
                return;
            }


            //For current WF2 builds we can start at //1400000000 safely. 
            long lStart = 1400000000;
            lStart -= 1000000;//skip a meg back
            if (lStart < 0) lStart = 0;

            long lEnd = 140737488355327;

            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, lStart, lEnd);
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_ScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);


            string scanString = "carRootNode";
            scan.StartScanForString(scanString, ui.GetMaxCarsCount());

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
                        reader.ReadProcessMemory((IntPtr)selectedCarAddress.transformAddress, readSize, out byteReadSize, readBuffer);

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
                    reader.ReadProcessMemory((IntPtr)selectedCarAddress.transformAddress, readSize, out byteReadSize, readBuffer);

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


        public string GetCarId(Int64 address)
        {

            ProcessMemoryReader reader = new ProcessMemoryReader();

            reader.ReadProcess = mainProcess;
            UInt64 readSize = 2;
            byte[] readBuffer = new byte[readSize];
            reader.OpenProcess();
            Int64 byteReadSize;
            reader.ReadProcessMemory((IntPtr)(address + "carRootNode".Length), readSize, out byteReadSize, readBuffer);
            reader.CloseHandle();

            if (byteReadSize == 0)
            {
                return "";
            }

            string id = System.Text.Encoding.UTF8.GetString(readBuffer);

            return id;
        }

        void scan_ScanCompleted(object sender, ScanCompletedEventArgs e)
        {
            ui.InitButtonStatusChanged(true);

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                ui.StatusTextChanged("Failed!");

                return;
            }


            carAddresses.Clear();

            foreach (long address in e.MemoryAddresses)
            {
                WF2CarAddress newAddress = new WF2CarAddress();
                newAddress.id = GetCarId(address);
                newAddress.baseAddress = address;
                newAddress.transformAddress = address - (((4 * 4 * 4) * 2) + 8);//offset backwards from found address to start of matrix
                carAddresses.Add(newAddress);
            }

            ui.RefreshCars(carAddresses);

            ui.StatusTextChanged("2. Select Car");
        }

        public void Initialize(WF2CarAddress selectedCar)
        {
            selectedCarAddress = selectedCar;

            if (selectedCarAddress == null)
                return;

            t = new Thread(ScanComplete);
            t.IsBackground = true;
            t.Start();
        }

        public void SetSelectedCarAddress(WF2CarAddress selectedCar)
        {
            selectedCarAddress = selectedCar;
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
