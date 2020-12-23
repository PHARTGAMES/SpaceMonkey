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
    public partial class FilterPicker : Form
    {
        public FilterPicker()
        {
            InitializeComponent();

            for(int i = 0; i < (int)FilterModuleCustom.FilterType.Max; ++i)
            {
                filterComboBox.Items.Add(((FilterModuleCustom.FilterType)i).ToString());
            }
            filterComboBox.SelectedIndex = 0;
        }

        private void filterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            int index = filterComboBox.SelectedIndex;

            FilterModuleCustom.Instance.AddFilter((FilterModuleCustom.FilterType)index, FilterUI.Instance.filterKey, true);

            this.Close();
        }
    }
}
