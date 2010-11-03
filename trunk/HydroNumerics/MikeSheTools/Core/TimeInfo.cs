using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.Core
{
  public class TimeInfo
  {
    private InputFile _input;

    internal TimeInfo(InputFile Input)
    {
      _input = Input;
    }

    /// <summary>
    /// Gets and sets the maximum allowed timestep for the saturated zone
    /// </summary>
    public TimeSpan MaxSZTimeStep
    {
      get
      {
        return TimeSpan.FromHours(_input.MIKESHE_FLOWMODEL.SimSpec.CompControlParaTimeStep.MaxAllowedSZTimeStep);
      }
      set
      {
        _input.MIKESHE_FLOWMODEL.SimSpec.CompControlParaTimeStep.MaxAllowedSZTimeStep = value.TotalHours;
      }
    }


    /// <summary>
    /// Gets and sets the start of the simulation
    /// </summary>
    public DateTime SimulationStart
    {
      get
      {
        return _input.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.StartTime;
      }
      set
      {
        _input.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.StartTime = value;
      }
    }

    /// <summary>
    /// Gets and sets the end of the simulation
    /// </summary>
    public DateTime SimulationEnd
    {
      get
      {
        return _input.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.EndTime;
      }
      set
      {
        _input.MIKESHE_FLOWMODEL.SimSpec.SimulationPeriod.EndTime = value;
      }
    }

  }
}
