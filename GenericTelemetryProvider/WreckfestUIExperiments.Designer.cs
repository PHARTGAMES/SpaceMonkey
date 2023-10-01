namespace GenericTelemetryProvider
{
    partial class WreckfestUIExperiments
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WreckfestUIExperiments));
            this.vehicleSelector = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.initializeButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.vehicleLabel = new System.Windows.Forms.Label();
            this.InvertYawToggle = new System.Windows.Forms.CheckBox();
            this.ZeroiseTiltToggle = new System.Windows.Forms.CheckBox();
            this.VelocityScale = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.StartAddress = new System.Windows.Forms.TextBox();
            this.GForceScale = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.UpdateRate = new System.Windows.Forms.TextBox();
            this.GForceClip = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // vehicleSelector
            // 
            this.vehicleSelector.FormattingEnabled = true;
            this.vehicleSelector.Items.AddRange(new object[] {
            "00",
            "01",
            "02",
            "03",
            "04",
            "05",
            "06",
            "07",
            "08",
            "09",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.vehicleSelector.Location = new System.Drawing.Point(18, 43);
            this.vehicleSelector.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.vehicleSelector.Name = "vehicleSelector";
            this.vehicleSelector.Size = new System.Drawing.Size(310, 28);
            this.vehicleSelector.TabIndex = 2;
            this.vehicleSelector.SelectedIndexChanged += new System.EventHandler(this.vehicleSelector_SelectedIndexChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(339, 20);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(366, 26);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(339, 60);
            this.initializeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(368, 71);
            this.initializeButton.TabIndex = 6;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(339, 140);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(368, 35);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(18, 383);
            this.matrixBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(686, 1613);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // vehicleLabel
            // 
            this.vehicleLabel.AutoSize = true;
            this.vehicleLabel.Location = new System.Drawing.Point(20, 20);
            this.vehicleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.vehicleLabel.Name = "vehicleLabel";
            this.vehicleLabel.Size = new System.Drawing.Size(61, 20);
            this.vehicleLabel.TabIndex = 12;
            this.vehicleLabel.Text = "Vehicle";
            // 
            // InvertYawToggle
            // 
            this.InvertYawToggle.AutoSize = true;
            this.InvertYawToggle.Checked = true;
            this.InvertYawToggle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.InvertYawToggle.Location = new System.Drawing.Point(20, 86);
            this.InvertYawToggle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.InvertYawToggle.Name = "InvertYawToggle";
            this.InvertYawToggle.Size = new System.Drawing.Size(110, 24);
            this.InvertYawToggle.TabIndex = 13;
            this.InvertYawToggle.Text = "Invert Yaw";
            this.InvertYawToggle.UseVisualStyleBackColor = true;
            this.InvertYawToggle.CheckedChanged += new System.EventHandler(this.InvertYawToggle_CheckedChanged);
            // 
            // ZeroiseTiltToggle
            // 
            this.ZeroiseTiltToggle.AutoSize = true;
            this.ZeroiseTiltToggle.Location = new System.Drawing.Point(20, 126);
            this.ZeroiseTiltToggle.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ZeroiseTiltToggle.Name = "ZeroiseTiltToggle";
            this.ZeroiseTiltToggle.Size = new System.Drawing.Size(112, 24);
            this.ZeroiseTiltToggle.TabIndex = 14;
            this.ZeroiseTiltToggle.Text = "Zeroise Tilt";
            this.ZeroiseTiltToggle.UseVisualStyleBackColor = true;
            this.ZeroiseTiltToggle.CheckedChanged += new System.EventHandler(this.ZeroiseTiltToggle_CheckedChanged);
            // 
            // VelocityScale
            // 
            this.VelocityScale.Location = new System.Drawing.Point(144, 82);
            this.VelocityScale.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.VelocityScale.Name = "VelocityScale";
            this.VelocityScale.Size = new System.Drawing.Size(49, 26);
            this.VelocityScale.TabIndex = 15;
            this.VelocityScale.Text = "1.0";
            this.VelocityScale.TextChanged += new System.EventHandler(this.VelocityScale_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(204, 88);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 20);
            this.label1.TabIndex = 16;
            this.label1.Text = "Velocity Scale";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(204, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 20);
            this.label2.TabIndex = 17;
            this.label2.Text = "Start Address";
            // 
            // StartAddress
            // 
            this.StartAddress.Location = new System.Drawing.Point(76, 162);
            this.StartAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.StartAddress.Name = "StartAddress";
            this.StartAddress.Size = new System.Drawing.Size(115, 26);
            this.StartAddress.TabIndex = 18;
            this.StartAddress.Text = "1500000000";
            this.StartAddress.TextChanged += new System.EventHandler(this.StartAddress_TextChanged);
            // 
            // GForceScale
            // 
            this.GForceScale.Location = new System.Drawing.Point(144, 122);
            this.GForceScale.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GForceScale.Name = "GForceScale";
            this.GForceScale.Size = new System.Drawing.Size(49, 26);
            this.GForceScale.TabIndex = 19;
            this.GForceScale.Text = "100";
            this.GForceScale.TextChanged += new System.EventHandler(this.GForceScale_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(204, 126);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 20);
            this.label3.TabIndex = 20;
            this.label3.Text = "G-Force Scale";
            // 
            // UpdateRate
            // 
            this.UpdateRate.Location = new System.Drawing.Point(18, 242);
            this.UpdateRate.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UpdateRate.Name = "UpdateRate";
            this.UpdateRate.Size = new System.Drawing.Size(49, 26);
            this.UpdateRate.TabIndex = 21;
            this.UpdateRate.Text = "1000";
            this.UpdateRate.TextChanged += new System.EventHandler(this.UpdateRate_TextChanged);
            // 
            // GForceClip
            // 
            this.GForceClip.Location = new System.Drawing.Point(18, 202);
            this.GForceClip.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.GForceClip.Name = "GForceClip";
            this.GForceClip.Size = new System.Drawing.Size(49, 26);
            this.GForceClip.TabIndex = 22;
            this.GForceClip.Text = "1500";
            this.GForceClip.TextChanged += new System.EventHandler(this.GForceClip_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(90, 206);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 20);
            this.label4.TabIndex = 23;
            this.label4.Text = "G Force Strip";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(92, 246);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 20);
            this.label5.TabIndex = 24;
            this.label5.Text = "Update (hz)";
            // 
            // WreckfestUIExperiments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 2017);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.GForceClip);
            this.Controls.Add(this.UpdateRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.GForceScale);
            this.Controls.Add(this.StartAddress);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VelocityScale);
            this.Controls.Add(this.ZeroiseTiltToggle);
            this.Controls.Add(this.InvertYawToggle);
            this.Controls.Add(this.vehicleLabel);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.vehicleSelector);
            this.Icon = GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WreckfestUIExperiments";
            this.Text = "WreckfestUIExperiments";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox vehicleSelector;
        private System.Windows.Forms.TextBox statusLabel;
        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.Label vehicleLabel;
        private System.Windows.Forms.CheckBox InvertYawToggle;
        private System.Windows.Forms.CheckBox ZeroiseTiltToggle;
        private System.Windows.Forms.TextBox VelocityScale;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox StartAddress;
        private System.Windows.Forms.TextBox GForceScale;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox UpdateRate;
        private System.Windows.Forms.TextBox GForceClip;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}