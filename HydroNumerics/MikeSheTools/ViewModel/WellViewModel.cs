using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;

using HydroNumerics.JupiterTools;
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

    public ObservableCollection<Tuple<string,IEnumerable<TimestampValue>>> Observations 
    { 
      get
      {
        var obs = new ObservableCollection<Tuple<string,IEnumerable<TimestampValue>>>();
        
        foreach(var I in _well.Intakes)
          obs.Add(new Tuple<string,IEnumerable<TimestampValue>>(I.ToString(), I.HeadObservations.Items.Where(_jvm._onlyRoFilter).Where(_jvm._periodFilter)));
        return obs;
      }
    }

    public ObservableCollection<Lithology> Lithology { get; private set; }


    public ObservableCollection<TimestampValue> SelectedObs { get; private set; }

    public bool HasChanges { get; set; }

    private JupiterViewModel _jvm;

    public WellViewModel(IWell Well, JupiterViewModel jvm)
    {
      _well = Well;
      _jvm =jvm;

      Screens = new ObservableCollection<Screen>();
      Cells = new ObservableCollection<CellViewModel>();

      if (_well is JupiterWell)
      {
        Lithology = new ObservableCollection<JupiterTools.Lithology>(((JupiterWell)_well).LithSamples);
      }
      else
        Lithology= new ObservableCollection<JupiterTools.Lithology>();


      foreach (IIntake I in _well.Intakes)
      {
        foreach (Screen s in I.Screens)
          Screens.Add(s);
      }

      if (Observations.Count>0)
        SelectedObs = new ObservableCollection<TimestampValue>(Observations.First().Second);
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

//    public ObservableCollection<Change> XHistory { get; set; }

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
