using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Nitrate;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.Core.Time;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
  [TestClass]
  public class ChangeZoomLevel
  {
    [TestMethod]
    public void TestMethod1()
    {
      DFS0 df = new DFS0(@"C:\Users\Jacob\Downloads\gridKlima20148.dfs0");
      var monthly = TSTools.ChangeZoomLevel(df.GetTimeSpanSeries(1), TimeStepUnit.Month, true);
      using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\Users\Jacob\Downloads\Monthly_20148.csv"))
      {
        sw.WriteLine("Year,Month,Precip");
      foreach(var v in monthly.Items)
        sw.WriteLine(v.Time.Year + ","+v.Time.Month+","+v.Value);
      }
    }
  }
}
