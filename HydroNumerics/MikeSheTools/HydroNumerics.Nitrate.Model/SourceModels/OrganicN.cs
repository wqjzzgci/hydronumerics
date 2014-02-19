using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class OrganicN : BaseModel, ISource
  {
    private XElement Configuration;
    private Dictionary<int, List<double>> deposition = new Dictionary<int, List<double>>();
    private int FirstYear;

    #region Constructors
    public OrganicN()
    {

    }


    #endregion



    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {


      if (Configuration != null)
      {
        var pars = Configuration.Element("Parameters");
        if (pars != null)
        {
          Par1 = pars.SafeParseDouble("p1") ?? _Par1;
          Par2 = pars.SafeParseDouble("p2") ?? _Par2;
          Par3 = pars.SafeParseDouble("p3") ?? _Par3;
          Par4 = pars.SafeParseDouble("p4") ?? _Par4;
          Par5 = pars.SafeParseDouble("p5") ?? _Par5;
          Par6 = pars.SafeParseDouble("p6") ?? _Par6;
        }
      }

      double coarsesand = 0.5;
      double finesand = 0.5;
      double slope = 0.1;
      double organicsoil = 0.1;

      foreach (var c in Catchments)
      {
        List<double> values = new List<double>();
        deposition.Add(c.ID, values);
        var precipyearly = Time2.TSTools.ChangeZoomLevel(c.Precipitation, Time2.TimeStepUnit.Year, true);
        for (int i = Start.Year; i <= End.Year; i++)
        {
          values.Add(EvaluateEquation(coarsesand, finesand, organicsoil, precipyearly.Items.First(v => v.Time.Year == i).Value, slope));
        }
      }
      FirstYear = Start.Year;
      NewMessage("Initialized.");
    }

    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      return deposition[c.ID][CurrentTime.Year - FirstYear];
    }


    public double EvaluateEquation(double CoarseSandPercentage, double FineSandPercentage, double HumusPercentage, double Precipitation, double Slope)
    {
      return Math.Exp(Par1 * Precipitation + Par2*FineSandPercentage*HumusPercentage + Par3 + Par4* CoarseSandPercentage +Par5 *Slope) * Math.Exp(Par6 / 2.0);
    }



    #region Properties
    private double _Par1 = .00027;
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

    private double _Par2 = 0.181;
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

    private double _Par3 = -0.473;
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

    private double _Par4 = -0.010;
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

    private double _Par5 = -0.025;
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

    private double _Par6 = 0.213;
    public double Par6
    {
      get { return _Par6; }
      set
      {
        if (_Par6 != value)
        {
          _Par6 = value;
          NotifyPropertyChanged("Par6");
        }
      }
    }
    #endregion


  }
}
