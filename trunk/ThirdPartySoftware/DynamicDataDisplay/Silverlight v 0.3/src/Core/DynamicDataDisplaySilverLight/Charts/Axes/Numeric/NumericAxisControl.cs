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
    public class NumericAxisControl : AxisControl<double>
    {
        public NumericAxisControl()
        {
            LabelProvider = new ExponentialLabelProvider();
            TicksProvider = new NumericTicksProvider();
            PartsProvider = new NumericSinglePartProvider();
            ConvertToDouble = d => d;
            Range = new Range<double>(0, 10);
        }
    }
}
