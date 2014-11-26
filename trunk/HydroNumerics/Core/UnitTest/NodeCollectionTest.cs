using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Core.UnitTest
{
  [TestClass]
  public class NodeCollectionTest
  {
    [TestMethod]
    public void TestMethod1()
    {
      var nc = new NodeCollection<double>();
      nc.Item = 10;
      nc.Children.Add(new NodeCollection<double>() { Item = 3 });

      Assert.AreEqual(10, nc.Item);
      Assert.AreEqual(3, nc.Children[0].Item);


    }
  }
}
