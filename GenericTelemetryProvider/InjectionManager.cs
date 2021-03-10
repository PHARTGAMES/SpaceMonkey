using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMonoInjector;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

namespace GenericTelemetryProvider
{
    public static class InjectionManager
    {
        public enum State
        {
            WaitingForProcess,
            GameProcessFound,
            Waiting,
            Success,
            Failed
        }

        static string status = "waiting";
        static State statusState = State.WaitingForProcess;
        static Mutex statusMutex = new Mutex(false);
        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)uint.MaxValue, (UIntPtr)uint.MaxValue);
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process, UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);

        public static void Monitor(string processName, byte[] dllContent, AutoResetEvent injectionEvent)
        {
            IntPtr assembly = IntPtr.Zero;
            string lastPid = null;
            string pidPath = processName + "enabler.lastppid";
            string pidfile = Path.Combine(Path.GetTempPath(), pidPath);
            InjectionManager.minimizeMemory();
            if (File.Exists(pidfile))
            {
                lastPid = File.ReadAllText(pidfile);
            }
            for (; ; )
            {
                try
                {
                    InjectionManager.minimizeMemory();
                    Thread.Sleep(1000);
                    Process process = Process.GetProcessesByName(processName).FirstOrDefault<Process>();
                    if (process != null)
                    {
                        InjectionManager.SetStatus("Game process found.", State.GameProcessFound);
                        injectionEvent.Set();

                        if (process.Id.ToString() == lastPid)
                        {
                            InjectionManager.SetStatus("Telemetry plugin already running in the current game.", State.Success);
                            injectionEvent.Set();
                        }
                        else
                        {
                            InjectionManager.SetStatus("Waiting 5s before plugin injection.", State.Waiting);
                            injectionEvent.Set();
                            Thread.Sleep(5000);
                            Injector injector;
                            Injector enabler = injector = new Injector(process.Id);
                            try
                            {
                                assembly = IntPtr.Zero;
                                try
                                {
                                    assembly = enabler.Inject(dllContent, "MonsterGamesTelemetry", "Loader", "Init");
                                    InjectionManager.minimizeMemory();
                                }
                                catch (InjectorException ie)
                                {
                                    string str = "Failed to add assembly: ";
                                    InjectorException ex = ie;
                                    InjectionManager.SetStatus(str + ((ex != null) ? ex.ToString() : null), State.Failed);
                                    injectionEvent.Set();
                                }
                                catch (Exception exc)
                                {
                                    string str2 = "Failed to add assembly (unknown error): ";
                                    Exception ex2 = exc;
                                    InjectionManager.SetStatus(str2 + ((ex2 != null) ? ex2.ToString() : null), State.Failed);
                                    injectionEvent.Set();
                                }
                                if (assembly == IntPtr.Zero)
                                {
                                    break;
                                }
                                InjectionManager.SetStatus("Telemetry plugin successfully injected", State.Success);
                                File.WriteAllText(pidfile, process.Id.ToString());
                                injectionEvent.Set();
                                while (!process.HasExited)
                                {
                                    Thread.Sleep(1);
                                }
                                try
                                {
                                    if (!process.HasExited)
                                    {
                                        enabler.Eject(assembly, "TelemetryExporter", "Loader", "Unload");
                                    }
                                    File.Delete(pidfile);
                                }
                                catch
                                {
                                }
                            }
                            finally
                            {
                                if (injector != null)
                                {
                                    ((IDisposable)injector).Dispose();
                                }
                            }
                        }
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                catch
                {
                    continue;
                }
                break;
            }
        }

        private static void SetStatus(string v1, State v2)
        {
            statusMutex.WaitOne();
            status = v1 + " State: " + v2.ToString();
            statusState = v2;
            statusMutex.ReleaseMutex();
        }

        public static string GetStatus()
        {
            statusMutex.WaitOne();
            string temp = status;
            statusMutex.ReleaseMutex();

            return temp;
        }

        public static State GetState()
        {
            statusMutex.WaitOne();
            State temp = statusState;
            statusMutex.ReleaseMutex();

            return temp;
        }
    }
}
