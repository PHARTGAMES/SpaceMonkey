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
    public partial class SquadronsUI : Form
    {

        SquadronsTelemetryProvider provider;

        string saveFilename = "Squadrons\\SquadronsConfig.txt";

        public SquadronsUI()
        {
            InitializeComponent();

            statusLabel.Text = "Waiting for Telemetry";

            LoadConfig();

            provider = new SquadronsTelemetryProvider();
            provider.gameUI = provider.ui = this;
            

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }


        void LoadConfig()
        {
            return;
            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                SquadronsConfig config = JsonConvert.DeserializeObject<SquadronsConfig>(text);

            }
        }

        void SaveConfig()
        {
            return;
            SquadronsConfig save = new SquadronsConfig();

            string output = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(saveFilename, output);
        }


        public void ProgressBarChanged(int progress)
        {
            Utils.SetProgressThreadSafe(progressBar1, progress);
        }


        public void StatusTextChanged(string text)
        {
            Utils.SetTextBoxThreadSafe(statusLabel, text);
        }

        public void InitButtonStatusChanged(bool enable)
        {
            Utils.EnableButtonThreadSafe(initializeButton, enable);
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
            statusLabel.Text = "Waiting For Squadrons";

            provider.StopAllThreads();
            provider.Stop();
            provider.Run();

        }
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            provider.StopAllThreads();
            provider.Stop();
            if (!IsDisposed)
                BeginInvoke(new Action<Form>((s) => { s.Dispose(); }), this);

            Application.ExitThread();
        }
    }

    public class SquadronsConfig
    {
    }


}
