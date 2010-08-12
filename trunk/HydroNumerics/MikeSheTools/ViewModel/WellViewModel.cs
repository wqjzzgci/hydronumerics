using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellViewModel:INotifyPropertyChanged
  {
    private IWell _well;
    private Model _mshe;

    private int _col;
    private int _row;

    public List<CellViewModel> _cells;

    public WellViewModel(IWell Well, Model Mshe)
    {
      _well = Well;
      _mshe = Mshe;

      _cells = new List<CellViewModel>();

      if (!_mshe.GridInfo.TryGetIndex(X, Y, out _col, out _row))
      {
      }

    }

    /// <summary>
    /// Gets the WellID
    /// </summary>
    public string WellID
    {
      get { return _well.ID; }
    }

    public double X
    {
      get { return _well.X; }
    }

    public double Y
    {
      get { return _well.Y; }
    }

  
#region INotifyPropertyChanged Members

public event PropertyChangedEventHandler  PropertyChanged;

#endregion
}
}
