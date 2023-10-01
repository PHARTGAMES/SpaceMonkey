using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    public partial class WRCGenUI : Form
    {

        WRCGenTelemetryProvider provider;

        string saveFilename = "WRCGen\\WRCGenConfig.txt";

        public WRCGenUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new WRCGenTelemetryProvider();
            provider.gameUI = provider.ui = this;

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }


        void LoadConfig()
        {

            if (File.Exists(MainConfig.installPath + saveFilename))
            {
                string text = File.ReadAllText(MainConfig.installPath + saveFilename);

                WRCGenConfig config = JsonConvert.DeserializeObject<WRCGenConfig>(text);

                portTextBox.Text = "" + config.port;
            }
        }

        void SaveConfig()
        {
            WRCGenConfig save = new WRCGenConfig();

            int.TryParse(portTextBox.Text, out save.port);

            string output = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(MainConfig.installPath + saveFilename, output);
        }

        public void StatusTextChanged(string text)
        {
            Utils.SetTextBoxThreadSafe(statusLabel, text);
        }

        public void DebugTextChanged(string text)
        {
            Utils.SetRichTextBoxThreadSafe(matrixBox, text);
        }


        private void statusLabel_TextChanged(object sender, EventArgs e)
        {

        }

        private void matrixBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            initializeButton.Enabled = false;
            statusLabel.Text = "Waiting For Telemetry";

            int.TryParse(portTextBox.Text, out provider.readPort);

            provider.Stop();
            provider.Run();

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            provider.StopAllThreads();
            provider.Stop();
            if (!IsDisposed)
            {
                Dispose();
            }

            Application.ExitThread();
        }

    }

    public class WRCGenConfig
    {
        public int port;
    }


}
