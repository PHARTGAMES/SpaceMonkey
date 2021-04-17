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
    public partial class WreckfestUI : Form
    {

        WreckfestTelemetryProvider provider;

        string saveFilename = "Wreckfest\\WreckfestConfig.txt";

        public WreckfestUI()
        {
            InitializeComponent();

            statusLabel.Text = "Select Vehicle & Click Initialize!";

            LoadConfig();

            provider = new WreckfestTelemetryProvider();
            provider.gameUI = provider.ui = this;

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }

        public void ScanButtonClicked(object sender, EventArgs e)
        {
            MainConfig.Instance.configData.CopyFileToDestinations(MainConfig.Instance.configData.packetFormat);

            initializeButton.Enabled = false;
            statusLabel.Text = "Please Wait";
            progressBar1.Value = 0;

            provider.vehicleString = vehicleSelector.Text;
            provider.Stop();
            provider.Run();
        }



        void LoadConfig()
        {

            if (File.Exists(saveFilename))
            {
                string text = File.ReadAllText(saveFilename);

                WreckfestConfig config = JsonConvert.DeserializeObject<WreckfestConfig>(text);

                for (int i = 0; i < vehicleSelector.Items.Count; ++i)
                {
                    if (((string)vehicleSelector.Items[i]).CompareTo(config.selectedVehicle) == 0)
                    {
                        Utils.SetComboBoxSelectedIndexThreadSafe(vehicleSelector, i);
                        break;
                    }
                }
            }

        }

        void SaveConfig()
        {
            WreckfestConfig save = new WreckfestConfig();

            save.selectedVehicle = (string)vehicleSelector.SelectedItem;

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


        private void vehicleSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void statusLabel_TextChanged(object sender, EventArgs e)
        {

        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            ScanButtonClicked(sender, e);
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
                BeginInvoke(new Action<Form>((s) => { s.Dispose(); }), this);

            Application.ExitThread();
        }



    }

    public class WreckfestConfig
    {
        public string selectedVehicle;        
    }


}
