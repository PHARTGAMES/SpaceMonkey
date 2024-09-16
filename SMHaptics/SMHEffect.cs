using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;

namespace SMHaptics
{
    public class SMHEffectDef
    {
        public string name;
        public string className;
        public string controlClassName;
        public SMHEffectConfig defaultConfig;
    }

    public static class SMHEffectDefs
    {
        public static readonly Dictionary<string, SMHEffectDef> Values = new Dictionary<string, SMHEffectDef>
        {
            {
                "Engine Effect",
                new SMHEffectDef
                {
                    name = "Engine Effect",
                    className = "SMHaptics.SMHEngineEffect",
                    controlClassName = "GenericTelemetryProvider.SMHEngineEffectControl",
                    defaultConfig = new SMHEngineEffectConfig
                    {
                        id = "SMHaptics.SMHEngineEffect",
                        enabled = true,
                        outputDeviceModuleName = "",
                        outputChannelIndex = 0,
                        waveform = 3,
                        gain = 1.0,
                        minFrequency = 10,
                        maxFrequency = 80,
                    }
                }
            }
        };

        public static SMHEffectDef GetByName(string name)
        {
            return Values[name];
        }

        public static SMHEffectDef GetByClassName(string className)
        {
            foreach (var effect in Values.Values)
            {
                if (effect.className == className)
                {
                    return effect;
                }
            }
            return null;
        }

    }

    public class SMHEffectConfig
    {
        public string id;
        public bool enabled = false;
        public string outputDeviceModuleName;
        public int outputChannelIndex = 0;
        public int waveform = 3; //sine
        public double gain;

        public delegate void ConfigChangedHandler(SMHEffectConfig config, string fieldName);

        public event ConfigChangedHandler ConfigChanged;

        protected void OnConfigChanged(string fieldName)
        {
            ConfigChanged?.Invoke(this, fieldName);
        }

        [JsonIgnore]
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    OnConfigChanged("enabled");
                }
            }
        }


        [JsonIgnore]
        public string OutputDeviceModuleName
        {
            get => outputDeviceModuleName;
            set
            {
                if (outputDeviceModuleName != value)
                {
                    outputDeviceModuleName = value;
                    OnConfigChanged("outputDeviceModuleName");
                }
            }
        }

        [JsonIgnore]
        public int OutputChannelIndex
        {
            get => outputChannelIndex;
            set
            {
                if (outputChannelIndex != value)
                {
                    outputChannelIndex = value;
                    OnConfigChanged("outputChannelIndex");
                }
            }
        }

        [JsonIgnore]
        public int Waveform
        {
            get => waveform;
            set
            {
                if (waveform != value)
                {
                    waveform = value;
                    OnConfigChanged("waveform");
                }
            }
        }

        [JsonIgnore]
        public double Gain
        {
            get => gain;
            set
            {
                if (gain != value)
                {
                    gain = value;
                    OnConfigChanged("gain");
                }
            }
        }


    }

    public class SMHEffect
    {
        public SMHEffectConfig effectConfig;
        public SMHOutputDevice outputDevice = null;

        public virtual void Init(SMHEffectConfig config)
        {
            effectConfig = config;
            effectConfig.ConfigChanged += ConfigChanged;

            outputDevice = SMHOutputManager.instance.GetDeviceByModuleName(effectConfig.outputDeviceModuleName);
            if (outputDevice != null)
            {
                outputDevice.Enable(effectConfig.enabled);
            }

        }

        public virtual void Destroy()
        {
            if (outputDevice != null)
                outputDevice.Enable(false);
            outputDevice = null;
        }

        public virtual void Update(CMCustomUDPData inputs)
        {
            if (outputDevice == null)
            {
                outputDevice = SMHOutputManager.instance.GetDeviceByModuleName(effectConfig.outputDeviceModuleName);
            }

            if (outputDevice != null)
            {
                outputDevice.Enable(effectConfig.enabled);
            }
        }


        public virtual void ConfigChanged(SMHEffectConfig config, string fieldName)
        {
            switch(fieldName)
            {
                case "enabled":
                    {
                        if (outputDevice != null)
                        {
                            outputDevice.Enable(effectConfig.enabled);
                        }
                        break;
                    }
                case "outputDeviceModuleName":
                    {
                        if(outputDevice != null)
                        {
                            outputDevice.Stop();
                        }
                            
                        outputDevice = SMHOutputManager.instance.GetDeviceByModuleName(effectConfig.outputDeviceModuleName);
                        if (outputDevice != null)
                        {
                            outputDevice.Enable(effectConfig.enabled);
                        }
                        break;
                    }
                case "outputChannelIndex":
                    {
                        break;
                    }
                case "waveform":
                    {
                        break;
                    }
                case "gain":
                    {
                        break;
                    }
            }
        }

    }


}
