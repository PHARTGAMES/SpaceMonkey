using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMHaptics;

namespace GenericTelemetryProvider
{
    public partial class SMHEffectPicker : Form
    {
        public SMHEffectPicker()
        {
            InitializeComponent();

            foreach(SMHEffectDef effectDef in SMHEffectDefs.Values.Values)
            {
                effectComboBox.Items.Add(effectDef.name);
            }

            effectComboBox.SelectedIndex = 0;
        }

        private void effectComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int index = effectComboBox.SelectedIndex;

            SMHEffectDef effectDef = SMHEffectDefs.GetByName(effectComboBox.SelectedItem as string);

            if(effectDef != null)
            {
                SMHapticsManager.instance.AddEffectToConfig(SMHapticsManager.instance.CreateEffect(effectDef.defaultConfig));
                HapticsUI.Instance.InitFromConfig();
            }

            this.Close();
        }
    }
}
