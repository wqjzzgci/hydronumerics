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
    private Dictionary<int, double> SandWetLandArea = new Dictionary<int, double>();
    private Dictionary<int, double> ClayWetLandArea = new Dictionary<int, double>();
    private Dictionary<int, double> AccuRain = new Dictionary<int, double>();


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
            Geometry = (IXYPolygon) ldata.Geometry,
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
            {
              double area = 0;
              if (l.SoilString == "ler")
              {
                if (!ClayWetLandArea.TryGetValue(c.ID, out area))
                  ClayWetLandArea.Add(c.ID, area);
                area += XYGeometryTools.CalculateSharedArea(c.Geometry, l.Geometry)/10000.0;
                ClayWetLandArea[c.ID] = area;
              }
              else
              {
                if (!SandWetLandArea.TryGetValue(c.ID, out area))
                  SandWetLandArea.Add(c.ID, area);
                area += XYGeometryTools.CalculateSharedArea(c.Geometry, l.Geometry) / 10000.0;
                SandWetLandArea[c.ID] = area;
              }
              c.Wetlands.Add(l);
            }
          }
      });
      NewMessage(wetlands.Count + " wetlands read and distributed on " + Catchments.Count(c => c.Wetlands.Count > 0) + " catchments.");

      foreach (var c in Catchments.Where(ca=>ca.Wetlands.Count>0))
      {
        AccuRain.Add(c.ID,c.Precipitation.GetTs(Time2.TimeStepUnit.Month).Average);
      }
    }

    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double nred = 0;

      if (SandWetLandArea.ContainsKey(c.ID) || ClayWetLandArea.ContainsKey(c.ID) && AccuRain[c.ID]!=0)
      {
        var precip = c.Precipitation.GetTs(Time2.TimeStepUnit.Month).GetValue(CurrentTime);

        double afs = Math.Abs( (precip - AccuRain[c.ID]) / AccuRain[c.ID] * Par1 + Par2);

        double area;
        if (ClayWetLandArea.TryGetValue(c.ID, out area))
        {
          if (CurrentTime.Month >= 5 & CurrentTime.Month <= 9)
            nred += Math.Pow(3.882 * afs, 0.7753) * area;
          else
            nred = Math.Pow(7.274 * afs, 0.3291) * area;
        }
        if (SandWetLandArea.TryGetValue(c.ID, out area))
        {
          if (CurrentTime.Month >= 5 & CurrentTime.Month <= 9)
            nred = Math.Pow(2.452 * afs, 0.7753) * area;
          else
            nred = Math.Pow(4.594 * afs, 0.3291) * area;
        }
        //Make sure we do not reduce more than what is available
        nred = Math.Max(0, Math.Min(CurrentMass, nred));
        nred /= (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0);
      }
      return nred;
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
