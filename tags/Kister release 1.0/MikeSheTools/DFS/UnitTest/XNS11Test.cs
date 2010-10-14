using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.DFS.UnitTest
{

  [TestFixture]
  public class XNS11Test
  {

    [Test]
    public void ReadXNS()
    {
      DFS0 df = new DFS0(@"C:\Users\Jacob\Work\HydroNumerics\MikeSheTools\TestData\Mike11\novomr6.xns11");
    }
  }
}
