﻿namespace GenericTelemetryProvider
{
    partial class WarplanesWW1UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarplanesWW1UI));
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.initializeButton = new System.Windows.Forms.Button();
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
            // WarplanesWW1UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 1695);
            this.Controls.Add(this.initializeButton);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.statusLabel);
            this.Icon = GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WarplanesWW1UI";
            this.Text = "Warplanes WW1 UI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox statusLabel;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.Button initializeButton;
    }
}