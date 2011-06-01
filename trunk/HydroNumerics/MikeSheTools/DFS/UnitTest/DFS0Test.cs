using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestClass()]
  public class DFS0Test
  {


    [TestMethod]
    public void WriteTest()
    {
      File.Copy(@"..\..\..\TestData\novomr4_indv_dfs0_ud1.dfs0", @"..\..\..\TestData\novomr4_indv_dfs0_ud1_copy.dfs0", true);
      DFS0 _dfs0 = new DFS0(@"..\..\..\TestData\novomr4_indv_dfs0_ud1_copy.dfs0");

      Assert.AreEqual(33316.7, _dfs0.GetData(0, 1), 1e-1);

      List<DateTime> Times = new List<DateTime>();

      foreach (var t in _dfs0.TimeSteps)
        Times.Add(t);

      _dfs0.SetData(_dfs0.NumberOfTimeSteps,1, 1);
        
      _dfs0.SetData(0, 1, 2560);
      Assert.AreEqual(2560, _dfs0.GetData(0, 1), 1e-1);

      _dfs0.SetData(10, 1, 2560.1);
      _dfs0.SetData(10, 2, 2560.1);

      _dfs0.Dispose();
      _dfs0 = new DFS0(@"..\..\..\TestData\novomr4_indv_dfs0_ud1_copy.dfs0");
      Assert.AreEqual(2560, _dfs0.GetData(0, 1), 1e-1);
      Assert.AreEqual(2560.1, _dfs0.GetData(10, 1), 1e-1);
      Assert.AreEqual(2560.2, _dfs0.GetData(10, 2), 1e-1);

      for (int i = 0; i < _dfs0.NumberOfTimeSteps; i++)
        Assert.AreEqual(Times[i], _dfs0.TimeSteps[i]);

      _dfs0.SetTime(10, Times[10].AddSeconds(1));
     
      _dfs0.Dispose();

      _dfs0 = new DFS0(@"..\..\..\TestData\novomr4_indv_dfs0_ud1_copy.dfs0");

      Assert.AreEqual(Times[10].AddSeconds(1), _dfs0.TimeSteps[10]);
      Assert.AreEqual(2560.1, _dfs0.GetData(10, 1), 1e-1);
      Assert.AreEqual(2560.2, _dfs0.GetData(10, 2), 1e-1);
      _dfs0.Dispose();


    }

    [TestMethod]
    public void WriteTest2()
    {
      File.Copy(@"..\..\..\TestData\Detailed timeseries output.dfs0", @"..\..\..\TestData\Detailed timeseries output_copy.dfs0", true);
      DFS0 _dfs0 = new DFS0(@"..\..\..\TestData\Detailed timeseries output_copy.dfs0");
      _dfs0.SetData(0, 1, 2560);
      Assert.AreEqual(2560, _dfs0.GetData(0, 1), 1e-1);

      _dfs0.SetData(10, 1, 2560.1);
      _dfs0.SetData(10, 2, 2560.1);

      _dfs0.Dispose();
      _dfs0 = new DFS0(@"..\..\..\TestData\Detailed timeseries output_copy.dfs0");
      Assert.AreEqual(2560, _dfs0.GetData(0, 1), 1e-1);
      Assert.AreEqual(2560.1, _dfs0.GetData(10, 1), 1e-1);
      Assert.AreEqual(2560.2, _dfs0.GetData(10, 2), 1e-1);
      _dfs0.Dispose();

    }


    [TestMethod]
    public void ReadDataTest()
    {
      DFS0 _dfs0 = new DFS0(@"..\..\..\TestData\novomr4_indv_dfs0_ud1.dfs0");
      Assert.AreEqual(33316.7, _dfs0.GetData(0, 1), 1e-1);
      _dfs0.Dispose();
    }

    [TestMethod]
    public void ReadItems()
    {
      DFS0 _data = new DFS0(@"..\..\..\TestData\Detailed timeseries output (2).dfs0");

      Assert.AreEqual(3, _data.GetTimeStep(new DateTime(2000, 1, 4, 11, 0, 0)));
      Assert.AreEqual(3, _data.GetTimeStep(new DateTime(2000, 1, 4, 12, 0, 0)));

      Assert.AreEqual(4, _data.GetTimeStep(new DateTime(2000, 1, 4, 13, 0, 0)));
      Assert.AreEqual(0, _data.GetTimeStep(new DateTime(1200, 1, 4, 13, 0, 0)));
      Assert.AreEqual(31, _data.GetTimeStep(new DateTime(2200, 1, 4, 13, 0, 0)));

      

      _data.Dispose();

    }
  }
}
