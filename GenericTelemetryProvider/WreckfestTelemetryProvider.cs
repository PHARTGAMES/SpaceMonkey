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

            //todo also scan for the 64 bit exe
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

            //Warning! This also covers 64 bit, so..

            if (mainProcess.MainWindowTitle.Contains("32bit") == false)
            {
                ui.StatusTextChanged("Wreckfest 32-bit supported only!");
                return;//todo work out why I can't re-click init
            }

            //For current WF builds we can start at //1400000000 safely. This is set in the UI

            long lStart = ui.StartAddressIndex;
            lStart -= 1000000;//skip a meg back
            if (lStart < 0) lStart = 0;

            RegularMemoryScan scan = new RegularMemoryScan(mainProcess, lStart, 34359720776);// 140737488355327); //32gig
            scan.ScanProgressChanged += new RegularMemoryScan.ScanProgressedEventHandler(scan_ScanProgressChanged);
            scan.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(scan_ScanCompleted);
            scan.ScanCanceled += new RegularMemoryScan.ScanCanceledEventHandler(scan_ScanCanceled);

            ui.StatusTextChanged("Starting Search from " + lStart);

            //Search string in WF memory
            string scanString = "carRootNode" + vehicleString;
            //incidentally, this would have been a better place to set the offset
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

            double frameDT = 0;

            while (!IsStopped)
            {
                try
                {
                    //temp
                    
                    
                  
                    /*
                    //Loop until the StopWatch has exceeded updateDelay. This uses up all of the CPU time on this thread!
                    while (true)
                    {
                        frameDT = sw.Elapsed.TotalSeconds;
                        if (frameDT >= (updateDelay / 1000.0f))
                            break;
                    }*/

                    //Instead; sleep by the remaining amount. This releases CPU time to the system.
                    double lrSleepMS = (updateDelay - sw.Elapsed.TotalMilliseconds);
                    
                    if (lrSleepMS < 0)
                    {
                        //We overran!
                    }
                    else
                    {
                        
                        if (lrSleepMS < 1f) lrSleepMS = 1f;//do not allow strangling of the CPU
                        
                        Thread.Sleep((int)lrSleepMS);
                    }
                
                    frameDT += sw.Elapsed.TotalSeconds;//this should hopefully just be exactly updateDelay * 1000
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

                    if (ProcessTransform(transform, (float)frameDT))
                    {
                        //success! - reset DT
                        frameDT = 0;
                    }
                    else
                    {
                        //duplicate data from WF - keep frameDT and accumlate
                    }

                    //debug
//                    TrackedValues.Add((float)filteredData.gforce_longitudinal);


                    updateDelay = (double)ui.SleepTime;//push this into the base class (todo, this not like this)

                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }

            }
            reader.CloseHandle();

            Console.WriteLine("Ending WreckFest Telemetry Provider");

            StopSending();

          //  Thread.CurrentThread.Join();//this seems to crash everything

            Console.WriteLine("Closing WreckFest Telemetry Provider");

        }

        public List<float> TrackedValues = new List<float>();

        public override bool ProcessTransform(Matrix4x4 newTransform, float inDT)
        {
            if (!base.ProcessTransform(newTransform, inDT))
                return false;

            if ((dt * 1000) < 1f)
            {
                ui.StatusTextChanged("BAD DT");
            }

            
            //Todo - Update this text with something much betterer 
            ui.DebugTextChanged(JsonConvert.SerializeObject(filteredData, Formatting.Indented) + "\n dt: " + dt + "\n steer: " + InputModule.Instance.controller.leftThumb.X + "\n accel: " + InputModule.Instance.controller.rightTrigger + "\n brake: " + InputModule.Instance.controller.leftTrigger);


#if truex
//Track scale of rubbish gforce data
            string lStr = "";
            for (int i=1;i<TrackedValues.Count;i++)
            {
                float lrDiff = TrackedValues[i] - TrackedValues[i - 1];
                //1700!
                if (Math.Abs(lrDiff) > 1500f)
                {
                    //                    lStr += i + ":" + TrackedValues[i].ToString("F2") + "\n";
                    //lStr += i + ":" + lrDiff.ToString("F2") + "\n";

                    lStr += TrackedValues[i-1].ToString("F2") + "->" + TrackedValues[i].ToString("F2")  + "\n";

                }
            }

            if (TrackedValues.Count > 6400) TrackedValues.RemoveAt(0);

            ui.DebugTextChanged(lStr);
#endif

            //Hm should I have just applied all my hacks here? :)


            SendFilteredData();

            return true;
        }

        void scan_ScanProgressChanged(object sender, ScanProgressChangedEventArgs e)
        {
//            ui.ProgressBarChanged(e.Progress);
            //pretty sure I know what it'll be ;)

            //mmm that casting.
            float lrProgress = (float)((double)e.MemoryAddress / (double)1700000000);//this is about the highest it could reasonably get
            int lnProgress = (int)(lrProgress * 100f);
            lnProgress %= 100;//if we wrap the memory address was guessed wrongly/has changed/other issue
            ui.ProgressBarChanged(lnProgress);

            ui.StatusTextChanged(e.MemoryAddress.ToString());
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

            memoryAddress = e.MemoryAddresses[0] - ((4 * 4 * 4) + 4); //offset backwards from found address to start of matrix

            //If possible, start scanning, say, a meg before this address, then scan the whole thing if it fails, so it's all quicker
            //1665611616
            ui.StatusTextChanged("Success @ " + memoryAddress);

            t = new Thread(ScanComplete);
            t.IsBackground = true;
            t.Start();
        }

        public override bool CalcPosition()
        {
            /*
            Vector3 rawVel = (currRawPos - lastRawPos) / dt;
            float rawVelMag = rawVel.Length();
            float lastVelMag = lastWorldVelocity.Length();

            if (rawVelMag <= lastVelMag * 0.25f && (lastVelMag < rawVelMag * 10000.0f || rawVelMag <= float.Epsilon))
            {
                return false;
            }
            
             */

            //If the remote game code hasn't updated, nwait for the next tick
            if (transform == lastTransform)
            {
                return false;//vehicle hasn't moved at all
            }

            Vector3 currRawPos = new Vector3(transform.M41, transform.M42, transform.M43);

            rawData.position_x = currRawPos.X;
            rawData.position_y = currRawPos.Y;
            rawData.position_z = currRawPos.Z;

            //Position MUST have changed (transform == last will proceed if the position is the same, but the orientation has changed)
            if (lastRawPos == currRawPos)
            {
                return false;//vehicle hasn't moved(but MAY have updated it's orientiation - this is the closest we'll get to an atomic read)
            }

            lastRawPos = currRawPos;

            //filter position
            FilterModuleCustom.Instance.Filter(rawData, ref filteredData, posKeyMask, true);

            //assign
            worldPosition = new Vector3((float)filteredData.position_x, (float)filteredData.position_y, (float)filteredData.position_z);

            return true;
        }

        public override void CalcAngles()
        {
            base.CalcAngles();

            rawData.roll = -(float)rawData.roll;
            if (ui.InvertYaw) //on by default
            {
                rawData.yaw = -(float)rawData.yaw;
                //Do these also need to be flipped? (or are they overwritten)
                rawData.yaw_acceleration = -(float)rawData.yaw_acceleration;
                rawData.yaw_velocity = -(float)rawData.yaw_velocity;
            }
            //allow only g-forces to affect motion rig
            if (ui.ZeroiseTilt)
            {
                rawData.pitch = 0f;
                rawData.yaw = 0f;
                rawData.roll = 0f;

                rawData.pitch_acceleration = 0f;
                rawData.yaw_acceleration = 0f;
                rawData.roll_acceleration = 0f;

                rawData.pitch_velocity = 0f;
                rawData.yaw_velocity = 0f;
                rawData.roll_velocity = 0f;
            }
        }

        public override void CalcVelocity()
        {
            base.CalcVelocity();//writes to Raw
            //reinvert the x velocity (it's inverted in the base)
            rawData.local_velocity_x = -(float)rawData.local_velocity_x;

            rawData.local_velocity_x = (float)rawData.local_velocity_x * ui.VelocityScaleFactor;
            rawData.local_velocity_y = (float)rawData.local_velocity_y * ui.VelocityScaleFactor;
            rawData.local_velocity_z = (float)rawData.local_velocity_z * ui.VelocityScaleFactor;
        }

        public override void SimulateEngine()
        {
            base.SimulateEngine();


        }

        //So. Either we have a bug, or WreckFest is giving us spurious horror
        //I think it's a bug - just had our speed at 4, but our long g force at a huge negative number
        public float Last_gforce_lateral;
        public float Last_gforce_longitudinal;
        public float Last_gforce_vertical;
        public override void CalcAcceleration()
        {
            base.CalcAcceleration();//writes to Filtered

            filteredData.gforce_lateral = (float)filteredData.gforce_lateral * ui.GForceScaleFactor;
            filteredData.gforce_longitudinal = (float)rawData.gforce_longitudinal * ui.GForceScaleFactor;
            filteredData.gforce_vertical = (float)filteredData.gforce_vertical * ui.GForceScaleFactor;

            //Logging
            //string newFileName = "Speed.csv";
            //File.AppendAllText(newFileName, ((float)(filteredData.speed)).ToString("F2") +"," + (float)filteredData.gforce_longitudinal + "\n");

            #region Detect_and_clip_gforce_errors
            if (Last_gforce_lateral == 0)
            {

            }
            else
            {
                //thus bus is still present without a particualr solution, but this throws away any absolutely ridiculuous g-force  changes
                //if our g-force change is too great, then simply use the previous value (is this a bug in wreckfest or how we read or... what?)
                if (Math.Abs(Last_gforce_lateral - (float)filteredData.gforce_lateral) > ui.GForceMaxClip)
                { 
                    filteredData.gforce_lateral = Last_gforce_lateral;
//                    ui.StatusTextChanged("Clip!");
                }
                if (Math.Abs(Last_gforce_longitudinal - (float)filteredData.gforce_longitudinal) > ui.GForceMaxClip)
                {
                   // ui.StatusTextChanged("Clip! " + Math.Abs(Last_gforce_longitudinal - (float)filteredData.gforce_longitudinal));

                    filteredData.gforce_longitudinal = Last_gforce_longitudinal;
                }
                if (Math.Abs(Last_gforce_vertical - (float)filteredData.gforce_vertical) > ui.GForceMaxClip)
                {
                    filteredData.gforce_vertical = Last_gforce_vertical;
                 //   ui.StatusTextChanged("Clip!");
                }
            }
            #endregion

            Last_gforce_lateral         = (float)filteredData.gforce_lateral;
            Last_gforce_longitudinal    = (float)filteredData.gforce_longitudinal;
            Last_gforce_vertical        = (float)filteredData.gforce_vertical;

        

        }

        public override void StopAllThreads()
        {
            Console.WriteLine("Stopping all WF threads...");
            base.StopAllThreads();


            if (t != null)
            {
                Console.WriteLine("Joining " + t.Name);
                t.Join();
            }
            Console.WriteLine("Exiting WF TP now");

        }


    }

}
