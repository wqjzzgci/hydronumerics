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

      string FileName = @"C:\Jacob\Work2\HydroNumerics\MikeSheTools\GridTools\GridTools.UnitTest\conf.xml";
      XDocument xd = new XDocument();
      XElement Ops = new XElement("GridOperations");
      xd.Add(Ops);

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", GridOperation.LayerSummation), 
        new XElement("DFS3FileName", "FileName"),
        new XElement("Items","1,2"),
        new XElement("Layers",""),
        new XElement("TimeSteps", ""),
        new XElement("DFS2OutputFileName", "Outputname")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", GridOperation.GridMath),
        new XElement("DFS2FileName1", "FileName1"),
        new XElement("Item", "1"),
        new XElement("MathOperation", MathOperator.Addition),
        new XElement("DFS2FileName2", "FileName2"),
        new XElement("Item", "2"),
        new XElement("DFS2OutputFileName", "Outputname")
        ));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", GridOperation.FactorMath),
  new XElement("DFSFileName", "FileName"),
        new XElement("Items", "1,2"),
        new XElement("TimeSteps", ""),
  new XElement("MathOperation", MathOperator.Multiplication),
  new XElement("Factor", "2.5"),
  new XElement("DFSOutputFileName", "Outputname")));

      Ops.Add(new XElement("GridOperation", new XAttribute("Type", GridOperation.TimeSummation),
new XElement("DFSFileName", "FileName"),
      new XElement("Items", "1,2"),
      new XElement("TimeSteps", ""),
new XElement("TimeInterval", Time.Week),
new XElement("DFSOutputFileName", "Outputname")));

          Ops.Add(new XElement("GridOperation", new XAttribute("Type", GridOperation.MonthlyMath),
  new XElement("DFSFileName", "FileName"),
        new XElement("Items", "1,2"),
        new XElement("TimeSteps", ""),
  new XElement("MathOperation", MathOperator.Division),
  new XElement("MonthlyValues", "1,2,3,4,5,6,7,8,9,10,11,12"),
  new XElement("DFSOutputFileName", "Outputname")

  ));


      xd.Save(FileName);

    }
  }
}
