using System;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.MikeSheTools.Irrigation;

namespace HydroNumerics.MikeSheTools.Irrigation.UnitTest
{
  [TestClass]
  public class ControllerTest
  {
    [TestMethod]
    public void SerializeTest()
    {
      Configuration Cf = new Configuration();

      Cf.BottomHeader = "BOTTOM";
      Cf.TopHeader = "TOP";
      Cf.XHeader = "XUTM";
      Cf.YHeader = "YUTM";
      Cf.IdHeader = "XUTM";
      Cf.SheFile = Path.GetFullPath(@"..\HydroNumerics\MikeSheTools\TestData\TestModel.she");
      Cf.MaxDepthHeader = "BOTTOM";
      Cf.MaxRateHeader = "XUTM";

      Cf.WellShapeFile = Path.GetFullPath(@"..\HydroNumerics\MikeSheTools\TestData\commandareas.shp");
      Cf.DeleteWellsAfterRun = false;

      XmlSerializer x = new XmlSerializer(Cf.GetType());
      System.IO.FileStream file = new System.IO.FileStream(@"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfiguration.xml", System.IO.FileMode.Create);

      x.Serialize(file, Cf);
      file.Dispose();
      
    }
    [TestMethod]
    public void DeSerializeTest()
    {
      XmlSerializer x = new XmlSerializer(typeof(Configuration));

      Configuration Cf = new Configuration();
      using (FileStream fs = new FileStream(@"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfigurationWithSpaces.xml", System.IO.FileMode.Open))
        Cf = (Configuration)x.Deserialize(fs);

      Assert.AreEqual(@"C:\Users\Ja cob\Work\HydroNumerics\MikeSheTools\TestData\commandareas.shp", Cf.WellShapeFile);
    }


    [TestMethod]
    public void RunTest()
    {
      XmlSerializer x = new XmlSerializer(typeof(Configuration));

      string xmlFileName = @"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfiguration.xml";
      Configuration Cf;
        using (FileStream fs =new System.IO.FileStream(xmlFileName, System.IO.FileMode.Open))
          Cf = (Configuration)x.Deserialize(fs);
     
      Controller C = new Controller(Cf);

      C.Run();

    }

    [TestMethod]
    public void RunTest2()
    {
      Program.Main(new string[]{@"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfiguration.xml"});

      Program.Main(new string[] { Path.GetFullPath(@"..\HydroNumerics\MikeSheTools\TestData\TestModel.she"), @"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfiguration.xml" });
      Program.Main(new string[] { @"..\HydroNumerics\MikeSheTools\TestData\IrrigationConfiguration.xml", Path.GetFullPath(@"..\HydroNumerics\MikeSheTools\TestData\TestModel.she") });
    }
    
  }
}
