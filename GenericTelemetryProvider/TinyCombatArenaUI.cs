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
    public partial class TinyCombatArenaUI : Form
    {

        TinyCombatArenaTelemetryProvider provider;

        string saveFilename = "TCA\\TCAConfig.txt";

        public TinyCombatArenaUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new TinyCombatArenaTelemetryProvider();
            provider.gameUI = provider.ui = this;
            

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
            TCAConfig save = new TCAConfig();

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
            statusLabel.Text = "Waiting For Tiny Combat Arena";

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

    public class TCAConfig
    {
    }


}
