namespace HydroNumerics.Time.Tools
{
    partial class TimeSeriesCreationDialog
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTimeStepLength = new System.Windows.Forms.TextBox();
            this.comboBoxTimeStepLength = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxNumberOfTimeSteps = new System.Windows.Forms.TextBox();
            this.IdLabel = new System.Windows.Forms.Label();
            this.IdTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(171, 56);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(195, 20);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(291, 175);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start date and time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Default time step length";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // textBoxTimeStepLength
            // 
            this.textBoxTimeStepLength.Location = new System.Drawing.Point(171, 95);
            this.textBoxTimeStepLength.Name = "textBoxTimeStepLength";
            this.textBoxTimeStepLength.Size = new System.Drawing.Size(100, 20);
            this.textBoxTimeStepLength.TabIndex = 4;
            this.textBoxTimeStepLength.Text = "1";
            // 
            // comboBoxTimeStepLength
            // 
            this.comboBoxTimeStepLength.FormattingEnabled = true;
            this.comboBoxTimeStepLength.Items.AddRange(new object[] {
            "Year",
            "Month",
            "Day",
            "Hour",
            "Minute",
            "Second"});
            this.comboBoxTimeStepLength.Location = new System.Drawing.Point(277, 94);
            this.comboBoxTimeStepLength.Name = "comboBoxTimeStepLength";
            this.comboBoxTimeStepLength.Size = new System.Drawing.Size(89, 21);
            this.comboBoxTimeStepLength.TabIndex = 5;
            this.comboBoxTimeStepLength.Text = "Day";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Number of Timesteps";
            // 
            // textBoxNumberOfTimeSteps
            // 
            this.textBoxNumberOfTimeSteps.Location = new System.Drawing.Point(171, 135);
            this.textBoxNumberOfTimeSteps.Name = "textBoxNumberOfTimeSteps";
            this.textBoxNumberOfTimeSteps.Size = new System.Drawing.Size(100, 20);
            this.textBoxNumberOfTimeSteps.TabIndex = 7;
            this.textBoxNumberOfTimeSteps.Text = "10";
            // 
            // IdLabel
            // 
            this.IdLabel.AutoSize = true;
            this.IdLabel.Location = new System.Drawing.Point(22, 22);
            this.IdLabel.Name = "IdLabel";
            this.IdLabel.Size = new System.Drawing.Size(106, 13);
            this.IdLabel.TabIndex = 8;
            this.IdLabel.Text = "Timeseries name (ID)";
            // 
            // IdTextBox
            // 
            this.IdTextBox.Location = new System.Drawing.Point(171, 19);
            this.IdTextBox.Name = "IdTextBox";
            this.IdTextBox.Size = new System.Drawing.Size(195, 20);
            this.IdTextBox.TabIndex = 9;
            this.IdTextBox.Text = "MyTimeSeries";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(198, 175);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // NewTimeSeriesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 227);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.IdTextBox);
            this.Controls.Add(this.IdLabel);
            this.Controls.Add(this.textBoxNumberOfTimeSteps);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxTimeStepLength);
            this.Controls.Add(this.textBoxTimeStepLength);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "NewTimeSeriesDialog";
            this.Text = "NewTimeSeriesDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTimeStepLength;
        private System.Windows.Forms.ComboBox comboBoxTimeStepLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxNumberOfTimeSteps;
        private System.Windows.Forms.Label IdLabel;
        private System.Windows.Forms.TextBox IdTextBox;
        private System.Windows.Forms.Button cancelButton;
    }
}