using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class LakeSink : BaseModel, ISink
  {

    private Dictionary<int, int> MO5;

    public LakeSink()
    {

    }


    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);

      if (Update)
      {
        Alpha = Configuration.SafeParseDouble("Alpha") ?? _Alpha;
        Beta = Configuration.SafeParseDouble("Beta") ?? _Beta;
        Par1 = Configuration.SafeParseDouble("Par1") ?? _Par1;
        ShapeFile = new SafeFile() { FileName = Configuration.SafeParseString("ShapeFileName") };
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("NameColumn") ?? "NAVN");
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("DepthColumn") ?? "Dybde");
        ShapeFile.ColumnNames.Add(Configuration.SafeParseString("InitNColumn") ?? "SoInitNmgL");

      }
    }

    public override void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      MO5 = new Dictionary<int, int>();
      MO5.Add(1, 1);
      MO5.Add(2, 2);
      MO5.Add(3, 3);
      MO5.Add(4, 4);
      MO5.Add(5, 5);
      MO5.Add(6, 6);
      MO5.Add(7, 6);
      MO5.Add(8, 5);
      MO5.Add(9, 4);
      MO5.Add(10, 3);
      MO5.Add(11, 2);
      MO5.Add(12, 1);

      Dictionary<string, Tuple<double, double>> LakeDepths = new Dictionary<string, Tuple<double, double>>();

      using (ShapeReader s = new ShapeReader(ShapeFile.FileName))
      {
        for (int i = 0; i < s.Data.NoOfEntries; i++)
        {
          LakeDepths.Add(s.Data.ReadString(i, ShapeFile.ColumnNames[0]), new Tuple<double,double>(s.Data.ReadDouble(i, ShapeFile.ColumnNames[1]),s.Data.ReadDouble(i, ShapeFile.ColumnNames[2])));
        }
      }

      foreach (var c in Catchments.Where(ca => ca.BigLake != null))
      {
        Tuple<double, double> depth;
        if (!LakeDepths.TryGetValue(c.BigLake.Name, out depth))
        {
          NewMessage(c.BigLake.Name + " removed! No entry found in " + ShapeFile.FileName);
          c.BigLake = null;
        }
        else if (c.M11Flow == null)
        {
          NewMessage(c.BigLake.Name + " removed! No Mike11 flow.");
          c.BigLake = null;
        }
        else
        {
          c.BigLake.Volume = c.BigLake.Geometry.GetArea() * depth.Item1;
          c.BigLake.RetentionTime = c.BigLake.Volume / (c.M11Flow.GetTs(Time2.TimeStepUnit.Month).Average * 365.0 * 86400.0);
          c.BigLake.CurrentNMass = depth.Item2 * c.BigLake.Volume;
        }
      }
    }

    /// <summary>
    /// Returns the reduction in kg/s
    /// </summary>
    /// <param name="c"></param>
    /// <param name="CurrentMass"></param>
    /// <param name="CurrentTime"></param>
    /// <returns></returns>
    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      if (c.BigLake == null)
        return 0;
      else
      {
        double Reducer;
        if (c.BigLake.RetentionTime > 1)
        {
          Reducer = (Par1 * MO5[CurrentTime.Month] - c.Temperature.GetValue(CurrentTime, Time2.InterpolationMethods.DeleteValue))/100.0;
        }
        else
        {
          //Get the lake temperature
          double T = LakeTemperatureFromAir(c.Temperature.GetValue(CurrentTime, Time2.InterpolationMethods.DeleteValue), c.Temperature.GetValue(CurrentTime.AddMonths(-1),Time2.InterpolationMethods.DeleteValue), CurrentTime);
          c.BigLake.Temperature.Items.Add(new Time2.TimeStampValue(CurrentTime, T));
          
          Reducer = Alpha * Math.Pow(Beta, T - 20.0);
        }


        c.BigLake.CurrentNMass += CurrentMass;
        double removedN = Reducer * c.BigLake.CurrentNMass;
        //From m3/s to m3
        double mflow = c.M11Flow.GetTs(Time2.TimeStepUnit.Month).GetValue(CurrentTime) * DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400;
        double NOut = (c.BigLake.CurrentNMass - removedN) / (c.BigLake.Volume +mflow) * mflow;
//        NOut = Math.Max(0,Math.Min(c.BigLake.CurrentNMass - removedN, NOut));

        c.BigLake.CurrentNMass = c.BigLake.CurrentNMass - removedN - NOut ;

        //Store some results
        c.BigLake.NitrateReduction.Items.Add(new Time2.TimeStampValue(CurrentTime, Reducer));
        c.BigLake.NitrateConcentration.Items.Add(new Time2.TimeStampValue(CurrentTime, c.BigLake.CurrentNMass / c.BigLake.Volume));
        c.BigLake.FlushingRatio.Items.Add(new Time2.TimeStampValue(CurrentTime, c.BigLake.Volume / mflow)); 

        return (CurrentMass - NOut)/( DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400);
      }
    }


    public double LakeTemperatureFromAir(double AirTemperature, double AirTemperaturePreviousMonth, DateTime CurrentTime)
    {
      return 1.517 * 0.3034 * AirTemperature + 0.1909 * AirTemperaturePreviousMonth + 0.6347 * AirTemperature * Math.Sin(Math.PI * CurrentTime.Month / 13.0);
    }


    #region Properties

   
    

    private double _Par1=6.117;
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

    private double _Alpha=0.455;
    public double Alpha
    {
      get { return _Alpha; }
      set
      {
        if (_Alpha != value)
        {
          _Alpha = value;
          NotifyPropertyChanged("Alpha");
        }
      }
    }

    private double _Beta=1.087;
    public double Beta
    {
      get { return _Beta; }
      set
      {
        if (_Beta != value)
        {
          _Beta = value;
          NotifyPropertyChanged("Beta");
        }
      }
    }

    private SafeFile _ShapeFileName;
    public SafeFile ShapeFile
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
    
    



    #endregion

  }
}
