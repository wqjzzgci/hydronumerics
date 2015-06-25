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
      var p = pr.ReadParticleFile(@"d:\temp\PTReg_DK4_R201405_kalib35.dbf", null);
      sw.Stop();
      var elapse = sw.Elapsed;

      MainModel target = new MainModel();
      target.LoadCatchments(@"F:\Oplandsmodel\Overfladevand\oplande\id15_NSTmodel_24112014.shp");

      pr.Catchments = target.AllCatchments.Values;

      pr.Distribute(p);

      pr.DebugPrint(@"d:\temp");

      int k = 0;

    }
  }
}
