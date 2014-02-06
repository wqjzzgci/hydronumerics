using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;


using HydroNumerics.Time2;
using HydroNumerics.Core;
using HydroNumerics.Geometry;

using DotSpatial.Data;

namespace HydroNumerics.Nitrate.Model
{
  public class Catchment:BaseViewModel
  {
    private SortedList<DateTime, State> States = new SortedList<DateTime, State>();

    public TimeStampSeries M11Flow { get; set; }
    public TimeStampSeries Precipitation { get; set; }
    public TimeStampSeries Temperature { get; set; }


    public Catchment(int ID)
    {
      this.ID = ID;
      UpstreamConnections = new List<Catchment>();
      Particles = new List<Particle>();
      SourceModels = new List<ISource>();
      InternalReduction = new List<IReductionModel>();
      GlobalReduction = new List<IReductionModel>();

    }


    #region Properties

   

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

    public List<IReductionModel> InternalReduction { get; internal set; }


    public List<IReductionModel> GlobalReduction { get; internal set; }

    #endregion

    /// <summary>
    /// Takes a time step
    /// </summary>
    /// <param name="Endtime"></param>
    public void MoveInTime(DateTime Endtime)
    {

      double output = 0;

      CurrentState = StateVariables.Rows.Find(new object[]{ID,Endtime});

      if (CurrentState == null)
      {
        CurrentState = StateVariables.NewRow();
        CurrentState["ID"] = ID;
        CurrentState["Time"] = Endtime;
        StateVariables.Rows.Add(CurrentState);
      }

      CurrentTime = Endtime;

      foreach (var S in SourceModels)
      {
        double value;
        if (S.Update)
        {
          value = S.GetValue(this, Endtime);
          CurrentState[S.Name] = value;
        }
        output += (double) CurrentState[S.Name];
      }


      foreach (var R in InternalReduction)
      {
        double value;
        if (R.Update)
        {
          value =R.GetReduction(this, output, Endtime);
          CurrentState[R.Name] = value;
        }
        output -= (double)CurrentState[R.Name];
      }

      foreach (var ups in UpstreamConnections)
        output += ups.GetDownStreamOutput(Endtime);


      //Do the global reductions
      foreach (var R in GlobalReduction)
      {
        double value;
        if (R.Update)
        {
          value = R.GetReduction(this, output, Endtime);
          CurrentState[R.Name] = value;

        }

        output -= (double)CurrentState[R.Name];
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

      CurrentState = StateVariables.Rows.Find(new object[] { ID, EndTime });

      //First look if we already have the time step. If not move in time
      if (CurrentState == null)
        MoveInTime(EndTime);

      //if (! ((double?) CurrentState["DownStreamOutput"]).HasValue)
      //  MoveInTime(EndTime);

      return ((double)CurrentState["DownStreamOutput"]);
    }



   
  }
}
