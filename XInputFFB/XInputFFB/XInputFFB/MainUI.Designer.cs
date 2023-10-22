
namespace XInputFFB
{
    partial class MainUI
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
            this.DetectInput = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // DetectInput
            // 
            this.DetectInput.Location = new System.Drawing.Point(220, 108);
            this.DetectInput.Name = "DetectInput";
            this.DetectInput.Size = new System.Drawing.Size(161, 50);
            this.DetectInput.TabIndex = 0;
            this.DetectInput.Text = "DetectInput";
            this.DetectInput.UseVisualStyleBackColor = true;
            this.DetectInput.Click += new System.EventHandler(this.DetectInput_Click);
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DetectInput);
            this.Name = "MainUI";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DetectInput;
    }
}

