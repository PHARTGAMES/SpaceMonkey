
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
            this.devicesFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mappingFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // DetectInput
            // 
            this.DetectInput.Location = new System.Drawing.Point(1045, 674);
            this.DetectInput.Name = "DetectInput";
            this.DetectInput.Size = new System.Drawing.Size(161, 50);
            this.DetectInput.TabIndex = 0;
            this.DetectInput.Text = "DetectInput";
            this.DetectInput.UseVisualStyleBackColor = true;
            this.DetectInput.Click += new System.EventHandler(this.DetectInput_Click);
            // 
            // devicesFlowPanel
            // 
            this.devicesFlowPanel.AutoScroll = true;
            this.devicesFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.devicesFlowPanel.Location = new System.Drawing.Point(12, 12);
            this.devicesFlowPanel.Name = "devicesFlowPanel";
            this.devicesFlowPanel.Size = new System.Drawing.Size(967, 308);
            this.devicesFlowPanel.TabIndex = 1;
            // 
            // mappingFlowPanel
            // 
            this.mappingFlowPanel.AutoScroll = true;
            this.mappingFlowPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mappingFlowPanel.Location = new System.Drawing.Point(12, 326);
            this.mappingFlowPanel.Name = "mappingFlowPanel";
            this.mappingFlowPanel.Size = new System.Drawing.Size(967, 398);
            this.mappingFlowPanel.TabIndex = 3;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 736);
            this.Controls.Add(this.mappingFlowPanel);
            this.Controls.Add(this.devicesFlowPanel);
            this.Controls.Add(this.DetectInput);
            this.Name = "MainUI";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button DetectInput;
        private System.Windows.Forms.FlowLayoutPanel devicesFlowPanel;
        private System.Windows.Forms.FlowLayoutPanel mappingFlowPanel;
    }
}

