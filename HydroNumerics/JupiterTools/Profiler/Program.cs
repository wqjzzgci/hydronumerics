using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools;
using HydroNumerics.Wells;

namespace Profiler
{
  class Program
  {
    static void Main(string[] args)
    {
      Reader R = new Reader(@"..\..\..\..\TestData\AlbertslundPcJupiter.mdb");


//      var well1 = R.WellsForNovana(true, true, false, false);

      var well2 = R.ReadWellsInSteps();
      R.ReadLithology(well2);
//      R.ReadWaterLevels(well2);
      JupiterXLFastReader rw = new JupiterXLFastReader(@"..\..\..\..\TestData\AlbertslundPcJupiter.mdb");
      rw.ReadWaterLevels(well2);
      var plants = R.ReadPlants(well2);




    }
  }
}
