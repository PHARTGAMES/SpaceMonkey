namespace GenericTelemetryProvider
{
    partial class FilterUI
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterUI));
            this.filterChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.keyComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutFilters = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.filterChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // filterChart
            // 
            chartArea1.Name = "ChartArea1";
            this.filterChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.filterChart.Legends.Add(legend1);
            this.filterChart.Location = new System.Drawing.Point(12, 12);
            this.filterChart.Name = "filterChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.filterChart.Series.Add(series1);
            this.filterChart.Size = new System.Drawing.Size(2377, 752);
            this.filterChart.TabIndex = 0;
            this.filterChart.Text = "FilterChart";
            this.filterChart.Click += new System.EventHandler(this.filterChart_Click);
            // 
            // keyComboBox
            // 
            this.keyComboBox.FormattingEnabled = true;
            this.keyComboBox.Location = new System.Drawing.Point(2395, 12);
            this.keyComboBox.Name = "keyComboBox";
            this.keyComboBox.Size = new System.Drawing.Size(338, 21);
            this.keyComboBox.TabIndex = 1;
            this.keyComboBox.SelectedIndexChanged += new System.EventHandler(this.keyComboBox_SelectedIndexChanged);
            // 
            // flowLayoutFilters
            // 
            this.flowLayoutFilters.Location = new System.Drawing.Point(2395, 39);
            this.flowLayoutFilters.Name = "flowLayoutFilters";
            this.flowLayoutFilters.Size = new System.Drawing.Size(338, 725);
            this.flowLayoutFilters.TabIndex = 2;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(2149, 12);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(240, 21);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "SAVE";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(2183, 97);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(175, 45);
            this.trackBar1.TabIndex = 4;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // FilterUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2745, 776);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.flowLayoutFilters);
            this.Controls.Add(this.keyComboBox);
            this.Controls.Add(this.filterChart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FilterUI";
            this.Text = "FilterUI";
            this.Load += new System.EventHandler(this.FilterUI_Load);
            ((System.ComponentModel.ISupportInitialize)(this.filterChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart filterChart;
        private System.Windows.Forms.ComboBox keyComboBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutFilters;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}