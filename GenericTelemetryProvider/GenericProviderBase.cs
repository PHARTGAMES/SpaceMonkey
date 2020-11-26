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
        protected MemoryMappedFile filteredMMF;
        protected MemoryMappedFile rawMMF;

        public virtual void Run()
        {
            mutex = new Mutex(false, "GenericTelemetryProviderMutex");

            filteredMMF = MemoryMappedFile.CreateNew("GenericTelemetryProviderFiltered", 10000);
            rawMMF = MemoryMappedFile.CreateNew("GenericTelemetryProviderRaw", 10000);

        }

        public virtual void Stop()
        {
            if (rawMMF != null)
                rawMMF.Dispose();
            rawMMF = null;

            if (filteredMMF != null)
                filteredMMF.Dispose();
            filteredMMF = null;
        }

    }
}
