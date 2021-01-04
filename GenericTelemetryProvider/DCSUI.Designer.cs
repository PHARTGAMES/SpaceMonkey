namespace GenericTelemetryProvider
{
    partial class DCSUI
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
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.initializeButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(12, 13);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(459, 20);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Enter UDP Receive Port then click Initialize!";
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(12, 98);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(459, 1079);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(232, 39);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(239, 53);
            this.initializeButton.TabIndex = 9;
            this.initializeButton.Text = "Initialize!";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(15, 56);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(211, 20);
            this.portTextBox.TabIndex = 11;
            this.portTextBox.Text = "6666";
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(12, 42);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(95, 13);
            this.portLabel.TabIndex = 13;
            this.portLabel.Text = "UDP Receive Port";
            // 
            // BeamNGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 1189);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.statusLabel);
            this.Name = "DCSUI";
            this.Text = "DCSUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox statusLabel;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label portLabel;
    }
}