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
using SMMotion;
using System.Numerics;

namespace GenericTelemetryProvider
{
    public partial class MotionUI : Form
    {

        public static MotionUI Instance;

        SMControlRigConfig config;
        bool ignoreChanges = false;

        public MotionUI()
        {
            Instance = this;

            InitializeComponent();
        }


        private void MotionUI_Load(Object sender, EventArgs e)
        {
            InitFromConfig();
        }


        public void InitFromConfig()
        {
            config = SMMotionManager.instance.configData.controlRig;

            ignoreChanges = true;

            rigWidthTextBox.Text = "" + config.RigWidth;
            rigLengthTextBox.Text = "" + config.RigLength;
            avoTextBox.Text = "" + config.ActuatorVerticalOffset;
            asTextBox.Text = "" + config.ActuatorStroke;
            alTextBox.Text = "" + config.ActuatorLength;
            hoxTextBox.Text = "" + config.HeadLocalOffset.X;
            hoyTextBox.Text = "" + config.HeadLocalOffset.Y;
            hozTextBox.Text = "" + config.HeadLocalOffset.Z;
            accScaleTextBox.Text = "" + config.AccelerationScale;
            accMaxTextBox.Text = "" + config.MaxAcceleration;
            enabledCheckBox.Checked = config.Enabled;


            ignoreChanges = false;

        }
               

 
        private void saveButton_Click(object sender, EventArgs e)
        {
            SMMotionManager.instance.SaveConfig();
        }


        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDisposed)
            {
                Dispose();
            }
            Application.ExitThread();
        }


        private void rigWidthTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.RigWidth = Utils.TextBoxSafeParseFloat(rigWidthTextBox, config.RigWidth);
        }

        private void rigLengthTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.RigLength = Utils.TextBoxSafeParseFloat(rigLengthTextBox, config.RigLength);
        }

        private void avoTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.ActuatorVerticalOffset = Utils.TextBoxSafeParseFloat(avoTextBox, config.ActuatorVerticalOffset);
        }

        private void asTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.ActuatorStroke = Utils.TextBoxSafeParseFloat(asTextBox, config.ActuatorStroke);
        }

        private void alTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.ActuatorLength = Utils.TextBoxSafeParseFloat(alTextBox, config.ActuatorLength);
        }

        private void hoxTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.HeadLocalOffset = new Vector3(Utils.TextBoxSafeParseFloat(hoxTextBox, config.HeadLocalOffset.X), config.HeadLocalOffset.Y, config.HeadLocalOffset.Z);
        }

        private void hoyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.HeadLocalOffset = new Vector3(config.HeadLocalOffset.X, Utils.TextBoxSafeParseFloat(hoyTextBox, config.HeadLocalOffset.Y), config.HeadLocalOffset.Z);
        }


        private void hozTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.HeadLocalOffset = new Vector3(config.HeadLocalOffset.X, config.HeadLocalOffset.Y, Utils.TextBoxSafeParseFloat(hozTextBox, config.HeadLocalOffset.Z));
        }

        private void accScaleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.AccelerationScale = Utils.TextBoxSafeParseFloat(accScaleTextBox, config.AccelerationScale);
        }

        private void accMaxTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.MaxAcceleration = Utils.TextBoxSafeParseFloat(accMaxTextBox, config.MaxAcceleration);

        }

        private void enabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            config.Enabled = enabledCheckBox.Checked;
        }
    }
}
