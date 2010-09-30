using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class ExtractTimeSeriesFromFile : Form
    {
        private TimeSeriesGroup timeSeriesGroup = null;

        public ExtractTimeSeriesFromFile()
        {
            InitializeComponent();
        }

        private BaseTimeSeries selectedTimeSeries = null;
        public BaseTimeSeries SelectedTimeseries 
        {
            get
            {
                return selectedTimeSeries;
            }
        }

        private void openFileButton_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open time series file file";
            openFileDialog.Filter = "Open time series file (*.xts)|*.xts";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName.Length > 3)
            {
                timeSeriesGroup = TimeSeriesGroupFactory.Create(openFileDialog.FileName);


                foreach (BaseTimeSeries timeSeries in timeSeriesGroup.Items)
                {
                    itemSelectionComboBox.Items.Add(timeSeries.Name);
                }

                itemSelectionComboBox.SelectedIndex = 0; 

            }

        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            selectedTimeSeries = timeSeriesGroup.Items[itemSelectionComboBox.SelectedIndex];
            Close();
        }
    }
}
