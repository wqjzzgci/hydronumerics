using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

using MathNet.Numerics.LinearAlgebra;


namespace HydroNumerics.MikeSheTools.Core.UnitTest
{
  [TestClass]
  public class ResultTest
  {
    private Results _res;

    [TestInitialize]
    public void ConstructorTest()
    {
      _res = new Model(@"..\..\..\TestData\TestModel.she").Results;
    }

    [TestMethod]
    public void DetailedM11Test()
    {
      var _karup = new Model(@"..\..\..\TestData\Karup_Example_DemoMode1.she").Results;

      var m11obs = _karup.Mike11Observations;
      Assert.AreEqual(1, m11obs.Count);

      var scaled = Time2.TSTools.ChangeZoomLevel(m11obs.First().Simulation, Time2.TimeStepUnit.Month, true);

    }

    [TestMethod]
    public void BigTest()
    {
      var _karup = new Model(@"E:\dhi\data\dkm\dk2\result\DK2_v3_gvf_PT_100p_24hr.she").Results;
      Stopwatch sw = new Stopwatch();
      sw.Start();
      var m11obs = _karup.Mike11Observations;
      sw.Stop();
      Assert.AreEqual(201, m11obs.Count);
      Assert.AreEqual(200, m11obs.Where(sim=>sim.Simulation!=null).Count());
      _karup.Dispose();
    }



    [TestMethod]
    public void PhreaticTest()
    {
      Assert.AreEqual(0, _res.PhreaticHead.TimeData(0)[2, 2, 0]);
      Assert.AreEqual(0.102842, _res.PhreaticHead.TimeData(5)[20, 14, 0], 1e-5);
      Assert.AreEqual(0.102842, _res.Heads.TimeData(5)[20, 14, 0], 1e-5);

      for (int i = 0; i < 30; i++)
      {
        for (int j = 0; j<4; j++)
          for (int k = 0; k< 20; k++)
            Assert.IsTrue(_res.PhreaticHead.TimeData(i)[j,k,0] == _res.Heads.TimeData(i)[j,k,0]);
      }

    }

    [TestCleanup]
    public void Dispose()
    {
      _res.Dispose();
    }
  }
}
