
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
            this.devicesFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mappingFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbComPort = new System.Windows.Forms.ComboBox();
            this.lblComPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
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
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(988, 664);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(218, 60);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbComPort
            // 
            this.cbComPort.FormattingEnabled = true;
            this.cbComPort.Location = new System.Drawing.Point(1060, 6);
            this.cbComPort.Name = "cbComPort";
            this.cbComPort.Size = new System.Drawing.Size(146, 24);
            this.cbComPort.TabIndex = 5;
            this.cbComPort.SelectedIndexChanged += new System.EventHandler(this.cbComPort_SelectedIndexChanged);
            // 
            // lblComPort
            // 
            this.lblComPort.AutoSize = true;
            this.lblComPort.Location = new System.Drawing.Point(985, 9);
            this.lblComPort.Name = "lblComPort";
            this.lblComPort.Size = new System.Drawing.Size(73, 17);
            this.lblComPort.TabIndex = 6;
            this.lblComPort.Text = "COM Port:";
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1218, 736);
            this.Controls.Add(this.lblComPort);
            this.Controls.Add(this.cbComPort);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.mappingFlowPanel);
            this.Controls.Add(this.devicesFlowPanel);
            this.Name = "MainUI";
            this.Text = "XInputFFB";
            this.Load += new System.EventHandler(this.MainUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.FlowLayoutPanel devicesFlowPanel;
        private System.Windows.Forms.FlowLayoutPanel mappingFlowPanel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbComPort;
        private System.Windows.Forms.Label lblComPort;
    }
}

