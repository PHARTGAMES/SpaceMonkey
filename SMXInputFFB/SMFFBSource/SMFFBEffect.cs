using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;

namespace SMFFBSource
{
    public class SMFFBEffectDef
    {
        public string name;
        public string className;
        public string controlClassName;
        public SMFFBEffectConfig defaultConfig;
    }

    public static class SMFFBEffectDefs
    {
        public static readonly Dictionary<string, SMFFBEffectDef> Values = new Dictionary<string, SMFFBEffectDef>
        {
            {
                "Wheel Steer Effect",
                new SMFFBEffectDef
                {
                    name = "Wheel Steer Effect",
                    className = "SMFFBSource.SMFFBWheelSteerEffect",
                    controlClassName = "GenericTelemetryProvider.SMFFBWheelSteerEffectControl",
                    defaultConfig = new SMFFBWheelSteerEffectConfig
                    {
                        id = "SMFFBSource.SMFFBWheelSteerEffect",
                        enabled = true
                    }
                }
            }
        };

        public static SMFFBEffectDef GetByName(string name)
        {
            return Values[name];
        }

        public static SMFFBEffectDef GetByClassName(string className)
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

    public class SMFFBEffectConfig
    {
        public string id;
        public bool enabled = false;

        public delegate void ConfigChangedHandler(SMFFBEffectConfig config, string fieldName);

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
    }

    public class SMFFBEffect
    {
        public SMFFBEffectConfig effectConfig;

        public virtual void Init(SMFFBEffectConfig config)
        {
            effectConfig = config;
            effectConfig.ConfigChanged += ConfigChanged;

        }

        public virtual void Destroy()
        {

        }

        public virtual void Update(CMCustomUDPData inputs, CMCustomUDPData outputs)
        {

        }


        public virtual void ConfigChanged(SMFFBEffectConfig config, string fieldName)
        {
            switch(fieldName)
            {
                case "enabled":
                    {
                        break;
                    }
            }

        }

    }


}
