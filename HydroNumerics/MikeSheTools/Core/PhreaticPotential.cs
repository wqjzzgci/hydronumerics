using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.Core
{
  internal class PhreaticPotential:IXYZTDataSet
  {
    IXYZTDataSet _potential;
    IXYZDataSet _bottomOfCell;
    IXYZDataSet _thicknessOfCell;
    private double _phreaticFactor = 0.5;
    private double _deleteValue;

    private int count = 0;

    //Buffer on the timesteps
    Dictionary<int, PhreaticPotentialData> _bufferedData = new Dictionary<int, PhreaticPotentialData>();
    private LinkedList<int> AccessList = new LinkedList<int>();
    
    private static object _lock = new object();

    internal PhreaticPotential(IXYZTDataSet Potential, MikeSheGridInfo Grid, double DeleteValue)
    {
      _deleteValue = DeleteValue;
      _potential = Potential;
      _bottomOfCell = Grid.LowerLevelOfComputationalLayers;
      _thicknessOfCell = Grid.ThicknessOfComputationalLayers;
    }


    #region IXYZTDataSet Members

    public IList<DateTime> TimeSteps
    {
      get
      {
        return _potential.TimeSteps;
      }
    }

    /// <summary>
    /// Returns the phreatic potential.
    /// Note that it returns a reference to a matrix
    /// </summary>
    /// <param name="TimeStep"></param>
    /// <returns></returns>
    public IMatrix3d TimeData(int TimeStep)
    {
      PhreaticPotentialData PC;
      lock (_lock)
      {
        if (!_bufferedData.TryGetValue(TimeStep, out PC))
        {
          count++;
          PC = new PhreaticPotentialData(_potential.TimeData(TimeStep), _bottomOfCell.Data, _thicknessOfCell.Data, _phreaticFactor, _deleteValue);
          AccessList.AddLast(TimeStep);
          AddToBuffer(TimeStep, PC);
        }
      AccessList.Remove(TimeStep);
      AccessList.AddLast(TimeStep);
      }
      return PC;
    }

    private void AddToBuffer(int TimeStep, PhreaticPotentialData PC)
    {
      if (AccessList.Count > DFS3.MaxEntriesInBuffer)
      {
        _bufferedData.Remove(AccessList.First());
        AccessList.RemoveFirst();
      }

      _bufferedData.Add(TimeStep, PC);

    }

    public IMatrix3d TimeData(DateTime TimeStep)
    {
      return TimeData(_potential.GetTimeStep(TimeStep));
    }

    public int GetTimeStep(DateTime TimeStep)
    {
      return _potential.GetTimeStep(TimeStep);
    }

    #endregion
  }
}
