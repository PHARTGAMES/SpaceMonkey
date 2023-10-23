
namespace XInputFFB
{
    partial class DIDeviceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.deviceEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // deviceEnabledCheckBox
            // 
            this.deviceEnabledCheckBox.AutoSize = true;
            this.deviceEnabledCheckBox.Location = new System.Drawing.Point(21, 20);
            this.deviceEnabledCheckBox.Name = "deviceEnabledCheckBox";
            this.deviceEnabledCheckBox.Size = new System.Drawing.Size(108, 21);
            this.deviceEnabledCheckBox.TabIndex = 1;
            this.deviceEnabledCheckBox.Text = "deviceName";
            this.deviceEnabledCheckBox.UseVisualStyleBackColor = true;
            this.deviceEnabledCheckBox.CheckedChanged += new System.EventHandler(this.deviceEnabledCheckBox_CheckedChanged);
            // 
            // DIDeviceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.deviceEnabledCheckBox);
            this.Name = "DIDeviceControl";
            this.Size = new System.Drawing.Size(930, 58);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox deviceEnabledCheckBox;
    }
}
