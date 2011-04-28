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
  public class WellViewModel : BaseViewModel, IDataErrorInfo
  {
    private JupiterViewModel _jvm;
    private IWell _well;
    private Model _mshe;

    private int _col;
    private int _row;


    public WellViewModel(IWell Well, JupiterViewModel jvm)
    {
      _well = Well;
      _jvm = jvm;
      DisplayName = _well.ID;
    }


    /// <summary>
    /// Gets the collection of cells. !Not ready yet!
    /// </summary>
    public ObservableCollection<CellViewModel> Cells { get; private set; }


    private ObservableCollection<ScreenViewModel> screens;
    /// <summary>
    /// Gets the collection of screens
    /// </summary>
    public ObservableCollection<ScreenViewModel> Screens
    {
      get
      {
        if (screens == null)
        {
          screens = new ObservableCollection<ScreenViewModel>();
          foreach (IIntake I in _well.Intakes)
          {
            foreach (Screen s in I.Screens)
              screens.Add(new ScreenViewModel(s, _jvm));
          }
        }
        return screens;
      }
    }


    /// <summary>
    /// Gets the observations using the filter from the JupiterViewModel
    /// </summary>
    public ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>> Observations
    {
      get
      {
        var obs = new ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>>();

        foreach (var I in _well.Intakes)
          obs.Add(new Tuple<string, IEnumerable<TimestampValue>>(I.ToString(), I.HeadObservations.Items.Where(_jvm._onlyRoFilter).Where(_jvm._periodFilter)));
        return obs;
      }
    }


    /// <summary>
    /// Gets the extractions in each intake
    /// </summary>
    public ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>> Extractions
    {
      get
      {
        var obs = new ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>>();
        foreach (var I in _well.Intakes)
          obs.Add(new Tuple<string, IEnumerable<TimestampValue>>(I.ToString(), I.Extractions.AsTimeStamps));
        return obs;
      }
    }


    private ObservableCollection<Lithology> lithology;
    /// <summary>
    /// Gets the collection of lithologies
    /// </summary>
    public ObservableCollection<Lithology> Lithology
    {
      get
      {
        if (lithology == null)
        {
          if (_well is JupiterWell)
          {
            lithology = new ObservableCollection<JupiterTools.Lithology>(((JupiterWell)_well).LithSamples);
          }
          else
            lithology = new ObservableCollection<JupiterTools.Lithology>();
        }
        return lithology;
      }
    }


    /// <summary>
    /// Something needs to be done here
    /// </summary>
    public ObservableCollection<TimestampValue> SelectedObs
    {
      get
      {
        if (Observations.Count > 0)
          return new ObservableCollection<TimestampValue>(Observations.First().Second);
        else
          return null;
      }
    }


    /// <summary>
    /// Returns true if the well has missing data.
    /// x,y==0 or intakes with missing screens
    /// </summary>
    public bool MissingData
    {
      get
      {
        return X == 0 || Y == 0 || _well.Intakes.Count() == 0 || Screens.Count == 0 || Screens.Any(var => var.MissingData);
      }
    }


    /// <summary>
    /// Connects to a Mike she model
    /// </summary>
    /// <param name="Mshe"></param>
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
      sc.Number = Intakes.Max(var1 => var1.Screens.Max(var => var.Number)) + 1;
      Screens.Add(new ScreenViewModel(sc, _jvm));

      ChangeDescription ch = new ChangeDescription(JupiterTables.SCREEN);

      ch.Comments.Add(comment);
      ch.Action = TableAction.InsertRow;

      ch.PrimaryKeys["BOREHOLENO"] = Intake.well.ID;
      ch.PrimaryKeys["SCREENNO"] = sc.Number.ToString();

      ch.ChangeValues.Add(new Change("TOP", top.ToString(), ""));
      ch.ChangeValues.Add(new Change("BOTTOM", bottom.ToString(), ""));


//      _jvm.Changes.Add(ch);
    }


    private ChangeDescription GetBoreHoleChange()
    {
      ChangeDescription xchange = new ChangeDescription(JupiterTables.BOREHOLE);

      xchange.Action = TableAction.EditValue;
      xchange.PrimaryKeys["BOREHOLENO"] = _well.ID;
      return xchange;
    }


    /// <summary>
    /// Gets and sets the X coordiante
    /// </summary>
    public double X
    {
      get { return _well.X; }
      set
      {
        if (value != _well.X)
        {
          ChangeDescription c = GetBoreHoleChange();
          c.ChangeValues.Add(new Change("UTMX", value.ToString(), _well.Location.X.ToString()));
          //_jvm.Changes.Add(c);
          _well.X = value;
          NotifyPropertyChanged("X");
        }
      }
    }

    /// <summary>
    /// Gets and sets the Y coordinate
    /// </summary>
    public double Y
    {
      get { return _well.Y; }
      set
      {
        if (value != _well.Y)
        {
          ChangeDescription c = GetBoreHoleChange();
          c.ChangeValues.Add(new Change("UTMY", value.ToString(), _well.Location.Y.ToString()));
          //_jvm.Changes.Add(c);
          _well.Y = value;
          NotifyPropertyChanged("Y");
        }
      }
    }

    /// <summary>
    /// Gets and sets the terrain elevation.
    /// </summary>
    public double Terrain
    {
      get { return _well.Terrain; }
      set
      {
        if (value != _well.Terrain)
        {
          ChangeDescription c = GetBoreHoleChange();
          c.ChangeValues.Add(new Change("ELEVATION", value.ToString(), _well.Terrain.ToString()));
          //_jvm.Changes.Add(c);
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

    /// <summary>
    /// Gets the Column in which the well is located. Only available if a Mike She model has been loaded.
    /// </summary>
    public int Column
    {
      get { return _col; }
      private set
      {
        if (_col != value)
        {
          _col = value;
          NotifyPropertyChanged("Column");
        }
      }
    }

    /// <summary>
    /// Gets the Row in which the well is located. Only available if a Mike She model has been loaded.
    /// </summary>
    public int Row
    {
      get { return _row; }
      private set
      {
        if (_row != value)
        {
          _row = value;
          NotifyPropertyChanged("Row");
        }
      }
    }

    #region IDataErrorInfo Members

    public string Error
    {
      get { throw new NotImplementedException(); }
    }

    string IDataErrorInfo.this[string columnName]
    {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}
