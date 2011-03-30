﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace GridTools.UnitTest
{
  [TestClass]
  public class ConfigurationWriter
  {
    [TestMethod]
    public void TestMethod1()
    {

      string FileName = @"..\..\..\Testdata\conf.xml";
      XDocument xd = new XDocument();
      XElement Ops = new XElement("GridOperations");
      xd.Add(Ops);

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "LayerSummation"),
        new XElement("DFS3FileName", @"..\..\..\Testdata\omr4_jag_3DSZ.dfs3"),
        new XElement("Layers","0,1"),
        new XElement("DFS2OutputFileName", @"..\..\..\Testdata\test.dfs2")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "GridMath"),
        new XElement("DFS2FileName1", "FileName1"),
        new XElement("Item1", "1"),
        new XElement("MathOperation", "+"),
        new XElement("DFS2FileName2", "FileName2"),
        new XElement("Item2", "2"),
        new XElement("DFS2OutputFileName", "Outputname")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "FactorMath"),
        new XElement("DFSFileName", "FileName"),
        new XElement("Items", "1,2"),
        new XElement("TimeSteps", ""),
        new XElement("MathOperation", "*"),
        new XElement("Factor", "2.5")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", "FileName"),
        new XElement("Items", "1,2"),
        new XElement("TimeInterval", "Week"),
        new XElement("DFSOutputFileName", "Outputname")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "MonthlyMath"),
        new XElement("DFSFileName", "FileName"),
        new XElement("Items", "1,2"),
        new XElement("TimeSteps", ""),
        new XElement("MathOperation","/"),
        new XElement("MonthlyValues", "1,2,3,4,5,6,7,8,9,10,11,12"),
        new XElement("DFSOutputFileName", "Outputname")

  ));


      xd.Save(FileName);

    }
  }
}