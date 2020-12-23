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
    public partial class GTAVUI : Form
    {

        GTAVTelemetryProvider provider;

        string saveFilename = "GTAV\\GTAVConfig.txt";

        public GTAVUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new GTAVTelemetryProvider();
            provider.ui = this;

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }


        void LoadConfig()
        {
            return;
            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                GTAVConfig config = JsonConvert.DeserializeObject<GTAVConfig>(text);

            }
        }

        void SaveConfig()
        {
            return;
            GTAVConfig save = new GTAVConfig();

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


        private void initializeButton_Click(object sender, EventArgs e)
        {
            MainConfig.Instance.configData.CopyFileToDestinations(MainConfig.Instance.configData.packetFormat);

            initializeButton.Enabled = false;
            statusLabel.Text = "Waiting For GTA V";

            provider.Stop();
            provider.Run();

        }

    }

    public class GTAVConfig
    {
    }


}
