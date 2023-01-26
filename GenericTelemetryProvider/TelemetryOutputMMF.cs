using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMCustomUDP;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.IO;

namespace GenericTelemetryProvider
{
    public class TelemetryOutputMMF : TelemetryOutput
    {
        protected Mutex mutex;
        protected MemoryMappedFile outputMMF;
        protected OutputConfigTypeDataMMF typedConfig;


        public override void Init(OutputConfigTypeData _outputConfig)
        {
            base.Init(_outputConfig);

            typedConfig = outputConfig as OutputConfigTypeDataMMF;

            mutex = new Mutex(false, typedConfig.mmfMutexName);
        }


        public override void StartSending()
        {
            base.StartSending();

            outputMMF = MemoryMappedFile.CreateOrOpen(typedConfig.mmfName, 10000);
        }

        public override void StopSending()
        {
            base.StopSending();

            if (outputMMF != null)
                outputMMF.Dispose();
            outputMMF = null;
        }

        public override void SendData(CMCustomUDPData _data)
        {
            base.SendData(_data);

            byte[] bytes = data.GetBytes();

            mutex.WaitOne();

            using (MemoryMappedViewStream stream = outputMMF.CreateViewStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(bytes);
            }

            mutex.ReleaseMutex();
        }

        public override OutputConfigTypeData GetConfigTypeData()
        {
            return outputConfig;
        }
    }
}
