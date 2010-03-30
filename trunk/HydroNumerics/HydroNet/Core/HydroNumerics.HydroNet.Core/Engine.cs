using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  /// <summary>
  /// This class runs the actual simulation
  /// It orders the network and performs timestepping.
  /// </summary>
  public class Engine
  {
    private IEnumerable<IWaterBody> _network;
    private bool _initialized = false;

    public Engine(IEnumerable<IWaterBody> Network)
    {
      _network = Network;
    }

    private void Initialize()
    {
      _initialized = true;
      //ToDO: sort network according to topology
      //Warn if there are WBs with no inflow.
    }

    /// <summary>
    /// Moves the entire network in time from start to end using the provided timestep.
    /// </summary>
    /// <param name="Start"></param>
    /// <param name="End"></param>
    /// <param name="TimeStep"></param>
    public void MoveInTime(DateTime Start, DateTime End, TimeSpan TimeStep)
    {
      foreach (IWaterBody IW in _network)
        IW.CurrentStartTime = Start;
      
      while (Start < End)
      {
        MoveInTime(TimeStep);
        Start += TimeStep;      
      }
    }

    /// <summary>
    /// Moves the entire network one timestep from the current time
    /// </summary>
    /// <param name="TimeStep"></param>
    public void MoveInTime(TimeSpan TimeStep)
    {
      if (!_initialized)
        Initialize();
      foreach (IWaterBody IW in _network)
        IW.MoveInTime(TimeStep);
    }
  }
}
