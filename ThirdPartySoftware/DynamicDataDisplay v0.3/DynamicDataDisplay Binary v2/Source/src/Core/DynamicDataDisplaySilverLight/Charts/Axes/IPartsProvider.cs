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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public interface IPartsProvider<T> where T:IComparable
    {
        Range<T>[] GetPartsSizes(Range<T> range);
        AxisPartControl<T> GetPart(Range<T> r);
        AxisControl<T> ParentAxis {get;set;}
        bool ShouldRemoveFromScreen(AxisPartControl<T> part, Range<T> range);
        void CleanCach();
    }
}
