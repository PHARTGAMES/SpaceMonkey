using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;


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


        public void Load()
        {

            if (File.Exists("gtp.txt"))
            {
                string configFilename = File.ReadAllText("gtp.txt");
                if(File.Exists(configFilename))
                {
                    saveFilename = configFilename;
                }
            }

            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                configData = JsonConvert.DeserializeObject<MainConfigData>(text);
            }
        }

        public void Save()
        {
            string output = JsonConvert.SerializeObject(configData, Formatting.Indented);

            File.WriteAllText(saveFilename, output);
        }

    }

    public class MainConfigData
    {
        public bool sendUDP;
        public string udpIP;
        public int udpPort;

        public bool fillMMF;

        public List<string> copyFormatDestinations;
        public string packetFormat;
        public string filterConfig;

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

            for(int i = 0; i < copyFormatDestinations.Count; ++i)
            {
                if(string.Compare(copyFormatDestinations[i], text) == 0)
                {
                    copyFormatDestinations.RemoveAt(i);
                    break;
                }
            }    
        }

        public void CopyFileToDestinations(string from)
        {
            if (copyFormatDestinations == null)
                return;

            if (!File.Exists(from))
            {
                Console.WriteLine(from + " does not exist");
                return;
            }

            foreach (string dest in copyFormatDestinations)
            {
                try
                {
                    File.Copy(from, dest, true);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
