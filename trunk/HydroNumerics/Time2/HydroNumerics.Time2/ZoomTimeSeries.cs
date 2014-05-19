using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Time2
{
  public class ZoomTimeSeries:BaseViewModel
  {


    Dictionary<TimeStepUnit, FixedTimeStepSeries> data = new Dictionary<TimeStepUnit, FixedTimeStepSeries>();

    /// <summary>
    /// Returns a timestampseries at the required timestep
    /// Returns a new empty timestampseries if no data exists.
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public FixedTimeStepSeries GetTs(TimeStepUnit TimeStep)
    {
      FixedTimeStepSeries toreturn;
      if (!data.TryGetValue(TimeStep, out toreturn))
      {
        if (data.Count == 0)
        {
          toreturn = new FixedTimeStepSeries() { TimeStepSize = TimeStep, Name = Name + "_" + TimeStep.ToString() };
        }

        if (data.ContainsKey(TimeStepUnit.Day))
          toreturn = TSTools.ChangeZoomLevel(data[TimeStepUnit.Day], TimeStep, Accumulate);
        else if (data.ContainsKey(TimeStepUnit.Month))
          toreturn = TSTools.ChangeZoomLevel(data[TimeStepUnit.Month], TimeStep, Accumulate);
        data.Add(TimeStep, toreturn);
      }
      return toreturn;
    }

    public void SetTs(FixedTimeStepSeries ts)
    {
      data.Clear();
      data.Add(ts.TimeStepSize, ts);
    }


    private bool _Accumulate =false;
    public bool Accumulate
    {
      get { return _Accumulate; }
      set
      {
        if (_Accumulate != value)
        {
          _Accumulate = value;
          NotifyPropertyChanged("Accumulate");
        }
      }
    }
    
  }
}
