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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class TimestampSeriesGrid : UserControl
    {
        private TimestampSeries timeSeriesData;


        public TimestampSeriesGrid(TimestampSeries timeSeriesData)
        {

            this.timeSeriesData = timeSeriesData;

            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
            // --- Date column ---
         
            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
            column1.DataPropertyName = "Time";
            column1.Name = "Time";
            column1.ReadOnly = false;
            //column1.DefaultCellStyle.Format = "dddd, dd MMMM yyyy HH:mm:ss";
            column1.DefaultCellStyle.Format = "dd MMMM yyyy HH:mm:ss";
            column1.Width = 200;
                 
            this.dataGridView1.Columns.Add(column1);

            // --- value column ---
            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
            column2.DataPropertyName = "Value";
            column2.Name = "Value";
            column2.ReadOnly = false;
            this.dataGridView1.Columns.Add(column2);

            this.dataGridView1.DataSource = timeSeriesData.TimeValues;

            this.dataGridView1.CurrentCellChanged += new EventHandler(dataGridView1_CurrentCellChanged);
        }



        public TimestampSeries TimeSeriesData
        {
            get { return timeSeriesData; }
            set 
            {
                timeSeriesData = value;
                this.dataGridView1.DataSource = timeSeriesData.TimeValues;
                Update();
                
            }
        }

        public void Update()
        {
            this.dataGridView1.Columns[1].Name = timeSeriesData.Name;

        }
	

         void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            this.timeSeriesData.NotifyDataMayHaveChanged();

            if (this.dataGridView1.CurrentRow != null)
            {
                this.timeSeriesData.SelectedRecord = this.dataGridView1.CurrentRow.Index;
            }
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                timeSeriesData.AppendValue(0);
            }
        }

        public void Paste()
        {

            try
            {
                string clipboardText = Clipboard.GetText();

                string[] strings = clipboardText.Split(new char[2] { '\r', '\n' });

                List<string> clipStrings = new List<string>();

                foreach (string str in strings)
                {
                    if (str.Length > 0)
                    {
                        clipStrings.Add(str);
                    }
                }

                bool stringsAreTabSeperatedTimeValues = false;
                foreach (string str in clipStrings)
                {
                    if (str.Contains("\t"))
                    {
                        stringsAreTabSeperatedTimeValues = true;
                    }
                }

                if (stringsAreTabSeperatedTimeValues)
                {
                    List<TimeValue> tvList = new List<TimeValue>();

                    foreach (string str in clipStrings)
                    {
                        string[] tvstrings = str.Split(new char[1] { '\t' });

                        DateTime dateTime = Convert.ToDateTime(tvstrings[0]);
                        double value = Convert.ToDouble(tvstrings[1]);


                        tvList.Add(new TimeValue(dateTime, value));
                    }

                    foreach (TimeValue tv in tvList)
                    {
                        //timeSeriesData.TimeValuesList.Add(tv);
                        timeSeriesData.AddTimeValueRecord(tv);
                    }
                }
                else
                {


                    int selectedRecord = dataGridView1.SelectedCells[0].RowIndex;

                    for (int i = 0; i < clipStrings.Count; i++)
                    {
                        if (selectedRecord + i < timeSeriesData.TimeValues.Count)
                        {
                            timeSeriesData.TimeValues[selectedRecord + i].Value = Convert.ToDouble(clipStrings[i]);
                        }
                        else
                        {
                            timeSeriesData.AppendValue(Convert.ToDouble(clipStrings[i]));
                        }
                    }
                }
                dataGridView1.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show("Paste operation failed. Details: " + e.Message);
            }

            

           
        }
    }
}
