using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.FormView
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      TimeSeriesGroup tsg = new TimeSeriesGroup();
      tsg.Items.Add(new TimespanSeries("Test", DateTime.Now, 10, 1, TimestepUnit.Days, 15));
      timeSeriesPlot1.TimeSeriesDataSet = tsg;

    }
  }
}
