using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
  [TestFixture]
  public class MsheLauncherTest
  {
    [Test]
    public void PreProcessTest()
    {

      MSheLauncher.PreprocessAndRun(@"..\..\TestData\TestModel.she", true);
      Console.WriteLine("simulation finished");
      Thread.Sleep(2000);
    }
  }
}
