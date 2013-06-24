using System;
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

      string FileName = @"..\..\..\Testdata\GridTools Input.xml";
      XDocument xd = new XDocument();
      XElement Ops = new XElement("GridOperations");
      xd.Add(Ops);

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "LayerSummation"),
        new XElement("DFS3FileName", @"c:\temp\TestModel_3DSZflow.dfs3"),
        new XElement("Items", "1"),
        new XElement("Layers", ""),
        new XElement("DFS2OutputFileName", @"c:\temp\SummedLayers.dfs2")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "GridMath"),
        new XElement("DFS2FileName1", @"c:\temp\SummedLayers.dfs2"),
        new XElement("Item1", "1"),
        new XElement("MathOperation", "+"),
        new XElement("DFS2FileName2", @"c:\temp\SummedLayers.dfs2"),
        new XElement("Item2", "2"),
        new XElement("DFS2OutputFileName", @"c:\temp\SummedItems.dfs2")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "FactorMath"),
        new XElement("DFSFileName", @"c:\temp\SummedItems.dfs2"),
        new XElement("Items", "1"),
        new XElement("TimeSteps", "1,4,5"),
        new XElement("MathOperation", "*"),
        new XElement("Factor", "2.5"),
        new XElement("DFSOutputFileName", @"c:\temp\SummedItemsFactored.dfs2")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "TimeSummation"),
        new XElement("DFSFileName", @"c:\temp\SummedItemsFactored.dfs2"),
        new XElement("Items", "1"),
        new XElement("TimeInterval", "Day"),
        new XElement("TimeIntervalSteps", "7"),
        new XElement("DFSOutputFileName", @"c:\temp\WeeklySum.dfs2")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "MonthlyMath"),
        new XElement("DFSFileName", @"c:\temp\SummedItems.dfs2"),
        new XElement("Items", "1"),
        new XElement("TimeSteps", "1-5"),
        new XElement("MathOperation", "/"),
        new XElement("MonthlyValues", "1.1,2000,3,4,5,6,7,8,9,10,11,12"),
        new XElement("DFSOutputFileName", @"c:\temp\SummedItemsMonthly.dfs2")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "Percentile"),
        new XElement("DFSFileName", @"c:\temp\TestModel_3DSZflow.dfs3"),
        new XElement("Item", "1"),
        new XElement("TimeSteps", ""),
        new XElement("Percentiles", "0.1,0.5,0.9"),
        new XElement("DFSOutputFileName", @"c:\temp\Percentiles.dfs3")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", "InsertPointValues"),
        new XElement("DFSFileName", @"c:\temp\TestModel_3DSZflow.dfs2"),
        new XElement("Item", "1"),
        new XElement("ClearValues", "true"),
        new XElement("Points", new XElement("Point", new XElement("X", "3.2"), new XElement("Y", "5.2"), new XElement("Z", "5.2"), new XElement("TimeStep", "1"), new XElement("Value", "5.2")))));


      xd.Save(FileName);
    }
  }
}
