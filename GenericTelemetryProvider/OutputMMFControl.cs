using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GenericTelemetryProvider
{
    public partial class OutputMMFControl : UserControl
    {

        public OutputConfigTypeData outputConfig;
        public OutputConfigTypeDataMMF typedConfig;
        bool ignoreChanges = false;
        public TelemetryOutputMMF output;

        public OutputMMFControl()
        {
            InitializeComponent();
        }

        public void SetOutput(TelemetryOutput _output)
        {
            output = _output as TelemetryOutputMMF;
            outputConfig = typedConfig = output.GetConfigTypeData() as OutputConfigTypeDataMMF;

            ignoreChanges = true;

            mmfName.Text = "" + typedConfig.mmfName;
            mmfMutexName.Text = "" + typedConfig.mmfMutexName;

            if (typedConfig.copyFormatDestinations != null)
            {
                formatDestinationsBox.Items.Clear();
                foreach (string entry in typedConfig.copyFormatDestinations)
                    formatDestinationsBox.Items.Add(entry);
            }

            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.installPath + outputConfig.packetFormat);
            packetFormatComboBox.Items.Clear();
            string[] files = Directory.GetFiles(MainConfig.installPath + "PacketFormats");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                packetFormatComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    packetFormatComboBox.SelectedItem = filename;
            }

            ignoreChanges = false;
        }

        private void mmfMutexName_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            typedConfig.mmfName = mmfName.Text;
        }

        private void mmfName_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            typedConfig.mmfMutexName = mmfMutexName.Text;
        }


        private void deleteButton_Click(object sender, EventArgs e)
        {
            OutputUI.Instance.DeleteControl(this);
        }

        private void heading_Click(object sender, EventArgs e)
        {

        }

        private void packetFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            if (packetFormatComboBox.SelectedItem == null)
                return;

            typedConfig.packetFormat = "PacketFormats\\" + (string)packetFormatComboBox.SelectedItem + ".xml";
        }

        private void formatDestinationsBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void destinationFindButton_Click(object sender, EventArgs e)
        {
            try
            {
                MTAOpenFileDialog dialog = new MTAOpenFileDialog();
                dialog.OnComplete = (obj, args) =>
                {
                    string result = (args as MTAOpenFileDialogEventArgs).FileName;

                    typedConfig.AddFormatDestination(result);
                    Utils.AddComboBoxEntryThreadSafe(formatDestinationsBox, result);
                };
                dialog.ShowDialog("CMCustomUDPFormat.xml");
            }
            catch (Exception exc)
            {
            }
        }

        private void deleteDestinationButton_Click(object sender, EventArgs e)
        {
            if (formatDestinationsBox.SelectedItem != null)
            {
                typedConfig.RemoveFormatDestination((string)formatDestinationsBox.SelectedItem);

                formatDestinationsBox.Items.RemoveAt(formatDestinationsBox.SelectedIndex);
                formatDestinationsBox.SelectedIndex = -1;
            }
        }
    }
}
