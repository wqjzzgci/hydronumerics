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

    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      if (Update)
      {
        var pars = Configuration.Element("Parameters");
        if (pars != null)
        {
          Par1 = pars.SafeParseDouble("p1") ?? _Par1;
          Par2 = pars.SafeParseDouble("p2") ?? _Par2;
          Par3 = pars.SafeParseDouble("p3") ?? _Par3;
          Par4 = pars.SafeParseDouble("p4") ?? _Par4;
          MaxConcentration = pars.SafeParseDouble("MaxConcentration") ?? _MaxConcentration;
        }

        Slope = new SafeFile() { FileName = Configuration.Element("SlopeFile").SafeParseString("DBFFileName") };
        Slope.ColumnNames.Add(Configuration.Element("SlopeFile").SafeParseString("IDColumn"));
        Slope.ColumnNames.Add(Configuration.Element("SlopeFile").SafeParseString("ValueColumn"));

        SoilTypeFile = new SafeFile() { FileName = Configuration.Element("SoilTypeFile").SafeParseString("DBFFileName") };
        SoilTypeFile.ColumnNames.Add(Configuration.Element("SoilTypeFile").SafeParseString("IDColumn"));
        SoilTypeFile.ColumnNames.Add(Configuration.Element("SoilTypeFile").SafeParseString("ValueColumn"));
        SoilTypeFile.ColumnNames.Add(Configuration.Element("SoilTypeFile").SafeParseString("SoilNameText"));

      }
    }


    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      //Reading the slopes
      Dictionary<int, double> Slopes = new Dictionary<int, double>();
      using (HydroNumerics.Geometry.Shapes.DBFReader dbf = new Geometry.Shapes.DBFReader(Slope.FileName))
      {
        for (int i = 0; i < dbf.NoOfEntries; i++)
        {
          int id = dbf.ReadInt(i, Slope.ColumnNames[0]);
          double value = dbf.ReadDouble(i, Slope.ColumnNames[1]);
          Slopes.Add(id, value);
        }
      }

      //Reading the soiltypes
      Dictionary<int, double> FineSand = new Dictionary<int, double>();
      Dictionary<int, double> CoarseSand = new Dictionary<int, double>();
      Dictionary<int, double> Humus = new Dictionary<int, double>();
      using (HydroNumerics.Geometry.Shapes.DBFReader dbf = new Geometry.Shapes.DBFReader(SoilTypeFile.FileName))
      {
        for (int i = 0; i < dbf.NoOfEntries; i++)
        {
          int id = dbf.ReadInt(i, SoilTypeFile.ColumnNames[0]);
          double value = dbf.ReadDouble(i, SoilTypeFile.ColumnNames[1]);
          string text = dbf.ReadString(i, SoilTypeFile.ColumnNames[2]).ToLower().Trim();
          switch (text)
          {
            case "coarse sandy soil":
              CoarseSand.Add(id, value);
              break;
            case "fine sandy soil":
              FineSand.Add(id, value);
              break;
            case "organic soil":
              Humus.Add(id, value);
              break;
            default:
              break;
          }
        }
      }


      foreach (var c in Catchments.Where(cp => cp.Precipitation != null & cp.M11Flow!=null && cp.M11Flow.Items.Count>0))
      {
        List<double> values = new List<double>();
        deposition.Add(c.ID, values);
        var precipyearly = Time2.TSTools.ChangeZoomLevel(c.Precipitation, Time2.TimeStepUnit.Year, true);
        var flowyearly = Time2.TSTools.ChangeZoomLevel(c.M11Flow, Time2.TimeStepUnit.Year, true);

        double slope = 0; 
        Slopes.TryGetValue(c.ID, out slope);
        double coarsesand = 0;
        CoarseSand.TryGetValue(c.ID, out coarsesand);
        double finesand = 0;
        FineSand.TryGetValue(c.ID, out finesand);
        double organicsoil = 0;
        Humus.TryGetValue(c.ID, out organicsoil);

        for (int i = Start.Year; i <= End.Year; i++)
        {
          values.Add(EvaluateEquation(coarsesand, finesand, organicsoil, precipyearly.Items.First(v => v.Time.Year == i).Value, slope) * flowyearly.Items.First(v=>v.Time.Year ==i).Value/365.0/86400.0);
        }
      }
      FirstYear = Start.Year;
      NewMessage("Initialized.");
    }

    /// <summary>
    /// Gets the value in kg/s
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      List<double> data;
      if (deposition.TryGetValue(c.ID, out data))
        return data[CurrentTime.Year - FirstYear];

      return 0;
    }

    /// <summary>
    /// Return the result in g/l
    /// </summary>
    /// <param name="CoarseSandPercentage"></param>
    /// <param name="FineSandPercentage"></param>
    /// <param name="HumusPercentage"></param>
    /// <param name="Precipitation"></param>
    /// <param name="Slope"></param>
    /// <returns></returns>
    public double EvaluateEquation(double CoarseSandPercentage, double FineSandPercentage, double HumusPercentage, double Precipitation, double Slope)
    {
      return Math.Max(MaxConcentration, Math.Exp(Par1 * Precipitation + Par2 + Par3 * FineSandPercentage +  Par4 * Slope)) ;

      //1. version
//      return Math.Max(MaxConcentration, Math.Exp(Par1 * Precipitation + Par2*FineSandPercentage*HumusPercentage + Par3 + Par4* CoarseSandPercentage +Par5 *Slope) * Math.Exp(Par6 / 2.0));
    }



    #region Properties

    private SafeFile _SoilTypeFile;
    public SafeFile SoilTypeFile
    {
      get { return _SoilTypeFile; }
      set
      {
        if (_SoilTypeFile != value)
        {
          _SoilTypeFile = value;
          NotifyPropertyChanged("SoilTypeFile");
        }
      }
    }
    

    private SafeFile  _Slope;
    public SafeFile  Slope
    {
      get { return _Slope; }
      set
      {
        if (_Slope != value)
        {
          _Slope = value;
          NotifyPropertyChanged("Slope");
        }
      }
    }
    

    private double _Par1 = .00023;
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

    private double _Par2 = -.445;
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

    private double _Par3 = -0.0088;
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

    private double _Par4 = -0.00228;
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


    private double _MaxConcentration = 2.5;
    public double MaxConcentration
    {
      get { return _MaxConcentration; }
      set
      {
        if (_MaxConcentration != value)
        {
          _MaxConcentration = value;
          NotifyPropertyChanged("MaxConcentration");
        }
      }
    }
    
    #endregion


  }
}
