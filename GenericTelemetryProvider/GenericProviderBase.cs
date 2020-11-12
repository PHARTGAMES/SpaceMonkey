using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Threading;
using System.IO;
using System.IO.MemoryMappedFiles;
namespace GenericTelemetryProvider
{
    public class GenericProviderBase
    {
        protected Mutex mutex;
        protected MemoryMappedFile mmf;

        public Action<string> statusChangedCallback;
        public Action<bool> initBtnStatusChangedCallback;
        public Action<int> progressBarChangedCallback;
        public Action<string> debugChangedCallback;

        public virtual void Run()
        {
            mutex = new Mutex(false, "GenericTelemetryProviderMutex");

            mmf = MemoryMappedFile.CreateNew("GenericTelemetryProvider", 10000);

        }

        public virtual void Stop()
        {
            if (mmf != null)
                mmf.Dispose();
            mmf = null;
        }

    }
}
