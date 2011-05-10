using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DHI.TimeSeries;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{
  [TestClass()]
  public class DFS0Test
  {


    [TestMethod]
    public void CompareToTsObject()
    {
      //Create the TSObject
      TSObject _tso = new TSObjectClass();
      TSItem _item = new TSItemClass();
      _item.DataType = ItemDataType.Type_Float;
      _item.ValueType = ItemValueType.Accumulated;
      _item.EumType = 171;
      _item.EumUnit = 1;
      _item.Name = "Name";
      _tso.Add(_item);

      DFS0 dfs = new DFS0(@"..\..\..\TestData\Mydfs.dfs0", 1);
      dfs.FirstItem.EumItem = DHI.Generic.MikeZero.eumItem.eumIElevation;
      dfs.FirstItem.EumUnit = DHI.Generic.MikeZero.eumUnit.eumUmeter;
      dfs.FirstItem.Name  = "Name";
      dfs.FirstItem.ValueType = DHI.Generic.MikeZero.DFS.DataValueType.Accumulated;

      DateTime start = DateTime.Now;
      
      DateTime _previousTimeStep = DateTime.MinValue;

      for (int i = 0; i < 100; i++)
      {
        _tso.Time.AddTimeSteps(1);
        _tso.Time.SetTimeForTimeStepNr(i + 1, start.AddDays(i));
        _item.SetDataForTimeStepNr(i + 1, (float)i);

        dfs.SetTime(i,start.AddDays(i));
        dfs.SetData(i, 1, i);
      }
      dfs.Dispose();
        _tso.Connection.FilePath = @"..\..\..\TestData\tsobject.dfs0";
        _tso.Connection.Save();

      



    }


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
