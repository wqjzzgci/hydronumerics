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
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using System.Collections.Generic;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public class DateTimeSinglePartProvider : IPartsProvider<DateTime>
    {
        private DateTimeAxisPart activePart = null;
        private Range<DateTime> activeRange;
        private Range<DateTime>[] resultArray = new Range<DateTime>[1];
        private bool inited = false;

        private void regenPart(Range<DateTime> range)
        {
            long length = range.Max.Ticks - range.Min.Ticks;
            DateTime start = (DateTime.MinValue.Ticks>range.Min.Ticks - length)?(DateTime.MinValue):(new DateTime(range.Min.Ticks - length));
            DateTime end = (DateTime.MaxValue.Ticks<range.Max.Ticks + length)?(DateTime.MaxValue):(new DateTime(range.Max.Ticks + length));
            activeRange = new Range<DateTime>(start,end);
            resultArray[0] = activeRange;

            activePart = new DateTimeAxisPart(range.Min, range.Max);
            activePart.Control.LabelProvider = parentAxis.LabelProvider.Clone();
            if (parentAxis.MayorLabelProvider != null) activePart.Control.MayorLabelProvider = parentAxis.MayorLabelProvider.Clone();
            activePart.Control.TicksProvider = parentAxis.TicksProvider.Clone();
            activePart.Control.ConvertToDouble = parentAxis.ConvertToDouble;
            activePart.Control.Placement = parentAxis.Placement;
            activePart.Control.Range = activeRange;
        }

        #region IPartsProvider<DateTime> Members

        public Range<DateTime>[] GetPartsSizes(Range<DateTime> range)
        {
            if (inited)
            {
                if (range.Max > activeRange.Max || range.Min < activeRange.Min)
                {
                    regenPart(range);
                }
            }
            else
            {
                regenPart(range);
                inited = true;
            }
            return resultArray;
        }


        public AxisPartControl<DateTime> GetPart(Range<DateTime> r)
        {
            //long delta = r.Max.Ticks - r.Min.Ticks;
            //double log = Math.Log(delta, 10);
            //double treshold = Math.Pow(10.0, log - 1);
            if (activePart == null || r!=activeRange)
            {
                regenPart(r);    

            }
            return activePart.Control;
        }
        private AxisControl<DateTime> parentAxis;

        public AxisControl<DateTime> ParentAxis
        {
            get
            {
                return parentAxis;
            }
            set
            {
                parentAxis = value;
            }
        }

        public void CleanCach()
        {
            inited = false;
            activePart = null;
        }

        public bool ShouldRemoveFromScreen(AxisPartControl<DateTime> part, Range<DateTime> range)
        {
            if ((part.Range.Max.CompareTo(range.Max) < 0 || part.Range.Min.CompareTo(range.Min) > 0)) return true;
            else return false;
        }

        #endregion
    }
}
