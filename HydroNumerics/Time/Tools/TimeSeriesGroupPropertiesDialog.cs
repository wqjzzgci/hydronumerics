using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroNumerics.Time.Core;
using HydroNumerics.Core;

namespace HydroNumerics.Time.Tools
{
    public partial class TimeSeriesGroupPropertiesDialog : Form
    {
       
        public TimeSeriesGroupPropertiesDialog(TimeSeriesGroup timeSeriesGroup)
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = timeSeriesGroup;
           
      
           
          
        }
    }
}
