using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class TimeStepInfo
  {
    public int TimeStepNumber { get; set; }
    public TimeSpan TimeStep { get; set; }
    public TimeSpan TotalTime { get; set; }
    public string MaxResidualElement { get; set; }
    public int NumberOfIterations { get; set; }
  }
}
