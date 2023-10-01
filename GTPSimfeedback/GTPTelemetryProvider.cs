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
using CMCustomUDP;
using Newtonsoft.Json;
using System.Globalization;
using GenericTelemetryProvider;
using Microsoft.Win32;

namespace GTPSimfeedback
{
    /// <summary>
    /// Generic Telemetry Provider
    /// </summary>
    public sealed class GTPTelemetryProvider : AbstractTelemetryProvider
    {
        private bool isStopped = true;                                  // flag to control the polling thread
        private Thread t;

        private IPEndPoint senderIP;                   // IP address of the sender for the udp connection used by the worker thread

        GTPConfig config;

        double smoothInTime = 3.0f;
        double smoothInTimer = 3.0f;
        double startWaitTime = 2.0f;
        double startWaitTimer = 2.0f;
        double dt;
        Stopwatch systemTimer;
        double lastSystemTimerSeconds = 0;


        /// <summary>
        /// Default constructor.
        /// Every TelemetryProvider needs a default constructor for dynamic loading.
        /// Make sure to call the underlying abstract class in the constructor.
        /// </summary>
        public GTPTelemetryProvider() : base()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            Author = "PEZZALUCIFER";
            Version = "v1.0";
            BannerImage = @"img\SMBanner.png"; // Image shown on top of the profiles tab
            IconImage = @"img\SMIcon.png";  // Icon used in the tree view for the profile
            TelemetryUpdateFrequency = 100;     // the update frequency in samples per second
            systemTimer = new Stopwatch();
            systemTimer.Start();
            lastSystemTimerSeconds = systemTimer.Elapsed.TotalSeconds;
            

            AppDomain currentDomain = AppDomain.CurrentDomain;

            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                try
                {
                    string assemblyName = args.Name.Split(',')[0];

                    string installPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\PHARTGAMES\\SpaceMonkeyTP", "install_path", null);
                    if (string.IsNullOrEmpty(installPath))
                    {
                        throw new Exception("SpaceMonkey Not Installed");
                    }
                    else
                    {
                        Assembly ass = Assembly.LoadFrom(installPath + assemblyName + ".dll");

                        return ass;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to load assembly: " + e.Message);
                    return null;
                }
            };

            LoadConfig("CMCustomUDP/SMConfig.txt");


        }

