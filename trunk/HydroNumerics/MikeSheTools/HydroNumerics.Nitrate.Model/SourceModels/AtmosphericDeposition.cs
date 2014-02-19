using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time2;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

using LinqToExcel;

namespace HydroNumerics.Nitrate.Model
{
  public class AtmosphericDeposition:BaseModel,ISource
  {
    private Dictionary<int, List<double>> deposition = new Dictionary<int, List<double>>();
    private int FirstYear;

    public AtmosphericDeposition()
    {
    }

    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      if (Update)
      {
        Shapefile = new SafeFile() { FileName = Configuration.Element("LocationFile").SafeParseString("ShapeFileName") };
        ExcelFile = new SafeFile() { FileName = Configuration.Element("DataFile").SafeParseString("ExcelFileName") };
      }
    }

    private SafeFile  _Shapefile;
    public SafeFile  Shapefile
    {
      get { return _Shapefile; }
      set
      {
        if (_Shapefile != value)
        {
          _Shapefile = value;
          NotifyPropertyChanged("Shapefile");
        }
      }
    }

    private SafeFile _ExcelFile;
    public SafeFile ExcelFile
    {
      get { return _ExcelFile; }
      set
      {
        if (_ExcelFile != value)
        {
          _ExcelFile = value;
          NotifyPropertyChanged("ExcelFile");
        }
      }
    }
    
    

    /// <summary>
    /// Returns the atmospheric deposition in kg/s
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetValue(Catchment c, DateTime CurrentTime)
    {
      return c.Geometry.GetArea() * deposition[c.ID][CurrentTime.Year - FirstYear];
    }

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      Dictionary<XYPoint, List<double>> Data = new Dictionary<XYPoint,List<double>>();
      var excel = new ExcelQueryFactory();
      excel.FileName = ExcelFile.FileName;
      using (ShapeReader sr = new ShapeReader(Shapefile.FileName))
      {
        var values = (from x in excel.Worksheet("Ndep_Tot")
                      select x).ToList();

        FirstYear = values.First()[0].Cast<int>();
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          int icoor = sr.Data.ReadInt(i, "i");
          int jcoor = sr.Data.ReadInt(i, "j");

          XYPoint point = (XYPoint)sr.ReadNext();
          
          //Create the timestampseries and set unit to kg/m2/s;
          var data = values.Where(v=>v[3].Cast<int>()==icoor & v[4].Cast<int>() ==jcoor).OrderBy(v=>v[0].Cast<int>()).Select(v=>v[6].Cast<double>()/(365.0*86400.0*1.0e6));
          
          if(data.Count()>0)
            Data.Add(point, new List<double>(data));

        }
      }


      foreach (var c in Catchments)
      {
        XYPolygon poly = null;

        if (c.Geometry is XYPolygon)
          poly = c.Geometry as XYPolygon;
        else if (c.Geometry is MultiPartPolygon)
          poly = ((MultiPartPolygon)c.Geometry).Polygons.First(); //Just use the first polygon

        

        if (poly != null)
        {
          var point = new XYPoint(poly.PlotPoints.First().Longitude, poly.PlotPoints.First().Latitude);

          var closestpoint = Data.Keys.Select(p => new Tuple<XYPoint, double>(p, p.GetDistance(point))).OrderBy(s => s.Item2).First().Item1;
          deposition.Add(c.ID, Data[closestpoint]);

        }

      }
      NewMessage("Initialized");
      



    }

  }
}
