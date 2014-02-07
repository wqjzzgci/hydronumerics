using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class OrganicN : BaseViewModel, ISource
  {
    private XElement Configuration;
    private Dictionary<int, List<double>> deposition = new Dictionary<int, List<double>>();
    private int FirstYear;

    #region Constructors
    public OrganicN()
    {

    }

    public OrganicN(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);
      this.Configuration = Configuration;


    }

    #endregion

    public bool Update { get; set; }


    public void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      if (Configuration != null)
      {
        Par1 = double.Parse(Configuration.Element("Parameters").Attribute("p1").Value);
        Par2 = double.Parse(Configuration.Element("Parameters").Attribute("p2").Value);
        Par3 = double.Parse(Configuration.Element("Parameters").Attribute("p3").Value);
        Par4 = double.Parse(Configuration.Element("Parameters").Attribute("p4").Value);
        Par5 = double.Parse(Configuration.Element("Parameters").Attribute("p5").Value);
      }

      double coarsesand = 0.5;
      double slope = 0.1;

      foreach (var c in Catchments)
      {

        var precipyearly = Time2.TSTools.ChangeZoomLevel(c.Precipitation, Time2.TimeStepUnit.Year, true);
        for (int i = Start.Year; i <= End.Year; i++)
        {
          


        }
      }
    }

    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      return deposition[c.ID][CurrentTime.Year - FirstYear];
    }


    public double EvaluateEquation(double CoarseSandPercentage, double Precipitation, double Slope)
    {
      return Math.Exp(Par1 + Math.Log(CoarseSandPercentage + 1) * Par2 + Math.Log(Precipitation * Par3) + Math.Log(Slope + 1) * Par4) * Math.Exp(Par5 / 2.0);
    }



    #region Properties
    private double _Par1;
    public double Par1
    {
      get { return _Par1; }
      set
      {
        if (_Par1 != value)
        {
          _Par1 = value;
          NotifyPropertyChanged("Par1");
        }
      }
    }

    private double _Par2;
    public double Par2
    {
      get { return _Par2; }
      set
      {
        if (_Par2 != value)
        {
          _Par2 = value;
          NotifyPropertyChanged("Par2");
        }
      }
    }

    private double _Par3;
    public double Par3
    {
      get { return _Par3; }
      set
      {
        if (_Par3 != value)
        {
          _Par3 = value;
          NotifyPropertyChanged("Par3");
        }
      }
    }

    private double _Par4;
    public double Par4
    {
      get { return _Par4; }
      set
      {
        if (_Par4 != value)
        {
          _Par4 = value;
          NotifyPropertyChanged("Par4");
        }
      }
    }

    private double _Par5;
    public double Par5
    {
      get { return _Par5; }
      set
      {
        if (_Par5 != value)
        {
          _Par5 = value;
          NotifyPropertyChanged("Par5");
        }
      }
    }
    



    #endregion


  }
}
