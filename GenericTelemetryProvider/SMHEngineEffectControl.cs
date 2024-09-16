using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMHaptics;

namespace GenericTelemetryProvider
{
    public partial class SMHEngineEffectControl : UserControl
    {

        public SMHEngineEffectConfig config;
        bool ignoreChanges = false;

        public SMHEngineEffectControl()
        {
            InitializeComponent();
        }

        public void SetConfig(SMHEffectConfig _config)
        {
            config = _config as SMHEngineEffectConfig;

            ignoreChanges = true;

            minFrequency.Text = "" + config.MinFrequency;
            maxFrequency.Text = "" + config.MaxFrequency;
            outputChannelIndex.Text = "" + config.OutputChannelIndex;

            gainTrackBar.Value = (int)(config.Gain * 100.0);
            enabledCheckBox.Checked = config.Enabled;


            List<string> deviceNames = SMHOutputManager.instance.GetDeviceNames();

            foreach (string deviceName in deviceNames)
            {
                outputDeviceComboBox.Items.Add(deviceName);
            }
            

            List<string> deviceModuleNames = SMHOutputManager.instance.GetDeviceModuleNames();

            outputDeviceComboBox.SelectedIndex = -1;
            for (int i = 0; i < deviceModuleNames.Count; ++i)
            {
                if(string.Compare(deviceModuleNames[i], config.OutputDeviceModuleName) == 0)
                {
                    outputDeviceComboBox.SelectedIndex = i;
                    break;
                }
            }

            ignoreChanges = false;
        }

        private void minFrequency_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.MinFrequency = Utils.TextBoxSafeParseInt(minFrequency, (int)config.MinFrequency);
        }

        private void maxFrequency_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.MaxFrequency = Utils.TextBoxSafeParseInt(maxFrequency, (int)config.MaxFrequency);
        }

        private void outputChannelIndex_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.OutputChannelIndex = Utils.TextBoxSafeParseInt(outputChannelIndex, (int)config.OutputChannelIndex);

        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            HapticsUI.Instance.DeleteControl(this);
        }

        private void outputDeviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            List<string> deviceNames = SMHOutputManager.instance.GetDeviceNames();

            List<string> deviceModuleNames = SMHOutputManager.instance.GetDeviceModuleNames();

            for(int i = 0; i < deviceNames.Count; ++i)
            {
                if (string.Compare(deviceNames[i], (string)outputDeviceComboBox.SelectedItem) == 0)
                {
                    config.OutputDeviceModuleName = deviceModuleNames[i];
                }
            }
        }

        private void enabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.Enabled = enabledCheckBox.Checked;
        }

        private void gainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.Gain = gainTrackBar.Value / 100.0;
        }
    }
}
