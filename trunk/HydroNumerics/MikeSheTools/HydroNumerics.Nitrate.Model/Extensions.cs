using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Linq;

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

    public static bool? SafeParseBool(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return bool.Parse(attrib.Value);
    }

    public static string SafeParseString(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return attrib.Value;
    }

    public static double? SafeParseDouble(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return double.Parse(attrib.Value);
    }

    public static int? SafeParseInt(this XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return int.Parse(attrib.Value);
    }



  }
}
