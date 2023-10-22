using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace XInputFFB
{

    public class XIDIMapConfig
    {
        public XInputControl m_xi;
        public string m_diDeviceID;
        public string m_diObjectID;
        public int m_diObjectIndex = -1;
        public bool m_invert;
    }

    public class XInputFFBInputMappingConfig
    {
        public List<XIDIMapConfig> m_inputMap = new List<XIDIMapConfig>();


    }

    public class XIDIMap
    {
        public XIDIMap(XIDIMapConfig a_config, DIDevice a_diDevice)
        {
            m_config = a_config;
            m_diDevice = a_diDevice;
        }

        public XIDIMapConfig m_config;
        public DIDevice m_diDevice;
    }

    public class XInputFFBInputMapping
    {
        public XInputFFBInputMappingConfig m_config;
        List<XIDIMap> m_runningMap = new List<XIDIMap>();
        bool m_stopThread = false;
        Thread thread;

        public void Load(string a_path)
        {
            string text = File.ReadAllText(a_path);

            m_config = JsonConvert.DeserializeObject<XInputFFBInputMappingConfig>(text);
        }

        public void Save(string a_path)
        {
            string output = JsonConvert.SerializeObject(m_config, Formatting.Indented);

            File.WriteAllText(a_path, output);
        }

        void RefreshRunning()
        {
            m_runningMap.Clear();

            foreach(XIDIMapConfig mapConfig in m_config.m_inputMap)
            {
                DIDevice device = DInputDeviceManager.Instance.GetDevice(mapConfig.m_diDeviceID);

                if(device != null)
                {
                    XIDIMap newMap = new XIDIMap(mapConfig, device);

                    device.Acquire();

                    m_runningMap.Add(newMap);
                }
            }
        }

        void StartRunning()
        {
            thread = new Thread(RunThread);
            thread.IsBackground = true;
            thread.Start();
        }

        void StopRunning()
        {

        }

        void RunThread()
        {
            RefreshRunning();

            //while(!m_stopThread)
            //{
            //    foreach(XIDIMap map in m_runningMap)
            //    {
            //        map.
            //    }
            //}

        }

    }
}
