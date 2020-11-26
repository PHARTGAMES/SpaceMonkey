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


namespace GenericTelemetryProvider
{
    public partial class Dirt5UI : Form
    {

        Dirt5TelemetryProvider provider;

        string saveFilename = "Dirt5Config.txt";

        public Dirt5UI()
        {
            InitializeComponent();

            statusLabel.Text = "Select Vehicle & Click Initialize!";

            LoadConfig();

            provider = new Dirt5TelemetryProvider();
            provider.ui = this;

            FilterModule.Instance.InitFromConfig("Dirt5Filters.txt"); //FIXME
//            FilterModule.Instance.SaveConfig();

        }

        public void ScanButtonClicked(object sender, EventArgs e)
        {

            initializeButton.Enabled = false;
            statusLabel.Text = "Please Wait";
            progressBar1.Value = 0;

            provider.vehicleString = vehicleSelector.Text;
            provider.Stop();
            provider.Run();
        }



        void LoadConfig()
        {
            string[] vehicles = System.IO.File.ReadAllLines("Dirt5Vehicles.txt");

            vehicleSelector.Items.AddRange(vehicles);

            if (File.Exists(saveFilename))
            {
                string[] saveData = System.IO.File.ReadAllLines(saveFilename);

                if (saveData != null && saveData.Length != 0)
                {
                    for (int i = 0; i < vehicles.Length; ++i)
                    {
                        if (vehicles[i] == saveData[0])
                        {
                            vehicleSelector.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        void SaveConfig()
        {
            string[] saveData = { (string)vehicleSelector.SelectedItem };

            File.WriteAllLines(saveFilename, saveData);
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
    }
}
