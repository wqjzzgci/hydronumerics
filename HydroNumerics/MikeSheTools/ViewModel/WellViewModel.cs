using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;

using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.Time.Core;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellViewModel:BaseViewModel 
  {
    private IWell _well;
    private Model _mshe;
    

    private int _col;
    private int _row;

    public ObservableCollection<CellViewModel> Cells { get; private set; }
    public ObservableCollection<Screen> Screens { get; private set; }
    public ObservableCollection<TimestampSeries> Observations { get; private set; }


    public bool HasChanges { get; set; }

    public WellViewModel(IWell Well)
    {
      _well = Well;

      Screens = new ObservableCollection<Screen>();
      Cells = new ObservableCollection<CellViewModel>();

      Observations = new ObservableCollection<TimestampSeries>();

      foreach (IIntake I in _well.Intakes)
      {
        foreach (Screen s in I.Screens)
          Screens.Add(s);
        Observations.Add(I.HeadObservations);
      }
    }

    public void LinkToMikeShe(Model Mshe)
    {
      _mshe = Mshe;
      if (!Mshe.GridInfo.TryGetIndex(X, Y, out _col, out _row))
      {
        Column = _col;
        Row = _row;
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
      set {
        if (value !=_well.X)
        {
          _well.X = value;
          NotifyPropertyChanged("X");
        }
      }
    }

    public ObservableCollection<Change> XHistory { get; set; }

    public double Y
    {
      get { return _well.Y; }
      set {
        if (value !=_well.Y)
        {
          _well.Y = value;
          NotifyPropertyChanged("Y");
        }
      }
    }

    public double Terrain
    {
      get { return _well.Terrain; }
      set {
        if (value !=_well.Terrain)
        {
          _well.Terrain = value;
          NotifyPropertyChanged("Terrain");
        }
      }
    }


    public int Column
    {
      get { return _col;}
      set 
      {
        if (_col!=value)
        {
          _col = value;
          NotifyPropertyChanged("Column");
        }
      }
    }

    public int Row
    {
      get { return _row; }
      set
      {
        if (_row != value)
        {
          _row = value;
          NotifyPropertyChanged("Row");
        }
      }
    }  
}
}
