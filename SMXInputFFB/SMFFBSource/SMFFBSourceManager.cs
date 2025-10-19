using CMCustomUDP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SMFFBSource
{

    public class SMFFBSourceManagerConfig
    {
        public List<SMFFBEffectConfig> effects = new List<SMFFBEffectConfig>();

    }

    public class SMFFBSourceManager
    {

        const string MMFName = "SMT_FFB_FRAME";
        const string MMFMutexName = "SMT_FFB_FRAME_MUTEX";

        private static SMFFBSourceManager _instance = null;
        public static SMFFBSourceManager instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new SMFFBSourceManager();
                }
                return _instance;
            }

        }

        string configFilename;
        string installPath;
        public SMFFBSourceManagerConfig configData = null;

        List<SMFFBEffect> effects = new List<SMFFBEffect>();

        CMCustomUDPData outputData = new CMCustomUDPData();

        protected Mutex mutex;
        protected MemoryMappedFile outputMMF;


        public virtual void Init(string _installPath)
        {
            installPath = _installPath;
        }

        public virtual void Input(CMCustomUDPData telemetryData)
        {
            foreach (SMFFBEffect effect in effects)
            {
                effect.Update(telemetryData, outputData);
            }

            SendOutputData();
        }

        void AddEffect(SMFFBEffect effect)
        {
            effects.Add(effect);
        }

        public virtual SMFFBEffect CreateEffect(SMFFBEffectConfig effectConfig)
        {
            SMFFBEffect newEffect = null;

            Type type = Type.GetType(effectConfig.id);

            if (type == null)
            {
                return null;
            }

            newEffect = Activator.CreateInstance(type) as SMFFBEffect;

            if(newEffect != null)
            {
                newEffect.Init(effectConfig);
                AddEffect(newEffect);
            }

            return newEffect;

        }

        public virtual void AddEffectToConfig(SMFFBEffect effect)
        {
            if (configData == null)
                configData = new SMFFBSourceManagerConfig();

            configData.effects.Add(effect.effectConfig);
        }

        public void Cleanup()
        {
            foreach(SMFFBEffect effect in effects)
            {
                effect.Destroy();
            }

            effects.Clear();


            if (outputMMF != null)
                outputMMF.Dispose();
            outputMMF = null;

        }

        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            if (!File.Exists(installPath + configFilename))
                return;

            Cleanup();

            configData = JsonConvert.DeserializeObject<SMFFBSourceManagerConfig>(File.ReadAllText(installPath + configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if(configData != null)
            {
                foreach(SMFFBEffectConfig effectConfig in configData.effects)
                {
                    CreateEffect(effectConfig);
                }
            }

            mutex = new Mutex(false, MMFMutexName);
            outputMMF = MemoryMappedFile.CreateOrOpen(MMFName, 10000);
            outputData.Init(installPath + "PacketFormats\\ffbPacketFormat.xml");

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

        public void SendOutputData()
        {
            if (outputMMF != null)
            {
                byte[] bytes = outputData.GetBytes();

                mutex.WaitOne();

                using (MemoryMappedViewStream stream = outputMMF.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    writer.Write(bytes);
                }

                mutex.ReleaseMutex();
            }


        }
    }
}
