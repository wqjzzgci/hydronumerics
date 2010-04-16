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
        private TimeSeriesGroup timeSeriesGroup;
        private TimespanSeriesGrid timespanSeriesGrid;
        public TimespanSeriesGridFrom()
        {
            InitializeComponent();

            timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.TimeSeriesList.Add(new TimeSeries());
            timespanSeriesGrid = new TimespanSeriesGrid(timeSeriesGroup.TimeSeriesList[0]);
            timespanSeriesGrid.Visible = true;

            timespanSeriesGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            panel1.Controls.Add(timespanSeriesGrid);
            this.Update();
        }
    }
}
