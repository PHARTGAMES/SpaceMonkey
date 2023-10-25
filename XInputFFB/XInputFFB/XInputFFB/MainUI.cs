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

namespace XInputFFB
{
    public partial class MainUI : Form
    {
        Thread m_testThread;
        bool m_stopThread = false;

        public MainUI(Action<bool> initCallback)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
        }


        public void StartXInputOutput()
        { 
            XInputFFBCom.Instance.COMPort = "COM5";
            XInputFFBCom.Instance.StartCMDMessenger();
        }

        void TestXInputFFBCom()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            
            while (!m_stopThread)
            {
                double elapsed = sw.Elapsed.TotalSeconds;

                double stickRange = 32768;

                Int16 stickVal = (Int16)(Math.Sin(elapsed) * stickRange);

                XInputFFBCom.Instance.SendControlStateStick(XInputControl.JOY_LEFT, stickVal, -stickVal);

                XInputFFBCom.Instance.SendControlStateButton(XInputControl.BUTTON_A, stickVal > 0);

                Thread.Sleep(10);
            }

            Thread.CurrentThread.Join();
        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

            m_stopThread = true;

            Thread.Sleep(0);

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

            RefreshDevicesList();
            RefreshMappingList();

            XInputFFBInputMapping.Instance.StartRunning();
        }

        void LoadConfig()
        {
            MainConfig.Instance.Load();

            XInputFFBInputMapping.Instance.Load(MainConfig.installPath + MainConfig.Instance.configData.m_mappingConfig);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            XInputFFBInputMapping.Instance.Save(MainConfig.installPath + MainConfig.Instance.configData.m_mappingConfig);
        }

        public void UpdateTelemetry(CMCustomUDPData a_telemetry)
        {

        }
    }
}
