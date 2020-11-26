//MIT License
//
//Copyright(c) 2019 PHARTGAMES
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
using SimFeedback.log;
using SimFeedback.telemetry;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Numerics;
using System.IO;
using System.IO.MemoryMappedFiles;
using GenericTelemetryProvider;

namespace GTPSimfeedback
{
    /// <summary>
    /// Generic Telemetry Provider
    /// </summary>
    public sealed class GTPTelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;

        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public GTPTelemetryProvider() : base()
        {
            Author = "PEZZALUCIFER";
            Version = "v1.0";
            BannerImage = @"img\banner_DIRT5.png"; // Image shown on top of the profiles tab
            IconImage = @"img\DIRT5.jpg";  // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "gtpsfx"; } }

        public override void Init(ILogger logger)
        {
            base.Init(logger);
            Log("Initializing GTPTelemetryProvider");
        }

        /// <summary>
        /// A list of all telemetry names of this provider.
        /// </summary>
        /// <returns>List of all telemetry names</returns>
        public override string[] GetValueList()
        {
            return GetValueListByReflection(typeof(GenericProviderData));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            if (isStopped)
            {
                LogDebug("Starting GTPTelemetryProvider");

                t = new Thread(Run);
                t.Start();
            }
        }


        /// <summary>
        /// Stop the polling thread
        /// </summary>
        public override void Stop()
        {
            LogDebug("Stopping GTPTelemetryProvider");
            isStopped = true;


            if (t != null) t.Join();
        }


        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            
            isStopped = false;

            GenericProviderData telemetryData = new GenericProviderData();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int readSize = telemetryData.GetSize();
            byte[] readBuffer;

            MemoryMappedFile mmf = null;

            while (!isStopped)
            {
                try
                {
                    float dt = (float)sw.ElapsedMilliseconds / 1000.0f;

                    while (true)
                    {
                        try
                        {
                            mmf = MemoryMappedFile.OpenExisting("GenericTelemetryProviderFiltered");

                            if (mmf != null)
                                break;
                            else
                                Thread.Sleep(1000);
                        }
                        catch (FileNotFoundException)
                        {
                            Thread.Sleep(1000);
                        }
                    }

                    Mutex mutex = Mutex.OpenExisting("GenericTelemetryProviderMutex");
                    mutex.WaitOne();
                    using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                    {
                        BinaryReader reader = new BinaryReader(stream);

                        readBuffer = reader.ReadBytes(readSize);
                    }
                    mutex.ReleaseMutex();

                    GenericProviderData telemetryToSend = GenericProviderData.FromByteArray(readBuffer);

                    // otherwise we are connected
                    IsConnected = true;

                    if(IsConnected)
                    { 
                        IsRunning = true;

                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new GTPTelemetryInfo(telemetryToSend));
                        RaiseEvent(OnTelemetryUpdate, args);

                        Thread.Sleep(1000 / 100);
                    }
                    else if (sw.ElapsedMilliseconds > 500)
                    {
                        IsRunning = false;
                    }
                }
                catch (Exception e)
                {
                    LogError("GTPTelemetryProvider Exception while processing data", e);
                    mmf.Dispose();
                    IsConnected = false;
                    IsRunning = false;
                    Thread.Sleep(1000);
                }

            }

            mmf.Dispose();
            IsConnected = false;
            IsRunning = false;

        }


      

    }


}
