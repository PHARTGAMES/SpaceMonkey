using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace XInputFFB
{
    class MainConfig
    {
        public static MainConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainConfig();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        static MainConfig instance;

        public MainConfigData configData = new MainConfigData();

        public static string saveFilename = "Configs\\defaultConfig.txt";
        public bool blockSave = false;

        public static string installPath = null;

        private void ResolveInstallDirectory()
        {
            if (installPath != null)
                return;

            installPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\PHARTGAMES\\SpaceMonkeyTP", "install_path", null);
        }

        public void Load()
        {
            ResolveInstallDirectory();

            if (string.IsNullOrEmpty(installPath))
                return;

            if (File.Exists(MainConfig.installPath + "xiffb.txt"))
            {
                string configFilename = File.ReadAllText(MainConfig.installPath + "xiffb.txt");
                if(File.Exists(MainConfig.installPath + configFilename))
                {
                    saveFilename = configFilename;
                }
            }

            if (File.Exists(MainConfig.installPath + saveFilename))
            {
                string text = File.ReadAllText(MainConfig.installPath + saveFilename);

                configData = JsonConvert.DeserializeObject<MainConfigData>(text);
            }
        }

        public void Save()
        {
            string output = JsonConvert.SerializeObject(configData, Formatting.Indented);

            File.WriteAllText(MainConfig.installPath + saveFilename, output);
        }
    }

    public class MainConfigData
    {
        public string m_comPort = "COM5";
        public string m_mappingConfig = "XIFFBMappings\\defaultMappings.txt";


    }

}
