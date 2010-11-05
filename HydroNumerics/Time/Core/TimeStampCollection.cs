using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.Time.Core
{
  public class TimeStampCollection:KeyedCollection<DateTime, TimestampValue>
  {
    public TimeStampCollection() : base() { }

    protected override DateTime GetKeyForItem(TimestampValue item)
    {
      return item.Time;
    }

  }
}
