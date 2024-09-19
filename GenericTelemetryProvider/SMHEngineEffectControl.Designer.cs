namespace GenericTelemetryProvider
{
    partial class SMHEngineEffectControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.heading = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.maxFrequency = new System.Windows.Forms.TextBox();
            this.minFrequency = new System.Windows.Forms.TextBox();
            this.outputChannelIndex = new System.Windows.Forms.TextBox();
            this.outputDeviceComboBox = new System.Windows.Forms.ComboBox();
            this.gainTrackBar = new System.Windows.Forms.TrackBar();
            this.minFreqLabel = new System.Windows.Forms.Label();
            this.maxFreqLabel = new System.Windows.Forms.Label();
            this.channelIndexLabel = new System.Windows.Forms.Label();
            this.gainLabel = new System.Windows.Forms.Label();
            this.outputDeviceLabel = new System.Windows.Forms.Label();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.deviceGroupBox = new System.Windows.Forms.GroupBox();
            this.effectGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.gainTrackBar)).BeginInit();
            this.deviceGroupBox.SuspendLayout();
            this.effectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.Location = new System.Drawing.Point(229, 21);
            this.heading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(176, 31);
            this.heading.TabIndex = 0;
            this.heading.Text = "Engine Effect";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(8, 398);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 28);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // maxFrequency
            // 
            this.maxFrequency.Location = new System.Drawing.Point(430, 147);
            this.maxFrequency.Margin = new System.Windows.Forms.Padding(4);
            this.maxFrequency.Name = "maxFrequency";
            this.maxFrequency.Size = new System.Drawing.Size(115, 22);
            this.maxFrequency.TabIndex = 2;
            this.maxFrequency.Text = "0";
            this.maxFrequency.TextChanged += new System.EventHandler(this.maxFrequency_TextChanged);
            // 
            // minFrequency
            // 
            this.minFrequency.Location = new System.Drawing.Point(430, 91);
            this.minFrequency.Margin = new System.Windows.Forms.Padding(4);
            this.minFrequency.Name = "minFrequency";
            this.minFrequency.Size = new System.Drawing.Size(115, 22);
            this.minFrequency.TabIndex = 3;
            this.minFrequency.Text = "0";
            this.minFrequency.TextChanged += new System.EventHandler(this.minFrequency_TextChanged);
            // 
            // outputChannelIndex
            // 
            this.outputChannelIndex.Location = new System.Drawing.Point(29, 175);
            this.outputChannelIndex.Margin = new System.Windows.Forms.Padding(4);
            this.outputChannelIndex.Name = "outputChannelIndex";
            this.outputChannelIndex.Size = new System.Drawing.Size(115, 22);
            this.outputChannelIndex.TabIndex = 4;
            this.outputChannelIndex.Text = "0";
            this.outputChannelIndex.TextChanged += new System.EventHandler(this.outputChannelIndex_TextChanged);
            // 
            // outputDeviceComboBox
            // 
            this.outputDeviceComboBox.FormattingEnabled = true;
            this.outputDeviceComboBox.Location = new System.Drawing.Point(28, 115);
            this.outputDeviceComboBox.Name = "outputDeviceComboBox";
            this.outputDeviceComboBox.Size = new System.Drawing.Size(272, 24);
            this.outputDeviceComboBox.TabIndex = 5;
            this.outputDeviceComboBox.SelectedIndexChanged += new System.EventHandler(this.outputDeviceComboBox_SelectedIndexChanged);
            // 
            // gainTrackBar
            // 
            this.gainTrackBar.LargeChange = 50;
            this.gainTrackBar.Location = new System.Drawing.Point(19, 235);
            this.gainTrackBar.Maximum = 100;
            this.gainTrackBar.Name = "gainTrackBar";
            this.gainTrackBar.Size = new System.Drawing.Size(141, 56);
            this.gainTrackBar.TabIndex = 6;
            this.gainTrackBar.Value = 100;
            this.gainTrackBar.Scroll += new System.EventHandler(this.gainTrackBar_Scroll);
            // 
            // minFreqLabel
            // 
            this.minFreqLabel.AutoSize = true;
            this.minFreqLabel.Location = new System.Drawing.Point(427, 70);
            this.minFreqLabel.Name = "minFreqLabel";
            this.minFreqLabel.Size = new System.Drawing.Size(134, 17);
            this.minFreqLabel.TabIndex = 7;
            this.minFreqLabel.Text = "Minimum Frequency";
            // 
            // maxFreqLabel
            // 
            this.maxFreqLabel.AutoSize = true;
            this.maxFreqLabel.Location = new System.Drawing.Point(427, 126);
            this.maxFreqLabel.Name = "maxFreqLabel";
            this.maxFreqLabel.Size = new System.Drawing.Size(137, 17);
            this.maxFreqLabel.TabIndex = 8;
            this.maxFreqLabel.Text = "Maximum Frequency";
            // 
            // channelIndexLabel
            // 
            this.channelIndexLabel.AutoSize = true;
            this.channelIndexLabel.Location = new System.Drawing.Point(26, 154);
            this.channelIndexLabel.Name = "channelIndexLabel";
            this.channelIndexLabel.Size = new System.Drawing.Size(97, 17);
            this.channelIndexLabel.TabIndex = 9;
            this.channelIndexLabel.Text = "Channel Index";
            // 
            // gainLabel
            // 
            this.gainLabel.AutoSize = true;
            this.gainLabel.Location = new System.Drawing.Point(26, 215);
            this.gainLabel.Name = "gainLabel";
            this.gainLabel.Size = new System.Drawing.Size(38, 17);
            this.gainLabel.TabIndex = 10;
            this.gainLabel.Text = "Gain";
            // 
            // outputDeviceLabel
            // 
            this.outputDeviceLabel.AutoSize = true;
            this.outputDeviceLabel.Location = new System.Drawing.Point(25, 95);
            this.outputDeviceLabel.Name = "outputDeviceLabel";
            this.outputDeviceLabel.Size = new System.Drawing.Size(51, 17);
            this.outputDeviceLabel.TabIndex = 11;
            this.outputDeviceLabel.Text = "Device";
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Location = new System.Drawing.Point(24, 52);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.enabledCheckBox.Size = new System.Drawing.Size(82, 21);
            this.enabledCheckBox.TabIndex = 12;
            this.enabledCheckBox.Text = "Enabled";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.enabledCheckBox_CheckedChanged);
            // 
            // deviceGroupBox
            // 
            this.deviceGroupBox.Controls.Add(this.outputDeviceComboBox);
            this.deviceGroupBox.Controls.Add(this.enabledCheckBox);
            this.deviceGroupBox.Controls.Add(this.outputChannelIndex);
            this.deviceGroupBox.Controls.Add(this.outputDeviceLabel);
            this.deviceGroupBox.Controls.Add(this.gainTrackBar);
            this.deviceGroupBox.Controls.Add(this.gainLabel);
            this.deviceGroupBox.Controls.Add(this.channelIndexLabel);
            this.deviceGroupBox.Location = new System.Drawing.Point(48, 70);
            this.deviceGroupBox.Name = "deviceGroupBox";
            this.deviceGroupBox.Size = new System.Drawing.Size(326, 321);
            this.deviceGroupBox.TabIndex = 13;
            this.deviceGroupBox.TabStop = false;
            this.deviceGroupBox.Text = "Output";
            // 
            // effectGroupBox
            // 
            this.effectGroupBox.Controls.Add(this.deviceGroupBox);
            this.effectGroupBox.Controls.Add(this.heading);
            this.effectGroupBox.Controls.Add(this.maxFreqLabel);
            this.effectGroupBox.Controls.Add(this.deleteButton);
            this.effectGroupBox.Controls.Add(this.minFreqLabel);
            this.effectGroupBox.Controls.Add(this.maxFrequency);
            this.effectGroupBox.Controls.Add(this.minFrequency);
            this.effectGroupBox.Location = new System.Drawing.Point(14, 13);
            this.effectGroupBox.Name = "effectGroupBox";
            this.effectGroupBox.Size = new System.Drawing.Size(585, 438);
            this.effectGroupBox.TabIndex = 14;
            this.effectGroupBox.TabStop = false;
            this.effectGroupBox.Text = "Engine Effect";
            // 
            // SMHEngineEffectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.effectGroupBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SMHEngineEffectControl";
            this.Size = new System.Drawing.Size(614, 469);
            ((System.ComponentModel.ISupportInitialize)(this.gainTrackBar)).EndInit();
            this.deviceGroupBox.ResumeLayout(false);
            this.deviceGroupBox.PerformLayout();
            this.effectGroupBox.ResumeLayout(false);
            this.effectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TextBox maxFrequency;
        private System.Windows.Forms.TextBox minFrequency;
        private System.Windows.Forms.TextBox outputChannelIndex;
        private System.Windows.Forms.ComboBox outputDeviceComboBox;
        private System.Windows.Forms.TrackBar gainTrackBar;
        private System.Windows.Forms.Label minFreqLabel;
        private System.Windows.Forms.Label maxFreqLabel;
        private System.Windows.Forms.Label channelIndexLabel;
        private System.Windows.Forms.Label gainLabel;
        private System.Windows.Forms.Label outputDeviceLabel;
        private System.Windows.Forms.CheckBox enabledCheckBox;
        private System.Windows.Forms.GroupBox deviceGroupBox;
        private System.Windows.Forms.GroupBox effectGroupBox;
    }
}
