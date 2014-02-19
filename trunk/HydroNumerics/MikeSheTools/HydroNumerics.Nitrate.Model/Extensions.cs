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
        for (int i = 0; i < data.Columns.Count; i++)
        {
          if (i == 0)
            sw.Write(data.Columns[i].ColumnName);
          else
            sw.Write("," + data.Columns[i].ColumnName);
        }
        sw.Write("\n");

        foreach (DataRow dr in data.Rows)
        {
          for (int i = 0; i < data.Columns.Count; i++)
          {
            if (i == 0)
              sw.Write(dr[i].ToString());
            else
              sw.Write("," + dr[i].ToString());
          }
          sw.Write("\n");
        }
      }
    }

    public static void FromCSV(this DataTable data, string filename)
    {
      using (StreamReader sr = new StreamReader(filename))
      {
        var headline = sr.ReadLine().Split(',');
        data.Columns.Add(headline[0], typeof(int));
        data.Columns.Add(headline[1], typeof(DateTime));

        foreach (var col in headline.Skip(2))
          data.Columns.Add(col, typeof(double));

        while (!sr.EndOfStream)
        {
          var rowdata = sr.ReadLine().Split(',');
          var newrow = data.NewRow();
          newrow[0] = int.Parse(rowdata[0]);
          newrow[1] = DateTime.Parse(rowdata[1]);

          for (int i = 2; i < rowdata.Count(); i++)
          {
            if (!string.IsNullOrEmpty(rowdata[i]))
              newrow[i] = double.Parse(rowdata[i]);
          }
          data.Rows.Add(newrow);
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
