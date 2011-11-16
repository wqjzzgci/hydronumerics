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
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;

namespace Microsoft.Research.DynamicDataDisplay.Charts
{
    public class NumericMultiplePartsProvider : IPartsProvider<double>
    {
        private AxesPartsCollection<double> storage = new AxesPartsCollection<double>();
        
        #region IPartsProvider<double> Members

        public void CleanCach() {
            storage.Clear();
        }

        //public Range<double> GetExtandedRange(Range<double> range)
        //{
        //    return range;
        //    double delta = range.Max - range.Min;
        //    int log = (int)Math.Round(Math.Log(delta,2));
        //    double step = Math.Pow(2.0, log);
        //    //return new Range<double>(Math.Floor(range.Min / step) * step, Math.Ceiling(range.Max / step) * step);
        //}

        public Range<double>[] GetPartsSizes(Range<double> range)
        {
            double start = range.Min;
            double finish = range.Max;

            double delta = finish - start;

            int log = (int)Math.Round(Math.Log(delta,2));
            double step = Math.Pow(2.0, log);

            double newStart = Math.Floor(start / step)*step;
            double newFinish = Math.Ceiling(finish/step)*step;

            double prevPoint = newStart;
            double x = prevPoint;
            List<Range<double>> res = new List<Range<double>>();

            while ((x+=step) <= newFinish)
            {
            res.Add(new Range<double>(prevPoint, x));
            prevPoint = x;
            }
            return res.ToArray();
        }

        public AxisPartControl<double> GetPart(Range<double> r)
        {
            double center = (r.Min + r.Max) / 2;

            IAxisPart<double> part = storage.GetPart(r, center);
            if (part != null) return part.Control;
            else { 
                NumericAxisPart numPart = new NumericAxisPart(r.Min,r.Max);
                numPart.Control.LabelProvider = parentAxis.LabelProvider.Clone();
                numPart.Control.TicksProvider = parentAxis.TicksProvider.Clone();
                numPart.Control.ConvertToDouble = parentAxis.ConvertToDouble;
                numPart.Control.Placement = parentAxis.Placement;
                numPart.Control.Range = r;
                
                storage.AddPart(numPart);
                return storage.GetPart(r, center).Control;
            }
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

        public bool ShouldRemoveFromScreen(AxisPartControl<double> part, Range<double> range)
        {
            if (part.Range.Max.CompareTo(range.Min) < 0 || part.Range.Min.CompareTo(range.Max) > 0) return true;
            else return false;
        }

        #endregion
    }
}
