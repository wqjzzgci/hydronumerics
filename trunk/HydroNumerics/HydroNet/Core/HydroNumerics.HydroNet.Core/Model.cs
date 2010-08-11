using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  [KnownType(typeof(Stream))]
  [KnownType(typeof(Lake))]
  public class Model
  {
    [DataMember]
    public List<IWaterBody> _waterBodies = new List<IWaterBody>();

    [DataMember]
    private List<IWaterSinkSource> _sinkSources = new List<IWaterSinkSource>();
    [DataMember]
    private List<IEvaporationBoundary> _evapoBoundaries = new List<IEvaporationBoundary>();

    [DataMember]
    public DateTime CurrentTime { get; private set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool Initialized { get; private set; }

    private List<GeoExchangeItem> _exchangeItems;
    private List<ExchangeItem> _itemsToLog;
    //private bool _initialized = false;


    /// <summary>
    /// Gets the list of ExchangeItems
    /// </summary>
    public List<GeoExchangeItem> ExchangeItems
    {
      get 
      {
        if (_exchangeItems == null)
        {
          _exchangeItems = new List<GeoExchangeItem>();
          foreach (IWaterBody IW in _waterBodies)
          {
            foreach (IWaterSinkSource IWS in IW.SinkSources)
              _exchangeItems.AddRange(IWS.ExchangeItems);
          }
        }      
        return _exchangeItems; 
      }
    }

    ///// <summary>
    ///// Gets the items that will be logged
    ///// </summary>
    //public List<ExchangeItem> ItemsToLog
    //{
    //  get
    //  {
    //    if (_itemsToLog == null)
    //    {
    //      _itemsToLog = new List<ExchangeItem>();
    //      _itemsToLog.AddRange(ExchangeItems.Where(var => var.IsOutput));
    //    }
    //    return _itemsToLog;
    //  }
    //}

    /// <summary>
    /// Gets the maximum time the model can run to
    /// </summary>
    public DateTime EndTime
    {
      get
      {
        return _waterBodies.Min(var => var.EndTime);
      }
    }

    #region Constuctors

    public Model()
    {
        Initialized = false;
    }


    #endregion


    #region Simulations methods

    /// <summary>
    /// Moves the entire network in time from CurrentTime to End using the provided timestep.
    /// When using this method the entire network will be filled with water at the beginning
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="TimeStep"></param>
    public void MoveInTime(DateTime End, TimeSpan TimeStep)
    {
      while (CurrentTime.Add(TimeStep) <= End)
      {
        MoveInTime(TimeStep);
      }

      if (CurrentTime > End)
      {
        MoveInTime(End.Subtract(CurrentTime.Subtract(TimeStep)));
        CurrentTime = End;
      }
    }

    /// <summary>
    /// Moves the entire network one timestep from the current time
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      if (!Initialized)
        Initialize();
      foreach (IWaterBody IW in _waterBodies)
        IW.MoveInTime(TimeStep);
      CurrentTime+=TimeStep;
    }

    /// <summary>
    /// Store the current State
    /// </summary>
    /// <returns>State identifier.</returns>
    public string KeepCurrentState() 
    {
      foreach (IWaterBody IW in _waterBodies)
        IW.KeepCurrentState("StateName");
      return "StateName";
    }

    /// <summary>
    /// Restores the state identified by the parameter stateID. If the state identifier identified by
    /// stateID is not known an exception is be trown.
    /// </summary>
    /// <param name="stateID">State identifier.</param>
    public void RestoreState(string stateID)
    {
      foreach (IWaterBody IW in _waterBodies)
        IW.RestoreState(stateID);
      CurrentTime = _waterBodies.First().CurrentTime;
    }

    /// <summary>
    /// Sets a state and fills the network with water
    /// </summary>
    /// <param name="stateID"></param>
    /// <param name="CurrentTime"></param>
    public void SetState(string stateID, DateTime CurrentTime, IWaterPacket WaterTypeToFillWith)
    {
      this.CurrentTime = CurrentTime;

      foreach (IWaterBody IW in _waterBodies)
        IW.SetState(stateID, CurrentTime, WaterTypeToFillWith.DeepClone(IW.Volume));
    }

    /// <summary>
    /// Clears a state from memory. If the state identifier identified by
    /// stateID is not known by the linkable component an exception should be trown.
    /// </summary>
    /// <param name="stateID">State identifier.</param>
    void ClearState(string stateID)
    {
    }

    #endregion

    /// <summary>
    /// Saves the model to a file.
    /// </summary>
    /// <param name="FileName"></param>
    public void Save(string FileName)
    {
      ModelFactory.SaveModel(FileName, this);
    }


    #region Private Methods
    public void Initialize()
    {
        if (!Initialized)
        {
            foreach (IWaterBody waterBody in _waterBodies)
            {
                foreach (IWaterSinkSource waterSinkSource in waterBody.SinkSources)
                {
                    waterSinkSource.Initialize();
                }
            }
            Initialized = true;
        }

      //ToDo: sort network according to topology
      //Warn if there are WBs with no inflow.
    }

#endregion
  }
}
