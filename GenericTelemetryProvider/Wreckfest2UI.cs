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

        public Wreckfest2UI()
        {
            InitializeComponent();

            statusLabel.Text = "1. Click Scan";

            LoadConfig();

            provider = new Wreckfest2TelemetryProvider();
            provider.gameUI = provider.ui = this;

            maxCarsComboBox.Items.Clear();
            for (int i = 1; i <= 24; ++i)
            {
                maxCarsComboBox.Items.Add("" + i);
            }

            maxCarsComboBox.SelectedIndex = maxCarsComboBox.Items.Count - 1;

            ignoreUIChanges = true;
            vehicleSelector.Enabled = false;
            initializeButton.Enabled = false;
            scanButton.Enabled = true;


            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig); 

        }

        public void RefreshCars(List<WF2CarAddress> cars)
        {
            carList = cars;

            Utils.UIThreadSafeLambda(vehicleSelector, () => {
                ignoreUIChanges = true;
                vehicleSelector.Items.Clear();

                ignoreUIChanges = true;
                vehicleSelector.Enabled = true;

                foreach (WF2CarAddress car in carList)
                {
                    ignoreUIChanges = true;
                    vehicleSelector.Items.Add(car.id);
                }

                if (vehicleSelector.Items.Count > 0)
                {
                    ignoreUIChanges = true;
                    vehicleSelector.SelectedIndex = 0;
                }
            });

        }

        public void ScanButtonClicked(object sender, EventArgs e)
        {
            initializeButton.Enabled = false;
            vehicleSelector.Enabled = false;
            statusLabel.Text = "Please Wait";
            progressBar1.Value = 0;

            provider.vehicleString = vehicleSelector.Text;
            provider.Stop();
            provider.Run();
        }



        void LoadConfig()
        {

            if (File.Exists(MainConfig.installPath + saveFilename))
            {
                string text = File.ReadAllText(MainConfig.installPath + saveFilename);

                Wreckfest2Config config = JsonConvert.DeserializeObject<Wreckfest2Config>(text);

            }

        }

        void SaveConfig()
        {
            Wreckfest2Config save = new Wreckfest2Config();

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
            Utils.EnableButtonThreadSafe(initializeButton, enable);
        }

        public void DebugTextChanged(string text)
        {
            Utils.SetRichTextBoxThreadSafe(matrixBox, text);
        }


        private void vehicleSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreUIChanges)
            {
                ignoreUIChanges = false;
                return;
            }

            if (vehicleSelector.SelectedIndex < carList.Count)
            {
                provider.SetSelectedCarAddress(carList[vehicleSelector.SelectedIndex]);
            }
            if (provider.IsStopped)
            {
                StatusTextChanged("3. Click Initialize");
            }
            SaveConfig();
        }

        private void statusLabel_TextChanged(object sender, EventArgs e)
        {

        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            if (vehicleSelector.SelectedIndex >= carList.Count)
                return;

            initializeButton.Enabled = false;
            scanButton.Enabled = true;

            StatusTextChanged("Running!");

            WF2CarAddress foundAddress = carList[vehicleSelector.SelectedIndex];

            provider.Initialize(foundAddress);
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

        private void scanButton_Click(object sender, EventArgs e)
        {
            ScanButtonClicked(sender, e);

        }

        private void maxCarsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public int GetMaxCarsCount()
        {
            return maxCarsComboBox.SelectedIndex + 1;
        }
    }

    public class Wreckfest2Config
    {
    }


}
