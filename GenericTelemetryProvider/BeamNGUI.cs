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
    public partial class BeamNGUI : Form
    {

        BeamNGTelemetryProvider provider;

        string saveFilename = "BeamNG\\BeamNGConfig.txt";

        public BeamNGUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new BeamNGTelemetryProvider();
            provider.gameUI = provider.ui = this;

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }


        void LoadConfig()
        {

            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                BeamNGConfig config = JsonConvert.DeserializeObject<BeamNGConfig>(text);

                portTextBox.Text = "" + config.port;
            }
        }

        void SaveConfig()
        {
            BeamNGConfig save = new BeamNGConfig();

            int.TryParse(portTextBox.Text, out save.port);

            string output = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(saveFilename, output);
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
            MainConfig.Instance.configData.CopyFileToDestinations(MainConfig.Instance.configData.packetFormat);

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
            this.Dispose();

            Application.ExitThread();
        }

    }

    public class BeamNGConfig
    {
        public int port;
    }


}
