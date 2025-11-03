using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SMFFBSource;

namespace GenericTelemetryProvider
{
    public partial class SMFFBWheelSteerEffectControl : UserControl
    {

        public SMFFBWheelSteerEffectConfig config;
        bool ignoreChanges = false;

        public SMFFBWheelSteerEffectControl()
        {
            InitializeComponent();
        }

        public void SetConfig(SMFFBEffectConfig _config)
        {
            config = _config as SMFFBWheelSteerEffectConfig;

            ignoreChanges = true;

            maxWheelAngle.Text = "" + config.MaxWheelAngle;
            constantForceAngleCurve.Text = "" + config.ConstantForceAngleCurve;
            minConstantForce.Text = "" + config.MinConstantForce;
            maxConstantForce.Text = "" + config.MaxConstantForce;
            constantForceSpeedCurve.Text = "" + config.ConstantForceSpeedCurve;
            minDampForce.Text = "" + config.MinDampForce;
            maxDampForce.Text = "" + config.MaxDampForce;
            dampForceSpeedCurve.Text = "" + config.DampForceSpeedCurve;
            minVibrationFrequency.Text = "" + config.MinVibrationFrequency;
            maxVibrationFrequency.Text = "" + config.MaxVibrationFrequency;
            minVibrationGain.Text = "" + config.MinVibrationGain;
            maxVibrationGain.Text = "" + config.MaxVibrationGain;
            vibrationSpeedCurve.Text = "" + config.vibrationSpeedCurve;

            ignoreChanges = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            HapticsUI.Instance.DeleteControl(this);
        }

        private void enabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

//            config.Enabled = enabledCheckBox.Checked;
        }

        private void maxWheelAngle_TextChanged(object sender, EventArgs e)
        {
            config.MaxWheelAngle = Utils.TextBoxSafeParseFloat(maxWheelAngle, (int)config.MaxWheelAngle);
        }

        private void constantForceAngleCurve_TextChanged(object sender, EventArgs e)
        {
            config.ConstantForceAngleCurve = Utils.TextBoxSafeParseFloat(constantForceAngleCurve, (int)config.ConstantForceAngleCurve);
        }

        private void minConstantForce_TextChanged(object sender, EventArgs e)
        {
            config.MinConstantForce = Utils.TextBoxSafeParseFloat(minConstantForce, (int)config.MinConstantForce);
        }

        private void maxConstantForce_TextChanged(object sender, EventArgs e)
        {
            config.MaxConstantForce = Utils.TextBoxSafeParseFloat(maxConstantForce, (int)config.MaxConstantForce);
        }

        private void constantForceSpeedCurve_TextChanged(object sender, EventArgs e)
        {
            config.ConstantForceSpeedCurve = Utils.TextBoxSafeParseFloat(constantForceSpeedCurve, (int)config.ConstantForceSpeedCurve);
        }

        private void minDampForce_TextChanged(object sender, EventArgs e)
        {
            config.MinDampForce = Utils.TextBoxSafeParseFloat(minDampForce, (int)config.MinDampForce);
        }

        private void maxDampForce_TextChanged(object sender, EventArgs e)
        {
            config.MaxDampForce = Utils.TextBoxSafeParseFloat(maxDampForce, (int)config.MaxDampForce);
        }

        private void dampForceSpeedCurve_TextChanged(object sender, EventArgs e)
        {
            config.DampForceSpeedCurve = Utils.TextBoxSafeParseFloat(dampForceSpeedCurve, (int)config.DampForceSpeedCurve);
        }

        private void minVibrationFrequency_TextChanged(object sender, EventArgs e)
        {
            config.MinVibrationFrequency = Utils.TextBoxSafeParseFloat(minVibrationFrequency, (int)config.MinVibrationFrequency);
        }

        private void maxVibrationFrequency_TextChanged(object sender, EventArgs e)
        {
            config.MaxVibrationFrequency = Utils.TextBoxSafeParseFloat(maxVibrationFrequency, (int)config.MaxVibrationFrequency);
        }

        private void minVibrationGain_TextChanged(object sender, EventArgs e)
        {
            config.MinVibrationGain = Utils.TextBoxSafeParseFloat(minVibrationGain, (int)config.MinVibrationGain);
        }

        private void maxVibrationGain_TextChanged(object sender, EventArgs e)
        {
            config.MaxVibrationGain = Utils.TextBoxSafeParseFloat(maxVibrationGain, (int)config.MaxVibrationGain);
        }

        private void vibrationSpeedCurve_TextChanged(object sender, EventArgs e)
        {
            config.VibrationSpeedCurve = Utils.TextBoxSafeParseFloat(vibrationSpeedCurve, (int)config.VibrationSpeedCurve);
        }
    }
}
