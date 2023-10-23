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

    public class XIDIDeviceConfig
    {
        public string m_deviceID;
        public bool m_enabled;
    }

    public enum XInputControlAxis
    {
        X,
        Y,
        NONE
    }

    public class XIDIMapConfig
    {
        public XInputControl m_xi;
        public XInputControlAxis m_axis;
        public string m_diDeviceID;
        public string m_diObjectID;
        public int m_diObjectIndex = -1;
        public bool m_invert;
    }

    public class XInputFFBInputMappingConfig
    {
        public List<XIDIDeviceConfig> m_devices = new List<XIDIDeviceConfig>();
        public List<XIDIMapConfig> m_inputMap = new List<XIDIMapConfig>();

        public void SetDeviceEnabled(string a_deviceID, bool a_enabled)
        {
            XIDIDeviceConfig deviceConfig = m_devices.Find((x) => x.m_deviceID == a_deviceID);

            if (deviceConfig == null)
            {
                deviceConfig = new XIDIDeviceConfig();
                m_devices.Add(deviceConfig);
            }

            deviceConfig.m_enabled = a_enabled;

        }

        public bool GetDeviceEnabled(string a_deviceID)
        {
            XIDIDeviceConfig deviceConfig = m_devices.Find((x) => x.m_deviceID == a_deviceID);

            if (deviceConfig == null)
                return false;

            return deviceConfig.m_enabled;
        }

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



    public struct XIControlMetadata
    {
        public XInputControl m_control;
        public string m_uiName;
        public XInputControlAxis m_axis;

    }

    public class XInputFFBInputMapping
    {
        public static XInputFFBInputMapping Instance;
        public XInputFFBInputMappingConfig m_config = new XInputFFBInputMappingConfig();
        List<XIDIMap> m_runningMap = new List<XIDIMap>();
        bool m_stopThread = false;
        Thread thread;


        public static List<XIControlMetadata> XIControlMetadataDefinitions = new List<XIControlMetadata>
        {
            new XIControlMetadata { m_control = XInputControl.BUTTON_LOGO, m_uiName = "GUIDE BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_A, m_uiName = "A BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_B, m_uiName = "B BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_X, m_uiName = "X BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_Y, m_uiName = "Y BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_LB, m_uiName = "LB BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_RB, m_uiName = "RB BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_BACK, m_uiName = "BACK BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_START, m_uiName = "START BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_L3, m_uiName = "L3 BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.BUTTON_R3, m_uiName = "R3 BTN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.DPAD_UP, m_uiName = "DPAD UP", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.DPAD_DOWN, m_uiName = "DPAD DOWN", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.DPAD_LEFT, m_uiName = "DPAD LEFT", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.DPAD_RIGHT, m_uiName = "DPAD RIGHT", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.TRIGGER_LEFT, m_uiName = "LEFT TRIGGER", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.TRIGGER_RIGHT, m_uiName = "RIGHT TRIGGER", m_axis = XInputControlAxis.NONE },
            new XIControlMetadata { m_control = XInputControl.JOY_LEFT, m_uiName = "LEFT STICK X", m_axis = XInputControlAxis.X },
            new XIControlMetadata { m_control = XInputControl.JOY_LEFT, m_uiName = "LEFT STICK Y", m_axis = XInputControlAxis.Y },
            new XIControlMetadata { m_control = XInputControl.JOY_RIGHT, m_uiName = "RIGHT STICK X", m_axis = XInputControlAxis.X },
            new XIControlMetadata { m_control = XInputControl.JOY_RIGHT, m_uiName = "RIGHT STICK Y", m_axis = XInputControlAxis.Y }
        };


        public XInputFFBInputMapping()
        {
            Instance = this;
        }

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

        public void SetDeviceEnabled(string a_deviceID, bool a_enabled)
        {
            m_config.SetDeviceEnabled(a_deviceID, a_enabled);

        }

        public bool GetDeviceEnabled(string a_deviceID)
        {
            return m_config.GetDeviceEnabled(a_deviceID);
        }


    }
}
