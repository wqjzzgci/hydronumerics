using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
  [TestClass]
  public class ConfigWriter
  {
    [TestMethod]
    public void TestMethod1()
    {
      XDocument xd = new XDocument();

      xd.Add(new XElement("Configuration"));

      xd.Save(@"d:\temp\config.xml");


    }
  }
}
