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

namespace XInputFFB
{
    public partial class MainUI : Form
    {
        XInputFFBCom m_ffbCom;
        Thread m_testThread;
        bool m_stopThread = false;
        DInputDeviceManager m_diDeviceManager;
        XInputFFBInputMapping m_ffbInputMapping;

        public MainUI()
        {
            InitializeComponent();

            m_diDeviceManager = new DInputDeviceManager();
            m_ffbInputMapping = new XInputFFBInputMapping();
        }


        public void StartXInputOutput()
        { 
            m_ffbCom = new XInputFFBCom();
            m_ffbCom.m_comPort = "COM5";
            m_ffbCom.StartCMDMessenger();
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

                m_ffbCom.SendControlStateStick(XInputControl.JOY_LEFT, stickVal, -stickVal);

                m_ffbCom.SendControlStateButton(XInputControl.BUTTON_A, stickVal > 0);

                Thread.Sleep(10);
            }

            Thread.CurrentThread.Join();
        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {

            m_stopThread = true;

            Thread.Sleep(0);

            if(m_ffbCom != null)
                m_ffbCom.StopCMDMessenger();

            if (!IsDisposed)
            {
                Dispose();
            }

            Application.ExitThread();
        }

        bool m_detecting = false;
        private void DetectInput_Click(object sender, EventArgs e)
        {
            if (m_detecting)
                return;

            m_detecting = true;
            m_diDeviceManager.DetectInput((List<DIInputDetectionResult> results) =>
            {
                foreach(DIInputDetectionResult result in results)
                {
                    Console.WriteLine($"Detected Input: {result.Identifier}, {result.m_delta}");
                }

                if(results.Count > 0)
                {
                    Console.WriteLine($"Best Input: {results[0].Identifier}");
                }

                m_detecting = false;
            });

        }

        void RefreshDevicesList()
        {
            devicesFlowPanel.Invoke((Action)delegate
            {
                devicesFlowPanel.Controls.Clear();

                foreach(DIDevice device in DInputDeviceManager.Instance.Devices)
                {
                    DIDeviceControl newControl = new DIDeviceControl();
                    newControl.SetDeviceName(device.ID);
                    newControl.SetEnabled(XInputFFBInputMapping.Instance.GetDeviceEnabled(device.ID));

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

                    mappingFlowPanel.Controls.Add(newControl);
                }
            });
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            RefreshDevicesList();
            RefreshMappingList();
        }


    }
}
