using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NAudio.CoreAudioApi;

namespace SMHaptics
{

    public class SMHOutputManager
    {
        private static SMHOutputManager _instance = null;
        public static SMHOutputManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SMHOutputManager();
                }
                return _instance;
            }

        }

        public List<SMHOutputDevice> outputDevices = new List<SMHOutputDevice>();

        public void EnumerateDevices()
        {
            outputDevices.Clear();

            var enumerator = new MMDeviceEnumerator();
            var wasapiDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (DirectSoundDeviceInfo deviceInfo in DirectSoundOut.Devices)
            {
                //find channel count and init
                foreach (var wasapiDevice in wasapiDevices)
                {
                    if (wasapiDevice.ID == deviceInfo.ModuleName)
                    {
                        SMHOutputDevice newDevice = new SMHOutputDevice();

                        newDevice.channelCount = wasapiDevice.AudioClient.MixFormat.Channels;

                        newDevice.deviceInfo = deviceInfo;

                        outputDevices.Add(newDevice);
                        break;
                    }
                }

            }
        }

        public SMHOutputDevice GetDeviceByModuleName(string moduleName)
        {
            return outputDevices.Find(x => x.deviceInfo.ModuleName.Equals(moduleName));
        }

        public List<string> GetDeviceNames()
        {
            List<string> deviceNames = new List<string>();

            foreach (SMHOutputDevice device in SMHOutputManager.instance.outputDevices)
            {
                deviceNames.Add(device.deviceInfo.Description);
            }

            return deviceNames;
        }

        public List<string> GetDeviceModuleNames()
        {
            List<string> deviceNames = new List<string>();

            foreach (SMHOutputDevice device in SMHOutputManager.instance.outputDevices)
            {
                deviceNames.Add(device.deviceInfo.ModuleName);
            }

            return deviceNames;
        }

    }

}
