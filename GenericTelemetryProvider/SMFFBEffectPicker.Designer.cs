namespace GenericTelemetryProvider
{
    partial class SMFFBEffectPicker
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
            this.effectComboBox.Location = new System.Drawing.Point(18, 19);
            this.effectComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.effectComboBox.Name = "effectComboBox";
            this.effectComboBox.Size = new System.Drawing.Size(412, 28);
            this.effectComboBox.TabIndex = 0;
            this.effectComboBox.SelectedIndexChanged += new System.EventHandler(this.effectComboBox_SelectedIndexChanged);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(18, 60);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(414, 82);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SMFFBEffectPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 155);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.effectComboBox);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SMFFBEffectPicker";
            this.Text = "Force Feedback Effect Picker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox effectComboBox;
        private System.Windows.Forms.Button okButton;
    }
}