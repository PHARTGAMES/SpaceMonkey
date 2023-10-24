using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using CMCustomUDP;

namespace XInputFFB
{
    public class XInputFFBClient
    {
        static MainUI m_mainForm;

        public static void Init(Action<bool> a_initCallback)
        {
            Thread thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Load and create your form from the embedded resource
                m_mainForm = new MainUI(a_initCallback);

                // Run the form on its own application thread
                Application.Run(m_mainForm);
            });

            thread.SetApartmentState(ApartmentState.STA); // Set the thread to run in STA mode (required for Windows Forms)
            thread.Start();
        }

        public static void UpdateTelemetry(CMCustomUDPData a_telemetry)
        {
            if (m_mainForm == null)
                return;

            m_mainForm.UpdateTelemetry(a_telemetry);
        }
    }
}
