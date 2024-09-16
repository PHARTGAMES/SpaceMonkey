namespace GenericTelemetryProvider
{
    partial class SMHEffectPicker
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
            this.effectComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // effectComboBox
            // 
            this.effectComboBox.FormattingEnabled = true;
            this.effectComboBox.Location = new System.Drawing.Point(16, 15);
            this.effectComboBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.effectComboBox.Name = "effectComboBox";
            this.effectComboBox.Size = new System.Drawing.Size(367, 24);
            this.effectComboBox.TabIndex = 0;
            this.effectComboBox.SelectedIndexChanged += new System.EventHandler(this.effectComboBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(16, 48);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(368, 66);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SMHEffectPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 124);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.effectComboBox);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SMHEffectPicker";
            this.Text = "Haptic Effect Picker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox effectComboBox;
        private System.Windows.Forms.Button okButton;
    }
}