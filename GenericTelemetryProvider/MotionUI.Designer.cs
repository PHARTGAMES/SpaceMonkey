namespace GenericTelemetryProvider
{
    partial class MotionUI
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
            this.saveButton = new System.Windows.Forms.Button();
            this.rigWidthTextBox = new System.Windows.Forms.TextBox();
            this.rigWidthLabel = new System.Windows.Forms.Label();
            this.rigLengthLabel = new System.Windows.Forms.Label();
            this.rigLengthTextBox = new System.Windows.Forms.TextBox();
            this.avoLabel = new System.Windows.Forms.Label();
            this.avoTextBox = new System.Windows.Forms.TextBox();
            this.asLabel = new System.Windows.Forms.Label();
            this.asTextBox = new System.Windows.Forms.TextBox();
            this.alLabel = new System.Windows.Forms.Label();
            this.alTextBox = new System.Windows.Forms.TextBox();
            this.hoxLabel = new System.Windows.Forms.Label();
            this.hoxTextBox = new System.Windows.Forms.TextBox();
            this.hoyLabel = new System.Windows.Forms.Label();
            this.hoyTextBox = new System.Windows.Forms.TextBox();
            this.hozLabel = new System.Windows.Forms.Label();
            this.hozTextBox = new System.Windows.Forms.TextBox();
            this.accScaleLabel = new System.Windows.Forms.Label();
            this.accScaleTextBox = new System.Windows.Forms.TextBox();
            this.accMaxLabel = new System.Windows.Forms.Label();
            this.accMaxTextBox = new System.Windows.Forms.TextBox();
            this.enabledCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(13, 13);
            this.saveButton.Margin = new System.Windows.Forms.Padding(4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(320, 26);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "SAVE";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // rigWidthTextBox
            // 
            this.rigWidthTextBox.Location = new System.Drawing.Point(12, 131);
            this.rigWidthTextBox.Name = "rigWidthTextBox";
            this.rigWidthTextBox.Size = new System.Drawing.Size(137, 22);
            this.rigWidthTextBox.TabIndex = 4;
            this.rigWidthTextBox.TextChanged += new System.EventHandler(this.rigWidthTextBox_TextChanged);
            // 
            // rigWidthLabel
            // 
            this.rigWidthLabel.AutoSize = true;
            this.rigWidthLabel.Location = new System.Drawing.Point(12, 108);
            this.rigWidthLabel.Name = "rigWidthLabel";
            this.rigWidthLabel.Size = new System.Drawing.Size(69, 17);
            this.rigWidthLabel.TabIndex = 5;
            this.rigWidthLabel.Text = "Rig Width";
            // 
            // rigLengthLabel
            // 
            this.rigLengthLabel.AutoSize = true;
            this.rigLengthLabel.Location = new System.Drawing.Point(174, 108);
            this.rigLengthLabel.Name = "rigLengthLabel";
            this.rigLengthLabel.Size = new System.Drawing.Size(77, 17);
            this.rigLengthLabel.TabIndex = 7;
            this.rigLengthLabel.Text = "Rig Length";
            // 
            // rigLengthTextBox
            // 
            this.rigLengthTextBox.Location = new System.Drawing.Point(174, 131);
            this.rigLengthTextBox.Name = "rigLengthTextBox";
            this.rigLengthTextBox.Size = new System.Drawing.Size(137, 22);
            this.rigLengthTextBox.TabIndex = 6;
            this.rigLengthTextBox.TextChanged += new System.EventHandler(this.rigLengthTextBox_TextChanged);
            // 
            // avoLabel
            // 
            this.avoLabel.AutoSize = true;
            this.avoLabel.Location = new System.Drawing.Point(11, 163);
            this.avoLabel.Name = "avoLabel";
            this.avoLabel.Size = new System.Drawing.Size(154, 17);
            this.avoLabel.TabIndex = 9;
            this.avoLabel.Text = "Actuator Vertical Offset";
            // 
            // avoTextBox
            // 
            this.avoTextBox.Location = new System.Drawing.Point(11, 186);
            this.avoTextBox.Name = "avoTextBox";
            this.avoTextBox.Size = new System.Drawing.Size(137, 22);
            this.avoTextBox.TabIndex = 8;
            this.avoTextBox.TextChanged += new System.EventHandler(this.avoTextBox_TextChanged);
            // 
            // asLabel
            // 
            this.asLabel.AutoSize = true;
            this.asLabel.Location = new System.Drawing.Point(174, 163);
            this.asLabel.Name = "asLabel";
            this.asLabel.Size = new System.Drawing.Size(106, 17);
            this.asLabel.TabIndex = 11;
            this.asLabel.Text = "Actuator Stroke";
            // 
            // asTextBox
            // 
            this.asTextBox.Location = new System.Drawing.Point(174, 186);
            this.asTextBox.Name = "asTextBox";
            this.asTextBox.Size = new System.Drawing.Size(137, 22);
            this.asTextBox.TabIndex = 10;
            this.asTextBox.TextChanged += new System.EventHandler(this.asTextBox_TextChanged);
            // 
            // alLabel
            // 
            this.alLabel.AutoSize = true;
            this.alLabel.Location = new System.Drawing.Point(338, 163);
            this.alLabel.Name = "alLabel";
            this.alLabel.Size = new System.Drawing.Size(109, 17);
            this.alLabel.TabIndex = 13;
            this.alLabel.Text = "Actuator Length";
            // 
            // alTextBox
            // 
            this.alTextBox.Location = new System.Drawing.Point(338, 186);
            this.alTextBox.Name = "alTextBox";
            this.alTextBox.Size = new System.Drawing.Size(137, 22);
            this.alTextBox.TabIndex = 12;
            this.alTextBox.TextChanged += new System.EventHandler(this.alTextBox_TextChanged);
            // 
            // hoxLabel
            // 
            this.hoxLabel.AutoSize = true;
            this.hoxLabel.Location = new System.Drawing.Point(12, 220);
            this.hoxLabel.Name = "hoxLabel";
            this.hoxLabel.Size = new System.Drawing.Size(97, 17);
            this.hoxLabel.TabIndex = 15;
            this.hoxLabel.Text = "Head Offset X";
            // 
            // hoxTextBox
            // 
            this.hoxTextBox.Location = new System.Drawing.Point(12, 243);
            this.hoxTextBox.Name = "hoxTextBox";
            this.hoxTextBox.Size = new System.Drawing.Size(137, 22);
            this.hoxTextBox.TabIndex = 14;
            this.hoxTextBox.TextChanged += new System.EventHandler(this.hoxTextBox_TextChanged);
            // 
            // hoyLabel
            // 
            this.hoyLabel.AutoSize = true;
            this.hoyLabel.Location = new System.Drawing.Point(174, 220);
            this.hoyLabel.Name = "hoyLabel";
            this.hoyLabel.Size = new System.Drawing.Size(97, 17);
            this.hoyLabel.TabIndex = 17;
            this.hoyLabel.Text = "Head Offset Y";
            // 
            // hoyTextBox
            // 
            this.hoyTextBox.Location = new System.Drawing.Point(174, 243);
            this.hoyTextBox.Name = "hoyTextBox";
            this.hoyTextBox.Size = new System.Drawing.Size(137, 22);
            this.hoyTextBox.TabIndex = 16;
            this.hoyTextBox.TextChanged += new System.EventHandler(this.hoyTextBox_TextChanged);
            // 
            // hozLabel
            // 
            this.hozLabel.AutoSize = true;
            this.hozLabel.Location = new System.Drawing.Point(338, 220);
            this.hozLabel.Name = "hozLabel";
            this.hozLabel.Size = new System.Drawing.Size(97, 17);
            this.hozLabel.TabIndex = 19;
            this.hozLabel.Text = "Head Offset Z";
            // 
            // hozTextBox
            // 
            this.hozTextBox.Location = new System.Drawing.Point(338, 243);
            this.hozTextBox.Name = "hozTextBox";
            this.hozTextBox.Size = new System.Drawing.Size(137, 22);
            this.hozTextBox.TabIndex = 18;
            this.hozTextBox.TextChanged += new System.EventHandler(this.hozTextBox_TextChanged);
            // 
            // accScaleLabel
            // 
            this.accScaleLabel.AutoSize = true;
            this.accScaleLabel.Location = new System.Drawing.Point(11, 282);
            this.accScaleLabel.Name = "accScaleLabel";
            this.accScaleLabel.Size = new System.Drawing.Size(125, 17);
            this.accScaleLabel.TabIndex = 21;
            this.accScaleLabel.Text = "Acceleration Scale";
            // 
            // accScaleTextBox
            // 
            this.accScaleTextBox.Location = new System.Drawing.Point(11, 305);
            this.accScaleTextBox.Name = "accScaleTextBox";
            this.accScaleTextBox.Size = new System.Drawing.Size(137, 22);
            this.accScaleTextBox.TabIndex = 20;
            this.accScaleTextBox.TextChanged += new System.EventHandler(this.accScaleTextBox_TextChanged);
            // 
            // accMaxLabel
            // 
            this.accMaxLabel.AutoSize = true;
            this.accMaxLabel.Location = new System.Drawing.Point(174, 282);
            this.accMaxLabel.Name = "accMaxLabel";
            this.accMaxLabel.Size = new System.Drawing.Size(148, 17);
            this.accMaxLabel.TabIndex = 23;
            this.accMaxLabel.Text = "Acceleration Maximum";
            // 
            // accMaxTextBox
            // 
            this.accMaxTextBox.Location = new System.Drawing.Point(174, 305);
            this.accMaxTextBox.Name = "accMaxTextBox";
            this.accMaxTextBox.Size = new System.Drawing.Size(137, 22);
            this.accMaxTextBox.TabIndex = 22;
            this.accMaxTextBox.TextChanged += new System.EventHandler(this.accMaxTextBox_TextChanged);
            // 
            // enabledCheckBox
            // 
            this.enabledCheckBox.AutoSize = true;
            this.enabledCheckBox.Location = new System.Drawing.Point(13, 63);
            this.enabledCheckBox.Name = "enabledCheckBox";
            this.enabledCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.enabledCheckBox.Size = new System.Drawing.Size(82, 21);
            this.enabledCheckBox.TabIndex = 24;
            this.enabledCheckBox.Text = "Enabled";
            this.enabledCheckBox.UseVisualStyleBackColor = true;
            this.enabledCheckBox.CheckedChanged += new System.EventHandler(this.enabledCheckBox_CheckedChanged);
            // 
            // MotionUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 358);
            this.Controls.Add(this.enabledCheckBox);
            this.Controls.Add(this.accMaxLabel);
            this.Controls.Add(this.accMaxTextBox);
            this.Controls.Add(this.accScaleLabel);
            this.Controls.Add(this.accScaleTextBox);
            this.Controls.Add(this.hozLabel);
            this.Controls.Add(this.hozTextBox);
            this.Controls.Add(this.hoyLabel);
            this.Controls.Add(this.hoyTextBox);
            this.Controls.Add(this.hoxLabel);
            this.Controls.Add(this.hoxTextBox);
            this.Controls.Add(this.alLabel);
            this.Controls.Add(this.alTextBox);
            this.Controls.Add(this.asLabel);
            this.Controls.Add(this.asTextBox);
            this.Controls.Add(this.avoLabel);
            this.Controls.Add(this.avoTextBox);
            this.Controls.Add(this.rigLengthLabel);
            this.Controls.Add(this.rigLengthTextBox);
            this.Controls.Add(this.rigWidthLabel);
            this.Controls.Add(this.rigWidthTextBox);
            this.Controls.Add(this.saveButton);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MotionUI";
            this.Text = "MotionUI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.MotionUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox rigWidthTextBox;
        private System.Windows.Forms.Label rigWidthLabel;
        private System.Windows.Forms.Label rigLengthLabel;
        private System.Windows.Forms.TextBox rigLengthTextBox;
        private System.Windows.Forms.Label avoLabel;
        private System.Windows.Forms.TextBox avoTextBox;
        private System.Windows.Forms.Label asLabel;
        private System.Windows.Forms.TextBox asTextBox;
        private System.Windows.Forms.Label alLabel;
        private System.Windows.Forms.TextBox alTextBox;
        private System.Windows.Forms.Label hoxLabel;
        private System.Windows.Forms.TextBox hoxTextBox;
        private System.Windows.Forms.Label hoyLabel;
        private System.Windows.Forms.TextBox hoyTextBox;
        private System.Windows.Forms.Label hozLabel;
        private System.Windows.Forms.TextBox hozTextBox;
        private System.Windows.Forms.Label accScaleLabel;
        private System.Windows.Forms.TextBox accScaleTextBox;
        private System.Windows.Forms.Label accMaxLabel;
        private System.Windows.Forms.TextBox accMaxTextBox;
        private System.Windows.Forms.CheckBox enabledCheckBox;
    }
}