using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Linq;

using HydroNumerics.Core;
using HydroNumerics.Time2;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.Nitrate.Model
{
  public class Calibrator:BaseModel
  {

    private double _DampingFactor=0.3;
    public double DampingFactor
    {
      get { return _DampingFactor; }
      set
      {
        if (_DampingFactor != value)
        {
          _DampingFactor = value;
          NotifyPropertyChanged("DampingFactor");
        }
      }
    }
    

    private int _MaxNoOfIterations =30;
    public int MaxNoOfIterations
    {
      get { return _MaxNoOfIterations; }
      set
      {
        if (_MaxNoOfIterations != value)
        {
          _MaxNoOfIterations = value;
          NotifyPropertyChanged("MaxNoOfIterations");
        }
      }
    }

    private double _AbsoluteConvergence =10;
    public double AbsoluteConvergence
    {
      get { return _AbsoluteConvergence; }
      set
      {
        if (_AbsoluteConvergence != value)
        {
          _AbsoluteConvergence = value;
          NotifyPropertyChanged("AbsoluteConvergence");
        }
      }
    }

    private double  _InternalRatio=1;
    public double  InternalRatio
    {
      get { return _InternalRatio; }
      set
      {
        if (_InternalRatio != value)
        {
          _InternalRatio = value;
          NotifyPropertyChanged("InternalRatio");
        }
      }
    }

    private double _MainRatio=1;
    public double MainRatio
    {
      get { return _MainRatio; }
      set
      {
        if (_MainRatio != value)
        {
          _MainRatio = value;
          NotifyPropertyChanged("MainRatio");
        }
      }
    }
    

    private MainViewModel MW;


        /// <summary>
    /// Reads and parses the configuration element
    /// </summary>
    /// <param name="Configuration"></param>
    public override void ReadConfiguration(XElement Configuration)
    {
      base.ReadConfiguration(Configuration);
      if (Update)
      {
        AbsoluteConvergence = Configuration.SafeParseDouble("AbsoluteConvergence") ?? _AbsoluteConvergence;
        MaxNoOfIterations = Configuration.SafeParseInt("MaxNoOfIterations") ?? _MaxNoOfIterations;
        DampingFactor = Configuration.SafeParseDouble("DampingFactor") ?? _DampingFactor;
        InternalRatio = Configuration.SafeParseDouble("InternalRatio") ?? _InternalRatio;
        MainRatio = Configuration.SafeParseDouble("MainRatio") ?? _MainRatio;
      }
    }

    public void Calibrate(MainViewModel MW, DateTime CStart, DateTime CEnd)
    {
      DateTime CurrentTime = CStart;

      this.MW = MW;
      List<Catchment> SortedCatchments = new List<Catchment>();

      ConceptualSourceReducer GWCor = new ConceptualSourceReducer();
      GWCor.Name = "Calibrator";
      GWCor.SourceModelName = "GroundWater";

      var LastConceptual = MW.InternalReductionModels.LastOrDefault(c => c.GetType() == typeof(ConceptualSourceReducer));
      if (LastConceptual == null)
        MW.InternalReductionModels.Insert(0, GWCor);
      else
        MW.InternalReductionModels.Insert(MW.InternalReductionModels.IndexOf(LastConceptual) + 1, GWCor);

      if (!MW.StateVariables.Columns.Contains(GWCor.Name))
        MW.StateVariables.Columns.Add(GWCor.Name, typeof(double));

      ConceptualSourceReducer IntCor = new ConceptualSourceReducer();
      IntCor.Name = "Calib_Int";
      MW.InternalReductionModels.Add(IntCor);
      if (!MW.StateVariables.Columns.Contains(IntCor.Name))
        MW.StateVariables.Columns.Add(IntCor.Name, typeof(double));

      ConceptualSourceReducer MainCor = new ConceptualSourceReducer();
      MainCor.Name = "Calib_Main";
      MW.MainStreamRecutionModels.Add(MainCor);
      if (!MW.StateVariables.Columns.Contains(MainCor.Name))
        MW.StateVariables.Columns.Add(MainCor.Name, typeof(double));

      DataTable dt = new DataTable();
      dt.Columns.Add("ID15", typeof(int));
      dt.Columns.Add("No_iterations", typeof(int));
      dt.Columns.Add("LastError", typeof(double));
      dt.Columns.Add("RedFactor", typeof(double));

      foreach (var item in MW.EndCatchments)
      {
        GetCatchmentsWithObs(item, SortedCatchments);
      }
      foreach (var item in MW.AllCatchments.Values)
      {
        GWCor.Reduction.Add(item.ID, 0);
        IntCor.Reduction.Add(item.ID, 0);
        MainCor.Reduction.Add(item.ID, 0);
      }
      int totaliter = 0;
      foreach (var v in SortedCatchments)
      {
        List<double> Errors = new List<double>();
        double localdamp = DampingFactor;
        double currentreducer = 0;
        double Error = double.MaxValue;
        int itercount = 0;


        NewMessage("Calibrating " + v.ID);
        while (Math.Abs(Error) > AbsoluteConvergence & itercount < MaxNoOfIterations)
        {
          v.ObsNitrate = null;
          v.SimNitrate = null;
          double accgws = 0;
          double accs = 0;
          double accsink = 0;
          double accmainsink = 0;
          double obssum = 0;

          CurrentTime = CStart;
          while (CurrentTime < CEnd)
          {
            v.MoveInTime(CurrentTime);
            double obsn =v.Measurements.Nitrate.GetValue(CurrentTime, InterpolationMethods.DeleteValue);
            if (obsn != v.Measurements.Nitrate.DeleteValue)
            {
              obssum += obsn;
              accgws += AccumulateUpstream(GWCor.SourceModelName, v, CurrentTime);
              foreach (var s in MW.InternalReductionModels)
                accsink += AccumulateUpstream(s.Name, v, CurrentTime);
              foreach (var s in MW.SourceModels.Where(ss => ss.Name != GWCor.SourceModelName))
                accs += AccumulateUpstream(s.Name, v, CurrentTime);
              foreach (var s in MW.MainStreamRecutionModels)
                accmainsink += AccumulateUpstream(s.Name, v, CurrentTime);
            }
            CurrentTime = CurrentTime.AddMonths(1);
          }

          double[] sim;
          double[] obs;
          v.ObsNitrate.AlignRemoveDeletevalues(v.SimNitrate, out obs, out sim);
          double simerror = obs.Sum() - sim.Sum();

          Error = (accs + accgws - accsink - accmainsink) - obssum;
          Errors.Add(Error);
          NewMessage  (Error.ToString());

          currentreducer = Error / accgws * localdamp;

          if (itercount > 5)
          {
            if (Error > Errors.Skip(itercount - 4).Take(2).Select(e => Math.Abs(e)).Max())
            {
              NewMessage("Reduce damping and resetting reducer to first value");
              localdamp *= 0.5;
              currentreducer = Errors.First() / accgws * localdamp; ;
            }
          }


          SendReducUpstream(v, GWCor.Reduction, currentreducer);
          SendReducUpstream(v, IntCor.Reduction, InternalRatio * currentreducer);
          SendReducUpstream(v, MainCor.Reduction, MainRatio *currentreducer);
          itercount++;
        }
        totaliter += itercount;

        var row = dt.NewRow();
        row[0] = v.ID;
        row[1] = itercount;
        row[2] = Error;
        row[3] = GWCor.Reduction[v.ID];
        dt.Rows.Add(row);

        NewMessage(v.ID + " calibrated in " + itercount + " iterations. Final error: " + Error + ". ReductionFactor: " + GWCor.Reduction[v.ID]);
      }
      NewMessage("Total number of model calls: " + totaliter);
      var outdir = Path.GetDirectoryName(MW.AlldataFile.FileName);
      GWCor.DebugPrint(outdir, MW.AllCatchments);
      IntCor.DebugPrint(outdir, MW.AllCatchments);
      MainCor.DebugPrint(outdir, MW.AllCatchments);

      using (ShapeWriter sw = new ShapeWriter(Path.Combine(outdir, "CalibrationResult")) { Projection = MainViewModel.projection })
      {
        for (int i = 0; i < dt.Rows.Count; i++)
        {
          GeoRefData gd = new GeoRefData() { Geometry = MW.AllCatchments[(int)dt.Rows[i][0]].Geometry };
          gd.Data = dt.Rows[i];
          sw.Write(gd);
        }
      }




    }

    private void GetCatchmentsWithObs(Catchment c, List<Catchment> Upstreams)
    {
      foreach (var upc in c.UpstreamConnections)
        GetCatchmentsWithObs(upc, Upstreams);
      if (c.Measurements != null)
        Upstreams.Add(c);
    }

    private void SendReducUpstream(Catchment c, Dictionary<int, double> Reduction, double MultiFactor)
    {
      foreach (var item in c.UpstreamConnections.Where(cc => cc.Measurements == null))
      {
        SendReducUpstream(item, Reduction, MultiFactor);
      }
      Reduction[c.ID] += MultiFactor;
    }

    private double AccumulateUpstream(string Column, Catchment c, DateTime Time)
    {

      double Value = 0;
      var staterow = MW.StateVariables.Rows.Find(new object[] { c.ID, Time });
      if (!staterow.IsNull(Column))
      {
        Value += (double)staterow[Column];
      }
      foreach (var upc in c.UpstreamConnections)
      {
        Value += AccumulateUpstream(Column, upc, Time);
      }
      return Value;
    }



  }
}
