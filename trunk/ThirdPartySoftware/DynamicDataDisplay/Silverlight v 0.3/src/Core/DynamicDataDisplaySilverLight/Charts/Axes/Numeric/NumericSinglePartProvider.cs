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

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
    public class NumericSinglePartProvider : IPartsProvider<double>
    {
        private NumericAxisPart activePart = null;
        private Range<double> activeRange;
        private Range<double>[] resultArray = new Range<double>[1];
        private bool inited = false;

        private void regenPart(Range<double> range)
        {
            double length = range.Max - range.Min;
            activeRange = new Range<double>(range.Min - length, range.Max + length);
            resultArray[0] = activeRange;

            activePart = new NumericAxisPart(activeRange.Min, activeRange.Max);
            activePart.Control.LabelProvider = parentAxis.LabelProvider.Clone();
            activePart.Control.TicksProvider = parentAxis.TicksProvider.Clone();
            activePart.Control.ConvertToDouble = parentAxis.ConvertToDouble;
            activePart.Control.Placement = parentAxis.Placement;
            activePart.Control.Range = activeRange;
        }

        #region IPartsProvider<double> Members

        public Range<double>[] GetPartsSizes(Range<double> range)
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

        public AxisPartControl<double> GetPart(Range<double> r)
        {
            //double delta = r.Max - r.Min;
            //double log = Math.Log(delta,10);
            //double treshold = Math.Pow(10.0, log-1);
            if ( activePart ==null || r!=activeRange)
            {
                regenPart(r);
            }
            return activePart.Control;
        }

        private AxisControl<double> parentAxis;

        public AxisControl<double> ParentAxis {
            get {
                return parentAxis;
            }
            set {
                parentAxis = value;
            }
        }

        public void CleanCach()
        {
            inited = false;
            activePart = null;
        }

        public bool ShouldRemoveFromScreen(AxisPartControl<double> part,Range<double> range)
        {
            if ((part.Range.Max.CompareTo(range.Max) < 0 || part.Range.Min.CompareTo(range.Min) > 0))
                return true;
            else return false;
        }

        #endregion
    }
}
