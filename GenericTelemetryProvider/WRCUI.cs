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
    public partial class WRCUI : Form
    {

        WRCTelemetryProvider provider;

        string saveFilename = "WRC\\WRCConfig.txt";

        public WRCUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new WRCTelemetryProvider();
            provider.gameUI = provider.ui = this;
            

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }


        void LoadConfig()
        {
            return;
            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                WRCConfig config = JsonConvert.DeserializeObject<WRCConfig>(text);

            }
        }

        void SaveConfig()
        {
            return;
            WRCConfig save = new WRCConfig();

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
            statusLabel.Text = "Waiting For WRC";

            provider.Stop();
            provider.Run();

        }

    }

    public class WRCConfig
    {
    }


}
