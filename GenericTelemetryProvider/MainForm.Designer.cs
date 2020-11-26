namespace GenericTelemetryProvider
{
    partial class MainForm
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Dirt5Button = new System.Windows.Forms.Button();
            this.Filters = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Dirt5Button
            // 
            this.Dirt5Button.Location = new System.Drawing.Point(12, 12);
            this.Dirt5Button.Name = "Dirt5Button";
            this.Dirt5Button.Size = new System.Drawing.Size(149, 44);
            this.Dirt5Button.TabIndex = 0;
            this.Dirt5Button.Text = "DIRT5";
            this.Dirt5Button.UseVisualStyleBackColor = true;
            this.Dirt5Button.Click += new System.EventHandler(this.Dirt5Button_Click);
            // 
            // Filters
            // 
            this.Filters.Location = new System.Drawing.Point(249, 12);
            this.Filters.Name = "Filters";
            this.Filters.Size = new System.Drawing.Size(159, 44);
            this.Filters.TabIndex = 1;
            this.Filters.Text = "Filters";
            this.Filters.UseVisualStyleBackColor = true;
            this.Filters.Click += new System.EventHandler(this.Filters_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 156);
            this.Controls.Add(this.Filters);
            this.Controls.Add(this.Dirt5Button);
            this.Name = "MainForm";
            this.Text = "GenericTelemetryProvider";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Dirt5Button;
        private System.Windows.Forms.Button Filters;
    }
}

