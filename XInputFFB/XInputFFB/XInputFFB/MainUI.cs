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

        public MainUI()
        {
            InitializeComponent();

            m_diDeviceManager = new DInputDeviceManager();

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
    }
}
