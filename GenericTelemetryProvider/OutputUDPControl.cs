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
    public partial class OutputUDPControl : UserControl
    {

        public OutputConfigTypeData outputConfig;
        public OutputConfigTypeDataUDP typedConfig;
        bool ignoreChanges = false;
        public TelemetryOutputUDP output;

        public OutputUDPControl()
        {
            InitializeComponent();
        }

        public void SetOutput(TelemetryOutput _output)
        {
            output = _output as TelemetryOutputUDP;
            outputConfig = typedConfig = output.GetConfigTypeData() as OutputConfigTypeDataUDP;

            ignoreChanges = true;


            udpIP.Text = "" + typedConfig.udpIP;
            udpPort.Text = "" + typedConfig.udpPort;

            if (typedConfig.copyFormatDestinations != null)
            {
                formatDestinationsBox.Items.Clear();
                foreach (string entry in typedConfig.copyFormatDestinations)
                    formatDestinationsBox.Items.Add(entry);
            }

            string selectedItem = Path.GetFileNameWithoutExtension(outputConfig.packetFormat);
            packetFormatComboBox.Items.Clear();
            string[] files = Directory.GetFiles("PacketFormats");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                packetFormatComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    packetFormatComboBox.SelectedItem = filename;
            }

            ignoreChanges = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            OutputUI.Instance.DeleteControl(this);
        }

        private void heading_Click(object sender, EventArgs e)
        {

        }

        private void OutputUDPControl_Load(object sender, EventArgs e)
        {

        }

        private void udpIP_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            typedConfig.udpIP = udpIP.Text;

        }

        private void udpPort_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
                return;

            typedConfig.udpPort = Utils.TextBoxSafeParseInt(udpPort, typedConfig.udpPort);
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
