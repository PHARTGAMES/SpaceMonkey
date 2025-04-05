namespace GenericTelemetryProvider
{
    partial class Wreckfest2UI
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
            this.statusLabel = new System.Windows.Forms.TextBox();
            this.initializeLobbyButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.matrixBox = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gamerTagTextBox = new System.Windows.Forms.TextBox();
            this.initializeIngameButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // statusLabel
            // 
            this.statusLabel.BackColor = System.Drawing.SystemColors.Control;
            this.statusLabel.Location = new System.Drawing.Point(12, 13);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(459, 20);
            this.statusLabel.TabIndex = 5;
            this.statusLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.statusLabel.TextChanged += new System.EventHandler(this.statusLabel_TextChanged);
            // 
            // initializeLobbyButton
            // 
            this.initializeLobbyButton.Location = new System.Drawing.Point(31, 71);
            this.initializeLobbyButton.Name = "initializeLobbyButton";
            this.initializeLobbyButton.Size = new System.Drawing.Size(199, 46);
            this.initializeLobbyButton.TabIndex = 6;
            this.initializeLobbyButton.Text = "Initialize at Lobby";
            this.initializeLobbyButton.UseVisualStyleBackColor = true;
            this.initializeLobbyButton.Click += new System.EventHandler(this.initializeButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(31, 123);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(425, 23);
            this.progressBar1.TabIndex = 7;
            this.progressBar1.Click += new System.EventHandler(this.progressBar1_Click);
            // 
            // matrixBox
            // 
            this.matrixBox.Location = new System.Drawing.Point(12, 152);
            this.matrixBox.Name = "matrixBox";
            this.matrixBox.Size = new System.Drawing.Size(459, 973);
            this.matrixBox.TabIndex = 8;
            this.matrixBox.Text = "";
            this.matrixBox.TextChanged += new System.EventHandler(this.matrixBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "1. Enter Gamer Tag";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "2.";
            // 
            // gamerTagTextBox
            // 
            this.gamerTagTextBox.Location = new System.Drawing.Point(115, 45);
            this.gamerTagTextBox.Name = "gamerTagTextBox";
            this.gamerTagTextBox.Size = new System.Drawing.Size(341, 20);
            this.gamerTagTextBox.TabIndex = 16;
            this.gamerTagTextBox.TextChanged += new System.EventHandler(this.gamerTagTextBox_TextChanged);
            // 
            // initializeIngameButton
            // 
            this.initializeIngameButton.Location = new System.Drawing.Point(251, 71);
            this.initializeIngameButton.Name = "initializeIngameButton";
            this.initializeIngameButton.Size = new System.Drawing.Size(205, 46);
            this.initializeIngameButton.TabIndex = 17;
            this.initializeIngameButton.Text = "Initialize In Game";
            this.initializeIngameButton.UseVisualStyleBackColor = true;
            this.initializeIngameButton.Click += new System.EventHandler(this.initializeIngameButton_Click);
            // 
            // Wreckfest2UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 1137);
            this.Controls.Add(this.initializeIngameButton);
            this.Controls.Add(this.gamerTagTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.matrixBox);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.initializeLobbyButton);
            this.Controls.Add(this.statusLabel);
            this.Icon = global::GenericTelemetryProvider.Properties.Resources.GTPIcon;
            this.Name = "Wreckfest2UI";
            this.Text = "Wreckfest2UI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox statusLabel;
        private System.Windows.Forms.Button initializeLobbyButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox matrixBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox gamerTagTextBox;
        private System.Windows.Forms.Button initializeIngameButton;
    }
}