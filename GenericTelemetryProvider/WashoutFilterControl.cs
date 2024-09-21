using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericTelemetryProvider
{


    public partial class WashoutFilterControl : UserControl
    {
        public WashoutFilter filter;
        private bool ignoreChanges = false;

        public WashoutFilterControl()
        {
            InitializeComponent();
        }

        public void SetFilter(WashoutFilter _filter)
        {
            filter = _filter;

            ignoreChanges = true;

            timeConstantTextBox.Text = filter.GetTimeConstant().ToString();

            ignoreChanges = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            FilterUI.Instance.DeleteControl(this);
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            FilterUI.Instance.MoveControl(this, -1);
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            FilterUI.Instance.MoveControl(this, 1);
        }

        private void timeConstantTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            // Update filter when the time constant changes
            filter.SetParameters(Utils.TextBoxSafeParseFloat(timeConstantTextBox, filter.GetTimeConstant()));
        }

    }

}
