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
    public class DateTimeMultiplePartsProvider : IPartsProvider<DateTime>
    {
        private AxesPartsCollection<DateTime> storage = new AxesPartsCollection<DateTime>();

        #region IPartsProvider<DateTime> Members

        public Range<DateTime>[] GetPartsSizes(Range<DateTime> range)
        {
            long start = range.Min.Ticks;
            long finish = range.Max.Ticks;

            double delta = finish - start;

            int log = (int)Math.Round(Math.Log(delta, 10));
            double step = Math.Pow(10.0, log);

            long newStart = (long)(Math.Floor(start / step) * step);
            long newFinish = (long)(Math.Ceiling(finish / step) * step);

            long prevPoint = newStart;
            long x = prevPoint;
            List<Range<DateTime>> res = new List<Range<DateTime>>();

            while ((x += (long)step) <= newFinish)
            {
                res.Add(new Range<DateTime>(new DateTime(prevPoint),new DateTime(x)));
                prevPoint = x;
            }
            return res.ToArray();
        }

        public AxisPartControl<DateTime> GetPart(Range<DateTime> r)
        {

            DateTime center = new DateTime((r.Min.Ticks + r.Max.Ticks) / 2);

            IAxisPart<DateTime> part = storage.GetPart(r, center);
            if (part != null) return part.Control;
            else
            {
                DateTimeAxisPart dnPart = new DateTimeAxisPart(r.Min, r.Max);
                dnPart.Control.LabelProvider = parentAxis.LabelProvider.Clone();
                if (parentAxis.MayorLabelProvider != null) dnPart.Control.MayorLabelProvider = parentAxis.MayorLabelProvider.Clone();
                dnPart.Control.TicksProvider = parentAxis.TicksProvider.Clone();
                dnPart.Control.ConvertToDouble = parentAxis.ConvertToDouble;
                dnPart.Control.Placement = parentAxis.Placement;
                dnPart.Control.Range = r;

                storage.AddPart(dnPart);
                return storage.GetPart(r, center).Control;
            }
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
            storage.Clear();
        }

        public bool ShouldRemoveFromScreen(AxisPartControl<DateTime> part,Range<DateTime> range)
        {
            if (part.Range.Max.CompareTo(range.Min) < 0 || part.Range.Min.CompareTo(range.Max) > 0) return true;
            else return false;
        }

        #endregion
    }
}