        /// <summary>
        /// Name of this TelemetryProvider.
        /// Used for dynamic loading and linking to the profile configuration.
        /// </summary>
        public override string Name { get { return "spacemonkeysfx"; } }

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
            return GetValueListByReflection(typeof(CMCustomUDPData));
        }

        /// <summary>
        /// Start the polling thread
        /// </summary>
        public override void Start()
        {
            

            if (isStopped)
            {
                LogDebug("Starting GTPTelemetryProvider");

                if (config.integrated)
                {
                    SMClient.Init((success) =>
                    {
                        SMClient.RegisterTelemetryCallback(TelemetryCallback);

                        IsConnected = true;
                        IsRunning = true;
                    });
                }


                if(config.receiveUDP || config.receiveMMF)
                {
                    t = new Thread(Run);
                    t.Start();
                }
            }
        }

        /// <summary>
        /// Stop the polling thread
        /// </summary>
        public override void Stop()
        {
            LogDebug("Stopping GTPTelemetryProvider");
            isStopped = true;

            if (t != null) 
                t.Join();
        }

        void LoadConfig(string filename)
        {
            config = new GTPConfig();

            if (!File.Exists(filename))
                return;

            config = JsonConvert.DeserializeObject<GTPConfig>(File.ReadAllText(filename));
        }

        public void TelemetryCallback(CMCustomUDPData telemetryData)
        {
            double systemTimerSeconds = systemTimer.Elapsed.TotalSeconds;
            dt = systemTimerSeconds - lastSystemTimerSeconds;
            lastSystemTimerSeconds = systemTimerSeconds;

            CMCustomUDPData telemetryToSend = new CMCustomUDPData();
            telemetryToSend.Copy(telemetryData);

            ProcessTelemetryExceptions(telemetryToSend);

            TelemetryEventArgs args = new TelemetryEventArgs(new GTPTelemetryInfo(telemetryToSend));
            RaiseEvent(OnTelemetryUpdate, args);
        }

        public void ProcessTelemetryExceptions(CMCustomUDPData telemetryToSend)
        {

            //wait for start because there is a delay between when telem starts sending and platform is updated
            if (startWaitTimer > 0.0f)
            {
                startWaitTimer -= dt;
                telemetryToSend.LerpAll(0.0f);
                //done, so smooth in
                if (startWaitTimer <= 0.0f)
                {
                    smoothInTimer = smoothInTime;
                }
            }
            else
            //smooth start transition
            if (smoothInTimer > 0.0f)
            {
                double lerp = 1.0 - (smoothInTimer / smoothInTime);
                smoothInTimer -= dt;
                telemetryToSend.LerpAll((float)lerp);
            }

        }

        /// <summary>
        /// The thread funktion to poll the telemetry data and send TelemetryUpdated events.
        /// </summary>
        private void Run()
        {
            
            isStopped = false;

            smoothInTimer = 0.0f;
            startWaitTimer = startWaitTime;

            CMCustomUDPData telemetryData = new CMCustomUDPData();
            telemetryData.Init("CMCustomUDP/CMCustomUDPFormat.xml");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Stopwatch processSW = new Stopwatch();

            int readSize = telemetryData.GetSize();
            byte[] readBuffer;

            UdpClient socket = null;
            if (config.receiveUDP)
            {
                socket = new UdpClient();
                socket.ExclusiveAddressUse = false;
                socket.Client.Bind(new IPEndPoint(IPAddress.Any, config.udpPort));
            }

            MemoryMappedFile mmf = null;

            while (!isStopped)
            {
                try
                {
                    dt = (float)sw.ElapsedMilliseconds / 1000.0f;

                    if (config.receiveUDP)
                    {
                        while (true && !isStopped)
                        {
                            if (socket.Available == 0)
                            {
                                if (sw.ElapsedMilliseconds > 500)
                                {
                                    Thread.Sleep(1000);
                                }

                                continue;
                            }
                            else
                                break;
                        }
                    }
                    else
                    {
                        while (true && !isStopped)
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
                    }

                    //check here if stopped so we can exit before a read
                    if (isStopped)
                    {
                        break;
                    }

                    if (config.receiveUDP)
                    {
                        readBuffer = socket.Receive(ref senderIP);
                    }
                    else
                    {
                        Mutex mutex = Mutex.OpenExisting("GenericTelemetryProviderMutex");
                        mutex.WaitOne();
                        using (MemoryMappedViewStream stream = mmf.CreateViewStream())
                        {
                            BinaryReader reader = new BinaryReader(stream);

                            readBuffer = reader.ReadBytes(readSize);
                        }
                        mutex.ReleaseMutex();
                    }

                    telemetryData.FromBytes(readBuffer);

                    CMCustomUDPData telemetryToSend = new CMCustomUDPData();
                    telemetryToSend.Copy(telemetryData);

                    ProcessTelemetryExceptions(telemetryToSend);
                    
                    IsConnected = true;

                    if(IsConnected)
                    { 
                        IsRunning = true;

                        sw.Restart();

                        TelemetryEventArgs args = new TelemetryEventArgs(
                            new GTPTelemetryInfo(telemetryToSend));
                        RaiseEvent(OnTelemetryUpdate, args);

                        bool sleep = true;
                        if(config.receiveUDP)
                        {
                            if (socket.Available != 0)
                            {
                                sleep = false;
                            }

                        }

                        if (sleep)
                        {
                            using (var sleeper = new ManualResetEvent(false))
                            {
                                int processTime = (int)processSW.ElapsedMilliseconds;
                                sleeper.WaitOne(Math.Max(0, 10 - processTime));
                            }
                            processSW.Restart();
                        }
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

            if(socket != null)
                socket.Close();
            if(mmf != null)
                mmf.Dispose();

            IsConnected = false;
            IsRunning = false;

        }

    }

    public class GTPConfig
    {
        public int udpPort = 6969;
        public bool receiveUDP = false;
        public bool receiveMMF = false;
        public bool integrated = true;
    }

}
