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
        public DIInputMapControl()
        {
            InitializeComponent();
        }

        public void Init(XIControlMetadata a_metadata)
        {
            m_metadata = a_metadata;

            lblXInput.Text = m_metadata.m_uiName;
        }


    }
}
