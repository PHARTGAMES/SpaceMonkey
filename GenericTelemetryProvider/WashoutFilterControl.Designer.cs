namespace GenericTelemetryProvider
{
    partial class WashoutFilterControl
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
            this.heading = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.timeConstantTextBox = new System.Windows.Forms.TextBox();
            this.timeConstantLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.Location = new System.Drawing.Point(92, 0);
            this.heading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(189, 31);
            this.heading.TabIndex = 0;
            this.heading.Text = "Washout Filter";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(8, 107);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 28);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(132, 107);
            this.upButton.Margin = new System.Windows.Forms.Padding(4);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(116, 28);
            this.upButton.TabIndex = 8;
            this.upButton.Text = "Move UP";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(256, 107);
            this.downButton.Margin = new System.Windows.Forms.Padding(4);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(116, 28);
            this.downButton.TabIndex = 9;
            this.downButton.Text = "Move DOWN";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // timeConstantTextBox
            // 
            this.timeConstantTextBox.Location = new System.Drawing.Point(132, 63);
            this.timeConstantTextBox.Name = "timeConstantTextBox";
            this.timeConstantTextBox.Size = new System.Drawing.Size(100, 22);
            this.timeConstantTextBox.TabIndex = 10;
            // 
            // timeConstantLabel
            // 
            this.timeConstantLabel.AutoSize = true;
            this.timeConstantLabel.Location = new System.Drawing.Point(132, 40);
            this.timeConstantLabel.Name = "timeConstantLabel";
            this.timeConstantLabel.Size = new System.Drawing.Size(99, 17);
            this.timeConstantLabel.TabIndex = 11;
            this.timeConstantLabel.Text = "Time Constant";
            // 
            // WashoutFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeConstantLabel);
            this.Controls.Add(this.timeConstantTextBox);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.heading);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WashoutFilterControl";
            this.Size = new System.Drawing.Size(379, 139);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.TextBox timeConstantTextBox;
        private System.Windows.Forms.Label timeConstantLabel;
    }
}
