using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{

  [TestClass]
  public class XNS11Test
  {

    [TestMethod]
    public void ReadXNS()
    {
      DFS0 df = new DFS0(@"..\..\..\TestData\Mike11\novomr6.xns11");
    }
  }
}
