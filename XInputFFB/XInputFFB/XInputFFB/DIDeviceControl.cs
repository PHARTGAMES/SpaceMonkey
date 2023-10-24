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
    public partial class DIDeviceControl : UserControl
    {
        XIDIDeviceConfig m_config;

        bool m_ignoreChanges = false;
        public DIDeviceControl()
        {
            InitializeComponent();
        }

        public void SetDataFromDeviceConfig(XIDIDeviceConfig a_config)
        {
            m_config = a_config;
            deviceEnabledCheckBox.Checked = m_config.m_enabled;
            deviceEnabledCheckBox.Text = m_config.m_deviceID;
            DInputDeviceManager.Instance.SetDeviceEnabled(m_config.m_deviceID, m_config.m_enabled);
        }

        private void deviceEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ignoreChanges)
            {
                m_ignoreChanges = false;
                return;
            }

            if (m_config != null)
            {
                m_config.m_enabled = deviceEnabledCheckBox.Checked;
                XInputFFBInputMapping.Instance.SetDeviceConfig(m_config);
                DInputDeviceManager.Instance.SetDeviceEnabled(m_config.m_deviceID, m_config.m_enabled);
            }
        }
    }
}
