using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HydroNumerics.MikeSheTools.Misc
{
  public class TimeSeries<T1>
  {
    public SortedDictionary<DateTime, T1> _entries = new SortedDictionary<DateTime, T1>();


    public void AddEntry(DateTime Time, T1 value)
    {
      RemoveEntry(Time);
      _entries.Add(Time, value);
    }

    public void RemoveEntry(DateTime Time)
    {
      if (_entries.ContainsKey(Time))
        _entries.Remove(Time);
    }

    public T1 GetValue(DateTime Time)
    {
      if (_entries.ContainsKey(Time))
        return _entries[Time];
      else
        return _entries.First(var => var.Key > Time).Value;
    }
  
  }
}
