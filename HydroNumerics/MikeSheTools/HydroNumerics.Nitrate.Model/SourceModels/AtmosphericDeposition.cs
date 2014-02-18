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

    private string _ExcelFileName;
    public string ExcelFileName
    {
      get { return _ExcelFileName; }
      set
      {
        if (_ExcelFileName != value)
        {
          _ExcelFileName = value;
          NotifyPropertyChanged("ExcelFileName");
        }
      }
    }
    


    public AtmosphericDeposition()
    {
    }

    public AtmosphericDeposition(XElement Configuration):base(Configuration)
    {
    }

    public bool Update { get; set; }

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

    public void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {

      if (Configuration != null)
      {
        ShapeFileName = Configuration.Element("ShapeFileName").Value;
        ExcelFileName = Configuration.Element("ExcelFileName").Value;
      }

      Dictionary<XYPoint, List<double>> Data = new Dictionary<XYPoint,List<double>>();
      var excel = new ExcelQueryFactory();
      excel.FileName = ExcelFileName;
      using (ShapeReader sr = new ShapeReader(ShapeFileName))
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
        var poly = c.Geometry as XYPolygon;

        if (poly != null)
        {
          var point = new XYPoint(poly.PlotPoints.First().Longitude, poly.PlotPoints.First().Latitude);

          var closestpoint = Data.Keys.Select(p => new Tuple<XYPoint, double>(p, p.GetDistance(point))).OrderBy(s => s.Item2).First().Item1;
          deposition.Add(c.ID, Data[closestpoint]);

        }



      }
      



    }

  }
}
