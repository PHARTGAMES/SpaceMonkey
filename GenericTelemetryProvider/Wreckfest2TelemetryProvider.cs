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
using System.Reflection;
using Microsoft.Win32;

namespace GenericTelemetryProvider
{
    public class WF2CarAddress
    {
        public string id;
        public Int64 baseAddress;
        public Int64 transformAddress;
    }


    public class WF2Player : StateMachineBase<WF2Player.State>
    {

        public enum State
        {
            None,
            Invalid,
            PlayerNameValid,
            ScanForLobbyPlayerId,
            ScanForIngamePlayerId,
            ScanForTransform,
            TransformFound
        }


        public string name = null;
        public string rootNodeName;
        public Int64 lobbyPlayerAddress = 0;
        public Int64 ingamePlayerAddress = 0;
        public Int64 transformAddress = 0; //address of transform
        private const string carRootNodePrefix = "carRootNode";
        public bool preferLobby = true;

        RegularMemoryScan memoryScanner = null;
        ProcessMemoryReader memoryReader = null;

        long memoryScanStart = 0;
        long memoryScanEnd = 140737488355327;

        Process wf2Process = null;

        public WF2Player() : base(State.None)
        {

        }


        public bool IsProcessAlive()
        {
            return wf2Process != null && wf2Process.HasExited;
        }

        public bool IsReadyToRead()
        {
            return activeState == State.TransformFound;
        }

        Process GetWF2Process()
        {
            if (wf2Process != null && !wf2Process.HasExited)
                return wf2Process;

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (process.ProcessName.Contains("Wreckfest"))
                {
                    wf2Process = process;
                    return process;
                }
            }

            return null;
        }

