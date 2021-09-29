namespace GenericTelemetryProvider
{
    partial class GenericTelemetryProvider
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenericTelemetryProvider));
            this.Dirt5Button = new System.Windows.Forms.Button();
            this.Filters = new System.Windows.Forms.Button();
            this.fillMMFCheckbox = new System.Windows.Forms.CheckBox();
            this.udpCheckBox = new System.Windows.Forms.CheckBox();
            this.ipLabel = new System.Windows.Forms.Label();
            this.udpIPTextBox = new System.Windows.Forms.TextBox();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.udpLabel = new System.Windows.Forms.Label();
            this.destinationsLabel = new System.Windows.Forms.Label();
            this.destinationFindButton = new System.Windows.Forms.Button();
            this.formatDestinationsBox = new System.Windows.Forms.ComboBox();
            this.deleteDestinationButton = new System.Windows.Forms.Button();
            this.configComboBox = new System.Windows.Forms.ComboBox();
            this.configLabel = new System.Windows.Forms.Label();
            this.addConfigButton = new System.Windows.Forms.Button();
            this.packetFormatComboBox = new System.Windows.Forms.ComboBox();
            this.packetFormatLabel = new System.Windows.Forms.Label();
            this.filtersComboBox = new System.Windows.Forms.ComboBox();
            this.filterConfigLabel = new System.Windows.Forms.Label();
            this.hkComboBox = new System.Windows.Forms.ComboBox();
            this.hotkeyLabel = new System.Windows.Forms.Label();
            this.hkWindowsCheckbox = new System.Windows.Forms.CheckBox();
            this.hkAltCheckbox = new System.Windows.Forms.CheckBox();
            this.hkShiftCheckBox = new System.Windows.Forms.CheckBox();
            this.hkCtrlCheckbox = new System.Windows.Forms.CheckBox();
            this.hkEnabledCheckbox = new System.Windows.Forms.CheckBox();
            this.wreckfestButton = new System.Windows.Forms.Button();
            this.beamNGButton = new System.Windows.Forms.Button();
            this.gtavButton = new System.Windows.Forms.Button();
            this.dcsButton = new System.Windows.Forms.Button();
            this.mgButton = new System.Windows.Forms.Button();
            this.WRCButton = new System.Windows.Forms.Button();
            this.RBRButton = new System.Windows.Forms.Button();
            this.SquadronsBtn = new System.Windows.Forms.Button();
            this.il2Btn = new System.Windows.Forms.Button();
            this.warplanesWW1Btn = new System.Windows.Forms.Button();
            this.vtolvrBtn = new System.Windows.Forms.Button();
            this.wreckfestExperimentsButton = new System.Windows.Forms.Button();
            this.overloadButton = new System.Windows.Forms.Button();
            this.balsaButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Dirt5Button
            // 
            this.Dirt5Button.Location = new System.Drawing.Point(18, 18);
            this.Dirt5Button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Dirt5Button.Name = "Dirt5Button";
            this.Dirt5Button.Size = new System.Drawing.Size(222, 70);
            this.Dirt5Button.TabIndex = 0;
            this.Dirt5Button.Text = "DIRT5";
            this.Dirt5Button.UseVisualStyleBackColor = true;
            this.Dirt5Button.Click += new System.EventHandler(this.Dirt5Button_Click);
            // 
            // Filters
            // 
            this.Filters.Location = new System.Drawing.Point(513, 633);
            this.Filters.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Filters.Name = "Filters";
            this.Filters.Size = new System.Drawing.Size(279, 68);
            this.Filters.TabIndex = 1;
            this.Filters.Text = "Filters";
            this.Filters.UseVisualStyleBackColor = true;
            this.Filters.Click += new System.EventHandler(this.Filters_Click);
            // 
            // fillMMFCheckbox
            // 
            this.fillMMFCheckbox.AutoSize = true;
            this.fillMMFCheckbox.Location = new System.Drawing.Point(514, 83);
            this.fillMMFCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fillMMFCheckbox.Name = "fillMMFCheckbox";
            this.fillMMFCheckbox.Size = new System.Drawing.Size(94, 24);
            this.fillMMFCheckbox.TabIndex = 3;
            this.fillMMFCheckbox.Text = "Fill MMF";
            this.fillMMFCheckbox.UseVisualStyleBackColor = true;
            this.fillMMFCheckbox.CheckedChanged += new System.EventHandler(this.fillMMFCheckbox_CheckedChanged);
            // 
            // udpCheckBox
            // 
            this.udpCheckBox.AutoSize = true;
            this.udpCheckBox.Location = new System.Drawing.Point(514, 118);
            this.udpCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.udpCheckBox.Name = "udpCheckBox";
            this.udpCheckBox.Size = new System.Drawing.Size(111, 24);
            this.udpCheckBox.TabIndex = 4;
            this.udpCheckBox.Text = "Send UDP";
            this.udpCheckBox.UseVisualStyleBackColor = true;
            this.udpCheckBox.CheckedChanged += new System.EventHandler(this.udpCheckBox_CheckedChanged);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(509, 149);
            this.ipLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(62, 20);
            this.ipLabel.TabIndex = 5;
            this.ipLabel.Text = "UDP IP";
            // 
            // udpIPTextBox
            // 
            this.udpIPTextBox.Location = new System.Drawing.Point(514, 173);
            this.udpIPTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.udpIPTextBox.Name = "udpIPTextBox";
            this.udpIPTextBox.Size = new System.Drawing.Size(186, 26);
            this.udpIPTextBox.TabIndex = 6;
            this.udpIPTextBox.Text = "127.0.0.1";
            this.udpIPTextBox.TextChanged += new System.EventHandler(this.udpIPTextBox_TextChanged);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(714, 173);
            this.portTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(78, 26);
            this.portTextBox.TabIndex = 7;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // udpLabel
            // 
            this.udpLabel.AutoSize = true;
            this.udpLabel.Location = new System.Drawing.Point(710, 149);
            this.udpLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.udpLabel.Name = "udpLabel";
            this.udpLabel.Size = new System.Drawing.Size(38, 20);
            this.udpLabel.TabIndex = 8;
            this.udpLabel.Text = "Port";
            // 
            // destinationsLabel
            // 
            this.destinationsLabel.AutoSize = true;
            this.destinationsLabel.Location = new System.Drawing.Point(509, 305);
            this.destinationsLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.destinationsLabel.Name = "destinationsLabel";
            this.destinationsLabel.Size = new System.Drawing.Size(206, 20);
            this.destinationsLabel.TabIndex = 10;
            this.destinationsLabel.Text = "Packet Format Destinations";
            // 
            // destinationFindButton
            // 
            this.destinationFindButton.Location = new System.Drawing.Point(756, 305);
            this.destinationFindButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.destinationFindButton.Name = "destinationFindButton";
            this.destinationFindButton.Size = new System.Drawing.Size(38, 38);
            this.destinationFindButton.TabIndex = 11;
            this.destinationFindButton.Text = "...";
            this.destinationFindButton.UseVisualStyleBackColor = true;
            this.destinationFindButton.Click += new System.EventHandler(this.destinationFindButton_Click);
            // 
            // formatDestinationsBox
            // 
            this.formatDestinationsBox.FormattingEnabled = true;
            this.formatDestinationsBox.Location = new System.Drawing.Point(514, 329);
            this.formatDestinationsBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.formatDestinationsBox.Name = "formatDestinationsBox";
            this.formatDestinationsBox.Size = new System.Drawing.Size(228, 28);
            this.formatDestinationsBox.TabIndex = 12;
            this.formatDestinationsBox.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.formatDestinationsBox.SelectedIndexChanged += new System.EventHandler(this.formatDestinationsBox_SelectedIndexChanged);
            // 
            // deleteDestinationButton
            // 
            this.deleteDestinationButton.Location = new System.Drawing.Point(758, 353);
            this.deleteDestinationButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.deleteDestinationButton.Name = "deleteDestinationButton";
            this.deleteDestinationButton.Size = new System.Drawing.Size(38, 38);
            this.deleteDestinationButton.TabIndex = 13;
            this.deleteDestinationButton.Text = "-";
            this.deleteDestinationButton.UseVisualStyleBackColor = true;
            this.deleteDestinationButton.Click += new System.EventHandler(this.deleteDestinationButton_Click);
            // 
            // configComboBox
            // 
            this.configComboBox.FormattingEnabled = true;
            this.configComboBox.Location = new System.Drawing.Point(514, 41);
            this.configComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.configComboBox.Name = "configComboBox";
            this.configComboBox.Size = new System.Drawing.Size(234, 28);
            this.configComboBox.TabIndex = 14;
            this.configComboBox.SelectedIndexChanged += new System.EventHandler(this.configComboBox_SelectedIndexChanged);
            // 
            // configLabel
            // 
            this.configLabel.AutoSize = true;
            this.configLabel.Location = new System.Drawing.Point(509, 17);
            this.configLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.configLabel.Name = "configLabel";
            this.configLabel.Size = new System.Drawing.Size(104, 20);
            this.configLabel.TabIndex = 15;
            this.configLabel.Text = "Select Config";
            // 
            // addConfigButton
            // 
            this.addConfigButton.Location = new System.Drawing.Point(758, 37);
            this.addConfigButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.addConfigButton.Name = "addConfigButton";
            this.addConfigButton.Size = new System.Drawing.Size(38, 38);
            this.addConfigButton.TabIndex = 16;
            this.addConfigButton.Text = "+";
            this.addConfigButton.UseVisualStyleBackColor = true;
            this.addConfigButton.Click += new System.EventHandler(this.addConfigButton_Click);
            // 
            // packetFormatComboBox
            // 
            this.packetFormatComboBox.FormattingEnabled = true;
            this.packetFormatComboBox.Location = new System.Drawing.Point(514, 249);
            this.packetFormatComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.packetFormatComboBox.Name = "packetFormatComboBox";
            this.packetFormatComboBox.Size = new System.Drawing.Size(274, 28);
            this.packetFormatComboBox.TabIndex = 17;
            this.packetFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.packetFormatComboBox_SelectedIndexChanged);
            // 
            // packetFormatLabel
            // 
            this.packetFormatLabel.AutoSize = true;
            this.packetFormatLabel.Location = new System.Drawing.Point(509, 225);
            this.packetFormatLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.packetFormatLabel.Name = "packetFormatLabel";
            this.packetFormatLabel.Size = new System.Drawing.Size(113, 20);
            this.packetFormatLabel.TabIndex = 18;
            this.packetFormatLabel.Text = "Packet Format";
            // 
            // filtersComboBox
            // 
            this.filtersComboBox.FormattingEnabled = true;
            this.filtersComboBox.Location = new System.Drawing.Point(514, 418);
            this.filtersComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.filtersComboBox.Name = "filtersComboBox";
            this.filtersComboBox.Size = new System.Drawing.Size(280, 28);
            this.filtersComboBox.TabIndex = 19;
            this.filtersComboBox.SelectedIndexChanged += new System.EventHandler(this.filtersComboBox_SelectedIndexChanged);
            // 
            // filterConfigLabel
            // 
            this.filterConfigLabel.AutoSize = true;
            this.filterConfigLabel.Location = new System.Drawing.Point(509, 393);
            this.filterConfigLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.filterConfigLabel.Name = "filterConfigLabel";
            this.filterConfigLabel.Size = new System.Drawing.Size(94, 20);
            this.filterConfigLabel.TabIndex = 20;
            this.filterConfigLabel.Text = "Filter Config";
            // 
            // hkComboBox
            // 
            this.hkComboBox.FormattingEnabled = true;
            this.hkComboBox.Location = new System.Drawing.Point(514, 491);
            this.hkComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkComboBox.Name = "hkComboBox";
            this.hkComboBox.Size = new System.Drawing.Size(168, 28);
            this.hkComboBox.TabIndex = 21;
            this.hkComboBox.SelectedIndexChanged += new System.EventHandler(this.hkComboBox_SelectedIndexChanged);
            // 
            // hotkeyLabel
            // 
            this.hotkeyLabel.AutoSize = true;
            this.hotkeyLabel.Location = new System.Drawing.Point(509, 466);
            this.hotkeyLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hotkeyLabel.Name = "hotkeyLabel";
            this.hotkeyLabel.Size = new System.Drawing.Size(117, 20);
            this.hotkeyLabel.TabIndex = 22;
            this.hotkeyLabel.Text = "Toggle Hot Key";
            // 
            // hkWindowsCheckbox
            // 
            this.hkWindowsCheckbox.AutoSize = true;
            this.hkWindowsCheckbox.Location = new System.Drawing.Point(514, 533);
            this.hkWindowsCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkWindowsCheckbox.Name = "hkWindowsCheckbox";
            this.hkWindowsCheckbox.Size = new System.Drawing.Size(112, 24);
            this.hkWindowsCheckbox.TabIndex = 23;
            this.hkWindowsCheckbox.Text = "+ Windows";
            this.hkWindowsCheckbox.UseVisualStyleBackColor = true;
            this.hkWindowsCheckbox.CheckedChanged += new System.EventHandler(this.hkWindowsCheckbox_CheckedChanged);
            // 
            // hkAltCheckbox
            // 
            this.hkAltCheckbox.AutoSize = true;
            this.hkAltCheckbox.Location = new System.Drawing.Point(514, 571);
            this.hkAltCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkAltCheckbox.Name = "hkAltCheckbox";
            this.hkAltCheckbox.Size = new System.Drawing.Size(67, 24);
            this.hkAltCheckbox.TabIndex = 24;
            this.hkAltCheckbox.Text = "+ Alt";
            this.hkAltCheckbox.UseVisualStyleBackColor = true;
            this.hkAltCheckbox.CheckedChanged += new System.EventHandler(this.hkAltCheckbox_CheckedChanged);
            // 
            // hkShiftCheckBox
            // 
            this.hkShiftCheckBox.AutoSize = true;
            this.hkShiftCheckBox.Location = new System.Drawing.Point(644, 533);
            this.hkShiftCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkShiftCheckBox.Name = "hkShiftCheckBox";
            this.hkShiftCheckBox.Size = new System.Drawing.Size(81, 24);
            this.hkShiftCheckBox.TabIndex = 25;
            this.hkShiftCheckBox.Text = "+ Shift";
            this.hkShiftCheckBox.UseVisualStyleBackColor = true;
            this.hkShiftCheckBox.CheckedChanged += new System.EventHandler(this.hkShiftCheckBox_CheckedChanged);
            // 
            // hkCtrlCheckbox
            // 
            this.hkCtrlCheckbox.AutoSize = true;
            this.hkCtrlCheckbox.Location = new System.Drawing.Point(644, 571);
            this.hkCtrlCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkCtrlCheckbox.Name = "hkCtrlCheckbox";
            this.hkCtrlCheckbox.Size = new System.Drawing.Size(72, 24);
            this.hkCtrlCheckbox.TabIndex = 26;
            this.hkCtrlCheckbox.Text = "+ Ctrl";
            this.hkCtrlCheckbox.UseVisualStyleBackColor = true;
            this.hkCtrlCheckbox.CheckedChanged += new System.EventHandler(this.hkCtrlCheckbox_CheckedChanged);
            // 
            // hkEnabledCheckbox
            // 
            this.hkEnabledCheckbox.AutoSize = true;
            this.hkEnabledCheckbox.Location = new System.Drawing.Point(698, 493);
            this.hkEnabledCheckbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.hkEnabledCheckbox.Name = "hkEnabledCheckbox";
            this.hkEnabledCheckbox.Size = new System.Drawing.Size(94, 24);
            this.hkEnabledCheckbox.TabIndex = 27;
            this.hkEnabledCheckbox.Text = "Enabled";
            this.hkEnabledCheckbox.UseVisualStyleBackColor = true;
            this.hkEnabledCheckbox.CheckedChanged += new System.EventHandler(this.hkEnabledCheckbox_CheckedChanged);
            // 
            // wreckfestButton
            // 
            this.wreckfestButton.Location = new System.Drawing.Point(20, 97);
            this.wreckfestButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.wreckfestButton.Name = "wreckfestButton";
            this.wreckfestButton.Size = new System.Drawing.Size(222, 70);
            this.wreckfestButton.TabIndex = 28;
            this.wreckfestButton.Text = "Wreckfest";
            this.wreckfestButton.UseVisualStyleBackColor = true;
            this.wreckfestButton.Click += new System.EventHandler(this.wreckfestButton_Click);
            // 
            // beamNGButton
            // 
            this.beamNGButton.Location = new System.Drawing.Point(20, 175);
            this.beamNGButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.beamNGButton.Name = "beamNGButton";
            this.beamNGButton.Size = new System.Drawing.Size(222, 70);
            this.beamNGButton.TabIndex = 29;
            this.beamNGButton.Text = "BeamNG Drive";
            this.beamNGButton.UseVisualStyleBackColor = true;
            this.beamNGButton.Click += new System.EventHandler(this.beamNGButton_Click);
            // 
            // gtavButton
            // 
            this.gtavButton.Location = new System.Drawing.Point(20, 255);
            this.gtavButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gtavButton.Name = "gtavButton";
            this.gtavButton.Size = new System.Drawing.Size(222, 70);
            this.gtavButton.TabIndex = 30;
            this.gtavButton.Text = "GTA V";
            this.gtavButton.UseVisualStyleBackColor = true;
            this.gtavButton.Click += new System.EventHandler(this.gtavButton_Click);
            // 
            // dcsButton
            // 
            this.dcsButton.Location = new System.Drawing.Point(20, 334);
            this.dcsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dcsButton.Name = "dcsButton";
            this.dcsButton.Size = new System.Drawing.Size(222, 70);
            this.dcsButton.TabIndex = 31;
            this.dcsButton.Text = "Digital Combat Simulator";
            this.dcsButton.UseVisualStyleBackColor = true;
            this.dcsButton.Click += new System.EventHandler(this.dcsButton_Click);
            // 
            // mgButton
            // 
            this.mgButton.Location = new System.Drawing.Point(249, 255);
            this.mgButton.Margin = new System.Windows.Forms.Padding(2);
            this.mgButton.Name = "mgButton";
            this.mgButton.Size = new System.Drawing.Size(222, 70);
            this.mgButton.TabIndex = 32;
            this.mgButton.Text = "Nascar Heat 4/5\r\nAll American Racing\r\nSprint Car Racing";
            this.mgButton.UseVisualStyleBackColor = true;
            this.mgButton.Click += new System.EventHandler(this.mgButton_Click);
            // 
            // WRCButton
            // 
            this.WRCButton.Location = new System.Drawing.Point(249, 175);
            this.WRCButton.Name = "WRCButton";
            this.WRCButton.Size = new System.Drawing.Size(222, 70);
            this.WRCButton.TabIndex = 33;
            this.WRCButton.Text = "WRC 7/8/9";
            this.WRCButton.UseVisualStyleBackColor = true;
            this.WRCButton.Click += new System.EventHandler(this.WRCButton_Click);
            // 
            // RBRButton
            // 
            this.RBRButton.Location = new System.Drawing.Point(249, 97);
            this.RBRButton.Name = "RBRButton";
            this.RBRButton.Size = new System.Drawing.Size(222, 70);
            this.RBRButton.TabIndex = 34;
            this.RBRButton.Text = "Richard Burns Rally\r\nNGP6";
            this.RBRButton.UseVisualStyleBackColor = true;
            this.RBRButton.Click += new System.EventHandler(this.RBRButton_Click);
            // 
            // SquadronsBtn
            // 
            this.SquadronsBtn.Location = new System.Drawing.Point(249, 18);
            this.SquadronsBtn.Name = "SquadronsBtn";
            this.SquadronsBtn.Size = new System.Drawing.Size(222, 70);
            this.SquadronsBtn.TabIndex = 35;
            this.SquadronsBtn.Text = "STAR WARS Squadrons";
            this.SquadronsBtn.UseVisualStyleBackColor = true;
            this.SquadronsBtn.Click += new System.EventHandler(this.SquadronsBtn_Click);
            // 
            // il2Btn
            // 
            this.il2Btn.Location = new System.Drawing.Point(249, 334);
            this.il2Btn.Name = "il2Btn";
            this.il2Btn.Size = new System.Drawing.Size(222, 70);
            this.il2Btn.TabIndex = 36;
            this.il2Btn.Text = "IL-2 Sturmovik";
            this.il2Btn.UseVisualStyleBackColor = true;
            this.il2Btn.Click += new System.EventHandler(this.il2Btn_Click);
            // 
            // warplanesWW1Btn
            // 
            this.warplanesWW1Btn.Location = new System.Drawing.Point(20, 412);
            this.warplanesWW1Btn.Name = "warplanesWW1Btn";
            this.warplanesWW1Btn.Size = new System.Drawing.Size(222, 70);
            this.warplanesWW1Btn.TabIndex = 37;
            this.warplanesWW1Btn.Text = "Warplanes WW1";
            this.warplanesWW1Btn.UseVisualStyleBackColor = true;
            this.warplanesWW1Btn.Click += new System.EventHandler(this.warplanesWW1Btn_Click);
            // 
            // vtolvrBtn
            // 
            this.vtolvrBtn.Location = new System.Drawing.Point(248, 412);
            this.vtolvrBtn.Name = "vtolvrBtn";
            this.vtolvrBtn.Size = new System.Drawing.Size(222, 70);
            this.vtolvrBtn.TabIndex = 38;
            this.vtolvrBtn.Text = "VTOL VR";
            this.vtolvrBtn.UseVisualStyleBackColor = true;
            this.vtolvrBtn.Click += new System.EventHandler(this.vtolvrBtn_Click);
            // 
            // wreckfestExperimentsButton
            // 
            this.wreckfestExperimentsButton.Location = new System.Drawing.Point(20, 642);
            this.wreckfestExperimentsButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.wreckfestExperimentsButton.Name = "wreckfestExperimentsButton";
            this.wreckfestExperimentsButton.Size = new System.Drawing.Size(222, 70);
            this.wreckfestExperimentsButton.TabIndex = 39;
            this.wreckfestExperimentsButton.Text = "Wreckfest\r\nExperimental";
            this.wreckfestExperimentsButton.UseVisualStyleBackColor = true;
            this.wreckfestExperimentsButton.Click += new System.EventHandler(this.wreckfestExperimentsButton_Click);
            // 
            // overloadButton
            // 
            this.overloadButton.Location = new System.Drawing.Point(20, 491);
            this.overloadButton.Margin = new System.Windows.Forms.Padding(2);
            this.overloadButton.Name = "overloadButton";
            this.overloadButton.Size = new System.Drawing.Size(222, 70);
            this.overloadButton.TabIndex = 40;
            this.overloadButton.Text = "Overload";
            this.overloadButton.UseVisualStyleBackColor = true;
            this.overloadButton.Click += new System.EventHandler(this.overloadButton_Click);
            // 
            // balsaButton
            // 
            this.balsaButton.Location = new System.Drawing.Point(246, 491);
            this.balsaButton.Margin = new System.Windows.Forms.Padding(2);
            this.balsaButton.Name = "balsaButton";
            this.balsaButton.Size = new System.Drawing.Size(222, 70);
            this.balsaButton.TabIndex = 41;
            this.balsaButton.Text = "Balsa";
            this.balsaButton.UseVisualStyleBackColor = true;
            this.balsaButton.Click += new System.EventHandler(this.balsaButton_Click);
            // 
            // GenericTelemetryProvider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 726);
            this.Controls.Add(this.balsaButton);
            this.Controls.Add(this.overloadButton);
            this.Controls.Add(this.wreckfestExperimentsButton);
            this.Controls.Add(this.vtolvrBtn);
            this.Controls.Add(this.warplanesWW1Btn);
            this.Controls.Add(this.il2Btn);
            this.Controls.Add(this.SquadronsBtn);
            this.Controls.Add(this.RBRButton);
            this.Controls.Add(this.WRCButton);
            this.Controls.Add(this.mgButton);
            this.Controls.Add(this.dcsButton);
            this.Controls.Add(this.gtavButton);
            this.Controls.Add(this.beamNGButton);
            this.Controls.Add(this.wreckfestButton);
            this.Controls.Add(this.hkEnabledCheckbox);
            this.Controls.Add(this.hkCtrlCheckbox);
            this.Controls.Add(this.hkShiftCheckBox);
            this.Controls.Add(this.hkAltCheckbox);
            this.Controls.Add(this.hkWindowsCheckbox);
            this.Controls.Add(this.hotkeyLabel);
            this.Controls.Add(this.hkComboBox);
            this.Controls.Add(this.filterConfigLabel);
            this.Controls.Add(this.filtersComboBox);
            this.Controls.Add(this.packetFormatLabel);
            this.Controls.Add(this.packetFormatComboBox);
            this.Controls.Add(this.addConfigButton);
            this.Controls.Add(this.configLabel);
            this.Controls.Add(this.configComboBox);
            this.Controls.Add(this.deleteDestinationButton);
            this.Controls.Add(this.formatDestinationsBox);
            this.Controls.Add(this.destinationFindButton);
            this.Controls.Add(this.destinationsLabel);
            this.Controls.Add(this.udpLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.udpIPTextBox);
            this.Controls.Add(this.ipLabel);
            this.Controls.Add(this.udpCheckBox);
            this.Controls.Add(this.fillMMFCheckbox);
            this.Controls.Add(this.Filters);
            this.Controls.Add(this.Dirt5Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "GenericTelemetryProvider";
            this.Text = "SpaceMonkey";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Dirt5Button;
        private System.Windows.Forms.Button Filters;
        private System.Windows.Forms.CheckBox fillMMFCheckbox;
        private System.Windows.Forms.CheckBox udpCheckBox;
        private System.Windows.Forms.Label ipLabel;
        private System.Windows.Forms.TextBox udpIPTextBox;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label udpLabel;
        private System.Windows.Forms.Label destinationsLabel;
        private System.Windows.Forms.Button destinationFindButton;
        private System.Windows.Forms.ComboBox formatDestinationsBox;
        private System.Windows.Forms.Button deleteDestinationButton;
        private System.Windows.Forms.ComboBox configComboBox;
        private System.Windows.Forms.Label configLabel;
        private System.Windows.Forms.Button addConfigButton;
        private System.Windows.Forms.ComboBox packetFormatComboBox;
        private System.Windows.Forms.Label packetFormatLabel;
        private System.Windows.Forms.ComboBox filtersComboBox;
        private System.Windows.Forms.Label filterConfigLabel;
        private System.Windows.Forms.ComboBox hkComboBox;
        private System.Windows.Forms.Label hotkeyLabel;
        private System.Windows.Forms.CheckBox hkWindowsCheckbox;
        private System.Windows.Forms.CheckBox hkAltCheckbox;
        private System.Windows.Forms.CheckBox hkShiftCheckBox;
        private System.Windows.Forms.CheckBox hkCtrlCheckbox;
        private System.Windows.Forms.CheckBox hkEnabledCheckbox;
        private System.Windows.Forms.Button wreckfestButton;
        private System.Windows.Forms.Button beamNGButton;
        private System.Windows.Forms.Button gtavButton;
        private System.Windows.Forms.Button dcsButton;
        private System.Windows.Forms.Button mgButton;
        private System.Windows.Forms.Button WRCButton;
        private System.Windows.Forms.Button RBRButton;
        private System.Windows.Forms.Button SquadronsBtn;
        private System.Windows.Forms.Button il2Btn;
        private System.Windows.Forms.Button warplanesWW1Btn;
        private System.Windows.Forms.Button vtolvrBtn;
        private System.Windows.Forms.Button wreckfestExperimentsButton;
        private System.Windows.Forms.Button overloadButton;
        private System.Windows.Forms.Button balsaButton;
    }
}

