using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HydroNumerics.Core
{
  public static class Extensions
  {
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
