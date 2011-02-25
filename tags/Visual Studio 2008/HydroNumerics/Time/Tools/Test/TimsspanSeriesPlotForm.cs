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
    public partial class TimsspanSeriesPlotForm : Form
    {
        private TimeSeriesGroup timeSeriesGroup;
        private TimeSeriesPlot timeSeriesPlot;

        public TimsspanSeriesPlotForm()
        {
            
            InitializeComponent();

            timeSeriesGroup = new TimeSeriesGroup();

            //TimestampSeries timestampSeries = new TimestampSeries();
            //timestampSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 5.0));
            //timestampSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 2, 0, 0, 0), 5.0));
            //timestampSeries.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 3, 0, 0, 0), 5.0));

            //TimestampSeries timestampSeries1 = new TimestampSeries();
            //timestampSeries1.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 1, 0, 0, 0), 2.0));
            //timestampSeries1.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 2, 0, 0, 0), 2.0));
            //timestampSeries1.AddTimeValueRecord(new TimeValue(new DateTime(2010, 1, 3, 0, 0, 0), 2.0));
            
            TimespanSeries timespanSeries = new TimespanSeries();
            timespanSeries.Items.Add(new TimespanValue(new Timespan(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 2, 0, 0, 0)), 2));
            timespanSeries.Items.Add(new TimespanValue(new Timespan(new DateTime(2010, 1, 2, 0, 0, 0), new DateTime(2010, 1, 3, 0, 0, 0)), 3));
            timespanSeries.Items.Add(new TimespanValue(new Timespan(new DateTime(2010, 1, 3, 0, 0, 0), new DateTime(2010, 1, 4, 0, 0, 0)), 6));
            
            //timeSeriesGroup.TimeSeriesList.Add(timestampSeries);
            //timeSeriesGroup.TimeSeriesList.Add(timestampSeries1);
            timeSeriesGroup.Items.Add(timespanSeries);
            timeSeriesPlot = new TimeSeriesPlot(timeSeriesGroup);
            timeSeriesPlot.Visible = true;


            timeSeriesPlot.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.Controls.Add(timeSeriesPlot);
            this.Update();
            

           
           
        }
    }
}
