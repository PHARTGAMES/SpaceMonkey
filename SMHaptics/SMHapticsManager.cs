using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace SMHaptics
{

    public class SMHapticsConfig
    {
        public List<SMHEffectConfig> effects = new List<SMHEffectConfig>();

    }

    public class SMHapticsManager
    {
        private static SMHapticsManager _instance = null;
        public static SMHapticsManager instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SMHapticsManager();
                }
                return _instance;
            }

        }

        string configFilename;
        string installPath;
        public SMHapticsConfig configData = null;

        List<SMHEffect> effects = new List<SMHEffect>();
        
        public virtual void Init(string _installPath)
        {
            installPath = _installPath;
            SMHOutputManager.instance.EnumerateDevices();
        }

        public virtual void Input(CMCustomUDPData telemetryData)
        {
            foreach(SMHEffect effect in effects)
            {
                effect.Update(telemetryData);
            }
        }

        void AddEffect(SMHEffect effect)
        {
            effects.Add(effect);
        }

        public virtual SMHEffect CreateEffect(SMHEffectConfig effectConfig)
        {
            SMHEffect newEffect = null;

            Type type = Type.GetType(effectConfig.id);

            if (type == null)
            {
                return null;
            }

            newEffect = Activator.CreateInstance(type) as SMHEffect;

            if(newEffect != null)
            {
                newEffect.Init(effectConfig);
                AddEffect(newEffect);
            }

            return newEffect;

        }

        public virtual void AddEffectToConfig(SMHEffect effect)
        {
            if (configData == null)
                configData = new SMHapticsConfig();

            configData.effects.Add(effect.effectConfig);
        }

        public void Cleanup()
        {
            foreach(SMHEffect effect in effects)
            {
                effect.Destroy();
            }

            effects.Clear();
        }

        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            if (!File.Exists(installPath + configFilename))
                return;

            Cleanup();

            configData = JsonConvert.DeserializeObject<SMHapticsConfig>(File.ReadAllText(installPath + configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if(configData != null)
            {
                foreach(SMHEffectConfig effectConfig in configData.effects)
                {
                    CreateEffect(effectConfig);
                }
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

        public void DeleteEffectAtIndex(int index)
        {
            if (configData == null)
                return;

            configData.effects.RemoveAt(index);
            effects.RemoveAt(index);
        }
    }
}
