using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


using HydroNumerics.Time2;
using HydroNumerics.Core;
using HydroNumerics.Geometry;


namespace HydroNumerics.Nitrate.Model
{
  public class Catchment : BaseViewModel
  {


    public Catchment(int ID)
    {
      this.ID = ID;
      UpstreamConnections = new List<Catchment>();
      Particles = new List<Particle>();
      SourceModels = new List<ISource>();
      InternalReduction = new List<ISink>();
      MainStreamReduction = new List<ISink>();

    }


    #region Properties


    private DMUStation _Measurements;
    public DMUStation Measurements
    {
      get { return _Measurements; }
      set
      {
        if (_Measurements != value)
        {
          _Measurements = value;
          NotifyPropertyChanged("Measurements");
        }
      }
    }
    

    public TimeStampSeries M11Flow { get; set; }
    public TimeStampSeries Precipitation { get; set; }
    public TimeStampSeries Temperature { get; set; }
    public TimeStampSeries Leaching { get; set; }


    private List<Lake> _Lakes = new List<Lake>();
    /// <summary>
    /// Gets the list of lakes within the catchment
    /// </summary>
    public List<Lake> Lakes
    {
      get { return _Lakes; }
    }
    
   

    private Catchment downstreamConnection;

    /// <summary>
    /// Gets and sets the downstream catchment
    /// </summary>
    public Catchment DownstreamConnection
    {
      get
      { return downstreamConnection;}
      set
      {
        if (value != downstreamConnection)
        {
          downstreamConnection = value;
          NotifyPropertyChanged("DownstreamConnection");
        }
      }
    }

    /// <summary>
    /// Gets the list of upstream catchments
    /// </summary>
    public List<Catchment> UpstreamConnections{get;private set;}

    /// <summary>
    /// Gets the list of particles ending up in this catchment
    /// </summary>
    public List<Particle> Particles { get; set; }


    private IXYPolygon _Geometry;
    /// <summary>
    /// Gets and sets the geometry
    /// </summary>
    public IXYPolygon Geometry
    {
      get { return _Geometry; }
      set
      {
        if (_Geometry != value)
        {
          _Geometry = value;
          NotifyPropertyChanged("Geometry");
        }
      }
    }

    private DateTime _CurrentTime;
    /// <summary>
    /// Gets the current time
    /// </summary>
    public DateTime CurrentTime
    {
      get { return _CurrentTime; }
      private set
      {
        if (_CurrentTime != value)
        {
          _CurrentTime = value;
          NotifyPropertyChanged("CurrentTime");
        }
      }
    }


    private DataRow _CurrentState;
    /// <summary>
    /// Gets the current state variables
    /// </summary>
    public DataRow CurrentState
    {
      get { return _CurrentState; }
      private set
      {
        if (_CurrentState != value)
        {
          _CurrentState = value;
          NotifyPropertyChanged("CurrentState");
        }
      }
    }

    public DataTable StateVariables { get; set; }
    
    public List<ISource> SourceModels { get; internal set; }

    public List<ISink> InternalReduction { get; internal set; }

    public List<ISink> MainStreamReduction { get; internal set; }

    #endregion


    public void Initialize(DateTime Start, DateTime End)
    {



    }

    /// <summary>
    /// Takes a time step
    /// </summary>
    /// <param name="Endtime"></param>
    public void MoveInTime(DateTime Endtime)
    {

      double output = 0;
      CurrentTime = Endtime;

      CurrentState = StateVariables.Rows.Find(new object[] { ID, CurrentTime });

      if (CurrentState == null)
      {
        CurrentState = StateVariables.NewRow();
        CurrentState["ID"] = ID;
        CurrentState["Time"] = CurrentTime;
        StateVariables.Rows.Add(CurrentState);
      }


      foreach (var S in SourceModels)
      {
        double value;
        if (S.Update)
        {
          value = S.GetValue(this, Endtime) * DateTime.DaysInMonth(Endtime.Year, Endtime.Month)*86400;
          CurrentState[S.Name] = value;
        }
        if (!CurrentState.IsNull(S.Name))
          output += (double)CurrentState[S.Name];
      }


      foreach (var R in InternalReduction)
      {
        double value;
        if (R.Update)
        {
          value =R.GetReduction(this, output, Endtime);
          CurrentState[R.Name] = value;
        }
        if (!CurrentState.IsNull(R.Name))
          output -= (double)CurrentState[R.Name];
      }

      foreach (var ups in UpstreamConnections)
        output += ups.GetDownStreamOutput(Endtime);


      //Do the global reductions
      foreach (var R in MainStreamReduction)
      {
        double value;
        if (R.Update)
        {
          value = R.GetReduction(this, output, Endtime);
          CurrentState[R.Name] = value;
        }
        if (!CurrentState.IsNull(R.Name))
          output -= (double)CurrentState[R.Name];
      }

      if(Precipitation!=null)
        CurrentState["Precipitation"] = Precipitation.GetValue(CurrentTime, InterpolationMethods.DeleteValue);
      if(Temperature!=null)
        CurrentState["Air Temperature"] = Temperature.GetValue(CurrentTime, InterpolationMethods.DeleteValue); 
      if(M11Flow !=null)
        CurrentState["M11Flow"] = M11Flow.GetValue(CurrentTime, InterpolationMethods.DeleteValue) * DateTime.DaysInMonth(CurrentTime.Year, CurrentTime.Month) * 86400;
      if(Leaching!=null)
        CurrentState["Leaching"] = Leaching.GetValue(CurrentTime, InterpolationMethods.DeleteValue)*DateTime.DaysInMonth(CurrentTime.Year,CurrentTime.Month)*86400;

      if (Measurements != null)
      {
        CurrentState["ObservedFlow"] = Measurements.Flow.GetValue(CurrentTime, InterpolationMethods.DeleteValue);
        CurrentState["ObservedNitrate"] = Measurements.Nitrate.GetValue(CurrentTime, InterpolationMethods.DeleteValue);
      }


      CurrentState["DownStreamOutput"] = output;
    }



    /// <summary>
    /// Gets the output from the catchment. Calling this method will make the catchment take a time step and call upstream catchments
    /// </summary>
    /// <param name="EndTime"></param>
    /// <returns></returns>
    public double GetDownStreamOutput(DateTime EndTime)
    {

      MoveInTime(EndTime);
      CurrentState = StateVariables.Rows.Find(new object[] { ID, EndTime });

      return ((double)CurrentState["DownStreamOutput"]);
    }



   
  }
}
