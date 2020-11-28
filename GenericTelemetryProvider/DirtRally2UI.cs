using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GenericTelemetryProvider
{
    public partial class DirtRally2UI : Form
    {
        public DirtRally2TelemetryProvider provider;

        public DirtRally2UI()
        {
            InitializeComponent();

            provider = new DirtRally2TelemetryProvider();

            FilterModule.Instance.InitFromConfig("DirtRally2Filters.txt");

            provider.Run();
        }
    }
}
