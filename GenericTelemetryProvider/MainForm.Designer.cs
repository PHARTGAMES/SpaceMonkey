namespace GenericTelemetryProvider
{
    partial class MainForm
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
            this.SuspendLayout();
            // 
            // Dirt5Button
            // 
            this.Dirt5Button.Location = new System.Drawing.Point(12, 12);
            this.Dirt5Button.Name = "Dirt5Button";
            this.Dirt5Button.Size = new System.Drawing.Size(149, 44);
            this.Dirt5Button.TabIndex = 0;
            this.Dirt5Button.Text = "DIRT5";
            this.Dirt5Button.UseVisualStyleBackColor = true;
            this.Dirt5Button.Click += new System.EventHandler(this.Dirt5Button_Click);
            // 
            // Filters
            // 
            this.Filters.Location = new System.Drawing.Point(167, 12);
            this.Filters.Name = "Filters";
            this.Filters.Size = new System.Drawing.Size(159, 44);
            this.Filters.TabIndex = 1;
            this.Filters.Text = "Filters";
            this.Filters.UseVisualStyleBackColor = true;
            this.Filters.Click += new System.EventHandler(this.Filters_Click);
            // 
            // fillMMFCheckbox
            // 
            this.fillMMFCheckbox.AutoSize = true;
            this.fillMMFCheckbox.Location = new System.Drawing.Point(469, 52);
            this.fillMMFCheckbox.Name = "fillMMFCheckbox";
            this.fillMMFCheckbox.Size = new System.Drawing.Size(65, 17);
            this.fillMMFCheckbox.TabIndex = 3;
            this.fillMMFCheckbox.Text = "Fill MMF";
            this.fillMMFCheckbox.UseVisualStyleBackColor = true;
            this.fillMMFCheckbox.CheckedChanged += new System.EventHandler(this.fillMMFCheckbox_CheckedChanged);
            // 
            // udpCheckBox
            // 
            this.udpCheckBox.AutoSize = true;
            this.udpCheckBox.Location = new System.Drawing.Point(469, 75);
            this.udpCheckBox.Name = "udpCheckBox";
            this.udpCheckBox.Size = new System.Drawing.Size(77, 17);
            this.udpCheckBox.TabIndex = 4;
            this.udpCheckBox.Text = "Send UDP";
            this.udpCheckBox.UseVisualStyleBackColor = true;
            this.udpCheckBox.CheckedChanged += new System.EventHandler(this.udpCheckBox_CheckedChanged);
            // 
            // ipLabel
            // 
            this.ipLabel.AutoSize = true;
            this.ipLabel.Location = new System.Drawing.Point(466, 95);
            this.ipLabel.Name = "ipLabel";
            this.ipLabel.Size = new System.Drawing.Size(43, 13);
            this.ipLabel.TabIndex = 5;
            this.ipLabel.Text = "UDP IP";
            // 
            // udpIPTextBox
            // 
            this.udpIPTextBox.Location = new System.Drawing.Point(469, 111);
            this.udpIPTextBox.Name = "udpIPTextBox";
            this.udpIPTextBox.Size = new System.Drawing.Size(125, 20);
            this.udpIPTextBox.TabIndex = 6;
            this.udpIPTextBox.Text = "127.0.0.1";
            this.udpIPTextBox.TextChanged += new System.EventHandler(this.udpIPTextBox_TextChanged);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(603, 111);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(53, 20);
            this.portTextBox.TabIndex = 7;
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // udpLabel
            // 
            this.udpLabel.AutoSize = true;
            this.udpLabel.Location = new System.Drawing.Point(600, 95);
            this.udpLabel.Name = "udpLabel";
            this.udpLabel.Size = new System.Drawing.Size(26, 13);
            this.udpLabel.TabIndex = 8;
            this.udpLabel.Text = "Port";
            // 
            // destinationsLabel
            // 
            this.destinationsLabel.AutoSize = true;
            this.destinationsLabel.Location = new System.Drawing.Point(466, 196);
            this.destinationsLabel.Name = "destinationsLabel";
            this.destinationsLabel.Size = new System.Drawing.Size(137, 13);
            this.destinationsLabel.TabIndex = 10;
            this.destinationsLabel.Text = "Packet Format Destinations";
            // 
            // destinationFindButton
            // 
            this.destinationFindButton.Location = new System.Drawing.Point(631, 196);
            this.destinationFindButton.Name = "destinationFindButton";
            this.destinationFindButton.Size = new System.Drawing.Size(25, 25);
            this.destinationFindButton.TabIndex = 11;
            this.destinationFindButton.Text = "...";
            this.destinationFindButton.UseVisualStyleBackColor = true;
            this.destinationFindButton.Click += new System.EventHandler(this.destinationFindButton_Click);
            // 
            // formatDestinationsBox
            // 
            this.formatDestinationsBox.FormattingEnabled = true;
            this.formatDestinationsBox.Location = new System.Drawing.Point(469, 212);
            this.formatDestinationsBox.Name = "formatDestinationsBox";
            this.formatDestinationsBox.Size = new System.Drawing.Size(153, 21);
            this.formatDestinationsBox.TabIndex = 12;
            this.formatDestinationsBox.DropDown += new System.EventHandler(this.AdjustWidthComboBox_DropDown);
            this.formatDestinationsBox.SelectedIndexChanged += new System.EventHandler(this.formatDestinationsBox_SelectedIndexChanged);
            // 
            // deleteDestinationButton
            // 
            this.deleteDestinationButton.Location = new System.Drawing.Point(632, 227);
            this.deleteDestinationButton.Name = "deleteDestinationButton";
            this.deleteDestinationButton.Size = new System.Drawing.Size(25, 25);
            this.deleteDestinationButton.TabIndex = 13;
            this.deleteDestinationButton.Text = "-";
            this.deleteDestinationButton.UseVisualStyleBackColor = true;
            this.deleteDestinationButton.Click += new System.EventHandler(this.deleteDestinationButton_Click);
            // 
            // configComboBox
            // 
            this.configComboBox.FormattingEnabled = true;
            this.configComboBox.Location = new System.Drawing.Point(469, 25);
            this.configComboBox.Name = "configComboBox";
            this.configComboBox.Size = new System.Drawing.Size(157, 21);
            this.configComboBox.TabIndex = 14;
            this.configComboBox.SelectedIndexChanged += new System.EventHandler(this.configComboBox_SelectedIndexChanged);
            // 
            // configLabel
            // 
            this.configLabel.AutoSize = true;
            this.configLabel.Location = new System.Drawing.Point(466, 9);
            this.configLabel.Name = "configLabel";
            this.configLabel.Size = new System.Drawing.Size(70, 13);
            this.configLabel.TabIndex = 15;
            this.configLabel.Text = "Select Config";
            // 
            // addConfigButton
            // 
            this.addConfigButton.Location = new System.Drawing.Point(632, 22);
            this.addConfigButton.Name = "addConfigButton";
            this.addConfigButton.Size = new System.Drawing.Size(25, 25);
            this.addConfigButton.TabIndex = 16;
            this.addConfigButton.Text = "+";
            this.addConfigButton.UseVisualStyleBackColor = true;
            this.addConfigButton.Click += new System.EventHandler(this.addConfigButton_Click);
            // 
            // packetFormatComboBox
            // 
            this.packetFormatComboBox.FormattingEnabled = true;
            this.packetFormatComboBox.Location = new System.Drawing.Point(469, 160);
            this.packetFormatComboBox.Name = "packetFormatComboBox";
            this.packetFormatComboBox.Size = new System.Drawing.Size(184, 21);
            this.packetFormatComboBox.TabIndex = 17;
            this.packetFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.packetFormatComboBox_SelectedIndexChanged);
            // 
            // packetFormatLabel
            // 
            this.packetFormatLabel.AutoSize = true;
            this.packetFormatLabel.Location = new System.Drawing.Point(466, 144);
            this.packetFormatLabel.Name = "packetFormatLabel";
            this.packetFormatLabel.Size = new System.Drawing.Size(76, 13);
            this.packetFormatLabel.TabIndex = 18;
            this.packetFormatLabel.Text = "Packet Format";
            // 
            // filtersComboBox
            // 
            this.filtersComboBox.FormattingEnabled = true;
            this.filtersComboBox.Location = new System.Drawing.Point(469, 270);
            this.filtersComboBox.Name = "filtersComboBox";
            this.filtersComboBox.Size = new System.Drawing.Size(188, 21);
            this.filtersComboBox.TabIndex = 19;
            this.filtersComboBox.SelectedIndexChanged += new System.EventHandler(this.filtersComboBox_SelectedIndexChanged);
            // 
            // filterConfigLabel
            // 
            this.filterConfigLabel.AutoSize = true;
            this.filterConfigLabel.Location = new System.Drawing.Point(466, 254);
            this.filterConfigLabel.Name = "filterConfigLabel";
            this.filterConfigLabel.Size = new System.Drawing.Size(62, 13);
            this.filterConfigLabel.TabIndex = 20;
            this.filterConfigLabel.Text = "Filter Config";
            // 
            // hkComboBox
            // 
            this.hkComboBox.FormattingEnabled = true;
            this.hkComboBox.Location = new System.Drawing.Point(469, 317);
            this.hkComboBox.Name = "hkComboBox";
            this.hkComboBox.Size = new System.Drawing.Size(113, 21);
            this.hkComboBox.TabIndex = 21;
            this.hkComboBox.SelectedIndexChanged += new System.EventHandler(this.hkComboBox_SelectedIndexChanged);
            // 
            // hotkeyLabel
            // 
            this.hotkeyLabel.AutoSize = true;
            this.hotkeyLabel.Location = new System.Drawing.Point(466, 301);
            this.hotkeyLabel.Name = "hotkeyLabel";
            this.hotkeyLabel.Size = new System.Drawing.Size(81, 13);
            this.hotkeyLabel.TabIndex = 22;
            this.hotkeyLabel.Text = "Toggle Hot Key";
            // 
            // hkWindowsCheckbox
            // 
            this.hkWindowsCheckbox.AutoSize = true;
            this.hkWindowsCheckbox.Location = new System.Drawing.Point(469, 345);
            this.hkWindowsCheckbox.Name = "hkWindowsCheckbox";
            this.hkWindowsCheckbox.Size = new System.Drawing.Size(79, 17);
            this.hkWindowsCheckbox.TabIndex = 23;
            this.hkWindowsCheckbox.Text = "+ Windows";
            this.hkWindowsCheckbox.UseVisualStyleBackColor = true;
            this.hkWindowsCheckbox.CheckedChanged += new System.EventHandler(this.hkWindowsCheckbox_CheckedChanged);
            // 
            // hkAltCheckbox
            // 
            this.hkAltCheckbox.AutoSize = true;
            this.hkAltCheckbox.Location = new System.Drawing.Point(469, 369);
            this.hkAltCheckbox.Name = "hkAltCheckbox";
            this.hkAltCheckbox.Size = new System.Drawing.Size(47, 17);
            this.hkAltCheckbox.TabIndex = 24;
            this.hkAltCheckbox.Text = "+ Alt";
            this.hkAltCheckbox.UseVisualStyleBackColor = true;
            this.hkAltCheckbox.CheckedChanged += new System.EventHandler(this.hkAltCheckbox_CheckedChanged);
            // 
            // hkShiftCheckBox
            // 
            this.hkShiftCheckBox.AutoSize = true;
            this.hkShiftCheckBox.Location = new System.Drawing.Point(556, 345);
            this.hkShiftCheckBox.Name = "hkShiftCheckBox";
            this.hkShiftCheckBox.Size = new System.Drawing.Size(56, 17);
            this.hkShiftCheckBox.TabIndex = 25;
            this.hkShiftCheckBox.Text = "+ Shift";
            this.hkShiftCheckBox.UseVisualStyleBackColor = true;
            this.hkShiftCheckBox.CheckedChanged += new System.EventHandler(this.hkShiftCheckBox_CheckedChanged);
            // 
            // hkCtrlCheckbox
            // 
            this.hkCtrlCheckbox.AutoSize = true;
            this.hkCtrlCheckbox.Location = new System.Drawing.Point(556, 369);
            this.hkCtrlCheckbox.Name = "hkCtrlCheckbox";
            this.hkCtrlCheckbox.Size = new System.Drawing.Size(50, 17);
            this.hkCtrlCheckbox.TabIndex = 26;
            this.hkCtrlCheckbox.Text = "+ Ctrl";
            this.hkCtrlCheckbox.UseVisualStyleBackColor = true;
            this.hkCtrlCheckbox.CheckedChanged += new System.EventHandler(this.hkCtrlCheckbox_CheckedChanged);
            // 
            // hkEnabledCheckbox
            // 
            this.hkEnabledCheckbox.AutoSize = true;
            this.hkEnabledCheckbox.Location = new System.Drawing.Point(592, 319);
            this.hkEnabledCheckbox.Name = "hkEnabledCheckbox";
            this.hkEnabledCheckbox.Size = new System.Drawing.Size(65, 17);
            this.hkEnabledCheckbox.TabIndex = 27;
            this.hkEnabledCheckbox.Text = "Enabled";
            this.hkEnabledCheckbox.UseVisualStyleBackColor = true;
            this.hkEnabledCheckbox.CheckedChanged += new System.EventHandler(this.hkEnabledCheckbox_CheckedChanged);
            // 
            // wreckfestButton
            // 
            this.wreckfestButton.Location = new System.Drawing.Point(13, 63);
            this.wreckfestButton.Name = "wreckfestButton";
            this.wreckfestButton.Size = new System.Drawing.Size(148, 45);
            this.wreckfestButton.TabIndex = 28;
            this.wreckfestButton.Text = "Wreckfest";
            this.wreckfestButton.UseVisualStyleBackColor = true;
            this.wreckfestButton.Click += new System.EventHandler(this.wreckfestButton_Click);
            // 
            // beamNGButton
            // 
            this.beamNGButton.Location = new System.Drawing.Point(13, 114);
            this.beamNGButton.Name = "beamNGButton";
            this.beamNGButton.Size = new System.Drawing.Size(148, 45);
            this.beamNGButton.TabIndex = 29;
            this.beamNGButton.Text = "BeamNG Drive";
            this.beamNGButton.UseVisualStyleBackColor = true;
            this.beamNGButton.Click += new System.EventHandler(this.beamNGButton_Click);
            // 
            // gtavButton
            // 
            this.gtavButton.Location = new System.Drawing.Point(13, 166);
            this.gtavButton.Name = "gtavButton";
            this.gtavButton.Size = new System.Drawing.Size(148, 45);
            this.gtavButton.TabIndex = 30;
            this.gtavButton.Text = "GTA V";
            this.gtavButton.UseVisualStyleBackColor = true;
            this.gtavButton.Click += new System.EventHandler(this.gtavButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 417);
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
            this.Name = "MainForm";
            this.Text = "GenericTelemetryProvider";
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
    }
}

