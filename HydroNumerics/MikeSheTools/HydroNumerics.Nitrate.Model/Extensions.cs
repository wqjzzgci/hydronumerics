using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace HydroNumerics.Nitrate.Model
{
  public static class Extensions
  {

    public static void ToCSV(this DataTable data, string filename)
    {
      using (StreamWriter sw = new StreamWriter(filename))
      {
        foreach (DataColumn c in data.Columns)
          sw.Write(c.ColumnName + ",");
        sw.Write("\n");

        foreach (DataRow dr in data.Rows)
        {
          foreach (DataColumn dc in data.Columns)
          {
            sw.Write(dr[dc].ToString() + ",");
          }
          sw.Write("\n");
        }
      }


    }
  }
}
