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
      Stopwatch sw = new Stopwatch();

      DistributedLeaching dl = new DistributedLeaching();
      sw.Start();
      dl.LoadFile(@"D:\DK_information\TestData\FileStructure\DaisyLeaching\Leaching_area_2.txt");
      sw.Stop();
      int k = 0;
    }
  }
}
