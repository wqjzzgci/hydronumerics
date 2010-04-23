#region Copyright
/*
* Copyright (c) 2010, Jan Gregersen (HydroInform) & Jacob Gudbjerg
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the names of Jan Gregersen (HydroInform) & Jacob Gudbjerg nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY "Jan Gregersen (HydroInform) & Jacob Gudbjerg" ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL "Jan Gregersen (HydroInform) & Jacob Gudbjerg" BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;


namespace HydroNumerics.Time.Tools
{
    public partial class TimeSeriesCreationDialog : Form
    {
        private BaseTimeSeries timeSeries = null;

        public TimeSeriesCreationDialog()
        {
            //timeSeriesData = new TimestampSeries();
            InitializeComponent();
            this.dateTimePicker1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy HH:mm:ss";
        }


        public BaseTimeSeries TimeSeries
        {
            get { return timeSeries; }

        }
	

        private void buttonOK_Click(object sender, EventArgs e)
        {
            DateTime dp = this.dateTimePicker1.Value;
            DateTime start = new DateTime(dp.Year, dp.Month, dp.Day, dp.Hour, dp.Minute, dp.Second);
            string name = this.IdTextBox.Text;
            int timeStepLength = Convert.ToInt32(this.textBoxTimeStepLength.Text);
            int numberOfTimesteps = Convert.ToInt32(this.textBoxNumberOfTimeSteps.Text);
            double defaultValue = Convert.ToDouble(this.DefaultValueTextBox.Text);
            TimestepUnit timeStepUnit = new TimestepUnit();
            

            if (this.comboBoxTimeStepLength.Text == "Years")
            {
                timeStepUnit = TimestepUnit.Years;
            }
            else if (this.comboBoxTimeStepLength.Text == "Months")
            {
                timeStepUnit = TimestepUnit.Months;
            }
            else if (this.comboBoxTimeStepLength.Text == "Days")
            {
                timeStepUnit = TimestepUnit.Days;
            }
            else if (this.comboBoxTimeStepLength.Text == "Hours")
            {
                timeStepUnit = TimestepUnit.Hours;
            }
            else if (this.comboBoxTimeStepLength.Text == "Minutes")
            {
                timeStepUnit = TimestepUnit.Minutes;
            }
            else if (this.comboBoxTimeStepLength.Text == "Seconds")
            {
                timeStepUnit = TimestepUnit.Seconds;
            }
            else
            {
                throw new System.Exception("invalid timestep length unit");
            }

            if (this.timestampBasedRadioButton.Checked)
            {
                timeSeries = new TimestampSeries(name, start, numberOfTimesteps, timeStepLength, timeStepUnit, defaultValue);
            }
            else if (this.timespanBasedRadioButton.Checked)
            {
                timeSeries = new TimespanSeries(name, start, numberOfTimesteps, timeStepLength, timeStepUnit, defaultValue);
            }
            else
            {
                throw new Exception("Unexpected exception in TimeSeriesCreationDialog");
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