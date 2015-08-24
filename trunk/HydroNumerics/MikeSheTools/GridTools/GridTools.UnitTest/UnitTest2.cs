using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GridTools.UnitTest
{
  [TestClass]
  public class UnitTest2
  {
    [TestMethod]
    public void TestMethod1()
    {
      Program_Accessor.Main(new string[] { @"d:\temp\Dk5_LayerSum.xml" });

    }
  }
}
