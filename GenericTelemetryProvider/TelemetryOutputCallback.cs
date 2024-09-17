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
    public class TelemetryOutputCallback : TelemetryOutput
    {
        protected OutputConfigTypeDataCallback typedConfig;
        protected Action<CMCustomUDPData> callback;


        public override void Init(OutputConfigTypeData _outputConfig)
        {
            base.Init(_outputConfig);

            typedConfig = outputConfig as OutputConfigTypeDataCallback;
        }


        public override void StartSending()
        {
            base.StartSending();

        }

        public override void StopSending()
        {
            base.StopSending();

        }

        public override void SendData(CMCustomUDPData _data)
        {
            base.SendData(_data);

            callback?.Invoke(_data);
        }

        public override OutputConfigTypeData GetConfigTypeData()
        {
            return outputConfig;
        }

        public void RegisterCallback(Action<CMCustomUDPData> cb)
        {
            callback += cb;
        }
    }
}
