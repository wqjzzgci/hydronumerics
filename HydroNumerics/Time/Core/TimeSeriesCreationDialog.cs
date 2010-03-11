using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HydroNumerics.Time.Core
{
    public partial class TimeSeriesCreationDialog : Form
    {
        private TimeSeries timeSeriesData = null;

        public TimeSeriesCreationDialog()
        {
            timeSeriesData = new TimeSeries();
            InitializeComponent();
            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy HH:mm:ss";
        }


        public TimeSeries TimeSeriesData
        {
            get { return timeSeriesData; }

        }
	

        private void buttonOK_Click(object sender, EventArgs e)
        {
            timeSeriesData.TimeValuesList.Clear();
            timeSeriesData.ID = this.IdTextBox.Text;

            DateTime dp = this.dateTimePicker1.Value;
            DateTime start = new DateTime(dp.Year, dp.Month, dp.Day, dp.Hour, dp.Minute, dp.Second);
            TimeValue timeValue = new TimeValue(start, 0);
            timeSeriesData.TimeValuesList.Add(timeValue);

            int timeStepLength = Convert.ToInt32(this.textBoxTimeStepLength.Text);

            for (int i = 0; i < Convert.ToInt32(this.textBoxNumberOfTimeSteps.Text)-1; i++)
            {
                //DateTime time = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);
                DateTime time = new DateTime();

                if (this.comboBoxTimeStepLength.Text == "Year")
                {
                    time = start.AddYears(timeStepLength * (i + 1));
                }
                else if (this.comboBoxTimeStepLength.Text == "Month")
                {
                    time = start.AddMonths(timeStepLength * (i + 1));
                }
                else if (this.comboBoxTimeStepLength.Text == "Day")
                {
                    time = start.AddDays(timeStepLength * (i + 1));
                }
                else if (this.comboBoxTimeStepLength.Text == "Hour")
                {
                    time = start.AddHours(timeStepLength * (i + 1));
                }
                else if (this.comboBoxTimeStepLength.Text == "Minute")
                {
                    time = start.AddMinutes(timeStepLength * (i + 1));
                }
                else if (this.comboBoxTimeStepLength.Text == "Second")
                {
                    time = start.AddSeconds(timeStepLength * (i + 1));
                }
                else
                {
                    throw new System.Exception("invalid timestep length unit");
                }
                
                
                
               
                
                timeSeriesData.TimeValuesList.Add(new TimeValue(time,0));
                
            }
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}