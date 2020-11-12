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
            this.initializeButton = new System.Windows.Forms.Button();
            this.vehicleSelector = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(227, 40);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(245, 46);
            this.initializeButton.TabIndex = 0;
            this.initializeButton.Text = "Initialize";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // vehicleSelector
            // 
            this.vehicleSelector.FormattingEnabled = true;
            this.vehicleSelector.Location = new System.Drawing.Point(13, 13);
            this.vehicleSelector.Name = "vehicleSelector";
            this.vehicleSelector.Size = new System.Drawing.Size(208, 21);
            this.vehicleSelector.TabIndex = 1;
            this.vehicleSelector.SelectedIndexChanged += new System.EventHandler(this.vehicleSelector_SelectedIndexChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(227, 92);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(245, 23);
            this.progressBar1.TabIndex = 2;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(227, 121);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(245, 617);
            this.matrixBox.TabIndex = 3;
            this.matrixBox.Text = "";
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(227, 14);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(245, 20);
            this.statusLabel.TabIndex = 4;
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.StatusBox_TextChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 750);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.vehicleSelector);
            this.Controls.Add(this.initializeButton);
            this.Name = "MainForm";
            this.Text = "GenericTelemetryProvider";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.ComboBox vehicleSelector;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.TextBox statusLabel;
    }
}

