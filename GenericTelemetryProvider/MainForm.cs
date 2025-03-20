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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Reflection;
using SMHaptics;

namespace GenericTelemetryProvider
{

    public partial class MainForm : Form
    {

        public Dirt5UI dirt5UI;
        WreckfestUI wreckfestUI;
        WreckfestUIExperiments wreckfestUIExperiments;
        BeamNGUI beamNGUI;
        GTAVUI gtavUI;
        DCSUI dcsUI;
        MonsterGamesUI mgUI;
        WRCUI wrcUI;
        RBRUI rbrUI;
        SquadronsUI squadronsUI;
        IL2UI il2UI;
        WarplanesWW1UI warplanesWW1UI;
        VTOLVRUI vtolVRUI;
        OverloadUI overloadUI;
        OpenMotionUI openMotionUI;
        WRCGenUI wrcGenUI;
        TinyCombatArenaUI tcaUI;
        EAWRCUI eaWRCUI;
        FilterUI filterUI;
        OutputUI outputUI;
        HapticsUI hapticsUI;
        SMTUI smtUI;
        UEVRUI uevrUI;
        public static MainForm Instance;
        public string versionString = "v1.2.0";

        bool ignoreConfigChanges = false;
        public bool integrated = false;

        const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDefaultDllDirectories(uint DirectoryFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int AddDllDirectory(string NewDirectory);

        Action<bool> loadCallback;

        public MainForm(Action<bool> _loadCallback = null)
        {
            loadCallback = _loadCallback;
            Instance = this;


            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
            Application.ThreadException += GlobalThreadExceptionHandler;

            Utils.TimeBeginPeriod(1);

            this.Text = "SpaceMonkey " + versionString;

            LoadConfig();

            loadCallback?.Invoke(true);

            InitHaptics();
        }

        // Handles exceptions that occur in non-UI threads (e.g., background workers)
        private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            if (ex is DllNotFoundException)
            {
                Console.WriteLine("DLL not found: " + ex.Message);
                // Handle the exception, log it, or display an error message
            }
            else
            {
                Console.WriteLine("An unhandled exception occurred: " + ex.Message);
            }
        }

        // Handles exceptions that occur in the main UI thread
        private static void GlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            if (ex is DllNotFoundException)
            {
                MessageBox.Show("DLL not found: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("An unhandled exception occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void RefreshConfigs()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.saveFilename);
            configComboBox.Items.Clear();
            string[] files = Directory.GetFiles(MainConfig.installPath + "Configs");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                configComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    configComboBox.SelectedItem = filename;
            }
        }


