namespace GenericTelemetryProvider
{
    partial class UEVRUI
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
            this.gameComboBox = new System.Windows.Forms.ComboBox();
            this.installButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // gameComboBox
            // 
            this.gameComboBox.FormattingEnabled = true;
            this.gameComboBox.Location = new System.Drawing.Point(12, 12);
            this.gameComboBox.Name = "gameComboBox";
            this.gameComboBox.Size = new System.Drawing.Size(459, 21);
            this.gameComboBox.TabIndex = 0;
            this.gameComboBox.SelectedIndexChanged += new System.EventHandler(this.gameComboBox_SelectedIndexChanged);
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(12, 39);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(459, 83);
            this.installButton.TabIndex = 1;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // UEVRUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 134);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.gameComboBox);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Name = "UEVRUI";
            this.Text = "UEVRUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox gameComboBox;
        private System.Windows.Forms.Button installButton;
    }
}