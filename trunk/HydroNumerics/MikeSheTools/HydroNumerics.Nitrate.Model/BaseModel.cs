using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class BaseModel:BaseViewModel
  {

    protected XElement Configuration;

    private bool _Update= true;
    /// <summary>
    /// Set to false if the model should not update input data but run using values from previous simulation.
    /// Default is true.
    /// </summary>
    public bool Update
    {
      get { return _Update; }
      set
      {
        if (_Update != value)
        {
          _Update = value;
          NotifyPropertyChanged("Update");
        }
      }
    }
    

    public BaseModel()
    {

    }

    public BaseModel(XElement Configuration)
    {
      this.Configuration = Configuration;
      Update = SafeParseBool(Configuration, "Update") ?? _Update;
      Name = SafeParseString(Configuration, "Name");

    }

    protected bool? SafeParseBool(XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return bool.Parse(attrib.Value);
    }

    protected string SafeParseString(XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return attrib.Value;
    }

    protected double? SafeParseDouble(XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return double.Parse(attrib.Value);
    }

    protected int? SafeParseInt(XElement Conf, string AttributeName)
    {
      var attrib = Conf.Attribute(AttributeName);
      if (attrib == null)
        return null;
      else
        return int.Parse(attrib.Value);
    }

  }
}
