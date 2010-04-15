using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  [DataContract]
  public class Model
  {
    [DataMember]
    public List<IWaterBody> _waterBodies = new List<IWaterBody>();
    [DataMember]
    private List<IWaterSinkSource> _sinkSources = new List<IWaterSinkSource>();
    [DataMember]
    private List<IEvaporationBoundary> _evapoBoundaries = new List<IEvaporationBoundary>();

    private bool _initialized = false;

    #region Simulations methods




    /// <summary>
    /// Moves the entire network in time from start to end using the provided timestep.
    /// When using this method the entire network will be filled with water at the beginning
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="TimeStep"></param>
    public void MoveInTime(DateTime Start, DateTime End, TimeSpan TimeStep, bool resetState)
    {
      if (resetState)
        foreach (IWaterBody IW in _waterBodies)
          IW.SetState("Empty", Start,new WaterPacket(IW.Volume));

      while ((Start += TimeStep) <= End)
      {
        MoveInTime(TimeStep);
      }
      if (Start>End)
        MoveInTime(End.Subtract(Start.Subtract(TimeStep)));
    }

    /// <summary>
    /// Moves the entire network one timestep from the current time
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      if (!_initialized)
        Initialize();
      foreach (IWaterBody IW in _waterBodies)
        IW.MoveInTime(TimeStep);
    }

    /// <summary>
    /// Store the current State
    /// </summary>
    /// <returns>State identifier.</returns>
    public string KeepCurrentState() 
    {
      return "State";
    
    }

    /// <summary>
    /// Restores the state identified by the parameter stateID. If the state identifier identified by
    /// stateID is not known an exception is be trown.
    /// </summary>
    /// <param name="stateID">State identifier.</param>
    void RestoreState(string stateID)
    {
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
    /// Saves the model to a file. Does not save output and states. Saves initial conditions?
    /// </summary>
    /// <param name="FileName"></param>
    public void Save(string FileName)
    {
      List<Type> knownTypes = new List<Type>();
      knownTypes.Add(typeof(WaterPacket));
      knownTypes.Add(typeof(WaterWithChemicals));
      knownTypes.Add(typeof(IsotopeWater));
      knownTypes.Add(typeof(Stream));
      knownTypes.Add(typeof(Lake));
      knownTypes.Add(typeof(EvaporationRateBoundary)); 
      knownTypes.Add(typeof(FlowBoundary));
      knownTypes.Add(typeof(GroundWaterBoundary));

      using (FileStream Fs = new FileStream(FileName, FileMode.Create))
      {
        DataContractSerializer ds = new DataContractSerializer(this.GetType(), knownTypes, int.MaxValue, false, true, null);
        ds.WriteObject(Fs, this);
      }
    }

    /// <summary>
    /// Opens a stored model from a file
    /// </summary>
    /// <param name="FileName"></param>
    public void Open(string FileName)
    {
      using (FileStream Fs = new FileStream(FileName, FileMode.Open))
      {
        DataContractSerializer ds = new DataContractSerializer(this.GetType(), null, int.MaxValue, false, true, null);
        Model M = (Model)ds.ReadObject(Fs);
        _waterBodies = M._waterBodies;
        _sinkSources = M._sinkSources;
        _evapoBoundaries = M._evapoBoundaries;
      }
    }

    #region Private Methods
    private void Initialize()
    {
      _initialized = true;
      //ToDo: sort network according to topology
      //Warn if there are WBs with no inflow.
    }

#endregion
  }
}
