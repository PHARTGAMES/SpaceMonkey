namespace GenericTelemetryProvider
{
    partial class KalmanVelocityFilterControl
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
            this.H = new System.Windows.Forms.TextBox();
            this.A = new System.Windows.Forms.TextBox();
            this.Q = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.upButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.R = new System.Windows.Forms.TextBox();
            this.P = new System.Windows.Forms.TextBox();
            this.X = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.Location = new System.Drawing.Point(38, 0);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(203, 26);
            this.heading.TabIndex = 0;
            this.heading.Text = "Kalman Noise Filter";
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(6, 109);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(87, 23);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // H
            // 
            this.H.Location = new System.Drawing.Point(99, 42);
            this.H.Name = "H";
            this.H.Size = new System.Drawing.Size(87, 20);
            this.H.TabIndex = 2;
            this.H.Text = "2";
            this.H.TextChanged += new System.EventHandler(this.H_TextChanged);
            // 
            // A
            // 
            this.A.Location = new System.Drawing.Point(6, 43);
            this.A.Name = "A";
            this.A.Size = new System.Drawing.Size(87, 20);
            this.A.TabIndex = 3;
            this.A.Text = "2";
            this.A.TextChanged += new System.EventHandler(this.A_TextChanged);
            // 
            // Q
            // 
            this.Q.Location = new System.Drawing.Point(192, 42);
            this.Q.Name = "Q";
            this.Q.Size = new System.Drawing.Size(87, 20);
            this.Q.TabIndex = 4;
            this.Q.Text = "0.5";
            this.Q.TextChanged += new System.EventHandler(this.Q_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "A";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "H";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(189, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Q";
            // 
            // upButton
            // 
            this.upButton.Location = new System.Drawing.Point(99, 108);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(87, 23);
            this.upButton.TabIndex = 8;
            this.upButton.Text = "Move UP";
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // downButton
            // 
            this.downButton.Location = new System.Drawing.Point(192, 108);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(87, 23);
            this.downButton.TabIndex = 9;
            this.downButton.Text = "Move DOWN";
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // R
            // 
            this.R.Location = new System.Drawing.Point(6, 82);
            this.R.Name = "R";
            this.R.Size = new System.Drawing.Size(87, 20);
            this.R.TabIndex = 10;
            this.R.Text = "2";
            this.R.TextChanged += new System.EventHandler(this.R_TextChanged);
            // 
            // P
            // 
            this.P.Location = new System.Drawing.Point(99, 82);
            this.P.Name = "P";
            this.P.Size = new System.Drawing.Size(87, 20);
            this.P.TabIndex = 11;
            this.P.Text = "2";
            this.P.TextChanged += new System.EventHandler(this.P_TextChanged);
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(192, 82);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(87, 20);
            this.X.TabIndex = 12;
            this.X.Text = "2";
            this.X.TextChanged += new System.EventHandler(this.X_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "R";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(97, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Initial P";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(190, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Initial X";
            // 
            // KalmanFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.X);
            this.Controls.Add(this.P);
            this.Controls.Add(this.R);
            this.Controls.Add(this.downButton);
            this.Controls.Add(this.upButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Q);
            this.Controls.Add(this.A);
            this.Controls.Add(this.H);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.heading);
            this.Name = "KalmanFilterControl";
            this.Size = new System.Drawing.Size(284, 138);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TextBox H;
        private System.Windows.Forms.TextBox A;
        private System.Windows.Forms.TextBox Q;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.TextBox R;
        private System.Windows.Forms.TextBox P;
        private System.Windows.Forms.TextBox X;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}
