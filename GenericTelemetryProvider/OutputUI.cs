using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericTelemetryProvider
{
    public partial class OutputUI : Form
    {
        public static OutputUI Instance;

        public OutputUI()
        {
            InitializeComponent();

            Instance = this;

            for (int i = 0; i < (int)OutputModule.OutputType.Max; ++i)
            {
                outputTypesComboBox.Items.Add(((OutputModule.OutputType)i).ToString());
            }
            outputTypesComboBox.SelectedIndex = 0;
        }

        private void OutputUI_Load(object sender, EventArgs e)
        {
            OutputModule.Instance.InitFromConfig(MainConfig.Instance.configData.outputConfig);

            RefreshUI();
        }

        private void addOutputBtn_Click(object sender, EventArgs e)
        {
            int index = outputTypesComboBox.SelectedIndex;
            OutputModule.Instance.AddOutput((OutputModule.OutputType)index, true);
        }

        private void outputTypesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            OutputModule.Instance.SaveConfig();
        }

        public void RefreshUI()
        {
            flowLayoutPanelOutputs.Invoke((Action)delegate
            {
                flowLayoutPanelOutputs.Controls.Clear();

                List<TelemetryOutput> outputs = OutputModule.Instance.telemetryOutputs;

                if(outputs != null)
                {
                    foreach(TelemetryOutput output in outputs)
                    {
                        if(output is TelemetryOutputMMF)
                        {
                            OutputMMFControl newControl = new OutputMMFControl();
                            newControl.SetOutput(output);

                            flowLayoutPanelOutputs.Controls.Add(newControl);
                        }
                        else
                        if (output is TelemetryOutputUDP)
                        {
                            OutputUDPControl newControl = new OutputUDPControl();
                            newControl.SetOutput(output);

                            flowLayoutPanelOutputs.Controls.Add(newControl);
                        }
                    }
                }

            });
        }
        public void DeleteControl(UserControl control)
        {
            int index = flowLayoutPanelOutputs.Controls.IndexOf(control);

            if (control is OutputMMFControl)
            {
                OutputModule.Instance.DeleteOutput(((OutputMMFControl)control).output);
            }
            else
            if (control is OutputUDPControl)
            {
                OutputModule.Instance.DeleteOutput(((OutputUDPControl)control).output);
            }
            flowLayoutPanelOutputs.Controls.Remove(control);
        }
    }
}
