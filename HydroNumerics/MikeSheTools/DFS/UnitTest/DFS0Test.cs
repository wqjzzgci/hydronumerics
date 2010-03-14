using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestFixture]
  public class DFS0Test
  {


    [Test]
    public void ReadDataTest()
    {
      DFS0 _dfs0 = new DFS0(@"..\..\TestData\novomr4_indv_dfs0_ud1.dfs0");
      Assert.AreEqual(33316.7, _dfs0.GetData(0, 1), 1e-1);
      _dfs0.Dispose();
    }

    [Test]
    public void ReadItems()
    {
      DFS0 _data = new DFS0(@"..\..\\TestData\Detailed timeseries output.dfs0");

      Assert.AreEqual(3, _data.GetTimeStep(new DateTime(2000, 1, 4, 11, 0, 0)));
      Assert.AreEqual(3, _data.GetTimeStep(new DateTime(2000, 1, 4, 12, 0, 0)));

      Assert.AreEqual(4, _data.GetTimeStep(new DateTime(2000, 1, 4, 13, 0, 0)));
      Assert.AreEqual(0, _data.GetTimeStep(new DateTime(1200, 1, 4, 13, 0, 0)));
      Assert.AreEqual(31, _data.GetTimeStep(new DateTime(2200, 1, 4, 13, 0, 0)));

      

      _data.Dispose();

    }
  }
}
