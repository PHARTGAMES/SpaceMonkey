using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using CMCustomUDP;
using System.IO.Ports;

namespace XInputFFB
{
    public partial class MainUI : Form
    {
        bool m_ignoreChanges = false;

        public MainUI(Action<bool> initCallback)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
        }


        public void StartXInputOutput()
        { 
            XInputFFBCom.Instance.COMPort = MainConfig.Instance.configData.m_comPort;
            XInputFFBCom.Instance.StartCMDMessenger();
        }

        public void StopXinputOutput()
        {
            XInputFFBCom.Instance.StopCMDMessenger();
        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

            XInputFFBCom.Instance.StopCMDMessenger();

            if (!IsDisposed)
            {
                Dispose();
            }

            Application.ExitThread();
        }


        void RefreshDevicesList()
        {
            devicesFlowPanel.Invoke((Action)delegate
            {
                devicesFlowPanel.Controls.Clear();

                foreach(DIDevice device in DInputDeviceManager.Instance.Devices)
                {
                    DIDeviceControl newControl = new DIDeviceControl();

                    newControl.SetDataFromDeviceConfig(XInputFFBInputMapping.Instance.GetDeviceConfig(device.ID));

                    devicesFlowPanel.Controls.Add(newControl);
                }
            });
        }

        void RefreshMappingList()
        {
            mappingFlowPanel.Invoke((Action)delegate
            {
                mappingFlowPanel.Controls.Clear();

                foreach (XIControlMetadata def in XInputFFBInputMapping.XIControlMetadataDefinitions)
                {
                    DIInputMapControl newControl = new DIInputMapControl();

                    newControl.Init(def);
                    newControl.SetDataFromMapConfig(XInputFFBInputMapping.Instance.GetMapConfig(def));

                    newControl.RecordAction += (XIDIMapConfig a_config) =>
                    {
                        if(a_config != null)
                        {
                            XInputFFBInputMapping.Instance.SetMapConfig(a_config);
                            XInputFFBInputMapping.Instance.StartRunning();
                        }

                    };

                    newControl.ForgetAction += () =>
                    {
                        XInputFFBInputMapping.Instance.StartRunning();
                    };

                    mappingFlowPanel.Controls.Add(newControl);
                }
            });
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            LoadConfig();

            RefreshComPort();

            RefreshDevicesList();
            RefreshMappingList();

            XInputFFBInputMapping.Instance.StartRunning();

            if(!string.IsNullOrEmpty(MainConfig.Instance.configData.m_comPort))
                StartXInputOutput();
        }



        void LoadConfig()
        {
            MainConfig.Instance.Load();

            XInputFFBInputMapping.Instance.Load(MainConfig.installPath + MainConfig.Instance.configData.m_mappingConfig);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            XInputFFBInputMapping.Instance.Save(MainConfig.installPath + MainConfig.Instance.configData.m_mappingConfig);

            MainConfig.Instance.Save();
        }

        public void UpdateTelemetry(CMCustomUDPData a_telemetry)
        {

        }

        void RefreshComPort()
        {
            string[] portNames = SerialPort.GetPortNames();

            cbComPort.Items.Clear();
            int selectedIndex = 0;
            for(int i = 0; i < portNames.Length; ++i)
            {
                cbComPort.Items.Add(portNames[i]);
                if (portNames[i] == MainConfig.Instance.configData.m_comPort)
                {
                    selectedIndex = i;
                }
            }

            m_ignoreChanges = true;
            cbComPort.SelectedIndex = selectedIndex;
        }

        private void cbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_ignoreChanges)
            {
                m_ignoreChanges = false;
                return;
            }

            StopXinputOutput();
            MainConfig.Instance.configData.m_comPort = cbComPort.SelectedItem.ToString();
            StartXInputOutput();
        }
    }
}
