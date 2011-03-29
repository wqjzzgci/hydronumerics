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
    private JupiterViewModel _jvm;
    private IWell _well;
    private Model _mshe;
    
    private int _col;
    private int _row;

    public ObservableCollection<CellViewModel> Cells { get; private set; }
    public ObservableCollection<ScreenViewModel> Screens { get; private set; }


    /// <summary>
    /// Gets the observations using the filter from the JupiterViewModel
    /// </summary>
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



    public WellViewModel(IWell Well, JupiterViewModel jvm)
    {
      _well = Well;
      _jvm =jvm;

      Screens = new ObservableCollection<ScreenViewModel>();
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
          Screens.Add(new ScreenViewModel(s, _jvm));
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
    /// Adds a screen to the intake
    /// </summary>
    /// <param name="Intake"></param>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    /// <param name="comment"></param>
    public void AddScreen(IIntake Intake, double top, double bottom, string comment)
    {
      Screen sc = new Screen(Intake);
      sc.DepthToTop = top;
      sc.DepthToBottom = bottom;
      sc.Number = Intakes.Max(var1=>var1.Screens.Max(var => var.Number)) + 1;
      Screens.Add(new ScreenViewModel(sc,_jvm));

      Change ch = new Change();
      ch.Comments.Add(comment);
      ch.Action = TableAction.InsertRow;
      ch.Table = JupiterTables.SCREEN;

      ch.PrimaryKeys.Add(new Tuple<string,string>("BOREHOLENO",Intake.well.ID));
      ch.PrimaryKeys.Add(new Tuple<string,string>("SCREENNO",sc.Number.ToString()));

      ch.ChangeValues.Add(new Treple<string,string,string>("TOP",top.ToString(),""));
      ch.ChangeValues.Add(new Treple<string,string,string>("BOTTOM",bottom.ToString(),""));
      

      _jvm.Changes.Add(ch);
    }


    private Change GetBoreHoleChange()
    {
      Change xchange = new ViewModel.Change();

      xchange.Action = TableAction.EditValue;
      xchange.Table = JupiterTables.BOREHOLE;
      xchange.PrimaryKeys.Add(new Tuple<string, string>("BOREHOLENO", WellID));

      return xchange;
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
          Change c = GetBoreHoleChange();
          c.ChangeValues.Add(new Treple<string,string,string>("UTMX",value.ToString(), _well.Location.X.ToString()));
          _jvm.Changes.Add(c);
          _well.X = value;
          NotifyPropertyChanged("X");
        }
      }
    }

    public double Y
    {
      get { return _well.Y; }
      set {
        if (value !=_well.Y)
        {
          Change c = GetBoreHoleChange();
          c.ChangeValues.Add(new Treple<string, string, string>("UTMY", value.ToString(), _well.Location.Y.ToString()));
          _jvm.Changes.Add(c);
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
          Change c = GetBoreHoleChange();
          c.ChangeValues.Add(new Treple<string, string, string>("ELEVATION", value.ToString(), _well.Terrain.ToString()));
          _jvm.Changes.Add(c);
          _well.Terrain = value;
          NotifyPropertyChanged("Terrain");
        }
      }
    }

    /// <summary>
    /// Gets the intakes
    /// </summary>
    public IEnumerable<IIntake> Intakes
    {
      get
      {
        return _well.Intakes;
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
