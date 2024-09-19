using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;
using System.IO;

namespace SMGSeat
{


    class SMGSeatControllerConfig
    {
        public bool enable;
        public SMGOutputDeviceConfig outputDevice;
        public float maxGForce;
        public float swayMultiplier = 1.0f;
        public float surgeMultiplier = 1.0f;

    }

    class SMGSeatController
    {
        private static SMGSeatController instance = null;
        public static SMGSeatController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SMGSeatController();
                }
                return instance;
            }

        }

        SMGOutputDevice outputDevice = new SMGOutputDevice();
        string installPath;
        string configFilename;
        SMGSeatControllerConfig configData;
        List<float> positions = new List<float>();

        public enum SeatRegion
        {
            BaseSwayLeft,
            BaseSwayRight,
            BackSwayLeft,
            BackSwayRight,
            BaseSurgeFront,
            BackSurgeRear
        }


        public virtual void Init(string _installPath)
        {
            installPath = _installPath;
        }

        public virtual void Input(CMCustomUDPData telemetryData)
        {
            if(outputDevice == null)
                return;

            if (!configData.enable)
                return;

            float sway = Math.Min(1.0f, Math.Max(-1.0f, ((float)telemetryData.gforce_lateral * configData.swayMultiplier) / configData.maxGForce));
            float surge = Math.Min(1.0f, Math.Max(-1.0f, ((float)telemetryData.gforce_longitudinal * configData.surgeMultiplier) / configData.maxGForce));

            positions[(int)SeatRegion.BaseSwayLeft] = Math.Abs(Math.Max(0.0f, sway)); 

            positions[(int)SeatRegion.BaseSwayRight] = Math.Min(0.0f, sway);

            positions[(int)SeatRegion.BackSwayLeft] = Math.Abs(Math.Max(0.0f, sway));

            positions[(int)SeatRegion.BackSwayRight] = Math.Min(0.0f, sway);

            positions[(int)SeatRegion.BaseSurgeFront] = Math.Min(0.0f, surge);

            positions[(int)SeatRegion.BackSurgeRear] = Math.Abs(Math.Max(0.0f, surge));

            outputDevice.SetPositions(positions);
        }


        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            if (!File.Exists(installPath + configFilename))
                return;

            Cleanup();

            configData = JsonConvert.DeserializeObject<SMGSeatControllerConfig>(File.ReadAllText(installPath + configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if (configData != null)
            {
                outputDevice.Init(configData.outputDevice);
            }
        }


        public void SaveConfig()
        {
            string outputString = JsonConvert.SerializeObject(configData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });


            File.WriteAllText(installPath + configFilename, outputString);
        }

        void Cleanup()
        {

        }


    }
}
