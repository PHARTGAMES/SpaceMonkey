using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMCustomUDP;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;

namespace SMMotion
{

    public class SMMotionConfig
    {
        public SMControlRigConfig controlRig;

    }


    public class SMMotionManager
    {
        private static SMMotionManager _instance = null;
        public static SMMotionManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SMMotionManager();
                }
                return _instance;
            }

        }

        protected Action<SMMControlState> callback;

        string configFilename;
        string installPath;
        public SMMotionConfig configData = null;

        SMMControlRig controlRig = new SMMControlRig_3D_4A(); //FIXME: hax fix me, maybe never XD

        public virtual void Init(string _installPath)
        {
            installPath = _installPath;
        }

        public virtual void Input(CMCustomUDPData telemetryData, float dt)
        {
            if(controlRig != null)
            {
                SMMControlState controlState = controlRig.Update(telemetryData, dt);
                if(controlState != null)
                    callback?.Invoke(controlState);
            }
        }

        public void RegisterCallback(Action<SMMControlState> cb)
        {
            callback += cb;
        }


        public void Cleanup()
        {

        }

        public void InitFromConfig(string filename)
        {
            configFilename = filename;

            if (!File.Exists(installPath + configFilename))
                return;

            Cleanup();

            configData = JsonConvert.DeserializeObject<SMMotionConfig>(File.ReadAllText(installPath + configFilename), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            if (configData != null)
            {
                //if(configData.controlRig == null)
                //{
                //    SMControlRigConfig defaultConfig = new SMControlRigConfig();

                //    defaultConfig.enabled = false;
                //    defaultConfig.actuatorStroke = 0.1f;
                //    defaultConfig.actuatorVerticalOffset = 0.0f;
                //    defaultConfig.actuatorLength = 0.2f;
                //    defaultConfig.headLocalOffset = new Vector3(0.0f, 1.01f, -0.2f);
                //    defaultConfig.rigLength = 0.93f;
                //    defaultConfig.rigWidth = 0.855f;
                //    defaultConfig.accelerationScale = 0.05f;
                //    defaultConfig.maxAcceleration = 5.20f;

                //    configData.controlRig = defaultConfig;
                //}

                controlRig.Init(configData.controlRig);
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

    }

}
