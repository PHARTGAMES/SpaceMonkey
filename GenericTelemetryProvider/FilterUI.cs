using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using CMCustomUDP;


namespace GenericTelemetryProvider
{
    public partial class FilterUI : Form
    {

        public static FilterUI Instance;

        public CMCustomUDPData.DataKey filterKey = CMCustomUDPData.DataKey.Max;
        Series filteredSeries;
        Series rawSeries;
        int chartSize = 30;

        public FilterUI()
        {
            Instance = this;

            InitializeComponent();
        }

        private void FilterUI_Load(Object sender, EventArgs e)
        { 
            filterChart.Series.Clear();

            var chart = filterChart.ChartAreas[0];

            chart.AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.AxisX.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.Format = "";
            chart.AxisY.LabelStyle.IsEndLabelVisible = true;

            filteredSeries = filterChart.Series.Add("filtered");
            filteredSeries.ChartType = SeriesChartType.Line;
            filteredSeries.Color = Color.Red;
            filteredSeries.IsVisibleInLegend = true;

            filteredSeries.Points.Clear();

            rawSeries = filterChart.Series.Add("raw");
            rawSeries.ChartType = SeriesChartType.Line;
            rawSeries.Color = Color.Blue;
            rawSeries.IsVisibleInLegend = true;

            rawSeries.Points.Clear();

            for(int key = 0; key < (int)CMCustomUDPData.DataKey.Max; ++key)
            {
                keyComboBox.Items.Add(((CMCustomUDPData.DataKey)key).ToString());
            }

            FilterModuleCustom.Instance.InitFromConfig(MainConfig.Instance.configData.filterConfig);

            keyComboBox.SelectedIndex = 0;

            trackBar1.Minimum = 5;
            trackBar1.Maximum = 150;
            
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000 / 100;
            timer.Tick += new EventHandler(RefreshChart);
            timer.Start();
        }

        private void filterChart_Click(object sender, EventArgs e)
        {

        }

        public void InitChartForKey(CMCustomUDPData.DataKey dataKey)
        {
            filterKey = dataKey;
            
            filterChart.Invoke((Action)delegate
            {
                var chart = filterChart.ChartAreas[0];

                chart.AxisX.Minimum = 0;
                chart.AxisX.Maximum = 1;
                chart.AxisY.Minimum = -chartSize;
                chart.AxisY.Maximum = chartSize;
                chart.AxisX.Interval = 0;
                chart.AxisY.Interval = 0;
            });

            flowLayoutFilters.Invoke((Action)delegate
            {

                flowLayoutFilters.Controls.Clear();

                List<FilterBase> filters = FilterModuleCustom.Instance.filters[(int)filterKey];

                if (filters != null)
                {
                    foreach (FilterBase filter in filters)
                    {
                        if (filter is NestedSmoothFilter)
                        {
                            SmoothFilterControl newControl = new SmoothFilterControl();
                            newControl.SetFilter((NestedSmoothFilter)filter);

                            flowLayoutFilters.Controls.Add(newControl);
                        }
                        else
                        if (filter is KalmanNoiseFilter)
                        {
                            KalmanFilterControl newControl = new KalmanFilterControl();
                            newControl.SetFilter((KalmanNoiseFilter)filter);

                            flowLayoutFilters.Controls.Add(newControl);
                        }
                        else
                        if (filter is MedianFilterWrapper)
                        {
                            MedianFilterControl newControl = new MedianFilterControl();
                            newControl.SetFilter((MedianFilterWrapper)filter);

                            flowLayoutFilters.Controls.Add(newControl);
                        }
                        else
                        if (filter is KalmanVelocityNoiseFilter)
                        {
                            KalmanVelocityFilterControl newControl = new KalmanVelocityFilterControl();
                            newControl.SetFilter((KalmanVelocityNoiseFilter)filter);

                            flowLayoutFilters.Controls.Add(newControl);
                        }

                    }
                }

                Button addButton = new Button();
                addButton.Text = @"ADD FILTER";
                addButton.Click += new System.EventHandler(this.AddButtonClick);

                flowLayoutFilters.Controls.Add(addButton);
            });

        }
        public void AddButtonClick(object sender, EventArgs e)
        {
            FilterPicker picker = new FilterPicker();

            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((FilterPicker)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(picker);

        }


        public void RefreshChart(object sender, EventArgs e)
        {
            if (filterKey == CMCustomUDPData.DataKey.Max)
                return;

            filterChart.Invoke((Action)delegate
            {
                var chart = filterChart.ChartAreas[0];
                filteredSeries.Points.Clear();
                rawSeries.Points.Clear();

                List<CMCustomUDPData> filteredData;
                FilterModuleCustom.Instance.GetFilteredHistory(out filteredData);
                List<CMCustomUDPData> rawData;
                FilterModuleCustom.Instance.GetRawHistory(out rawData);

                chart.AxisX.Maximum = filteredData.Count();

                for (int i = 0; i < filteredData.Count; ++i)
                {
                    CMCustomUDPData data = filteredData[i];

                    filteredSeries.Points.AddXY(i, data.GetValue(filterKey));
                }

                for (int i = 0; i < rawData.Count; ++i)
                {
                    CMCustomUDPData data = rawData[i];

                    rawSeries.Points.AddXY(i, data.GetValue(filterKey));
                }

            });

        }

        private void keyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitChartForKey((CMCustomUDPData.DataKey)keyComboBox.SelectedIndex);
        }

