using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using HydroNumerics.Time.Tools;

namespace HydroNumerics.Time.Tools.Test
{
    public partial class TimespanSeriesGridFrom : Form
    {
        //private TimeSeriesGroup timeSeriesGroup;
        private TimespanSeriesGrid timespanSeriesGrid;
        public TimespanSeriesGridFrom()
        {
            InitializeComponent();

            //timeSeriesGroup = new TimeSeriesGroup();
            //timeSeriesGroup.TimeSeriesList.Add(new TimeSeries());
            TimespanSeries timespanSeries = new TimespanSeries();
            timespanSeries.Items.Add(new TimespanValue(new Timespan(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0)), 4.5));
            timespanSeries.AppendValue(3.3);
            timespanSeries.AppendValue(6.6);
            //timespanSeriesGrid = new TimespanSeriesGrid(timeSeriesGroup.TimeSeriesList[0]);
            timespanSeriesGrid = new TimespanSeriesGrid(timespanSeries);
            timespanSeriesGrid.Visible = true;

            timespanSeriesGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            panel1.Controls.Add(timespanSeriesGrid);
            this.Update();
        }
    }
}
