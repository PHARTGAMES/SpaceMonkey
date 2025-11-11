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
using SMFFBSource;


namespace GenericTelemetryProvider
{
    public partial class FFBUI : Form
    {

        public static FFBUI Instance;


        public FFBUI()
        {
            Instance = this;

            InitializeComponent();
        }

        private void FFBUI_Load(Object sender, EventArgs e)
        {
            InitFromConfig();
        }


        public void InitFromConfig()
        {

            flowLayoutEffects.Invoke((Action)delegate
            {
                flowLayoutEffects.Controls.Clear();

                if (SMFFBSourceManager.instance.configData != null)
                {

                    foreach (SMFFBEffectConfig effectConfig in SMFFBSourceManager.instance.configData.effects)
                    {
                        SMFFBEffectDef effectDef = SMFFBEffectDefs.GetByClassName(effectConfig.id);

                        Type type = Type.GetType(effectDef.controlClassName);

                        if (type != null)
                        {
                            Control newControl = Activator.CreateInstance(type) as Control;

                            if (newControl != null)
                            {
                                if (newControl is SMFFBWheelSteerEffectControl)
                                {
                                    (newControl as SMFFBWheelSteerEffectControl).SetConfig(effectConfig);
                                }

                                flowLayoutEffects.Controls.Add(newControl);
                            }
                        }
                    }

                }

                Button addButton = new Button();
                addButton.Text = @"ADD EFFECT";
                addButton.Click += new System.EventHandler(this.AddButtonClick);

                flowLayoutEffects.Controls.Add(addButton);
            });
        }


        public void InitChartForKey(CMCustomUDPData.DataKey dataKey)
        {
            //filterKey = dataKey;
            
            //filterChart.Invoke((Action)delegate
            //{
            //    var chart = filterChart.ChartAreas[0];

            //    chart.AxisX.Minimum = 0;
            //    chart.AxisX.Maximum = 1;
            //    chart.AxisY.Minimum = -chartSize;
            //    chart.AxisY.Maximum = chartSize;
            //    chart.AxisX.Interval = 0;
            //    chart.AxisY.Interval = 0;
            //});

            //flowLayoutFilters.Invoke((Action)delegate
            //{

            //    flowLayoutFilters.Controls.Clear();

            //    List<FilterBase> filters = FilterModuleCustom.Instance.filters[(int)filterKey];

            //    if (filters != null)
            //    {
            //        foreach (FilterBase filter in filters)
            //        {
            //            if (filter is NestedSmoothFilter)
            //            {
            //                SmoothFilterControl newControl = new SmoothFilterControl();
            //                newControl.SetFilter((NestedSmoothFilter)filter);

            //                flowLayoutFilters.Controls.Add(newControl);
            //            }
            //            else
            //            if (filter is KalmanNoiseFilter)
            //            {
            //                KalmanFilterControl newControl = new KalmanFilterControl();
            //                newControl.SetFilter((KalmanNoiseFilter)filter);

            //                flowLayoutFilters.Controls.Add(newControl);
            //            }
            //            else
            //            if (filter is MedianFilterWrapper)
            //            {
            //                MedianFilterControl newControl = new MedianFilterControl();
            //                newControl.SetFilter((MedianFilterWrapper)filter);

            //                flowLayoutFilters.Controls.Add(newControl);
            //            }
            //            else
            //            if (filter is KalmanVelocityNoiseFilter)
            //            {
            //                KalmanVelocityFilterControl newControl = new KalmanVelocityFilterControl();
            //                newControl.SetFilter((KalmanVelocityNoiseFilter)filter);

            //                flowLayoutFilters.Controls.Add(newControl);
            //            }

            //        }
            //    }

            //    Button addButton = new Button();
            //    addButton.Text = @"ADD FILTER";
            //    addButton.Click += new System.EventHandler(this.AddButtonClick);

            //    flowLayoutFilters.Controls.Add(addButton);
            //});

        }


        public void AddButtonClick(object sender, EventArgs e)
        {
            SMFFBEffectPicker picker = new SMFFBEffectPicker();

            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((SMFFBEffectPicker)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(picker);

        }


 

        public void DeleteControl(UserControl control)
        {
            int index = flowLayoutEffects.Controls.IndexOf(control);

            SMFFBSourceManager.instance.DeleteEffectAtIndex(index);

            flowLayoutEffects.Controls.Remove(control);
        }

        public void MoveControl(UserControl control, int direction)
        {
            int index = flowLayoutEffects.Controls.GetChildIndex(control);

            index = Math.Min(flowLayoutEffects.Controls.Count-2, Math.Max(0, index + direction));

            flowLayoutEffects.Controls.SetChildIndex(control, index);

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SMFFBSourceManager.instance.SaveConfig();
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
