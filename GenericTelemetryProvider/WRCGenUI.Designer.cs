namespace GenericTelemetryProvider
{
    partial class WRCGenUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WRCGenUI));
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
            this.statusLabel.Location = new System.Drawing.Point(16, 16);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(611, 22);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Enter UDP Receive Port then click Initialize!";
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(16, 121);
            this.matrixBox.Margin = new System.Windows.Forms.Padding(4);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(611, 1327);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(309, 48);
            this.initializeButton.Margin = new System.Windows.Forms.Padding(4);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(319, 65);
            this.initializeButton.TabIndex = 9;
            this.initializeButton.Text = "Initialize!";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(20, 69);
            this.portTextBox.Margin = new System.Windows.Forms.Padding(4);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(280, 22);
            this.portTextBox.TabIndex = 11;
            this.portTextBox.Text = "20777";
            this.portTextBox.TextChanged += new System.EventHandler(this.portTextBox_TextChanged);
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(16, 52);
            this.portLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(122, 17);
            this.portLabel.TabIndex = 13;
            this.portLabel.Text = "UDP Receive Port";
            // 
            // WRCGenUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 1463);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.statusLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WRCGenUI";
            this.Text = "WRCGenUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
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