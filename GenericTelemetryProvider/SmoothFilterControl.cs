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
    public partial class SmoothFilterControl : UserControl
    {

        public NestedSmoothFilter filter;
        bool ignoreChanges = false;

        public SmoothFilterControl()
        {
            InitializeComponent();
        }

        public void SetFilter(NestedSmoothFilter _filter)
        {
            filter = _filter;

            ignoreChanges = true;

            nestCount.Text = "" + filter.GetNestCount();
            stepCount.Text = "" + filter.GetSampleCount();
            maxDelta.Text = "" + filter.GetMaxDelta();

            ignoreChanges = false;
        }

        private void nestCount_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseInt(nestCount, filter.GetNestCount()),
                Utils.TextBoxSafeParseInt(stepCount, filter.GetSampleCount()),
                Utils.TextBoxSafeParseFloat(maxDelta, filter.GetMaxDelta()));
        }

        private void stepCount_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseInt(nestCount, filter.GetNestCount()),
                Utils.TextBoxSafeParseInt(stepCount, filter.GetSampleCount()),
                Utils.TextBoxSafeParseFloat(maxDelta, filter.GetMaxDelta()));
        }

        private void maxDelta_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseInt(nestCount, filter.GetNestCount()),
                Utils.TextBoxSafeParseInt(stepCount, filter.GetSampleCount()),
                Utils.TextBoxSafeParseFloat(maxDelta, filter.GetMaxDelta()));
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
    }
}