        public void DeleteControl(UserControl control)
        {
            int index = flowLayoutFilters.Controls.IndexOf(control);

            if (control is SmoothFilterControl)
            {
                FilterModuleCustom.Instance.DeleteFilter(((SmoothFilterControl)control).filter, filterKey);
            }
            else
            if (control is KalmanFilterControl)
            {
                FilterModuleCustom.Instance.DeleteFilter(((KalmanFilterControl)control).filter, filterKey);
            }
            else
            if (control is MedianFilterControl)
            {
                FilterModuleCustom.Instance.DeleteFilter(((MedianFilterControl)control).filter, filterKey);
            }
            else
            if (control is KalmanVelocityFilterControl)
            {
                FilterModuleCustom.Instance.DeleteFilter(((KalmanVelocityFilterControl)control).filter, filterKey);
            }
            flowLayoutFilters.Controls.Remove(control);
        }

        public void MoveControl(UserControl control, int direction)
        {
            int index = flowLayoutFilters.Controls.GetChildIndex(control);

            index = Math.Min(flowLayoutFilters.Controls.Count-2, Math.Max(0, index + direction));

            flowLayoutFilters.Controls.SetChildIndex(control, index);

            if (control is SmoothFilterControl)
            {
                FilterModuleCustom.Instance.MoveFilter(((SmoothFilterControl)control).filter, filterKey, direction);
            }
            else
            if (control is KalmanFilterControl)
            {
                FilterModuleCustom.Instance.MoveFilter(((KalmanFilterControl)control).filter, filterKey, direction);
            }
            else
            if (control is MedianFilterControl)
            {
                FilterModuleCustom.Instance.MoveFilter(((MedianFilterControl)control).filter, filterKey, direction);
            }
            else
            if (control is KalmanVelocityFilterControl)
            {
                FilterModuleCustom.Instance.MoveFilter(((KalmanVelocityFilterControl)control).filter, filterKey, direction);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            FilterModuleCustom.Instance.SaveConfig();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            chartSize = trackBar1.Value;

            var chart = filterChart.ChartAreas[0];

            chart.AxisY.Minimum = -chartSize;
            chart.AxisY.Maximum = chartSize;
            
        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDisposed)
            {
                Dispose();
            }
            Application.ExitThread();
        }
    }
}
