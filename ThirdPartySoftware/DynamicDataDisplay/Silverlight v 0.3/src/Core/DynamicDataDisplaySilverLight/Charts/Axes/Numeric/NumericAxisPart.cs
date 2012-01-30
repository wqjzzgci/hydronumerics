using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
    public class NumericAxisPart : IAxisPart<double>
    {        
        private double min, max;
        private AxisPartControl<double> control = null;

        #region IAxisPart<double> Members

        public double Min
        {
            get
            {
                return min;
            }
        }

        public double Max
        {
            get
            {
                return max;
            }
        }
        
        public double Center
        {
            get { return (min + max) / 2; }
        }

        public NumericAxisPart(double min, double max) {
            this.min = min;
            this.max = max;

            Control = new AxisPartControl<double>();            
        }

        public AxisPartControl<double> Control
        {
            get
            {
                return control;
            }
            set
            {
                control = value;
            }
        }

        #endregion
    }
}
