using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
  [TestClass]
  public class ParticleReaderTest
  {
    [TestMethod]
    public void TestMethod1()
    {

      ParticleReader pr = new ParticleReader();
      Stopwatch sw = new Stopwatch();
      sw.Start();
      var p = pr.ReadParticleFile(@"F:\results\PTReg_DK1_R201407_kalib23.shp", null, false);
      sw.Stop();
      var elapse = sw.Elapsed;

      MainModel target = new MainModel();
      target.LoadCatchments(@"F:\Oplandsmodel\Overfladevand\oplande\32262368.shp");

      pr.Catchments = target.AllCatchments.Values;

      pr.Distribute(p);

      pr.DebugPrint(@"d:\temp");

      int k = 0;

    }
  }
}
