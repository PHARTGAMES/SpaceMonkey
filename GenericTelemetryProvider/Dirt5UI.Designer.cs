namespace GenericTelemetryProvider
{
    partial class Dirt5UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dirt5UI));
            this.vehicleSelector = new System.Windows.Forms.ComboBox();
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.initializeButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.vehicleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // vehicleSelector
            // 
            this.vehicleSelector.FormattingEnabled = true;
            this.vehicleSelector.Location = new System.Drawing.Point(12, 28);
            this.vehicleSelector.Name = "vehicleSelector";
            this.vehicleSelector.Size = new System.Drawing.Size(208, 21);
            this.vehicleSelector.TabIndex = 2;
            this.vehicleSelector.SelectedIndexChanged += new System.EventHandler(this.vehicleSelector_SelectedIndexChanged);
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(226, 13);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(245, 20);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(226, 39);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(245, 46);
            this.initializeButton.TabIndex = 6;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(226, 91);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(245, 23);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(12, 120);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(459, 1005);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // vehicleLabel
            // 
            this.vehicleLabel.AutoSize = true;
            this.vehicleLabel.Location = new System.Drawing.Point(13, 13);
            this.vehicleLabel.Name = "vehicleLabel";
            this.vehicleLabel.Size = new System.Drawing.Size(42, 13);
            this.vehicleLabel.TabIndex = 12;
            this.vehicleLabel.Text = "Vehicle";
            // 
            // Dirt5UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 1137);
            this.Controls.Add(this.vehicleLabel);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.vehicleSelector);
            this.Icon = GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Name = "Dirt5UI";
            this.Text = "Dirt5UI";
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
    }
}