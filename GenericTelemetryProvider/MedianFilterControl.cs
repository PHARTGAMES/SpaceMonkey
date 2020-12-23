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
    public partial class MedianFilterControl : UserControl
    {

        public MedianFilterWrapper filter;
        bool ignoreChanges = false;

        public MedianFilterControl()
        {
            InitializeComponent();
        }

        public void SetFilter(MedianFilterWrapper _filter)
        {
            filter = _filter;

            ignoreChanges = true;

            stepCount.Text = "" + filter.GetSampleCount();

            ignoreChanges = false;
        }


        private void stepCount_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseInt(stepCount, filter.GetSampleCount()));
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

        private void heading_Click(object sender, EventArgs e)
        {

        }
    }
}
