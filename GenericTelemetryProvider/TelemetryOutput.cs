using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;

namespace GenericTelemetryProvider
{
    public class TelemetryOutput
    {
        protected OutputConfigTypeData outputConfig;
        protected CMCustomUDPData data = new CMCustomUDPData();

        public virtual void Init(OutputConfigTypeData _outputConfig)
        {
            outputConfig = _outputConfig;
            data.Init(MainConfig.installPath + _outputConfig.packetFormat);
        }

        public virtual void StartSending()
        {
            outputConfig.CopyFileToDestinations();
        }

        public virtual void StopSending()
        {

        }

        public virtual void SendData(CMCustomUDPData _data)
        {
            data.Copy(_data, false);
        }

        public virtual OutputConfigTypeData GetConfigTypeData()
        {
            return null;
        }
    }
}
