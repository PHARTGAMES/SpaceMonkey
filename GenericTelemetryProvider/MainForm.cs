using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Sojaner.MemoryScanner;
using System.Numerics;
using System.IO;
using System.IO.MemoryMappedFiles;


namespace GenericTelemetryProvider
{

    public delegate void ButtonClickedCallback(object sender, EventArgs e);


    public partial class MainForm : Form
    {

        private delegate void SafeCallProgressDelegate(int progress);
        private delegate void SafeCallBoolDelegate(bool value);
        private delegate void SafeCallStringDelegate(string value);

        Dirt5TelemetryProvider provider;

        public ButtonClickedCallback onBtnClicked;

        string saveFilename = "Dirt5Config.txt";


        public MainForm()
        {
            InitializeComponent();

            onBtnClicked = ScanButtonClicked;
            statusLabel.Text = "Select Vehicle & Click Initialize!";

            LoadConfig();

            provider = new Dirt5TelemetryProvider();
            provider.debugChangedCallback += DebugTextChanged;
            provider.statusChangedCallback += StatusTextChanged;
            provider.progressBarChangedCallback += ProgressBarChanged;
        }

        private void initializeButton_Click(object sender, EventArgs e)
        {
            onBtnClicked(sender, e);
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




        void SetTextBoxThreadSafe(TextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                SafeCallStringDelegate d = new SafeCallStringDelegate((x) => { textBox.Text = x; });
                textBox.Invoke(d, new object[] { text });
            }
            else
                textBox.Enabled = true;
        }
        void SetRichTextBoxThreadSafe(RichTextBox textBox, string text)
        {
            if (textBox.InvokeRequired)
            {
                SafeCallStringDelegate d = new SafeCallStringDelegate((x) => { textBox.Text = x; });
                textBox.Invoke(d, new object[] { text });
            }
            else
                textBox.Enabled = true;
        }

        void EnableButtonThreadSafe(Button button, bool value)
        {
            if (button.InvokeRequired)
            {
                SafeCallBoolDelegate d = new SafeCallBoolDelegate((x) => { button.Enabled = x; });
                button.Invoke(d, new object[] { value });
            }
            else
                button.Enabled = true;

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

        void ProgressBarChanged(int progress)
        {
            if (progressBar1.InvokeRequired)
            {
                SafeCallProgressDelegate d = new SafeCallProgressDelegate((x) => { progressBar1.Value = x; });
                progressBar1.Invoke(d, new object[] { progress });
            }
            else
                progressBar1.Value = progress;

        }

        void StatusTextChanged(string text)
        {
            SetTextBoxThreadSafe(statusLabel, text);
        }

        void InitButtonStatusChanged(bool enable)
        {
            EnableButtonThreadSafe(initializeButton, enable);
        }

        void DebugTextChanged(string text)
        {
            SetRichTextBoxThreadSafe(matrixBox, text);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
         
        private void StatusBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void vehicleSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveConfig();
        }
    }
}
