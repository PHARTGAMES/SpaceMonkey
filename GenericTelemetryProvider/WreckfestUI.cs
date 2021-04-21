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
#region CHART
        System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private double f(int i)
        {
            var f1 = 59894 - (8128 * i) + (262 * i * i) - (1.6 * i * i * i);
            return f1;
        }

        private void InitComp()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            //
            // chart1
            //
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            // this.chart1.Size = new System.Drawing.Size(284, 212);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "FakeChart";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
        }
        void InitChart()
        {

            InitComp();
            chart1.Series.Clear();
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
            };

            this.chart1.Series.Add(series1);

            for (int i = 0; i < 100; i++)
            {
                series1.Points.AddXY(i, f(i));
            }
            chart1.Invalidate();
        }
#endregion

        WreckfestTelemetryProvider provider;

        string saveFilename = "Wreckfest\\WreckfestConfig.txt";

        public void ThreadStart()
        {
            //never called
        }

        public WreckfestUI()
        {
            InitializeComponent();

            statusLabel.Text = "Select Vehicle & Click Initialize!";

            LoadConfig();

            provider = new WreckfestTelemetryProvider();
            provider.gameUI = provider.ui = this;

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig);

            //reset the boxes to the default values! (can WinForms do an UpdateOnStart thing?)
            GForceClip.Text = GForceMaxClip.ToString();
            UpdateRate.Text = (1000 / SleepTime).ToString();
            GForceScale.Text = GForceScaleFactor.ToString();
            StartAddress.Text = StartAddressIndex.ToString();
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

        public bool InvertYaw = true;

        private void InvertYawToggle_CheckedChanged(object sender, EventArgs e)
        {
            InvertYaw = InvertYawToggle.Checked;
        }
        public bool ZeroiseTilt = false;
        private void ZeroiseTiltToggle_CheckedChanged(object sender, EventArgs e)
        {
            ZeroiseTilt = ZeroiseTiltToggle.Checked;
        }
        public float VelocityScaleFactor = 1f;
        private void VelocityScale_TextChanged(object sender, EventArgs e)
        {
            float lrScale;

            if (float.TryParse(VelocityScale.Text, out lrScale))
                VelocityScaleFactor = lrScale;
            else
                VelocityScale.Text = VelocityScaleFactor.ToString();//revert to current
        }
        //this is annoying, it's not auto-validated at runtime
        public long StartAddressIndex = 1400000000;//Keep reducing this until there's no chance of missing (lowest so far has been 14.5)
        private void StartAddress_TextChanged(object sender, EventArgs e)
        {
            long lnAddress;

            if (long.TryParse(StartAddress.Text, out lnAddress))
                StartAddressIndex = lnAddress;
            else
                StartAddress.Text = StartAddressIndex.ToString();//revert to current
        }
        //threaded-ish way of getting data onto the generic chart - only we have no thread runnin HERE and no message pump
        public List<float> maChartedValues = new List<float>();
        internal void AddChartedValue(float lrVal)
        {
            maChartedValues.Add(lrVal);

            if (chart1.Series != null)
            {
                chart1.Series.Clear();
            }
            var series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line
            };

            this.chart1.Series.Add(series1);

            for (int i = 0; i < maChartedValues.Count; i++)
            {
                series1.Points.AddXY(i, maChartedValues[i]);
            }
            chart1.Invalidate();

        }

        //Ok wow, yes. So SRS seems to be treating these directly - this means that if we're accelerating properly, we get a G-forces of about 0.9
        //Which is equivalent to a (very!) fast car - however, the 'max telemetry' values in SRS are only set on a per-int basis
        //To get around this, scale up by 100, so fast acceleration is 90, not 0.9

        //However; I need to detect/parse out/fix/hide the MASSIVE nasty holes
        //I suspect this should be 9.8
        public float GForceScaleFactor = 10f;
        private void GForceScale_TextChanged(object sender, EventArgs e)
        {
            float lrScale;

            if (float.TryParse(GForceScale.Text, out lrScale))
                GForceScaleFactor = lrScale;
            else
                GForceScale.Text = GForceScaleFactor.ToString();//revert to current
        }

        //todo - this should match the updateDelayt in the generic base
        public int SleepTime = 10;//default is 10 ms or 100 updates/sec

        private void UpdateRate_TextChanged(object sender, EventArgs e)
        {
            int lnScale;

            if (int.TryParse(UpdateRate.Text, out lnScale))
            {
                SleepTime = 1000 / lnScale;
            }
            else
                UpdateRate.Text = (1000 / SleepTime).ToString();//revert to current
        }

        private void GForceClip_TextChanged(object sender, EventArgs e)
        {
            float lrClip;

            if (float.TryParse(GForceClip.Text, out lrClip))
                GForceMaxClip = lrClip;
            else
                GForceClip.Text = GForceMaxClip.ToString();//revert to current
        }

        public float GForceMaxClip = 150f;
     
    }

    public class WreckfestConfig
    {
        public string selectedVehicle;        
    }


}
