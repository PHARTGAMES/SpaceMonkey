namespace GenericTelemetryProvider
{
    partial class OutputUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputUI));
            this.flowLayoutPanelOutputs = new System.Windows.Forms.FlowLayoutPanel();
            this.outputTypesComboBox = new System.Windows.Forms.ComboBox();
            this.addOutputBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowLayoutPanelOutputs
            // 
            this.flowLayoutPanelOutputs.Location = new System.Drawing.Point(12, 60);
            this.flowLayoutPanelOutputs.Name = "flowLayoutPanelOutputs";
            this.flowLayoutPanelOutputs.Size = new System.Drawing.Size(503, 682);
            this.flowLayoutPanelOutputs.TabIndex = 0;
            // 
            // outputTypesComboBox
            // 
            this.outputTypesComboBox.FormattingEnabled = true;
            this.outputTypesComboBox.Location = new System.Drawing.Point(12, 12);
            this.outputTypesComboBox.Name = "outputTypesComboBox";
            this.outputTypesComboBox.Size = new System.Drawing.Size(273, 24);
            this.outputTypesComboBox.TabIndex = 1;
            this.outputTypesComboBox.SelectedIndexChanged += new System.EventHandler(this.outputTypesComboBox_SelectedIndexChanged);
            // 
            // addOutputBtn
            // 
            this.addOutputBtn.Location = new System.Drawing.Point(292, 12);
            this.addOutputBtn.Name = "addOutputBtn";
            this.addOutputBtn.Size = new System.Drawing.Size(109, 42);
            this.addOutputBtn.TabIndex = 2;
            this.addOutputBtn.Text = "<- Add Output";
            this.addOutputBtn.UseVisualStyleBackColor = true;
            this.addOutputBtn.Click += new System.EventHandler(this.addOutputBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(407, 13);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(106, 41);
            this.saveBtn.TabIndex = 3;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // OutputUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 754);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.addOutputBtn);
            this.Controls.Add(this.outputTypesComboBox);
            this.Controls.Add(this.flowLayoutPanelOutputs);
            this.Icon = GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Name = "OutputUI";
            this.Text = "OutputUI";
            this.Load += new System.EventHandler(this.OutputUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelOutputs;
        private System.Windows.Forms.ComboBox outputTypesComboBox;
        private System.Windows.Forms.Button addOutputBtn;
        private System.Windows.Forms.Button saveBtn;
    }
}