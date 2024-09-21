using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;

namespace GenericTelemetryProvider
{
    public class TelemetryOutputUDP : TelemetryOutput
    {
        TelemetrySender telemetrySender = new TelemetrySender();
        OutputConfigTypeDataUDP typedConfig;

        public override void Init(OutputConfigTypeData _outputConfig)
        {
            base.Init(_outputConfig);

            typedConfig = outputConfig as OutputConfigTypeDataUDP;
        }

        public override void StartSending()
        {
            base.StartSending();

            telemetrySender.StartSending(typedConfig.udpIP, typedConfig.udpPort);
        }

        public override void StopSending()
        {
            base.StopSending();

            telemetrySender.StopSending();
        }

        public override void SendData(CMCustomUDPData _data, float _dt)
        {
            base.SendData(_data, _dt);

            byte[] bytes = data.GetBytes();

            telemetrySender.SendAsync(bytes);
        }

        public override OutputConfigTypeData GetConfigTypeData()
        {
            return outputConfig;
        }
    }
}
