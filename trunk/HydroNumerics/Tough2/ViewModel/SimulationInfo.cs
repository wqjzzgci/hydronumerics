using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public enum CauseOfStop
  {
    StoppedByUser,
    TotalTimeReached,
    TotalNumberOfTimeStepsReached,
    ConvergenceFailure
  }

  public class SimulationInfo
  {
    public CauseOfStop ExitInfo {get;set;}
    public TimeSpan TotalRuntime { get; set; }
  }
}
