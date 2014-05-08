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

    private SafeFile  _ShapeFile;
    public SafeFile  ShapeFile
    {
      get { return _ShapeFile; }
      set
      {
        if (_ShapeFile != value)
        {
          _ShapeFile = value;
          NotifyPropertyChanged("ShapeFile");
        }
      }
    }

    private SafeFile _DBFFile;
    public SafeFile DBFFile
    {
      get { return _DBFFile; }
      set
      {
        if (_DBFFile != value)
        {
          _DBFFile = value;
          NotifyPropertyChanged("DBFFile");
        }
      }
    }
    
    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      if (Update)
      {
        var shapeconf = Configuration.Element("LocationFile");
        ShapeFile = new SafeFile { FileName = shapeconf.SafeParseString("ShapeFileName") };
        ShapeFile.ColumnNames.Add(shapeconf.SafeParseString("IDColumn"));

        var dbfconf = Configuration.Element("DataFile");
        DBFFile = new SafeFile { FileName = dbfconf.SafeParseString("DBFFileName") };
        DBFFile.ColumnNames.Add(dbfconf.SafeParseString("IDColumn"));
        DBFFile.ColumnNames.Add(dbfconf.SafeParseString("YearColumn"));
        DBFFile.ColumnNames.Add(dbfconf.SafeParseString("ValueColumn"));
      }
    }


    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      //Source, Catchment
      Dictionary<string, int> Sources = new Dictionary<string,int>();
      Dictionary<string, XYPoint> PointSources = new Dictionary<string,XYPoint>();

      List<XYPoint> points = new List<XYPoint>();

      //Read in the points
      using (ShapeReader sr = new ShapeReader(ShapeFile.FileName))
      {
        foreach (var gd in sr.GeoData)
        {
          XYPoint xp = gd.Geometry as XYPoint;
          if (!PointSources.ContainsKey(gd.Data[ShapeFile.ColumnNames[0]].ToString())) //Some sources are listed multiple times
            PointSources.Add(gd.Data[ShapeFile.ColumnNames[0]].ToString(), gd.Geometry as XYPoint);
        }
      }
      NewMessage("Distributing sources in catchments");
      //Distribute points on catchments
      Parallel.ForEach(PointSources, (p) =>
      {
        foreach (var c in Catchments)
        {
          if (c.Geometry.Contains(p.Value.X, p.Value.Y))
          {
            if (!c.CoastalZones.Any(co=>co.Contains(p.Value.X, p.Value.Y)))
              lock (Lock)
                Sources.Add(p.Key, c.ID);
              break;
          }
        }
      });
      NewMessage(PointSources.Count +" point sources distributed on " + Sources.Values.Distinct().Count().ToString() + " catchments");



     



      NewMessage("Reading outlet data");
      //Read source data and distrubute on catchments
      using (DBFReader dbf = new DBFReader(DBFFile.FileName))
      {
        for (int i = 0; i < dbf.NoOfEntries; i++)
        {
          string id = dbf.ReadString(i, DBFFile.ColumnNames[0]).Trim();
          int year = dbf.ReadInt(i, DBFFile.ColumnNames[1]);
          double value = dbf.ReadDouble(i, DBFFile.ColumnNames[2]);

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

      NewMessage("Initialized");
    }

    #region Properties

    



    #endregion

  }
}
