using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.CompilerServices;

namespace GenericTelemetryProvider
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

            //string[] files = Directory.GetFiles(Environment.CurrentDirectory, "gtp.txt", SearchOption.AllDirectories);

            //if (files.Length > 0)
            //{

            //    installPath = Path.GetDirectoryName(files[0]) + "\\";
            //    Console.WriteLine("Found install path at " + installPath);
            //}
            //else
            //{
            //    Console.WriteLine("Could not find gtp.txt");
            //}


            RegistryKey localKey;
            if (Environment.Is64BitOperatingSystem)
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            installPath = localKey.OpenSubKey("SOFTWARE\\PHARTGAMES\\SpaceMonkeyTP").GetValue("install_path").ToString();

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Load()
        {
            ResolveInstallDirectory();

            if (string.IsNullOrEmpty(installPath))
                return;

            if (File.Exists(MainConfig.installPath + "gtp.txt"))
            {
                string configFilename = File.ReadAllText(MainConfig.installPath + "gtp.txt");
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
        public string filterConfig = "Filters\\defaultFilters.txt";

        public string outputConfig = "Outputs\\default_MMF_UDP.txt";

        public string hapticsConfig = "Haptics\\defaultHaptics.txt";

        public string motionConfig = "Motion\\defaultMotion.txt";

        public class HotkeyConfig
        {
            public Keys key;
            public bool enabled;
            public bool windows;
            public bool alt;
            public bool ctrl;
            public bool shift;
        }

        public HotkeyConfig hotkey = new HotkeyConfig();

    }

}
