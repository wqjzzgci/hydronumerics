using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;



using HydroNumerics.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class PointSource : BaseModel, ISource
  {

    private Dictionary<int, Dictionary<int, double>> YearlyData = new Dictionary<int, Dictionary<int, double>>();
    private object Lock = new object();

    public PointSource()
    { }

    public PointSource(XElement Configuration):base(Configuration)
    {

    }

    /// <summary>
    /// Returns the source rate to the catchment in kg/s at the current time
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>

    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      Dictionary<int, double> timevalues;
      if (YearlyData.TryGetValue(c.ID, out timevalues))
      {
        double value;
        if (timevalues.TryGetValue(CurrentTime.Year, out value))
          return value;
      }
      return 0;
    }


    public void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      if (Configuration != null)
      {
        var shapeconf = Configuration.Element("ShapeFileName");
        ShapeFileName = shapeconf.Value;
        ShapeIDColumnName = SafeParseString(shapeconf, "IDColumn");

        var dbfconf = Configuration.Element("DBFFileName");
        DBFFileName = dbfconf.Value;
        DBFIDColumnName = SafeParseString(dbfconf, "IDColumn");
        DBFYearColumn = SafeParseString(dbfconf, "YearColumn");
        DBFValueColumn = SafeParseString(dbfconf, "ValueColumn");
      }

      //Source, Catchment
      Dictionary<string, int> Sources = new Dictionary<string,int>();
      Dictionary<string, XYPoint> PointSources = new Dictionary<string,XYPoint>();

      List<XYPoint> points = new List<XYPoint>();

      //Read in the points
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
      {
        foreach (var gd in sr.GeoData)
        {
          XYPoint xp = gd.Geometry as XYPoint;
          PointSources.Add(gd.Data[ShapeIDColumnName].ToString(), gd.Geometry as XYPoint);
        }
      }

      //Distribute points on catchments
      Parallel.ForEach(PointSources, (p) =>
      {
        foreach (var c in Catchments)
        {
          if (c.Geometry.Contains(p.Value.X, p.Value.Y))
          {
            lock (Lock)
              Sources.Add(p.Key, c.ID);
            break;
          }
        }
      });

      //Read source data and distrubute on catchments
      using (DBFReader dbf = new DBFReader(DBFFileName))
      {
        for (int i = 0; i < dbf.NoOfEntries; i++)
        {
          string id = dbf.ReadString(i,_DBFIDColumnName).Trim();
          int year = dbf.ReadInt(i, DBFYearColumn);
          double value = dbf.ReadDouble(i, DBFValueColumn);

          int catchid;

          if(Sources.TryGetValue(id, out catchid))
          {
            Dictionary<int, double> timevalues;
            if (!YearlyData.TryGetValue(catchid, out timevalues))
            {
              timevalues = new Dictionary<int, double>();
              YearlyData.Add(catchid, timevalues);
            }
            if (timevalues.ContainsKey(year))
              timevalues[year] += value;
            else
              timevalues.Add(year, value);
          }
        }
      }

      foreach (var c in YearlyData.Values)
        foreach (var val in c.ToList())
          c[val.Key] = val.Value/ (365 * 86400);

    }

    #region Properties

    private string _DBFValueColumn ="totaln";
    public string DBFValueColumn
    {
      get { return _DBFValueColumn; }
      set
      {
        if (_DBFValueColumn != value)
        {
          _DBFValueColumn = value;
          NotifyPropertyChanged("DBFValueColumn");
        }
      }
    }
    

    private string _DBFYearColumn = "aar";
    public string DBFYearColumn
    {
      get { return _DBFYearColumn; }
      set
      {
        if (_DBFYearColumn != value)
        {
          _DBFYearColumn = value;
          NotifyPropertyChanged("DBFYearColumn");
        }
      }
    }
    

    private string _DBFIDColumnName ="id_2012";
    public string DBFIDColumnName
    {
      get { return _DBFIDColumnName; }
      set
      {
        if (_DBFIDColumnName != value)
        {
          _DBFIDColumnName = value;
          NotifyPropertyChanged("DBFIDColumnName");
        }
      }
    }
    

    private string _ShapeIDColumnName ="Id_2012";
    public string ShapeIDColumnName
    {
      get { return _ShapeIDColumnName; }
      set
      {
        if (_ShapeIDColumnName != value)
        {
          _ShapeIDColumnName = value;
          NotifyPropertyChanged("ShapeIDColumnName");
        }
      }
    } 


    private string _ShapeFileName;
    public string ShapeFileName
    {
      get { return _ShapeFileName; }
      set
      {
        if (_ShapeFileName != value)
        {
          _ShapeFileName = value;
          NotifyPropertyChanged("ShapeFileName");
        }
      }
    }

    private string _DBFFileName;
    public string DBFFileName
    {
      get { return _DBFFileName; }
      set
      {
        if (_DBFFileName != value)
        {
          _DBFFileName = value;
          NotifyPropertyChanged("DBFFileName");
        }
      }
    }
        

    #endregion

  }
}
