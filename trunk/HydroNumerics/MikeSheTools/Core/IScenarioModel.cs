﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Core
{
  public interface IScenarioModel
  {
     void BeginRun();
     event EventHandler SimulationFinished;
     List<CalibrationParameter> Parameters { get; }
     void Load(string fileName);
     string DisplayName { get; }
     ObservableCollection<string> ResultFileNames { get; }
  }
}
