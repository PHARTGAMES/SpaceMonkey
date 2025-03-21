﻿namespace GenericTelemetryProvider
{
    partial class SmoothFilterControl
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
            this.stepCount = new System.Windows.Forms.TextBox();
            this.nestCount = new System.Windows.Forms.TextBox();
            this.maxDelta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.Location = new System.Drawing.Point(72, 0);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(142, 26);
            this.heading.TabIndex = 0;
            this.heading.Text = "Smooth Filter";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(6, 69);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(87, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // stepCount
            // 
            this.stepCount.Location = new System.Drawing.Point(99, 42);
            this.stepCount.Name = "stepCount";
            this.stepCount.Size = new System.Drawing.Size(87, 20);
            this.stepCount.TabIndex = 2;
            this.stepCount.Text = "2";
            this.stepCount.TextChanged += new System.EventHandler(this.stepCount_TextChanged);
            // 
            // nestCount
            // 
            this.nestCount.Location = new System.Drawing.Point(6, 43);
            this.nestCount.Name = "nestCount";
            this.nestCount.Size = new System.Drawing.Size(87, 20);
            this.nestCount.TabIndex = 3;
            this.nestCount.Text = "2";
            this.nestCount.TextChanged += new System.EventHandler(this.nestCount_TextChanged);
            // 
            // maxDelta
            // 
            this.maxDelta.Location = new System.Drawing.Point(192, 42);
            this.maxDelta.Name = "maxDelta";
            this.maxDelta.Size = new System.Drawing.Size(87, 20);
            this.maxDelta.TabIndex = 4;
            this.maxDelta.Text = "0.5";
            this.maxDelta.TextChanged += new System.EventHandler(this.maxDelta_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "NestCount";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "SampleCount";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "MaxDelta";
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(99, 68);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(87, 23);
            this.upButton.TabIndex = 8;
            this.upButton.Text = "Move UP";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(192, 68);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(87, 23);
            this.downButton.TabIndex = 9;
            this.downButton.Text = "Move DOWN";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // SmoothFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maxDelta);
            this.Controls.Add(this.nestCount);
            this.Controls.Add(this.stepCount);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.heading);
            this.Name = "SmoothFilterControl";
            this.Size = new System.Drawing.Size(284, 98);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TextBox stepCount;
        private System.Windows.Forms.TextBox nestCount;
        private System.Windows.Forms.TextBox maxDelta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
    }
}
