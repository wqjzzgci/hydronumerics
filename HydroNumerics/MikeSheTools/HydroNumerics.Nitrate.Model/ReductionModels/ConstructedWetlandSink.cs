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
    Dictionary<string, WetLandsReducer> SoilEquations = new Dictionary<string, WetLandsReducer>();


    public ConstructedWetlandSink()
    {
    }



    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {
        ShapeFile = new SafeFile() { FileName = Configuration.SafeParseString("ShapeFileName") };
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("YearColumn"));
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("NameColumn"));
        Par1 = Configuration.SafeParseDouble("Par1")??_Par1;
        Par2 = Configuration.SafeParseDouble("Par2") ?? _Par2;

        foreach (var soileq in Configuration.Elements("Soil"))
        {
          WetLandsReducer wr = new WetLandsReducer();
          wr.ReadConfiguration(soileq);
          SoilEquations.Add(wr.Name, wr);
        }
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
            Name = ldata.Data[ShapeFile.ColumnNames[1]].ToString(),
          };
          int startyear = int.Parse(ldata.Data[ShapeFile.ColumnNames[0]].ToString());
          if (startyear != 0)
            w.StartTime = new DateTime(startyear, 1, 1);
          else
            w.StartTime = Start;
          wetlands.Add(w);
        }
      }

      //Get the soil type
      Parallel.ForEach(wetlands, l =>
      {
        foreach (var soil in MainViewModel.SoilTypes)
        {
          if (l.Geometry.OverLaps((IXYPolygon)soil.Geometry))
          {
            if ((int)soil.Data[0] < 4)
              l.SoilString = "sand";
            else
              l.SoilString = "ler";
            break;
          }
        }
      });

      Parallel.ForEach(wetlands, l =>
      {
        foreach (var c in Catchments)
          if (c.Geometry.OverLaps(l.Geometry as IXYPolygon))
          {
            lock (Lock)
            {
              c.Wetlands.Add(l);
            }
          }
      });
      NewMessage(wetlands.Count + " wetlands read and distributed on " + Catchments.Count(c => c.Wetlands.Count > 0) + " catchments.");
    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      double nred = 0;

      var CurrentWetLands = c.Wetlands.Where(w => w.StartTime <= CurrentTime).ToList();

      if (CurrentWetLands.Count > 0)
      {
        var precip = c.Precipitation.GetTs(Time2.TimeStepUnit.Month).GetValue(CurrentTime);

        double accurain = c.Precipitation.GetTs(Time2.TimeStepUnit.Month).Average;
        double afs = Math.Abs((precip - accurain) / accurain * Par1 + Par2);

        foreach(var w in CurrentWetLands)
        {
          nred += SoilEquations[w.SoilString].GetReduction(CurrentTime, afs) * XYGeometryTools.CalculateSharedArea(c.Geometry, w.Geometry) / 10000.0;
        }

        //Make sure we do not reduce more than what is available
        nred = Math.Max(0, Math.Min(CurrentMass, nred));
        nred /= (DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400.0);
      }
      return nred * MultiplicationPar + AdditionPar;
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
