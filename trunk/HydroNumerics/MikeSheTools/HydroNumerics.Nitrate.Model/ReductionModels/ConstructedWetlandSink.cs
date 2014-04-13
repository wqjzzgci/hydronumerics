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
  public class ConstructedWetlandSink : BaseModel, ISink
  {

    private object Lock = new object();

    public ConstructedWetlandSink()
    {
    }



    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {
        ShapeFile = new SafeFile() { FileName = Configuration.SafeParseString("ShapeFileName") };
        Par1 = Configuration.SafeParseDouble("Par1")??_Par1;
        Par2 = Configuration.SafeParseDouble("Par2") ?? _Par2;
      }
    }



    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {

      List<Wetland> wetlands = new List<Wetland>();
      using (ShapeReader sr = new ShapeReader(ShapeFile.FileName))
      {
        foreach (var ldata in sr.GeoData)
        {
          Wetland w = new Wetland
          {
            Geometry = ldata.Geometry,
            Name = ldata.Data["Titel"].ToString(),
            SoilString = ldata.Data["jord_simp"].ToString()
          };
          int startyear = int.Parse(ldata.Data["Startaar"].ToString());
          if (startyear != 0)
            w.StartTime = new DateTime(startyear, 1, 1);
          wetlands.Add(w);
        }
      }

      Parallel.ForEach(wetlands.Where(w=>w.Geometry is XYPolygon), l =>
      {
        foreach (var c in Catchments)
          if (c.Geometry.OverLaps(l.Geometry as XYPolygon))
          {
            lock (Lock)
              c.Wetlands.Add(l);
            break;
          }
      });

      NewMessage(wetlands.Count + " wetlands read and distributed on " + Catchments.Count(c => c.Wetlands.Count > 0) + " catchments.");
    }

    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      return 0;
    }


    private SafeFile _ShapeFile;
    public SafeFile ShapeFile
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

    private double _Par1=17.4;
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

    private double _Par2=15.7;
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
    

  }
}
