using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model.UnitTest
{
  [TestClass]
  public class Tools
  {
    [TestMethod]
    public void TestMethod1()
    {
      Dictionary<int, int> dmuTOId15 = new Dictionary<int, int>();
      using (StreamReader sr = new StreamReader(@"D:\DK_information\Overfladevand\stationer\maol.txt"))
      {
        sr.ReadLine();
        while (!sr.EndOfStream)
        {
          var data = sr.ReadLine().Split(new string[] { "\t" }, StringSplitOptions.None);

          dmuTOId15.Add(int.Parse(data[1]), int.Parse(data[3]));

        }

      }
      using (ShapeWriter sw = new ShapeWriter(@"D:\DK_information\Overfladevand\stationer\stationer2.shp"))
      {
      using (ShapeReader sh = new ShapeReader(@"D:\DK_information\Overfladevand\stationer\stationer.shp"))
      {
        var dt = sh.Data.Read();
        dt.Columns.Add("ID15", typeof(int));
        for(int i =0;i< dt.Rows.Count;i++)
        {

          int dmunr = int.Parse(dt.Rows[i][0].ToString());

          int id15;
          if(dmuTOId15.TryGetValue(dmunr, out id15))
          {
            dt.Rows[i]["ID15"] = id15;
          }

          sw.Write(new Geometry.GeoRefData() { Geometry = sh.ReadNext(i), Data = dt.Rows[i] });


        }
      }
    }

    }
  }
}
