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