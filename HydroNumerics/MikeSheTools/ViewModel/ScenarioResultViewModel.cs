using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class ScenarioResultViewModel:NotifyPropertyChangedBase
  {

    public ScenarioResultViewModel(DirectoryInfo d)
    {
      int i =d.FullName.LastIndexOf(@"_", 0);
      if (i>0)
      {
      int n;
      if (int.TryParse(d.FullName.Substring(i, d.FullName.Length-1), out n))
        Number = n;
      }
      LatestWrite = d.LastWriteTime;
      NumberOfFiles = d.GetFiles().Count();


    }

    public int Number { get; set; }

    public DateTime LatestWrite { get; set; }

    public int NumberOfFiles { get; set; }
  }
}