        public void TryRefreshGamerTag()
        {
            if (string.IsNullOrEmpty(name))
            {
                //get gamertag from registry for Steam
                RegistryKey localKey;
                if (Environment.Is64BitOperatingSystem)
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                else
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);

                if (localKey != null)
                {
                    string steamName = localKey.OpenSubKey("Software\\Valve\\Steam").GetValue("LastGameNameUsed").ToString();

                    if (!string.IsNullOrEmpty(steamName))
                    {
                        name = steamName;
                    }
                }
            }
        }

        // Invalid state functions
        private void State_Invalid_Enter()
        {

        }
        private void State_Invalid_Update() 
        {
            TryRefreshGamerTag();

            if (!string.IsNullOrEmpty(name))
            {
                EnterState(State.PlayerNameValid);
            }
        }

        private void State_Invalid_Exit() { }

        // PlayerNameValid state functions
        private void State_PlayerNameValid_Enter()
        {
        }
        private void State_PlayerNameValid_Update()
        {
            if(preferLobby)
            {
                EnterState(State.ScanForLobbyPlayerId);
            }
            else
            {
                EnterState(State.ScanForIngamePlayerId);
            }
        }
        private void State_PlayerNameValid_Exit() { }

        // ScanForLobbyPlayerId state functions
        private void State_ScanForLobbyPlayerId_Enter()
        {
            Process process = GetWF2Process();
            if (process == null)
            {
                EnterState(State.Invalid);
                return;
            }

            if (memoryScanner != null)
            {
                memoryScanner.CancelScan();
            }

            memoryScanner = new RegularMemoryScan(process, memoryScanStart, memoryScanEnd);
            memoryScanner.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(ScanForLobbyPlayerId_Completed);

            memoryScanner.StartScanForString(name);
        }


        private void ScanForLobbyPlayerId_Completed(object sender, ScanCompletedEventArgs e)
        {

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                //couldn't find anything, maybe we can find the ingame addresses
                EnterState(State.ScanForIngamePlayerId);
                return;
            }

            if (!ValidateMemoryReader())
            {
                EnterState(State.Invalid);
                return;
            }

            string foundNodeId = null;
            long foundAddress = 0;

            foreach (long address in e.MemoryAddresses)
            {
                string nodeId = GetLobbyNodeIdForAddress(address);

                if (!string.IsNullOrEmpty(nodeId))
                {
                    foundNodeId = nodeId;
                    foundAddress = address;
                    break;
                }
            }

            if (foundNodeId == null)
            {
                lobbyPlayerAddress = 0;
                EnterState(State.ScanForIngamePlayerId);
                return;
            }

            lobbyPlayerAddress = foundAddress;
            rootNodeName = carRootNodePrefix + foundNodeId;

            Console.WriteLine("ScanForLobbyPlayerId_Completed: foundNodeId " + foundNodeId);

            //time to scan for the transform
            EnterState(State.ScanForTransform);

        }



        private void State_ScanForLobbyPlayerId_Update() { }
        private void State_ScanForLobbyPlayerId_Exit() { }


        // ScanForTransform state functions

        private void State_ScanForTransform_Enter()
        {
            Process process = GetWF2Process();
            if (process == null)
            {
                EnterState(State.Invalid);
                return;
            }

            if (memoryScanner != null)
            {
                memoryScanner.CancelScan();
            }


            memoryScanner = new RegularMemoryScan(process, memoryScanStart, memoryScanEnd);
            memoryScanner.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(ScanForTransform_Completed);

            memoryScanner.StartScanForString(rootNodeName, 1);
        }


        private void ScanForTransform_Completed(object sender, ScanCompletedEventArgs e)
        {

            transformAddress = 0;

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                Console.WriteLine("ScanForTransform_Completed e.MemoryAddresses == null || e.MemoryAddresses.Length == 0");
                //couldn't find anything, back to invalid
                EnterState(State.Invalid);
                return;
            }

            //resolve transform address
            foreach (long address in e.MemoryAddresses)
            {
                transformAddress = address - (((4 * 4 * 4) * 2) + 8);//offset backwards from found address to start of matrix
                break;
            }

            //back to invalid 
            if (transformAddress == 0)
            {
                Console.WriteLine("ScanForTransform_Completed transformAddress == 0");
                EnterState(State.Invalid);
                return;
            }

            EnterState(State.TransformFound);
        }



        private void State_ScanForTransform_Update() { }
        private void State_ScanForTransform_Exit() { }


        // TransformFound state functions
        private void State_TransformFound_Enter() { }
        private void State_TransformFound_Update() { }
        private void State_TransformFound_Exit() { }

        // ScanForIngamePlayerId state functions
        private void State_ScanForIngamePlayerId_Enter()
        {
            Process process = GetWF2Process();
            if (process == null)
            {
                EnterState(State.Invalid);
                return;
            }

            if (memoryScanner != null)
            {
                memoryScanner.CancelScan();
            }

            memoryScanner = new RegularMemoryScan(process, memoryScanStart, memoryScanEnd);
            memoryScanner.ScanCompleted += new RegularMemoryScan.ScanCompletedEventHandler(ScanForIngamePlayerId_Completed);

            memoryScanner.StartScanForString(name);

        }

        private void ScanForIngamePlayerId_Completed(object sender, ScanCompletedEventArgs e)
        {

            if (e.MemoryAddresses == null || e.MemoryAddresses.Length == 0)
            {
                //couldn't find anything
                EnterState(State.ScanForLobbyPlayerId);
                return;
            }

            if (!ValidateMemoryReader())
            {
                EnterState(State.Invalid);
                return;
            }

            string foundNodeId = null;
            long foundAddress = 0;

            foreach (long address in e.MemoryAddresses)
            {
                string nodeId = GetIngameNodeIdForAddress(address);

                if (!string.IsNullOrEmpty(nodeId))
                {
                    foundNodeId = nodeId;
                    foundAddress = address;
                    break;
                }
            }

            if (foundNodeId == null)
            {
                ingamePlayerAddress = 0;
                EnterState(State.ScanForLobbyPlayerId);
                return;
            }

            ingamePlayerAddress = foundAddress;
            rootNodeName = carRootNodePrefix + foundNodeId;

            Console.WriteLine("ScanForIngamePlayerId_Completed: foundNodeId " + foundNodeId);

            //time to scan for the transform
            EnterState(State.ScanForTransform);

        }


        private void State_ScanForIngamePlayerId_Update() { }
        private void State_ScanForIngamePlayerId_Exit() { }


        public bool ValidateMemoryReader()
        {
            Process process = GetWF2Process();

            if (process == null)
            {
                EnterState(State.Invalid);
                return false;
            }


            if (memoryReader != null && memoryReader.ReadProcess.Id != process.Id)
            {
                memoryReader.CloseHandle();
                memoryReader.ReadProcess = process;
                memoryReader.OpenProcess();
            }

            if (memoryReader == null)
            {
                memoryReader = new ProcessMemoryReader();
                memoryReader.ReadProcess = process;
                memoryReader.OpenProcess();
            }

            return true;

        }

        public void CleanupMemoryReader()
        {
            if (memoryReader != null)
            {
                memoryReader.CloseHandle();
                memoryReader = null;
            }
        }

        public IntPtr ReadProcessMemory(long address, UInt64 bytesToRead, out Int64 bytesRead, byte[] buffer)
        {
            return (IntPtr)memoryReader.ReadProcessMemory((IntPtr)address, bytesToRead, out bytesRead, buffer);
        }

        //address is address of player name, assumed to be in lobby
        //read out 4bytes at 32 bytes before address to get string identifying player position on grid.
        string GetLobbyNodeIdForAddress(long address)
        {
            UInt64 readSize = 4;
            byte[] readBuffer = new byte[readSize];
            Int64 byteReadSize;
            memoryReader.ReadProcessMemory((IntPtr)address - 32, readSize, out byteReadSize, readBuffer);

            if (byteReadSize == 0)
                return null;

            string valueString = System.Text.Encoding.ASCII.GetString(readBuffer).TrimEnd('\0');

            return Utils.ConvertOrdinalToTwoDigit(valueString, 0, 23);
        }


        string GetIngameNodeIdForAddress(long address)
        {
            UInt64 readSize = 3;
            byte[] readBuffer = new byte[readSize];
            Int64 byteReadSize;
            memoryReader.ReadProcessMemory((IntPtr)address + 32, readSize, out byteReadSize, readBuffer);

            if (byteReadSize == 0)
                return null;

            //make sure we read car here
            if (!(readBuffer[0] == 'c' && readBuffer[1] == 'a' && readBuffer[2] == 'r'))
            {
                return null;
            }

            //try read node index here
            memoryReader.ReadProcessMemory((IntPtr)address - 8, 1, out byteReadSize, readBuffer);

            if (byteReadSize == 0)
                return null;

            int readValue = (int)readBuffer[0];

            //sanity check range
            if (readValue > 23 || readValue < 0)
                return null;

            return $"{readValue:D2}";
        }
    }


    class Wreckfest2TelemetryProvider : GenericProviderBase
    {
        WF2CarAddress selectedCarAddress = null;
        Thread t;

        public Wreckfest2UI ui;

        public List<WF2CarAddress> carAddresses = new List<WF2CarAddress>();

        public WF2Player player;

        private int maxMissedFrames = 5;



        public override void Run()
        {
            base.Run();

            ui.InitButtonStatusChanged(true);

            player = new WF2Player();
            player.OnStateEnter += OnPlayerStateEnter;

            player.TryRefreshGamerTag();

            t = new Thread(ThreadUpdate);
            t.IsBackground = true;
            t.Start();
        }

        public void OnPlayerStateEnter(WF2Player.State state)
        {
            ui.StatusTextChanged("State: " + state.ToString());

            switch (state)
            {
                case WF2Player.State.None:
                {
                    ui.scanning = false;
                    break;
                }

                case WF2Player.State.Invalid:
                {
                    ui.scanning = false;
                    break;
                }

                case WF2Player.State.PlayerNameValid:
                {
                    ui.scanning = false;
                    ui.GamerTagTextChanged(player.name);
                    break;
                }

                case WF2Player.State.ScanForLobbyPlayerId:
                {
                    ui.scanning = true;
                    break;
                }

                case WF2Player.State.ScanForIngamePlayerId:
                {
                    ui.scanning = true;
                    break;
                }

                case WF2Player.State.ScanForTransform:
                {
                    ui.scanning = true;
                    break;
                }

                case WF2Player.State.TransformFound:
                {
                    ui.scanning = false;
                    ui.ProgressBarChanged(100);
                    break;
                }
            }
        }

        void LogDifferences(UInt64 readSize, byte[] readBuffer, byte[] differenceBuffer)
        {
            return;
            string differenceLog = "";

            for (int i = 0; i < (int)readSize; ++i)
            {
                if (readBuffer[i] != differenceBuffer[i])
                {
                    if(i >= 0 && i <= 15)
                    {
                        differenceLog += "forward\n";
                    } else
                    if (i >= 16 && i <= 31)
                    {
                        differenceLog += "up\n";
                    }
                    else
                    if (i >= 32 && i <= 47)
                    {
                        differenceLog += "right\n";
                    }
                    else
                    if (i >= 48 && i <= 63)
                    {
                        differenceLog += "translation\n";
                    }
                    
                }
            }

            if (!string.IsNullOrEmpty(differenceLog))
                Console.WriteLine($"Differences: \n {differenceLog}");

        }


        void ThreadUpdate()
        {
            StartSending();
            UInt64 readSize = 4 * 4 * 4;
            byte[] readBuffer = new byte[readSize];
            byte[] lastReadBuffer = new byte[readSize];
            byte[] differenceBuffer = new byte[readSize];
            float frameRateSecs = 1.0f / 60.0f;
            //float frameRateSecs = 1.0f / 120.0f;
            Stopwatch missedFrameSW = new Stopwatch();
            Stopwatch dtSW = new Stopwatch();
            dtSW.Start();
            Stopwatch readWaitSW = new Stopwatch();
            readWaitSW.Start();
            Stopwatch differenceSW = new Stopwatch();
            differenceSW.Start();



            Stopwatch actualNotDifferentSW = new Stopwatch();
            actualNotDifferentSW.Start();

            float dt = 0.0f;
            while (!IsStopped)
            {
                try
                {
                    player.Update();

                    if(player.IsReadyToRead() && player.ValidateMemoryReader())
                    {

                        Matrix4x4 transform = Matrix4x4.Identity;
                        Int64 byteReadSize;
                        bool different = false;
                        bool validFrame = true;
                        missedFrameSW.Restart();
                        float useDT = frameRateSecs;

                        do
                        {
                            //read
                            player.ReadProcessMemory((long)player.transformAddress, readSize, out byteReadSize, readBuffer);

                            if (byteReadSize == 0)
                            {
                                validFrame = false;
                                break;
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
                            if (different)
                            {
                                actualNotDifferentSW.Restart();
//                                Console.WriteLine("-----------------New Frame------------------\n");

                                LogDifferences(readSize, readBuffer, lastReadBuffer);

                                Buffer.BlockCopy(readBuffer, 0, differenceBuffer, 0, readBuffer.Length);
                                
                                float maxNotDifferentDuration = 2.0f / 1000.0f;
                                differenceSW.Restart();
                                bool notDifferent = false;
                                float actualNotDifferentDuration = 0;
                                //wait until transform stops being different for at least notDifferentDuration
                                do
                                {
                                    notDifferent = true;
                                    //read
                                    player.ReadProcessMemory((long)player.transformAddress, readSize, out byteReadSize, readBuffer);

                                    if (byteReadSize == 0)
                                    {
                                        validFrame = false;
                                        break;
                                    }

                                    //check if different
                                    for (int i = 0; i < (int)readSize; ++i)
                                    {
                                        if (readBuffer[i] != differenceBuffer[i])
                                        {
                                            notDifferent = false;
                                            break;
                                        }
                                    }

                                    //data changed, reset timer
                                    if(!notDifferent)
                                    {
                                        LogDifferences(readSize, readBuffer, differenceBuffer);

                                        Buffer.BlockCopy(readBuffer, 0, differenceBuffer, 0, readBuffer.Length);
                                        differenceSW.Restart();
                                        notDifferent = true;
                                        actualNotDifferentDuration = 0;
                                    }
                                    else
                                    {
//                                        Console.WriteLine("Transform unchanged while waiting for unchanged");
                                        if(actualNotDifferentDuration == 0)
                                        {
                                            actualNotDifferentDuration = (float)actualNotDifferentSW.Elapsed.TotalMilliseconds;
                                        }

                                        float notDiffDurationSecs = (float)differenceSW.Elapsed.TotalSeconds;
                                        if (notDiffDurationSecs >= maxNotDifferentDuration)
                                        {
                                            notDifferent = false;
  //                                          Console.WriteLine($"Time taken to stop being different MS: {actualNotDifferentDuration}");

                                            if(notDiffDurationSecs >= (maxNotDifferentDuration * 2.0f))
                                            {
                                                Console.WriteLine($"--------------------------TOO LONG Secs: {notDiffDurationSecs}-------------------------");
                                            }

                                        }

                                    }

                                } while (notDifferent);

                                dt = (float)dtSW.Elapsed.TotalSeconds;
                                dtSW.Restart();

                            }
                            else // check if memory is unchanged for maxMissedFrames, if it is we need to go to invalid state
                            {
                                if(((float)missedFrameSW.Elapsed.TotalSeconds / frameRateSecs) > maxMissedFrames && !player.IsProcessAlive())
                                {
                                    byteReadSize = 0;
                                    validFrame = false;
                                    break;
                                }
                            }

                        } while (!different);

                        if (validFrame)
                        {
                            //read transform
                            player.ReadProcessMemory((long)player.transformAddress, readSize, out byteReadSize, readBuffer);
                        }

                        if (byteReadSize == 0)
                        {
                            player.EnterState(WF2Player.State.Invalid);
                            Console.WriteLine("Unable to read process memory, process terminated!?");
                            continue;
                        }

                        Buffer.BlockCopy(readBuffer, 0, lastReadBuffer, 0, readBuffer.Length);

                        float[] floats = new float[4 * 4];

                        Buffer.BlockCopy(readBuffer, 0, floats, 0, readBuffer.Length);

                        Matrix4x4 newTransform = new Matrix4x4(floats[0], floats[1], floats[2], floats[3]
                                    , floats[4], floats[5], floats[6], floats[7]
                                    , floats[8], floats[9], floats[10], floats[11]
                                    , floats[12], floats[13], floats[14], floats[15]);



                        ProcessTransform(newTransform, useDT);

                        if(dt > useDT * 1.6f)
                        {
//                            Console.WriteLine("-----------long frame, laaame----------");
                        }

//                        Console.WriteLine($"dt: {dt}");
                        //Console.WriteLine($"useDT: {useDT}");
                    }
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }
            }

            player.CleanupMemoryReader();

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

        public void Initialize(bool preferLobby)
        {
            player.preferLobby = preferLobby;
            player.EnterState(WF2Player.State.Invalid);
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

        public void GamerTagChanged(string gamerTag)
        {
            player.name = gamerTag;
            if(string.IsNullOrEmpty(gamerTag))
            {
                player.EnterState(WF2Player.State.Invalid);
            }
        }

    }

}
