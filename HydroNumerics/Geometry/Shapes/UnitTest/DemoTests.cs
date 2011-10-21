using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HydroNumerics.Geometry.Shapes.UnitTest
{
  [TestClass]
  public class DemoTests
  {
    [TestMethod]
    public void TestMethod1()
    {
      using (ShapeWriter shpout = new ShapeWriter(@"c:\temp\drain"))
      {
        using (ShapeReader shp = new ShapeReader(@"C:\Users\Jacob\Dropbox\FunFirm\DrænPortal\GIS\Dræn\Tokkerupgaard_draen.shp"))
        {
          DataTable dt = new DataTable();
          dt.Columns.Add(new DataColumn("ID", typeof(int)));


          int count = 0;

          foreach (var v in shp.GeoData)
          {
            if (count==0)
              foreach (DataColumn c in v.Data.Table.Columns)
              {
                dt.Columns.Add(new DataColumn(c.ColumnName, c.DataType));
              }

            GeoRefData grf = new GeoRefData();
            grf.Geometry = v.Geometry;
            grf.Data = dt.NewRow();
            grf.Data["ID"] = count;



            foreach (DataColumn dc in v.Data.Table.Columns)
            {
              grf.Data[dc.ColumnName] = v.Data[dc];
            }

            shpout.Write(grf);
            count++;
          }
        }
      }
    }



  }
}

