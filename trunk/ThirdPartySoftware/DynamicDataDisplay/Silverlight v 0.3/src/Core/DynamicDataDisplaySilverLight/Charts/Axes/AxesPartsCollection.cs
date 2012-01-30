using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Microsoft.Research.DynamicDataDisplay;

namespace Microsoft.Research.DynamicDataDisplay.Charts.Axes
{
    public class AxesPartsCollection<T> where T: IComparable
    {
        private List<IAxisPart<T>> storage = new List<IAxisPart<T>>();

        public void AddPart(IAxisPart<T> part)
        {
            storage.Add(part);
        }

        public void Clear()
        {
            storage.Clear();
        }

        public IAxisPart<T> GetPart(Range<T> range, T center)
        {
            var res = from part in storage where (center.CompareTo(part.Min)>0 && center.CompareTo(part.Max)<0) select part;
            if (res.Count() == 1)
            {
                return res.First();
            }
            else return null;

        }
    }
}
