using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Numerics;
using System.IO;
using System.IO.MemoryMappedFiles;
using CMCustomUDP;

namespace GenericTelemetryProvider
{

    public partial class GenericTelemetryProvider : Form
    {

        public Dirt5UI dirt5UI;
        WreckfestUI wreckfestUI;
        BeamNGUI beamNGUI;
        GTAVUI gtavUI;
        DCSUI dcsUI;
        MonsterGamesUI mgUI;
        WRCUI wrcUI;
        RBRUI rbrUI;
        SquadronsUI squadronsUI;
        IL2UI il2UI;
        FilterUI filterUI;
        public static GenericTelemetryProvider Instance;
        public string versionString = "v1.0.3";

        bool ignoreConfigChanges = false;

        public GenericTelemetryProvider()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Instance = this;

            this.Text = "SpaceMonkey " + versionString;
            CMCustomUDPData.formatFilename = "PacketFormats\\defaultFormat.xml";
            if(Directory.Exists("Configs"))
            {
                LoadConfig();
            }
        }

        void RefreshConfigs()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.saveFilename);
            configComboBox.Items.Clear();
            string[] files = Directory.GetFiles("Configs");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                configComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    configComboBox.SelectedItem = filename;
            }
        }

        void RefreshPacketFormats()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.Instance.configData.packetFormat);
            packetFormatComboBox.Items.Clear();
            string[] files = Directory.GetFiles("PacketFormats");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                packetFormatComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    packetFormatComboBox.SelectedItem = filename;
            }
        }

        void RefreshFilters()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.Instance.configData.filterConfig);
            filtersComboBox.Items.Clear();
            string[] files = Directory.GetFiles("Filters");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                filtersComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    filtersComboBox.SelectedItem = filename;
            }
        }

        void RefreshOtherSettings()
        {
            udpCheckBox.Checked = MainConfig.Instance.configData.sendUDP;
            fillMMFCheckbox.Checked = MainConfig.Instance.configData.fillMMF;
            udpIPTextBox.Text = MainConfig.Instance.configData.udpIP;
            portTextBox.Text = "" + MainConfig.Instance.configData.udpPort;

            if (MainConfig.Instance.configData.copyFormatDestinations != null)
            {
                formatDestinationsBox.Items.Clear();
                foreach (string entry in MainConfig.Instance.configData.copyFormatDestinations)
                    formatDestinationsBox.Items.Add(entry);
            }

            if (!string.IsNullOrEmpty(MainConfig.Instance.configData.packetFormat))
                CMCustomUDPData.formatFilename = MainConfig.Instance.configData.packetFormat;

        }

        void RefreshHotkey()
        {
            hkComboBox.Items.Clear();

            var keys = Enum.GetValues(typeof(Keys));

            foreach(Keys key in keys)
            {
                hkComboBox.Items.Add(key);
                if(key == MainConfig.Instance.configData.hotkey.key)
                {
                    hkComboBox.SelectedItem = key;
                }    
            }

            hkEnabledCheckbox.Checked = MainConfig.Instance.configData.hotkey.enabled;
            hkShiftCheckBox.Checked = MainConfig.Instance.configData.hotkey.shift;
            hkCtrlCheckbox.Checked = MainConfig.Instance.configData.hotkey.ctrl;
            hkAltCheckbox.Checked = MainConfig.Instance.configData.hotkey.alt;
            hkWindowsCheckbox.Checked = MainConfig.Instance.configData.hotkey.windows;
        }



        public void LoadConfig()
        { 
            MainConfig.Instance.Load();

            ignoreConfigChanges = true;
            RefreshConfigs();
            RefreshPacketFormats();
            RefreshFilters();
            RefreshHotkey();
            RefreshOtherSettings();
            ignoreConfigChanges = false;

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
 
            Application.Exit();
        }

        private void Dirt5Button_Click(object sender, EventArgs e)
        {
            if (dirt5UI != null && !dirt5UI.IsDisposed)
            {
                dirt5UI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), dirt5UI);
            }
            dirt5UI = new Dirt5UI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((Dirt5UI)form).ShowDialog()));
            x.Start(dirt5UI);

        }

        private void Filters_Click(object sender, EventArgs e)
        {

            if (filterUI != null && !filterUI.IsDisposed)
            {
                filterUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), filterUI);
            }

            filterUI = new FilterUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => 
            { 
                ((FilterUI)form).ShowDialog();
            }));
            x.Start(filterUI);
        }

        private void udpIPTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.udpIP = udpIPTextBox.Text;
            MainConfig.Instance.Save();
        }

        private void portTextBox_TextChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            int.TryParse(portTextBox.Text, out MainConfig.Instance.configData.udpPort);
            MainConfig.Instance.Save();
        }

        private void udpCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.sendUDP = udpCheckBox.Checked;
            MainConfig.Instance.Save();
        }

        private void fillMMFCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.fillMMF = fillMMFCheckbox.Checked;
            MainConfig.Instance.Save();

        }

        private void destinationFindButton_Click(object sender, EventArgs e)
        {
            try
            {
                MTAOpenFileDialog dialog = new MTAOpenFileDialog();
                dialog.OnComplete = (obj, args) =>
                {
                    string result = (args as MTAOpenFileDialogEventArgs).FileName;

                    MainConfig.Instance.configData.AddFormatDestination(result);
                    MainConfig.Instance.Save();
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
            if(formatDestinationsBox.SelectedItem != null)
            {
                MainConfig.Instance.configData.RemoveFormatDestination((string)formatDestinationsBox.SelectedItem);
                MainConfig.Instance.Save();

                formatDestinationsBox.Items.RemoveAt(formatDestinationsBox.SelectedIndex);
                formatDestinationsBox.SelectedIndex = -1;
            }
        }

        private void formatDestinationsBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AdjustWidthComboBox_DropDown(object sender, EventArgs e)
        {
            var senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            using (Graphics g = senderComboBox.CreateGraphics())
            {
                Font font = senderComboBox.Font;

                int vertScrollBarWidth = (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                        ? SystemInformation.VerticalScrollBarWidth : 0;

                var itemsList = senderComboBox.Items.Cast<object>().Select(item => item.ToString());

                foreach (string s in itemsList)
                {
                    int newWidth = (int)g.MeasureString(s, font).Width + vertScrollBarWidth;

                    if (width < newWidth)
                    {
                        width = newWidth;
                    }
                }
            }
            senderComboBox.DropDownWidth = width;
        }

        private void addConfigButton_Click(object sender, EventArgs e)
        {
            if (configComboBox.SelectedItem == null)
                return;

            string selectedItem = (string)configComboBox.SelectedItem;

            NameChangeForm picker = new NameChangeForm();

            picker.SetNameText(selectedItem);

            picker.okCallback = (s) => 
            {
                if (string.IsNullOrEmpty(s))
                    return;

                if(string.Compare(selectedItem, s) == 0)
                {
                    s += "(Copy)";
                }

                File.Copy("Configs\\" + selectedItem + ".txt", "Configs\\" + s + ".txt", true);

                File.WriteAllText("gtp.txt", "Configs\\" + s + ".txt");

                LoadConfig();
            };

            picker.cancelCallback = () =>
            {
            };

            picker.ShowDialog();

        }

        private void configComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            if (configComboBox.SelectedItem == null)
                return;

            File.WriteAllText("gtp.txt", "Configs\\" + configComboBox.SelectedItem + ".txt");

            LoadConfig();
        }

        private void packetFormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            if (packetFormatComboBox.SelectedItem == null)
                return;

            CMCustomUDPData.formatFilename = MainConfig.Instance.configData.packetFormat = "PacketFormats\\" + (string)packetFormatComboBox.SelectedItem + ".xml";
            MainConfig.Instance.Save();
        }

        private void filtersComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            if (filtersComboBox.SelectedItem == null)
                return;

            MainConfig.Instance.configData.filterConfig = "Filters\\" + (string)filtersComboBox.SelectedItem + ".txt";
            MainConfig.Instance.Save();
        }

        private void hkWindowsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.hotkey.windows = hkWindowsCheckbox.Checked;
            MainConfig.Instance.Save();
        }

        private void hkAltCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.hotkey.alt = hkAltCheckbox.Checked;
            MainConfig.Instance.Save();
        }

        private void hkShiftCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.hotkey.shift = hkShiftCheckBox.Checked;
            MainConfig.Instance.Save();
        }

        private void hkCtrlCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;


            MainConfig.Instance.configData.hotkey.ctrl = hkCtrlCheckbox.Checked;
            MainConfig.Instance.Save();
        }

        private void hkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.hotkey.key = (Keys)hkComboBox.SelectedItem;
            MainConfig.Instance.Save();
        }

        private void hkEnabledCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            MainConfig.Instance.configData.hotkey.enabled = hkEnabledCheckbox.Checked;
            MainConfig.Instance.Save();
        }

        private void wreckfestButton_Click(object sender, EventArgs e)
        {

            if (wreckfestUI != null && !wreckfestUI.IsDisposed)
            {
                wreckfestUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), wreckfestUI);
            }

            wreckfestUI = new WreckfestUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WreckfestUI)form).ShowDialog()));
            x.Start(wreckfestUI);
        }

        private void beamNGButton_Click(object sender, EventArgs e)
        {
            if (beamNGUI != null && !beamNGUI.IsDisposed)
            {
                beamNGUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), beamNGUI);
            }

            beamNGUI = new BeamNGUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((BeamNGUI)form).ShowDialog()));
            x.Start(beamNGUI);
        }

        private void gtavButton_Click(object sender, EventArgs e)
        {
            if (gtavUI != null && !gtavUI.IsDisposed)
            {
                gtavUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), gtavUI);
            }

            gtavUI = new GTAVUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((GTAVUI)form).ShowDialog()));
            x.Start(gtavUI);
        }

        private void dcsButton_Click(object sender, EventArgs e)
        {
            if (dcsUI != null && !dcsUI.IsDisposed)
            {
                dcsUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), dcsUI);
            }

            dcsUI = new DCSUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((DCSUI)form).ShowDialog()));
            x.Start(dcsUI);

        }

        private void mgButton_Click(object sender, EventArgs e)
        {
            if (mgUI != null && !mgUI.IsDisposed)
            {
                mgUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), mgUI);
            }
              

            mgUI = new MonsterGamesUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((MonsterGamesUI)form).ShowDialog()));
            x.Start(mgUI);
        }

        private void WRCButton_Click(object sender, EventArgs e)
        {
            if (wrcUI != null && !wrcUI.IsDisposed)
            {
                wrcUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), wrcUI);
            }

            wrcUI = new WRCUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WRCUI)form).ShowDialog()));
            x.Start(wrcUI);
        }

        private void RBRButton_Click(object sender, EventArgs e)
        {
            if (rbrUI != null && !rbrUI.IsDisposed)
            {
                rbrUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), wrcUI);
            }

            rbrUI = new RBRUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((RBRUI)form).ShowDialog()));
            x.Start(rbrUI);
        }

        private void SquadronsBtn_Click(object sender, EventArgs e)
        {
            if (squadronsUI != null && !squadronsUI.IsDisposed)
            {
                squadronsUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), squadronsUI);
            }

            squadronsUI = new SquadronsUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((SquadronsUI)form).ShowDialog()));
            x.Start(squadronsUI);
        }

        private void il2Btn_Click(object sender, EventArgs e)
        {
            if (il2UI != null && !il2UI.IsDisposed)
            {
                il2UI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), il2UI);
            }

            il2UI = new IL2UI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((IL2UI)form).ShowDialog()));
            x.Start(il2UI);
        }
    }
}
