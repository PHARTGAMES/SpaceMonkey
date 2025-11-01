using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFFBSource;

namespace GenericTelemetryProvider
{
    public partial class SMFFBEffectPicker : Form
    {
        public SMFFBEffectPicker()
        {
            InitializeComponent();

            foreach(SMFFBEffectDef effectDef in SMFFBEffectDefs.Values.Values)
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

            SMFFBEffectDef effectDef = SMFFBEffectDefs.GetByName(effectComboBox.SelectedItem as string);

            if(effectDef != null)
            {
                SMFFBSourceManager.instance.AddEffectToConfig(SMFFBSourceManager.instance.CreateEffect(effectDef.defaultConfig));
                HapticsUI.Instance.InitFromConfig();
            }

            this.Close();
        }
    }
}
