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
    public partial class KalmanFilterControl : UserControl
    {

        public KalmanNoiseFilter filter;
        bool ignoreChanges = false;

        public KalmanFilterControl()
        {
            InitializeComponent();
        }

        public void SetFilter(KalmanNoiseFilter _filter)
        {
            filter = _filter;

            ignoreChanges = true;

            A.Text = "" + filter.GetA();
            H.Text = "" + filter.GetH();
            Q.Text = "" + filter.GetQ();
            R.Text = "" + filter.GetR();
            P.Text = "" + filter.GetP();
            X.Text = "" + filter.GetX();
 
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
        private void A_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));

        }

        private void H_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));
        }

        private void Q_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));
        }

        private void R_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));
        }

        private void P_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));
        }

        private void X_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            filter.SetParameters(Utils.TextBoxSafeParseFloat(A, filter.GetA()),
                Utils.TextBoxSafeParseFloat(H, filter.GetH()),
                Utils.TextBoxSafeParseFloat(Q, filter.GetQ()),
                Utils.TextBoxSafeParseFloat(R, filter.GetR()),
                Utils.TextBoxSafeParseFloat(P, filter.GetP()),
                Utils.TextBoxSafeParseFloat(X, filter.GetX()));

        }

    }
}
