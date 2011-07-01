using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Core
{
  public class MikeSheScenarioModel:Model,IScenarioModel
  {

    public MikeSheScenarioModel(string shefilename)
      : base(shefilename)
    { }

    public ObservableCollection<string> ResultFileNames { get; private set; }

  }
}
