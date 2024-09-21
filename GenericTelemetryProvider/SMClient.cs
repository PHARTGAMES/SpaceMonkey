using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using GenericTelemetryProvider;
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    public class SMClient
    {
        static MainForm mainForm;

        public static void Init(Action<bool> initCallback)
        {
            Thread thread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Load and create your form from the embedded resource
                mainForm = new MainForm(initCallback);

                // Run the form on its own application thread
                Application.Run(mainForm);
            });

            thread.SetApartmentState(ApartmentState.STA); // Set the thread to run in STA mode (required for Windows Forms)
            thread.Start();
        }

        public static void RegisterTelemetryCallback(Action<CMCustomUDPData, float> callback)
        {
            mainForm.RegisterTelemetryCallback(callback);
        }

    }
}
