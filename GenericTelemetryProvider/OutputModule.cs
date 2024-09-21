using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using CMCustomUDP;

namespace GenericTelemetryProvider
{
    class OutputModule
    {
        public enum OutputType
        {
            MMF,
            UDP,
            Callback,
            Max
        }

        public enum State
        {
            Stopped,
            Sending
        }

        State state = State.Stopped;

        public static OutputModule Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new OutputModule();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        static OutputModule instance;

        string configFilename;

        OutputConfigData configData;

        public List<TelemetryOutput> telemetryOutputs = new List<TelemetryOutput>();

        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            configData = JsonConvert.DeserializeObject<OutputConfigData>(File.ReadAllText(MainConfig.installPath + configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            for(int i = telemetryOutputs.Count-1; i >= 0; --i)
            {
                if(!(telemetryOutputs[i] is TelemetryOutputCallback))
                {
                    telemetryOutputs.RemoveAt(i);
                }
            }

            foreach (OutputConfigTypeData outConfig in configData.outputConfigTypes)
            {
                if (outConfig is OutputConfigTypeDataUDP)
                {
                    TelemetryOutput newOutput = new TelemetryOutputUDP();
                    newOutput.Init(outConfig);

                    telemetryOutputs.Add(newOutput);
                }

                if (outConfig is OutputConfigTypeDataMMF)
                {
                    TelemetryOutput newOutput = new TelemetryOutputMMF();
                    newOutput.Init(outConfig);

                    telemetryOutputs.Add(newOutput);
                }
            }
        }


        public void StartSending()
        {
            foreach(TelemetryOutput output in telemetryOutputs)
            {
                output.StartSending();
            }

            state = State.Sending;
        }

        public void StopSending()
        {
            foreach (TelemetryOutput output in telemetryOutputs)
            {
                output.StopSending();
            }

            state = State.Stopped;
        }

        public void SendData(CMCustomUDPData data, float dt)
        {
            foreach (TelemetryOutput output in telemetryOutputs)
            {
                output.SendData(data, dt);
            }
        }

        public void SaveConfig()
        {
            configData = new OutputConfigData();

            foreach (TelemetryOutput output in telemetryOutputs)
            {
                //callback type is transient
                if (output is TelemetryOutputCallback)
                    continue;

                configData.outputConfigTypes.Add(output.GetConfigTypeData());
            }

            string outputString = JsonConvert.SerializeObject(configData, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });


            File.WriteAllText(MainConfig.installPath + configFilename, outputString);
        }

        public TelemetryOutput AddOutput(OutputType outputType, bool updateUI)
        {
            TelemetryOutput newOutput = null;
            switch (outputType)
            {
                case OutputType.MMF:
                    {
                        newOutput = new TelemetryOutputMMF();
                        OutputConfigTypeDataMMF newDataType = new OutputConfigTypeDataMMF();

                        newOutput.Init(newDataType);

                        telemetryOutputs.Add(newOutput);

                        break;
                    }
                case OutputType.UDP:
                    {
                        newOutput = new TelemetryOutputUDP();
                        OutputConfigTypeDataUDP newDataType = new OutputConfigTypeDataUDP();

                        newOutput.Init(newDataType);

                        telemetryOutputs.Add(newOutput);

                        break;
                    }
                case OutputType.Callback:
                    {
                        //only one callback output allowed.. for now :)
                        foreach(TelemetryOutput output in telemetryOutputs)
                        {
                            if (output is TelemetryOutputCallback)
                            {
                                newOutput = output;
                                break;
                            }
                        }

                        //already have a callback output
                        if (newOutput != null)
                            break;

                        newOutput = new TelemetryOutputCallback();
                        OutputConfigTypeDataCallback newDataType = new OutputConfigTypeDataCallback();

                        newOutput.Init(newDataType);

                        telemetryOutputs.Add(newOutput);

                        break;
                    }
            }

            if (updateUI && OutputUI.Instance != null)
            {
                OutputUI.Instance.RefreshUI();
            }

            return newOutput;
        }

        public void DeleteOutput(TelemetryOutput output)
        {
            output.StopSending();
            telemetryOutputs.Remove(output);
        }
    }

    [System.Serializable]
    public class OutputConfigData
    {
        public List<OutputConfigTypeData> outputConfigTypes = new List<OutputConfigTypeData>();

    }

    [System.Serializable]
    public class OutputConfigTypeData
    {
        public string packetFormat = "PacketFormats\\defaultPacketFormat.xml";

        public List<string> copyFormatDestinations;

        public void AddFormatDestination(string text)
        {
            if (copyFormatDestinations == null)
                copyFormatDestinations = new List<string>();

            copyFormatDestinations.Add(text);
        }

        public void RemoveFormatDestination(string text)
        {
            if (copyFormatDestinations == null)
                return;

            for (int i = 0; i < copyFormatDestinations.Count; ++i)
            {
                if (string.Compare(copyFormatDestinations[i], text) == 0)
                {
                    copyFormatDestinations.RemoveAt(i);
                    break;
                }
            }
        }

        public void CopyFileToDestinations()
        {
            if (copyFormatDestinations == null)
                return;

            if (!File.Exists(MainConfig.installPath + packetFormat))
            {
                Console.WriteLine(MainConfig.installPath + packetFormat + " does not exist");
                return;
            }

            foreach (string dest in copyFormatDestinations)
            {
                try
                {
                    File.Copy(MainConfig.installPath + packetFormat, dest, true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

    }

    [System.Serializable]
    public class OutputConfigTypeDataUDP : OutputConfigTypeData
    {
        public string udpIP = "127.0.0.1";
        public int udpPort = 10001;
    }

    [System.Serializable]
    public class OutputConfigTypeDataMMF : OutputConfigTypeData
    {
        public string mmfName = "GenericTelemetryProviderFiltered";
        public string mmfMutexName = "GenericTelemetryProviderMutex";
    }

    [System.Serializable]
    public class OutputConfigTypeDataCallback : OutputConfigTypeData
    {
    }
}
