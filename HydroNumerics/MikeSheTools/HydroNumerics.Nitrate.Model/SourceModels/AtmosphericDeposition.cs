using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time2;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;


using NPOI;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;


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
      return deposition[c.ID][CurrentTime.Year - FirstYear] * MultiplicationPar + AdditionPar; 
    }

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      Dictionary<XYPoint, List<double>> Data = new Dictionary<XYPoint,List<double>>();

      XSSFWorkbook hssfwb;
      using (FileStream file = new FileStream(ExcelFile.FileName, FileMode.Open, FileAccess.Read))
      {
        hssfwb = new XSSFWorkbook(file);
      }


      List<IRow> DataRows = new List<IRow>();
      var sheet = hssfwb.GetSheet("Ndep_Tot");
      for (int row = 1; row <= sheet.LastRowNum; row++)
      {
        if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
        {
          DataRows.Add(sheet.GetRow(row));
        }
      }


      using (ShapeReader sr = new ShapeReader(Shapefile.FileName))
      {
        FirstYear = (int)DataRows.First().Cells[0].NumericCellValue;
        for (int i = 0; i < sr.Data.NoOfEntries; i++)
        {
          int icoor = sr.Data.ReadInt(i, "i");
          int jcoor = sr.Data.ReadInt(i, "j");

          XYPoint point = (XYPoint)sr.ReadNext();
          
          //Create the timestampseries and set unit to kg/m2/s;
          var data = DataRows.Where(v => (int)v.Cells[3].NumericCellValue == icoor & (int)v.Cells[4].NumericCellValue == jcoor).OrderBy(v => (int)v.Cells[0].NumericCellValue).Select(v => v.Cells[6].NumericCellValue / (365.0 * 86400.0 * 1.0e6)).ToList();


          if(data.Count()>0)
            Data.Add(point, data);

        }
      }


      foreach (var c in Catchments)
      {
        XYPolygon poly = null;

        if (c.Geometry is XYPolygon)
          poly = c.Geometry as XYPolygon;
        else if (c.Geometry is MultiPartPolygon)
          poly = ((MultiPartPolygon)c.Geometry).Polygons.First(); //Just use the first polygon

        double LakeArea = c.Lakes.Sum(l => l.Geometry.GetArea()); //Get the area of the lakes
        if (c.BigLake != null) //Add the big lake
          LakeArea += c.BigLake.Geometry.GetArea();
        
        if (poly != null)
        {
          var point = new XYPoint(poly.PlotPoints.First().Longitude, poly.PlotPoints.First().Latitude); //Take one point in the polygon
          var closestpoint = Data.Keys.Select(p => new Tuple<XYPoint, double>(p, p.GetDistance(point))).OrderBy(s => s.Item2).First().Item1;
          deposition.Add(c.ID, new List<double>(Data[closestpoint].Select(v=>v*LakeArea)));
        }

      }
      NewMessage("Initialized");
      



    }

  }
}
