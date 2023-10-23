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
        public Action<bool> OnCheckedChanged;

        bool ignoreChanges = false;
        public DIDeviceControl()
        {
            InitializeComponent();
        }

        public void SetDeviceName(string a_deviceName)
        {
            deviceEnabledCheckBox.Text = a_deviceName;
        }

        public void SetEnabled(bool a_enabled)
        {
            ignoreChanges = true;
            deviceEnabledCheckBox.Checked = a_enabled;
        }

        private void deviceEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                ignoreChanges = false;
                return;
            }

            OnCheckedChanged?.Invoke(deviceEnabledCheckBox.Checked);
        }
    }
}
