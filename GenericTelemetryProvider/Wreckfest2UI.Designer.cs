namespace GenericTelemetryProvider
{
    partial class Wreckfest2UI
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
            this.vehicleSelector = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.initializeButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.vehicleLabel = new System.Windows.Forms.Label();
            this.scanButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxCarsComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
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
            this.vehicleSelector.Location = new System.Drawing.Point(261, 61);
            this.vehicleSelector.Name = "vehicleSelector";
            this.vehicleSelector.Size = new System.Drawing.Size(195, 21);
            this.vehicleSelector.TabIndex = 2;
            this.vehicleSelector.SelectedIndexChanged += new System.EventHandler(this.vehicleSelector_SelectedIndexChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(12, 13);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(459, 20);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(31, 131);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(425, 46);
            this.initializeButton.TabIndex = 6;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(31, 88);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(189, 23);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(12, 195);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(459, 930);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // vehicleLabel
            // 
            this.vehicleLabel.AutoSize = true;
            this.vehicleLabel.Location = new System.Drawing.Point(258, 45);
            this.vehicleLabel.Name = "vehicleLabel";
            this.vehicleLabel.Size = new System.Drawing.Size(68, 13);
            this.vehicleLabel.TabIndex = 12;
            this.vehicleLabel.Text = "2. Select Car";
            // 
            // scanButton
            // 
            this.scanButton.Location = new System.Drawing.Point(125, 39);
            this.scanButton.Name = "scanButton";
            this.scanButton.Size = new System.Drawing.Size(95, 43);
            this.scanButton.TabIndex = 13;
            this.scanButton.Text = "Scan";
            this.scanButton.UseVisualStyleBackColor = true;
            this.scanButton.Click += new System.EventHandler(this.scanButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "1.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 131);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "3.";
            // 
            // maxCarsComboBox
            // 
            this.maxCarsComboBox.FormattingEnabled = true;
            this.maxCarsComboBox.Location = new System.Drawing.Point(31, 61);
            this.maxCarsComboBox.Name = "maxCarsComboBox";
            this.maxCarsComboBox.Size = new System.Drawing.Size(88, 21);
            this.maxCarsComboBox.TabIndex = 16;
            this.maxCarsComboBox.SelectedIndexChanged += new System.EventHandler(this.maxCarsComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(32, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Max Cars";
            // 
            // Wreckfest2UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 1137);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.maxCarsComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.scanButton);
            this.Controls.Add(this.vehicleLabel);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.vehicleSelector);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Name = "Wreckfest2UI";
            this.Text = "Wreckfest2UI";
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
        private System.Windows.Forms.Button scanButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox maxCarsComboBox;
        private System.Windows.Forms.Label label3;
    }
}