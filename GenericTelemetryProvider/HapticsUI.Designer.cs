namespace GenericTelemetryProvider
{
    partial class HapticsUI
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
            this.flowLayoutEffects = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutEffects
            // 
            this.flowLayoutEffects.Location = new System.Drawing.Point(13, 47);
            this.flowLayoutEffects.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutEffects.Name = "flowLayoutEffects";
            this.flowLayoutEffects.Size = new System.Drawing.Size(1102, 895);
            this.flowLayoutEffects.TabIndex = 2;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(13, 13);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(320, 26);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "SAVE";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // HapticsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1128, 955);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.flowLayoutEffects);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "HapticsUI";
            this.Text = "HapticsUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.HapticsUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutEffects;
        private System.Windows.Forms.Button saveButton;
    }
}