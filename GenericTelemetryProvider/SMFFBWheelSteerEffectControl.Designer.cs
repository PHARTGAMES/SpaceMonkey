namespace GenericTelemetryProvider
{
    partial class SMFFBWheelSteerEffectControl
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
            this.deleteButton = new System.Windows.Forms.Button();
            this.maxWheelAngle = new System.Windows.Forms.TextBox();
            this.maxWheelAngleLabel = new System.Windows.Forms.Label();
            this.effectGroupBox = new System.Windows.Forms.GroupBox();
            this.dampForceSpeedCurveLbl = new System.Windows.Forms.Label();
            this.dampForceSpeedCurve = new System.Windows.Forms.TextBox();
            this.maxDampForceLbl = new System.Windows.Forms.Label();
            this.maxDampForce = new System.Windows.Forms.TextBox();
            this.minDampForceLbl = new System.Windows.Forms.Label();
            this.minDampForce = new System.Windows.Forms.TextBox();
            this.constantForceSpeedCurveLbl = new System.Windows.Forms.Label();
            this.constantForceSpeedCurve = new System.Windows.Forms.TextBox();
            this.maxConstantForceLbl = new System.Windows.Forms.Label();
            this.maxConstantForce = new System.Windows.Forms.TextBox();
            this.minConstantForceLbl = new System.Windows.Forms.Label();
            this.minConstantForce = new System.Windows.Forms.TextBox();
            this.constantForceAngleCurveLbl = new System.Windows.Forms.Label();
            this.constantForceAngleCurve = new System.Windows.Forms.TextBox();
            this.effectGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(17, 342);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(116, 28);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "DELETE";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // maxWheelAngle
            // 
            this.maxWheelAngle.Location = new System.Drawing.Point(302, 74);
            this.maxWheelAngle.Margin = new System.Windows.Forms.Padding(4);
            this.maxWheelAngle.Name = "maxWheelAngle";
            this.maxWheelAngle.Size = new System.Drawing.Size(115, 25);
            this.maxWheelAngle.TabIndex = 3;
            this.maxWheelAngle.Text = "0";
            this.maxWheelAngle.TextChanged += new System.EventHandler(this.maxWheelAngle_TextChanged);
            // 
            // maxWheelAngleLabel
            // 
            this.maxWheelAngleLabel.AutoSize = true;
            this.maxWheelAngleLabel.Location = new System.Drawing.Point(299, 50);
            this.maxWheelAngleLabel.Name = "maxWheelAngleLabel";
            this.maxWheelAngleLabel.Size = new System.Drawing.Size(132, 20);
            this.maxWheelAngleLabel.TabIndex = 7;
            this.maxWheelAngleLabel.Text = "Max Wheel Angle";
            // 
            // effectGroupBox
            // 
            this.effectGroupBox.Controls.Add(this.dampForceSpeedCurveLbl);
            this.effectGroupBox.Controls.Add(this.dampForceSpeedCurve);
            this.effectGroupBox.Controls.Add(this.maxDampForceLbl);
            this.effectGroupBox.Controls.Add(this.maxDampForce);
            this.effectGroupBox.Controls.Add(this.minDampForceLbl);
            this.effectGroupBox.Controls.Add(this.minDampForce);
            this.effectGroupBox.Controls.Add(this.constantForceSpeedCurveLbl);
            this.effectGroupBox.Controls.Add(this.constantForceSpeedCurve);
            this.effectGroupBox.Controls.Add(this.maxConstantForceLbl);
            this.effectGroupBox.Controls.Add(this.maxConstantForce);
            this.effectGroupBox.Controls.Add(this.minConstantForceLbl);
            this.effectGroupBox.Controls.Add(this.minConstantForce);
            this.effectGroupBox.Controls.Add(this.constantForceAngleCurveLbl);
            this.effectGroupBox.Controls.Add(this.constantForceAngleCurve);
            this.effectGroupBox.Controls.Add(this.deleteButton);
            this.effectGroupBox.Controls.Add(this.maxWheelAngleLabel);
            this.effectGroupBox.Controls.Add(this.maxWheelAngle);
            this.effectGroupBox.Location = new System.Drawing.Point(14, 13);
            this.effectGroupBox.Name = "effectGroupBox";
            this.effectGroupBox.Size = new System.Drawing.Size(585, 400);
            this.effectGroupBox.TabIndex = 14;
            this.effectGroupBox.TabStop = false;
            this.effectGroupBox.Text = "Wheel Steer Effect";
            // 
            // dampForceSpeedCurveLbl
            // 
            this.dampForceSpeedCurveLbl.AutoSize = true;
            this.dampForceSpeedCurveLbl.Location = new System.Drawing.Point(299, 273);
            this.dampForceSpeedCurveLbl.Name = "dampForceSpeedCurveLbl";
            this.dampForceSpeedCurveLbl.Size = new System.Drawing.Size(193, 20);
            this.dampForceSpeedCurveLbl.TabIndex = 22;
            this.dampForceSpeedCurveLbl.Text = "Damp Force Speed Curve";
            // 
            // dampForceSpeedCurve
            // 
            this.dampForceSpeedCurve.Location = new System.Drawing.Point(302, 297);
            this.dampForceSpeedCurve.Margin = new System.Windows.Forms.Padding(4);
            this.dampForceSpeedCurve.Name = "dampForceSpeedCurve";
            this.dampForceSpeedCurve.Size = new System.Drawing.Size(115, 25);
            this.dampForceSpeedCurve.TabIndex = 21;
            this.dampForceSpeedCurve.Text = "0";
            this.dampForceSpeedCurve.TextChanged += new System.EventHandler(this.dampForceSpeedCurve_TextChanged);
            // 
            // maxDampForceLbl
            // 
            this.maxDampForceLbl.AutoSize = true;
            this.maxDampForceLbl.Location = new System.Drawing.Point(300, 197);
            this.maxDampForceLbl.Name = "maxDampForceLbl";
            this.maxDampForceLbl.Size = new System.Drawing.Size(130, 20);
            this.maxDampForceLbl.TabIndex = 20;
            this.maxDampForceLbl.Text = "Max Damp Force";
            // 
            // maxDampForce
            // 
            this.maxDampForce.Location = new System.Drawing.Point(303, 221);
            this.maxDampForce.Margin = new System.Windows.Forms.Padding(4);
            this.maxDampForce.Name = "maxDampForce";
            this.maxDampForce.Size = new System.Drawing.Size(115, 25);
            this.maxDampForce.TabIndex = 19;
            this.maxDampForce.Text = "0";
            this.maxDampForce.TextChanged += new System.EventHandler(this.maxDampForce_TextChanged);
            // 
            // minDampForceLbl
            // 
            this.minDampForceLbl.AutoSize = true;
            this.minDampForceLbl.Location = new System.Drawing.Point(299, 123);
            this.minDampForceLbl.Name = "minDampForceLbl";
            this.minDampForceLbl.Size = new System.Drawing.Size(126, 20);
            this.minDampForceLbl.TabIndex = 18;
            this.minDampForceLbl.Text = "Min Damp Force";
            // 
            // minDampForce
            // 
            this.minDampForce.Location = new System.Drawing.Point(302, 147);
            this.minDampForce.Margin = new System.Windows.Forms.Padding(4);
            this.minDampForce.Name = "minDampForce";
            this.minDampForce.Size = new System.Drawing.Size(115, 25);
            this.minDampForce.TabIndex = 17;
            this.minDampForce.Text = "0";
            this.minDampForce.TextChanged += new System.EventHandler(this.minDampForce_TextChanged);
            // 
            // constantForceSpeedCurveLbl
            // 
            this.constantForceSpeedCurveLbl.AutoSize = true;
            this.constantForceSpeedCurveLbl.Location = new System.Drawing.Point(26, 273);
            this.constantForceSpeedCurveLbl.Name = "constantForceSpeedCurveLbl";
            this.constantForceSpeedCurveLbl.Size = new System.Drawing.Size(219, 20);
            this.constantForceSpeedCurveLbl.TabIndex = 16;
            this.constantForceSpeedCurveLbl.Text = "Constant Force Speed Curve ";
            // 
            // constantForceSpeedCurve
            // 
            this.constantForceSpeedCurve.Location = new System.Drawing.Point(29, 297);
            this.constantForceSpeedCurve.Margin = new System.Windows.Forms.Padding(4);
            this.constantForceSpeedCurve.Name = "constantForceSpeedCurve";
            this.constantForceSpeedCurve.Size = new System.Drawing.Size(115, 25);
            this.constantForceSpeedCurve.TabIndex = 15;
            this.constantForceSpeedCurve.Text = "0";
            this.constantForceSpeedCurve.TextChanged += new System.EventHandler(this.constantForceSpeedCurve_TextChanged);
            // 
            // maxConstantForceLbl
            // 
            this.maxConstantForceLbl.AutoSize = true;
            this.maxConstantForceLbl.Location = new System.Drawing.Point(26, 197);
            this.maxConstantForceLbl.Name = "maxConstantForceLbl";
            this.maxConstantForceLbl.Size = new System.Drawing.Size(152, 20);
            this.maxConstantForceLbl.TabIndex = 14;
            this.maxConstantForceLbl.Text = "Max Constant Force";
            // 
            // maxConstantForce
            // 
            this.maxConstantForce.Location = new System.Drawing.Point(29, 221);
            this.maxConstantForce.Margin = new System.Windows.Forms.Padding(4);
            this.maxConstantForce.Name = "maxConstantForce";
            this.maxConstantForce.Size = new System.Drawing.Size(115, 25);
            this.maxConstantForce.TabIndex = 13;
            this.maxConstantForce.Text = "0";
            this.maxConstantForce.TextChanged += new System.EventHandler(this.maxConstantForce_TextChanged);
            // 
            // minConstantForceLbl
            // 
            this.minConstantForceLbl.AutoSize = true;
            this.minConstantForceLbl.Location = new System.Drawing.Point(25, 123);
            this.minConstantForceLbl.Name = "minConstantForceLbl";
            this.minConstantForceLbl.Size = new System.Drawing.Size(148, 20);
            this.minConstantForceLbl.TabIndex = 12;
            this.minConstantForceLbl.Text = "Min Constant Force";
            // 
            // minConstantForce
            // 
            this.minConstantForce.Location = new System.Drawing.Point(28, 147);
            this.minConstantForce.Margin = new System.Windows.Forms.Padding(4);
            this.minConstantForce.Name = "minConstantForce";
            this.minConstantForce.Size = new System.Drawing.Size(115, 25);
            this.minConstantForce.TabIndex = 11;
            this.minConstantForce.Text = "0";
            this.minConstantForce.TextChanged += new System.EventHandler(this.minConstantForce_TextChanged);
            // 
            // constantForceAngleCurveLbl
            // 
            this.constantForceAngleCurveLbl.AutoSize = true;
            this.constantForceAngleCurveLbl.Location = new System.Drawing.Point(26, 50);
            this.constantForceAngleCurveLbl.Name = "constantForceAngleCurveLbl";
            this.constantForceAngleCurveLbl.Size = new System.Drawing.Size(209, 20);
            this.constantForceAngleCurveLbl.TabIndex = 10;
            this.constantForceAngleCurveLbl.Text = "Constant Force Angle Curve";
            // 
            // constantForceAngleCurve
            // 
            this.constantForceAngleCurve.Location = new System.Drawing.Point(29, 74);
            this.constantForceAngleCurve.Margin = new System.Windows.Forms.Padding(4);
            this.constantForceAngleCurve.Name = "constantForceAngleCurve";
            this.constantForceAngleCurve.Size = new System.Drawing.Size(115, 25);
            this.constantForceAngleCurve.TabIndex = 9;
            this.constantForceAngleCurve.Text = "0";
            this.constantForceAngleCurve.TextChanged += new System.EventHandler(this.constantForceAngleCurve_TextChanged);
            // 
            // SMFFBWheelSteerEffectControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.effectGroupBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SMFFBWheelSteerEffectControl";
            this.Size = new System.Drawing.Size(614, 432);
            this.effectGroupBox.ResumeLayout(false);
            this.effectGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.TextBox maxWheelAngle;
        private System.Windows.Forms.Label maxWheelAngleLabel;
        private System.Windows.Forms.GroupBox effectGroupBox;
        private System.Windows.Forms.Label constantForceSpeedCurveLbl;
        private System.Windows.Forms.TextBox constantForceSpeedCurve;
        private System.Windows.Forms.Label maxConstantForceLbl;
        private System.Windows.Forms.TextBox maxConstantForce;
        private System.Windows.Forms.Label minConstantForceLbl;
        private System.Windows.Forms.TextBox minConstantForce;
        private System.Windows.Forms.Label constantForceAngleCurveLbl;
        private System.Windows.Forms.TextBox constantForceAngleCurve;
        private System.Windows.Forms.Label dampForceSpeedCurveLbl;
        private System.Windows.Forms.TextBox dampForceSpeedCurve;
        private System.Windows.Forms.Label maxDampForceLbl;
        private System.Windows.Forms.TextBox maxDampForce;
        private System.Windows.Forms.Label minDampForceLbl;
        private System.Windows.Forms.TextBox minDampForce;
    }
}
