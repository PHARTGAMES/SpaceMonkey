using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XInputFFB
{
    public partial class DIInputMapControl : UserControl
    {
        public XIControlMetadata m_metadata;

        public Action<XIDIMapConfig> RecordAction;
        public Action ForgetAction;
        XIDIMapConfig m_mapConfig;

        bool m_ignoreChanges = false;

        public DIInputMapControl()
        {
            InitializeComponent();
        }

        public void Init(XIControlMetadata a_metadata)
        {
            m_metadata = a_metadata;

            lblXInput.Text = m_metadata.m_uiName;
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            XInputFFBInputMapping.Instance.StopRunning();

            DInputDeviceManager.Instance.DetectInput((List<DIInputDetectionResult> results) =>
            {
                foreach (DIInputDetectionResult result in results)
                {
                    Console.WriteLine($"Detected Input: {result.Identifier}, {result.m_delta}");
                }

                if (results.Count > 0)
                {
                    DIInputDetectionResult bestResult = results[0];
                    Console.WriteLine($"Best Input: {bestResult.Identifier}");

                    if(m_mapConfig == null)
                        m_mapConfig = new XIDIMapConfig();

                    m_mapConfig.m_diDeviceID = bestResult.m_deviceID;
                    m_mapConfig.m_diObjectID = bestResult.m_objectID;
                    m_mapConfig.m_invert = invertCheckBox.Checked;
                    m_mapConfig.m_xiControl = m_metadata.m_control;
                    m_mapConfig.m_axis = m_metadata.m_axis;

                    if (m_metadata.m_control == XInputControl.TRIGGER_LEFT || m_metadata.m_control == XInputControl.TRIGGER_RIGHT)
                    {
                        m_mapConfig.m_minOutput = 0;
                        m_mapConfig.m_maxOutput = 255;
                    }
                    else
                    if (m_metadata.m_control == XInputControl.JOY_LEFT || m_metadata.m_control == XInputControl.JOY_RIGHT)
                    {
                        m_mapConfig.m_minOutput = -32767;
                        m_mapConfig.m_maxOutput = 32767;
                    }

                    SetDataFromMapConfig(m_mapConfig);

                    RecordAction?.Invoke(m_mapConfig);
                }
            });
        }

        public void SetDataFromMapConfig(XIDIMapConfig a_mapConfig)
        {
            if (a_mapConfig == null)
                return;

            m_mapConfig = a_mapConfig;
            m_ignoreChanges = true;
            Utils.SetLabelThreadSafe(lblDIInput, m_mapConfig.UIString);
            m_ignoreChanges = true;
            Utils.SetTextBoxThreadSafe(tbMinInput, "" + m_mapConfig.m_minInput);
            m_ignoreChanges = true;
            Utils.SetTextBoxThreadSafe(tbMaxInput, "" + m_mapConfig.m_maxInput);
            m_ignoreChanges = true;
            invertCheckBox.Checked = m_mapConfig.m_invert;
        }

        private void btnForget_Click(object sender, EventArgs e)
        {
            XInputFFBInputMapping.Instance.StopRunning();

            if (m_mapConfig != null)
                XInputFFBInputMapping.Instance.RemoveMapConfig(m_mapConfig);

            m_mapConfig = null;

            m_ignoreChanges = true;
            invertCheckBox.Checked = false;
            lblValue.Text = "Value";
            lblDIInput.Text = "NONE";

            ForgetAction?.Invoke();
        }

        private void invertCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ignoreChanges)
            {
                m_ignoreChanges = false;
                return;
            }

            if (m_mapConfig != null)
            {
                m_mapConfig.m_invert = invertCheckBox.Checked;
                XInputFFBInputMapping.Instance.SetMapConfig(m_mapConfig);
            }
        }

        private void tbMinInput_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChanges)
            {
                m_ignoreChanges = false;
                return;
            }

            if(int.TryParse(tbMinInput.Text, out int value))
            {
                m_mapConfig.m_minInput = value;
                XInputFFBInputMapping.Instance.SetMapConfig(m_mapConfig);
            }
        }

        private void tbMaxInput_TextChanged(object sender, EventArgs e)
        {
            if (m_ignoreChanges)
            {
                m_ignoreChanges = false;
                return;
            }

            if (int.TryParse(tbMaxInput.Text, out int value))
            {
                m_mapConfig.m_maxInput = value;
                XInputFFBInputMapping.Instance.SetMapConfig(m_mapConfig);
            }
        }

    }
}