        void RefreshFilters()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.Instance.configData.filterConfig);
            filtersComboBox.Items.Clear();
            string[] files = Directory.GetFiles(MainConfig.installPath + "Filters");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                filtersComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    filtersComboBox.SelectedItem = filename;
            }
        }


        void RefreshOutputs()
        {
            string selectedItem = Path.GetFileNameWithoutExtension(MainConfig.Instance.configData.outputConfig);
            outputsComboBox.Items.Clear();
            string[] files = Directory.GetFiles(MainConfig.installPath + "Outputs");
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file);
                outputsComboBox.Items.Add(filename);

                if (string.Compare(filename, selectedItem) == 0)
                    outputsComboBox.SelectedItem = filename;
            }
        }

        void RefreshOtherSettings()
        {
            


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

            AssemblyResolveSetup();

            ignoreConfigChanges = true;
            RefreshConfigs();
            RefreshFilters();
            RefreshOutputs();
            RefreshHotkey();
            RefreshOtherSettings();
            ignoreConfigChanges = false;
        }

        void AssemblyResolveSetup()
        {
            try
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;

                AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
                {
                    string assemblyName = args.Name.Split(',')[0];

                    Assembly ass = Assembly.LoadFrom(MainConfig.installPath + assemblyName + ".dll");

                    return ass;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load assembly: " + e.Message);
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.TimeEndPeriod(1);

            //for standalone only
            if (!integrated)
            {
                try
                {
                    Application.Exit();
                }
                catch
                {

                }
            }
        }

        private void Dirt5Button_Click(object sender, EventArgs e)
        {
            if (dirt5UI != null && !dirt5UI.IsDisposed)
            {
                dirt5UI.Dispose();
                dirt5UI = null;
            }
            dirt5UI = new Dirt5UI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((Dirt5UI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(dirt5UI);

        }

        private void Filters_Click(object sender, EventArgs e)
        {

            if (filterUI != null && !filterUI.IsDisposed)
            {
                filterUI.Dispose();
                filterUI = null;
            }

            filterUI = new FilterUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => 
            { 
                ((FilterUI)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(filterUI);
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

                File.Copy(MainConfig.installPath + "Configs\\" + selectedItem + ".txt", MainConfig.installPath + "Configs\\" + s + ".txt", true);

                File.WriteAllText(MainConfig.installPath + "gtp.txt", "Configs\\" + s + ".txt");

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

            File.WriteAllText(MainConfig.installPath + "gtp.txt", "Configs\\" + configComboBox.SelectedItem + ".txt");

            LoadConfig();
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
                wreckfestUI.Dispose();
                wreckfestUI = null;
            }

            wreckfestUI = new WreckfestUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WreckfestUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(wreckfestUI);
        }

        private void wreckfestExperimentsButton_Click(object sender, EventArgs e)
        {
            return;
            if (wreckfestUIExperiments != null && !wreckfestUIExperiments.IsDisposed)
            {
                wreckfestUIExperiments.Dispose();
                wreckfestUIExperiments = null;
            }

            wreckfestUIExperiments = new WreckfestUIExperiments();

            wreckfestUIExperiments.ShowDialog();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WreckfestUIExperiments)form).ThreadStart()));

            x.IsBackground = true;
            x.Start(wreckfestUI);
        }

        private void beamNGButton_Click(object sender, EventArgs e)
        {
            if (beamNGUI != null && !beamNGUI.IsDisposed)
            {
                beamNGUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), beamNGUI);
            }

            beamNGUI = new BeamNGUI();
            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((BeamNGUI)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(beamNGUI);

        }

        private void gtavButton_Click(object sender, EventArgs e)
        {
            if (gtavUI != null && !gtavUI.IsDisposed)
            {
                gtavUI.Dispose();
                gtavUI = null;
            }

            gtavUI = new GTAVUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((GTAVUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(gtavUI);
        }

        private void dcsButton_Click(object sender, EventArgs e)
        {
            if (dcsUI != null && !dcsUI.IsDisposed)
            {
                dcsUI.Dispose();
                dcsUI = null;
            }

            dcsUI = new DCSUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((DCSUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(dcsUI);

        }

        private void mgButton_Click(object sender, EventArgs e)
        {
            if (mgUI != null && !mgUI.IsDisposed)
            {
                mgUI.Dispose();
                mgUI = null;
            }
              

            mgUI = new MonsterGamesUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((MonsterGamesUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(mgUI);
        }

        private void WRCButton_Click(object sender, EventArgs e)
        {
            if (wrcUI != null && !wrcUI.IsDisposed)
            {
                wrcUI.Dispose();
                wrcUI = null;
            }

            wrcUI = new WRCUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WRCUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(wrcUI);
        }

        private void RBRButton_Click(object sender, EventArgs e)
        {
            if (rbrUI != null && !rbrUI.IsDisposed)
            {
                rbrUI.Dispose();
                rbrUI = null;
            }

            rbrUI = new RBRUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((RBRUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(rbrUI);
        }

        private void SquadronsBtn_Click(object sender, EventArgs e)
        {
            if (squadronsUI != null && !squadronsUI.IsDisposed)
            {
                squadronsUI.Dispose();
                squadronsUI = null;
            }

            squadronsUI = new SquadronsUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((SquadronsUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(squadronsUI);
        }

        private void il2Btn_Click(object sender, EventArgs e)
        {
            if (il2UI != null && !il2UI.IsDisposed)
            {
                il2UI.Dispose();
                il2UI = null;
            }

            il2UI = new IL2UI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((IL2UI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(il2UI);
        }

        private void warplanesWW1Btn_Click(object sender, EventArgs e)
        {
            if (warplanesWW1UI != null && !warplanesWW1UI.IsDisposed)
            {
                warplanesWW1UI.Dispose();
                warplanesWW1UI = null;
            }

            warplanesWW1UI = new WarplanesWW1UI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WarplanesWW1UI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(warplanesWW1UI);
        }

        private void vtolvrBtn_Click(object sender, EventArgs e)
        {
            if (vtolVRUI != null && !vtolVRUI.IsDisposed)
            {
                vtolVRUI.Dispose();
                vtolVRUI = null;
            }

            vtolVRUI = new VTOLVRUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((VTOLVRUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(vtolVRUI);
        }

        private void overloadButton_Click(object sender, EventArgs e)
        {
            if (overloadUI != null && !overloadUI.IsDisposed)
            {
                overloadUI.Dispose();
                overloadUI = null;
            }

            overloadUI = new OverloadUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((OverloadUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(overloadUI);
        }

        private void openMotionBtn_Click(object sender, EventArgs e)
        {
            if (openMotionUI != null && !openMotionUI.IsDisposed)
            {
                openMotionUI.Dispose();
                openMotionUI = null;
            }

            openMotionUI = new OpenMotionUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((OpenMotionUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(openMotionUI);
        }

        private void wrcGenBtn_Click(object sender, EventArgs e)
        {
            if (wrcGenUI != null && !wrcGenUI.IsDisposed)
            {
                wrcGenUI.Dispose();
                wrcGenUI = null;
            }

            wrcGenUI = new WRCGenUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((WRCGenUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(wrcGenUI);
        }

        private void tinyCombatArenaButton_Click(object sender, EventArgs e)
        {
            if (tcaUI != null && !tcaUI.IsDisposed)
            {
                tcaUI.Dispose();
                tcaUI = null;
            }

            tcaUI = new TinyCombatArenaUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((TinyCombatArenaUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(tcaUI);
        }

        private void EAWRCBtn_Click(object sender, EventArgs e)
        {
            if (eaWRCUI != null && !eaWRCUI.IsDisposed)
            {
                eaWRCUI.BeginInvoke(new Action<Form>((s) => { s.Close(); }), eaWRCUI);
            }

            eaWRCUI = new EAWRCUI();
            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((EAWRCUI)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(eaWRCUI);
        }

        private void OutputsBtn_Click(object sender, EventArgs e)
        {
            if (outputUI != null && !outputUI.IsDisposed)
            {
                outputUI.Dispose();
                outputUI = null;
            }

            outputUI = new OutputUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((OutputUI)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(outputUI);
        }

        private void outputsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreConfigChanges)
                return;

            if (outputsComboBox.SelectedItem == null)
                return;

            MainConfig.Instance.configData.outputConfig = "Outputs\\" + (string)outputsComboBox.SelectedItem + ".txt";
            MainConfig.Instance.Save();
        }

        public void RegisterTelemetryCallback(Action<CMCustomUDPData> callback)
        {
            integrated = true;

            TelemetryOutputCallback callbackOutput = (TelemetryOutputCallback)OutputModule.Instance.AddOutput(OutputModule.OutputType.Callback, false);

            if(callbackOutput != null)
            {
                callbackOutput.RegisterCallback(callback);
            }
        }

        public void InitHaptics()
        {
            SMHapticsManager.instance.Init(MainConfig.installPath);
            SMHapticsManager.instance.InitFromConfig(MainConfig.Instance.configData.hapticsConfig);

            RegisterTelemetryCallback(SMHapticsManager.instance.Input);

            //SMHEngineEffectConfig engineConfig = new SMHEngineEffectConfig();

            //engineConfig.enabled = true;
            //engineConfig.gain = 1.0;
            //engineConfig.outputChannelIndex = 1;
            //engineConfig.outputDeviceModuleName = "{0.0.0.00000000}.{e0f80a10-91f6-46ee-878f-477f7c3aa586}";
            //engineConfig.waveform = 3;
            //engineConfig.id = "SMHaptics.SMHEngineEffect";
            //engineConfig.minFrequency = 10;
            //engineConfig.maxFrequency = 80;

            //SMHapticsManager.instance.CreateEffect(engineConfig);

            //SMHOutputDevice outputDevice = SMHOutputManager.instance.GetDeviceByModuleName(engineConfig.outputDeviceModuleName);
            //if(outputDevice != null)
            //{
            //    outputDevice.Enable(true);
            //}

        }

        private void HapticsBtn_Click(object sender, EventArgs e)
        {
            if (hapticsUI != null && !hapticsUI.IsDisposed)
            {
                hapticsUI.Dispose();
                hapticsUI = null;
            }

            hapticsUI = new HapticsUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) =>
            {
                ((HapticsUI)form).ShowDialog();
            }));
            x.IsBackground = true;
            x.Start(hapticsUI);
        }

        private void UEVRBtn_Click(object sender, EventArgs e)
        {
            if (smtUI != null && !smtUI.IsDisposed)
            {
                smtUI.Dispose();
                smtUI = null;
            }

            smtUI = new SMTUI();

            Thread x = new Thread(new ParameterizedThreadStart((form) => ((SMTUI)form).ShowDialog()));
            x.IsBackground = true;
            x.Start(smtUI);


            if (uevrUI != null && !uevrUI.IsDisposed)
            {
                uevrUI.Dispose();
                uevrUI = null;
            }

            uevrUI = new UEVRUI();

            Thread y = new Thread(new ParameterizedThreadStart((form) => ((UEVRUI)form).ShowDialog()));
            y.IsBackground = true;
            y.Start(uevrUI);


        }

    }
}
