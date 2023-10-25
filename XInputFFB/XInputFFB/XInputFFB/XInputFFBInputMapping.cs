using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using SharpDX.DirectInput;

namespace XInputFFB
{

    [Serializable]
    public class XIDIDeviceConfig
    {
        public string m_deviceID;
        public bool m_enabled;
    }

    [Serializable]
    public enum XInputControlAxis
    {
        X,
        Y,
        NONE
    }

    [Serializable]
    public class XIDIMapConfig
    {
        public XInputControl m_xiControl;
        public XInputControlAxis m_axis;
        public string m_diDeviceID;
        public string m_diObjectID;
        public bool m_invert;

        [JsonIgnore]
        public string UIString
        {
            get
            {
                string objectString = m_diObjectID;

                return $"{m_diDeviceID}, {objectString} ";
            }
        }
    }

    [Serializable]
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

    public struct XIStickState
    {
        public int m_x;
        public int m_y;
    }

    public class XInputFFBInputMapping
    {
        static XInputFFBInputMapping m_instance;

        public static XInputFFBInputMapping Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new XInputFFBInputMapping();
                }
                return m_instance;
            }
        }

        public XInputFFBInputMappingConfig m_config = new XInputFFBInputMappingConfig();
        List<XIDIMap> m_runningMap = new List<XIDIMap>();
        Dictionary<DIDevice, JoystickState> m_runningDeviceStates = new Dictionary<DIDevice, JoystickState>();
        List<DIDevice> m_runningDevices = new List<DIDevice>();
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

            foreach (DIDevice device in m_runningDevices)
            {
                device.Unacquire();
            }
            m_runningDeviceStates.Clear();
            m_runningDevices.Clear();
            m_runningMap.Clear();

            foreach(XIDIMapConfig mapConfig in m_config.m_inputMap)
            {
                DIDevice device = DInputDeviceManager.Instance.GetDevice(mapConfig.m_diDeviceID);

                if(device != null)
                {
                    XIDIMap newMap = new XIDIMap(mapConfig, device);

                    if(!m_runningDevices.Contains(device))
                    {
                        device.Acquire();
                        m_runningDevices.Add(device);
                    }
                    m_runningMap.Add(newMap);
                }
            }
        }

        public void StartRunning()
        {
            thread = new Thread(RunThread);
            thread.IsBackground = true;
            thread.Start();
        }

        public void StopRunning()
        {
            m_stopThread = true;

            while(m_stopThread)
                Thread.Sleep(1);
        }

        void RunThread()
        {
            RefreshRunning();

            Dictionary<XInputControl, XIStickState> stickStates = new Dictionary<XInputControl, XIStickState>();

            while (!m_stopThread)
            {
                stickStates.Clear();

                //read device state
                foreach (DIDevice device in m_runningDevices)
                {
                    m_runningDeviceStates[device] = device.PollState();
                }

                //pass on device state to mapped output on XInputFFBCom
                foreach (XIDIMap map in m_runningMap)
                {
                    JoystickState joystickState = m_runningDeviceStates[map.m_diDevice];

                    switch (map.m_config.m_xiControl)
                    {
                        case XInputControl.BUTTON_LOGO:
                        case XInputControl.BUTTON_A:
                        case XInputControl.BUTTON_B:
                        case XInputControl.BUTTON_X:
                        case XInputControl.BUTTON_Y:
                        case XInputControl.BUTTON_LB:
                        case XInputControl.BUTTON_RB:
                        case XInputControl.BUTTON_BACK:
                        case XInputControl.BUTTON_START:
                        case XInputControl.BUTTON_L3:
                        case XInputControl.BUTTON_R3:
                        case XInputControl.DPAD_UP:
                        case XInputControl.DPAD_DOWN:
                        case XInputControl.DPAD_LEFT:
                        case XInputControl.DPAD_RIGHT:
                            {
                                XInputFFBCom.Instance.SendControlStateButton(map.m_config.m_xiControl, joystickState.GetInputState<bool>(map.m_config.m_diObjectID));
                                break;
                            }

                        case XInputControl.TRIGGER_LEFT:
                        case XInputControl.TRIGGER_RIGHT:
                            {
                                XInputFFBCom.Instance.SendControlStateTrigger(map.m_config.m_xiControl, joystickState.GetInputState<int>(map.m_config.m_diObjectID));
                                break;
                            }

                        case XInputControl.JOY_LEFT:
                        case XInputControl.JOY_RIGHT:
                            {
                                int diState = joystickState.GetInputState<int>(map.m_config.m_diObjectID);
                                if (map.m_config.m_axis == XInputControlAxis.X)
                                {
                                    if(stickStates.TryGetValue(map.m_config.m_xiControl, out XIStickState stickState))
                                    {
                                        stickState.m_x = diState;
                                        stickStates[map.m_config.m_xiControl] = stickState;
                                    }
                                    else
                                    {
                                        stickStates.Add(map.m_config.m_xiControl, new XIStickState { m_x = diState, m_y = 0 });
                                    }
                                }
                                else
                                if (map.m_config.m_axis == XInputControlAxis.Y)
                                {
                                    if (stickStates.TryGetValue(map.m_config.m_xiControl, out XIStickState stickState))
                                    {
                                        stickState.m_y = diState;
                                        stickStates[map.m_config.m_xiControl] = stickState;
                                    }
                                    else
                                    {
                                        stickStates.Add(map.m_config.m_xiControl, new XIStickState { m_x = 0, m_y = diState });
                                    }
                                }

                                break;
                            }

                    }

                }

                if (stickStates.ContainsKey(XInputControl.JOY_LEFT))
                {
                    XIStickState stickState = stickStates[XInputControl.JOY_LEFT];

                    XInputFFBCom.Instance.SendControlStateStick(XInputControl.JOY_LEFT, stickState.m_x, stickState.m_y);
                }

                if (stickStates.ContainsKey(XInputControl.JOY_RIGHT))
                {
                    XIStickState stickState = stickStates[XInputControl.JOY_RIGHT];

                    XInputFFBCom.Instance.SendControlStateStick(XInputControl.JOY_RIGHT, stickState.m_x, stickState.m_y);
                }
            }

            m_stopThread = false;
            Thread.CurrentThread.Join();
        }

        public void SetDeviceEnabled(string a_deviceID, bool a_enabled)
        {
            m_config.SetDeviceEnabled(a_deviceID, a_enabled);

        }

        public bool GetDeviceEnabled(string a_deviceID)
        {
            return m_config.GetDeviceEnabled(a_deviceID);
        }

        public void SetMapConfig(XIDIMapConfig a_config)
        {
            XIDIMapConfig foundConfig = m_config.m_inputMap.Find((x) => x.m_xiControl == a_config.m_xiControl && x.m_axis == a_config.m_axis);

            if (foundConfig != null)
            {
                m_config.m_inputMap[m_config.m_inputMap.IndexOf(foundConfig)] = a_config;
            }
            else
                m_config.m_inputMap.Add(a_config);
        }

        public void RemoveMapConfig(XIDIMapConfig a_config)
        {
            XIDIMapConfig foundConfig = m_config.m_inputMap.Find((x) => x.m_xiControl == a_config.m_xiControl && x.m_axis == a_config.m_axis);

            m_config.m_inputMap.Remove(foundConfig);
        }

        public XIDIMapConfig GetMapConfig(XIControlMetadata a_metadataDef)
        {
            XIDIMapConfig foundConfig = m_config.m_inputMap.Find((x) => x.m_xiControl == a_metadataDef.m_control && x.m_axis == a_metadataDef.m_axis);

            return foundConfig;
        }

        public void SetDeviceConfig(XIDIDeviceConfig a_config)
        {
            XIDIDeviceConfig foundConfig = m_config.m_devices.Find((x) => x.m_deviceID == a_config.m_deviceID);

            if (foundConfig != null)
            {
                m_config.m_devices[m_config.m_devices.IndexOf(foundConfig)] = a_config;
            }
            else
                m_config.m_devices.Add(a_config);
        }

        public XIDIDeviceConfig GetDeviceConfig(string a_deviceID)
        {
            XIDIDeviceConfig foundConfig = m_config.m_devices.Find((x) => x.m_deviceID == a_deviceID);

            if (foundConfig == null)
            {
                foundConfig = new XIDIDeviceConfig();
                foundConfig.m_deviceID = a_deviceID;
                m_config.m_devices.Add(foundConfig);
            }

            return foundConfig;
        }
    }
}
