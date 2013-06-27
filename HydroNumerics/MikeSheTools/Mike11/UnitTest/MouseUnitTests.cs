using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.MikeUrban;

namespace HydroNumerics.MikeSheTools.Mike11.UnitTest
{
  [TestClass]
  public class MouseUnitTests
  {
    [TestMethod]
    public void TestMethod1()
    {
      MouseSetup ms = new MouseSetup(@"C:\Users\Jacob\Projekter\GEUS\Silkeborg\JacobGudbjerg\networkBase.mex");

      ms.CreateBranches();
      ms.SaveToShape(@"C:\Users\Jacob\Projekter\GEUS\Silkeborg\setup.shp");

    }
  }
}
