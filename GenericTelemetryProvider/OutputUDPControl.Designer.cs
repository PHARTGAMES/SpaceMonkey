namespace GenericTelemetryProvider
{
    partial class OutputUDPControl
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
            this.packetFormatComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.destinationFindButton = new System.Windows.Forms.Button();
            this.formatDestinationsBox = new System.Windows.Forms.ComboBox();
            this.deleteDestinationButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.udpIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.udpPort = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // heading
            // 
            this.heading.AutoSize = true;
            this.heading.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.heading.Location = new System.Drawing.Point(1, 1);
            this.heading.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.heading.Name = "heading";
            this.heading.Size = new System.Drawing.Size(161, 31);
            this.heading.TabIndex = 0;
            this.heading.Text = "Output UDP";
            this.heading.Click += new System.EventHandler(this.heading_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(193, 8);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 28);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // packetFormatComboBox
            // 
            this.packetFormatComboBox.FormattingEnabled = true;
            this.packetFormatComboBox.Location = new System.Drawing.Point(7, 60);
            this.packetFormatComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.packetFormatComboBox.Name = "packetFormatComboBox";
            this.packetFormatComboBox.Size = new System.Drawing.Size(244, 24);
            this.packetFormatComboBox.TabIndex = 17;
            this.packetFormatComboBox.SelectedIndexChanged += new System.EventHandler(this.packetFormatComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 105);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 17);
            this.label3.TabIndex = 10;
            this.label3.Text = "Packet Format Destinations";
            // 
            // destinationFindButton
            // 
            this.destinationFindButton.Location = new System.Drawing.Point(222, 105);
            this.destinationFindButton.Margin = new System.Windows.Forms.Padding(4);
            this.destinationFindButton.Name = "destinationFindButton";
            this.destinationFindButton.Size = new System.Drawing.Size(34, 30);
            this.destinationFindButton.TabIndex = 11;
            this.destinationFindButton.Text = "...";
            this.destinationFindButton.UseVisualStyleBackColor = true;
            this.destinationFindButton.Click += new System.EventHandler(this.destinationFindButton_Click);
            // 
            // formatDestinationsBox
            // 
            this.formatDestinationsBox.FormattingEnabled = true;
            this.formatDestinationsBox.Location = new System.Drawing.Point(7, 124);
            this.formatDestinationsBox.Margin = new System.Windows.Forms.Padding(4);
            this.formatDestinationsBox.Name = "formatDestinationsBox";
            this.formatDestinationsBox.Size = new System.Drawing.Size(203, 24);
            this.formatDestinationsBox.TabIndex = 12;
            this.formatDestinationsBox.SelectedIndexChanged += new System.EventHandler(this.formatDestinationsBox_SelectedIndexChanged);
            // 
            // deleteDestinationButton
            // 
            this.deleteDestinationButton.Location = new System.Drawing.Point(224, 143);
            this.deleteDestinationButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteDestinationButton.Name = "deleteDestinationButton";
            this.deleteDestinationButton.Size = new System.Drawing.Size(34, 30);
            this.deleteDestinationButton.TabIndex = 13;
            this.deleteDestinationButton.Text = "-";
            this.deleteDestinationButton.UseVisualStyleBackColor = true;
            this.deleteDestinationButton.Click += new System.EventHandler(this.deleteDestinationButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 41);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 17);
            this.label4.TabIndex = 18;
            this.label4.Text = "Packet Format";
            // 
            // udpIP
            // 
            this.udpIP.Location = new System.Drawing.Point(7, 188);
            this.udpIP.Margin = new System.Windows.Forms.Padding(4);
            this.udpIP.Name = "udpIP";
            this.udpIP.Size = new System.Drawing.Size(166, 22);
            this.udpIP.TabIndex = 6;
            this.udpIP.Text = "127.0.0.1";
            this.udpIP.TextChanged += new System.EventHandler(this.udpIP_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 169);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 17);
            this.label1.TabIndex = 5;
            this.label1.Text = "UDP IP";
            // 
            // udpPort
            // 
            this.udpPort.Location = new System.Drawing.Point(7, 243);
            this.udpPort.Margin = new System.Windows.Forms.Padding(4);
            this.udpPort.Name = "udpPort";
            this.udpPort.Size = new System.Drawing.Size(70, 22);
            this.udpPort.TabIndex = 7;
            this.udpPort.TextChanged += new System.EventHandler(this.udpPort_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 224);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Port";
            // 
            // OutputUDPControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.heading);
            this.Controls.Add(this.packetFormatComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.destinationFindButton);
            this.Controls.Add(this.formatDestinationsBox);
            this.Controls.Add(this.deleteDestinationButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.udpIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.udpPort);
            this.Controls.Add(this.label2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OutputUDPControl";
            this.Size = new System.Drawing.Size(320, 287);
            this.Load += new System.EventHandler(this.OutputUDPControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label heading;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ComboBox packetFormatComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button destinationFindButton;
        private System.Windows.Forms.ComboBox formatDestinationsBox;
        private System.Windows.Forms.Button deleteDestinationButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox udpIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox udpPort;
        private System.Windows.Forms.Label label2;
    }
}
