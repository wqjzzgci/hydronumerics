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
    public class DateTimeAxisPart : IAxisPart<DateTime>
    {
        private DateTime min, max;

        public DateTimeAxisPart(DateTime Min, DateTime Max)
        {
            min = Min;
            max = Max;
            control = new AxisPartControl<DateTime>();
        }

        #region IAxisPart<DateTime> Members

        public DateTime Min
        {
            get { return min; }
        }

        public DateTime Max
        {
            get { return max; }
        }

        public DateTime Center
        {
            get {
                return new DateTime(((min.Ticks) + max.Ticks) / 2);
            }
        }
        private AxisPartControl<DateTime> control;
        public AxisPartControl<DateTime> Control
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
