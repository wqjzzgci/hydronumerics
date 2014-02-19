using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  class Program
  {
    static void Main(string[] args)
    {
      MainViewModel m = new MainViewModel();
      m.ReadConfiguration(args[0].ToString());
      m.Initialize();
      m.Run();
    }
  }
}
