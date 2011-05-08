using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
  [TestClass]
  public class MsheLauncherTest
  {
    [TestMethod]
    public void PreProcessTest()
    {

      MSheLauncher.PreprocessAndRun(@"..\..\TestData\TestModel.she", true);
      Console.WriteLine("simulation finished");
      Thread.Sleep(2000);
    }
  }
}
