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
    public partial class TimestampGridTestForm : Form
    {
        private TimeSeriesGroup timeSeriesGroup;
        TimestampSeriesGrid timeSeriesGridControl;
        public TimestampGridTestForm()
        {
            InitializeComponent();
            timeSeriesGroup = new TimeSeriesGroup();
            timeSeriesGroup.TimeSeriesList.Add(new TimeSeries());
            timeSeriesGridControl = new TimestampSeriesGrid(timeSeriesGroup.TimeSeriesList[0]);
            timeSeriesGridControl.Visible = true;

            timeSeriesGridControl.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            panel1.Controls.Add(timeSeriesGridControl);
            this.Update();
        }
            
        
    }
}
