using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;

using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.Time.Core;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellViewModel : BaseViewModel
  {
    private ChangesViewModel CVM;
    private IWell _well;
    private Model _mshe;

    private int _col;
    private int _row;

    private ChangeDescriptionViewModel changeViewModel = null;
    public ChangeDescriptionViewModel CurrentChange
    {
      get
      {
        return changeViewModel;
      }
      set
      {
        if (changeViewModel != value)
        {
          changeViewModel = value;
          NotifyPropertyChanged("CurrentChange");
        }
      }
    }



    public WellViewModel(IWell Well, ChangesViewModel cvm)
    {
      _well = Well;
      CVM = cvm;
      DisplayName = _well.ID;
    }

    public string URL
    {
      get
      {
        string s=String.Format("http://jupiter.geus.dk/JupiterWWW/boreServlet?redel=boreRapport&dgunr={0}&submit=Vis+boringsdata", _well.ID);
        return s;
      }
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
            {
              ScreenViewModel svm = new ScreenViewModel(s);
              svm.PropertyChanged += new PropertyChangedEventHandler(svm_PropertyChanged);
              screens.Add(svm);
            }
          }
        }
        return screens;
      }
    }

    void svm_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ScreenViewModel s = sender as ScreenViewModel;
      if (CurrentChange == null)
        CurrentChange = new ChangeDescriptionViewModel(CVM.ChangeController.GetScreenChange(s._screen));

      if (e.PropertyName == "DepthToBottom")
        CVM.ChangeController.ChangeBottomOnScreen(CurrentChange.changeDescription, s._screen, s.DepthToBottom);
      else if (e.PropertyName == "DepthToTop")
        CVM.ChangeController.ChangeTopOnScreen(CurrentChange.changeDescription, s._screen, s.DepthToTop);

      NotifyPropertyChanged("MissingData");
    }
        

    /// <summary>
    /// Gets the observations
    /// </summary>
    public ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>> Observations
    {
      get
      {
        var obs = new ObservableCollection<Tuple<string, IEnumerable<TimestampValue>>>();

        foreach (var I in _well.Intakes)
          obs.Add(new Tuple<string, IEnumerable<TimestampValue>>(I.ToString(), I.HeadObservations.Items));
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
    /// Gets and sets the X coordiante
    /// </summary>
    public double X
    {
      get { return _well.X; }
      set
      {
        if (value != _well.X)
        {
          ChangeDescription c = CVM.ChangeController.ChangeXOnWell(_well, value);

          _well.X = value;
          NotifyPropertyChanged("X");
          NotifyPropertyChanged("MissingData");
          CVM.AddChange(new ChangeDescriptionViewModel(c), true);
          
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
          ChangeDescription c = CVM.ChangeController.ChangeYOnWell(_well, value);
          if (CurrentChange == null)
            CurrentChange = new ChangeDescriptionViewModel(c);
          else
            CurrentChange.changeDescription.ChangeValues.Add(c.ChangeValues.First());

          _well.Y = value;
          NotifyPropertyChanged("Y");
          NotifyPropertyChanged("MissingData");
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
          ChangeDescription c = CVM.ChangeController.ChangeTerrainOnWell(_well, value);
          if (CurrentChange == null)
            CurrentChange = new ChangeDescriptionViewModel(c);
          else
            CurrentChange.changeDescription.ChangeValues.Add(c.ChangeValues.First());
          _well.Terrain = value;
          NotifyPropertyChanged("Terrain");
        }
      }
    }

    /// <summary>
    /// Gets and sets a boolean to indicate whether this well is used for extraction
    /// </summary>
    public bool IsUsedForExtraction
    {
      get
      {
        return _well.UsedForExtraction;
      }
      set
      {
        if (_well.UsedForExtraction != value)
        {
          _well.UsedForExtraction = value;
          NotifyPropertyChanged("IsUsedForExtraction");
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

    #region Commands
    RelayCommand applyCommand;
    RelayCommand addScreenCommand;

    public ICommand ApplyCommand
    {
      get
      {
        if (applyCommand == null)
          applyCommand = new RelayCommand(param => ApplyChange(), param => CanApplyChange);
        return applyCommand;
      }
    }

    public ICommand AddScreenCommand
    {
      get
      {
        if (addScreenCommand == null)
          addScreenCommand = new RelayCommand(param => AddScreen(), param => CanAddScreen);
        return addScreenCommand;
      }
    }

    private bool CanAddScreen
    {
      get
      {
        return CurrentChange==null & _well.Intakes.Count()>0;
      }
    }

    private void AddScreen()
    {
      Screen sc = new Screen(_well.Intakes.First());
      sc.Number = _well.Intakes.Max(var1 => var1.Screens.Max(var => var.Number)) + 1;
      ScreenViewModel svm = new ScreenViewModel(sc);
      svm.PropertyChanged += new PropertyChangedEventHandler(svm_PropertyChanged);
      CurrentChange = new ChangeDescriptionViewModel(CVM.ChangeController.NewScreen(sc));
      Screens.Add(svm);
    }


    private bool CanApplyChange
    {
      get
      {
        return CurrentChange != null;
      }
    }

    private void ApplyChange()
    {
        CurrentChange.IsApplied = true;
        CVM.AddChange(CurrentChange, false);
        CurrentChange = null;
    }





    #endregion

  }
}
