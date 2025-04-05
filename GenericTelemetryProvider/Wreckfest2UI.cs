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
    public partial class Wreckfest2UI : Form
    {

        Wreckfest2TelemetryProvider provider;
        List<WF2CarAddress> carList;
        string saveFilename = "Wreckfest2\\Wreckfest2Config.txt";
        bool ignoreUIChanges = false;
        public bool scanning = false;

        public Wreckfest2UI()
        {
            InitializeComponent();

            provider = new Wreckfest2TelemetryProvider();
            provider.gameUI = provider.ui = this;

            ignoreUIChanges = true;
            initializeLobbyButton.Enabled = false;


            Timer progressTimer = new Timer();
            progressTimer.Interval = 100; // update every 100ms (adjust as needed)
            progressTimer.Tick += ProgressTimer_Tick;
            progressTimer.Start();

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig);

            statusLabel.Text = "Enter Gamer Tag";

            provider.Stop();
            provider.Run();

            LoadConfig();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            Utils.UIThreadSafeLambda(progressBar1, () =>
            {
                if (scanning)
                {
                    // Increase by 5%
                    if (progressBar1.Value >= progressBar1.Maximum)
                    {
                        progressBar1.Value = progressBar1.Minimum;
                    }
                    else
                    {
                        progressBar1.Value = Math.Min(progressBar1.Value + 5, progressBar1.Maximum);
                    }
                }
            });
        }

        void LoadConfig()
        {

            if (File.Exists(MainConfig.installPath + saveFilename))
            {
                string text = File.ReadAllText(MainConfig.installPath + saveFilename);

                Wreckfest2Config config = JsonConvert.DeserializeObject<Wreckfest2Config>(text);

                if(!string.IsNullOrEmpty(config.gamerTag))
                {
                    provider.GamerTagChanged(config.gamerTag);
                    ignoreUIChanges = true;
                    Utils.SetTextBoxThreadSafe(gamerTagTextBox, config.gamerTag);
                }
            }

        }

        void SaveConfig()
        {
            Wreckfest2Config save = new Wreckfest2Config();

            save.gamerTag = provider.player.name;

            string output = JsonConvert.SerializeObject(save, Formatting.Indented);

            File.WriteAllText(MainConfig.installPath + saveFilename, output);
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
            Utils.EnableButtonThreadSafe(initializeLobbyButton, enable);
        }

        public void DebugTextChanged(string text)
        {
            Utils.SetRichTextBoxThreadSafe(matrixBox, text);
        }

        public void GamerTagTextChanged(string text)
        {
            ignoreUIChanges = true;
            Utils.SetTextBoxThreadSafe(gamerTagTextBox, text);
        }

        private void statusLabel_TextChanged(object sender, EventArgs e)
        {

        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            provider.Initialize(true);
        }

        private void initializeIngameButton_Click(object sender, EventArgs e)
        {
            provider.Initialize(false);
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void matrixBox_TextChanged(object sender, EventArgs e)
        {

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


        private void gamerTagTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreUIChanges)
            {
                ignoreUIChanges = false;
                return;
            }

            provider.GamerTagChanged(gamerTagTextBox.Text);

            SaveConfig();
        }


    }

    public class Wreckfest2Config
    {
        public string gamerTag;
    }


}
