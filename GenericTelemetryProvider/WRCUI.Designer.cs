namespace GenericTelemetryProvider
{
    partial class WRCUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WRCUI));
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.initializeButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(18, 20);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(686, 26);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.Text = "Click Initialize!";
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(18, 150);
            this.matrixBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(686, 1658);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // initializeButton
            // 
            this.initializeButton.Location = new System.Drawing.Point(348, 60);
            this.initializeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.initializeButton.Name = "initializeButton";
            this.initializeButton.Size = new System.Drawing.Size(358, 82);
            this.initializeButton.TabIndex = 9;
            this.initializeButton.Text = "Initialize!";
            this.initializeButton.UseVisualStyleBackColor = true;
            this.initializeButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(28, 60);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(313, 82);
            this.progressBar1.TabIndex = 10;
            // 
            // WRCUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 1695);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.statusLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WRCUI";
            this.Text = "WRC UI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox statusLabel;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.Button initializeButton;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}