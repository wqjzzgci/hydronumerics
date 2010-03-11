using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace HydroNumerics.Time.Core
{
    public partial class TimeSeriesGrid : UserControl
    {
        private TimeSeries timeSeriesData;


        public TimeSeriesGrid(TimeSeries timeSeriesData)
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

            this.dataGridView1.DataSource = timeSeriesData.TimeValuesList;

            this.dataGridView1.CurrentCellChanged += new EventHandler(dataGridView1_CurrentCellChanged);
        }



        public TimeSeries TimeSeriesData
        {
            get { return timeSeriesData; }
            set 
            {
                timeSeriesData = value;
                this.dataGridView1.DataSource = timeSeriesData.TimeValuesList;
                Update();
                
            }
        }

        public void Update()
        {
            this.dataGridView1.Columns[1].Name = timeSeriesData.ID;

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
                timeSeriesData.AddTimeValueRecord();
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
                        if (selectedRecord + i < timeSeriesData.TimeValuesList.Count)
                        {
                            timeSeriesData.TimeValuesList[selectedRecord + i].Value = Convert.ToDouble(clipStrings[i]);
                        }
                        else
                        {
                            timeSeriesData.AddData(Convert.ToDouble(clipStrings[i]));
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
