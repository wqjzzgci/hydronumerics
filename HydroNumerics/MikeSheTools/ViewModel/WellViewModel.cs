﻿using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;
using Microsoft.Research.DynamicDataDisplay.DataSources;

using HydroNumerics.Core;
using HydroNumerics.Core.WPF;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.JupiterTools;
using HydroNumerics.JupiterTools.JupiterPlus;
using HydroNumerics.Time.Core;


namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class WellViewModel : BaseViewModel, Geometry.IXYPoint
  {
    private ChangesViewModel CVM;
    private IWell _well;
    private Model _mshe;
    private int _col;
    private int _row;
    private int CurrentIntakeIndex = 0;

    public WellViewModel(IWell Well, ChangesViewModel cvm)
    {
      _well = Well;
      CVM = cvm;
      Name = _well.ID;
    }

    /// <summary>
    /// Gets an URL to the well at GEUS' homepage
    /// </summary>
    public string URL
    {
      get
      {
        string s=String.Format("http://jupiter.geus.dk/JupiterWWW/boreServlet?redel=boreRapport&dgunr={0}&submit=Vis+boringsdata", _well.ID);
        s = String.Format("http://data.geus.dk/JupiterWWW/borerapport.jsp?redel=borerapport&dgunr={0}&submit=vis+boringsdata", _well.ID);
        return s;
      }
    }


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
              ScreenViewModel svm = new ScreenViewModel(s, CVM);
              screens.Add(svm);
            }
          }
        }
        return screens;
      }
    }

    private SortedList<IIntake, ObservableDataSource<TimestampValue>> headObservations;

    /// <summary>
    /// Gets the head observations. The key is the well number + the intake number
    /// </summary>
    public SortedList<IIntake, ObservableDataSource<TimestampValue>> HeadObservations
    {
      get
      {
        if (headObservations == null)
        {
          headObservations = new SortedList<IIntake, ObservableDataSource<TimestampValue>>();
          foreach (var I in _well.Intakes)
          {
            var obs =new ObservableDataSource<TimestampValue>(I.HeadObservations.Items);
            obs.Collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Collection_CollectionChanged);
            headObservations.Add(I, obs);
          }
        }
        return headObservations;
      }
    }

    /// <summary>
    /// Comes here when an entry is deleted from the head observations
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.OldItems.Count > 0)
      {
        TimestampValue tsv = e.OldItems[0] as TimestampValue;

        IIntake intake = HeadObservations.Keys[CurrentIntakeIndex];
        ChangeDescription c = CVM.ChangeController.GetRemoveWatlevel(intake, tsv.Time);
        ChangeDescriptionViewModel cv =new ChangeDescriptionViewModel(c);
        cv.IsDirty = true;
        cv.IsApplied = true;
        CVM.AddChange(cv, true);
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
        {
          I.Extractions.Sort();
          obs.Add(new Tuple<string, IEnumerable<TimestampValue>>(I.ToString(), I.Extractions.AsTimeStamps));
        }
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
        if (HeadObservations.Count > 0)
          return HeadObservations.Values[CurrentIntakeIndex].Collection;
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
        return _well.HasMissingData() | _well.Intakes.Any(I=>I.HasMissingdData());
      }
    }

    public bool HasFixableErrors
    {
      get
      {
        if (_well.HasMissingData() | _well.Intakes.Any(I=>I.HasMissingdData()))
          return _well.CanFixErrors();
        else
          return false;
      }
    }

    public void Fix()
    {
      if (_well.CanFixErrors())
      {
        WasFixed = true;
        statusString = statusString + _well.FixErrors();
        RaisePropertyChanged("WasFixed");
        RaisePropertyChanged("HasFixableErrors");
        RaisePropertyChanged("MissingData");
        RaisePropertyChanged("StatusString");
        screens = null;
        RaisePropertyChanged("Screens");
      }
    }

    private string statusString="";
    public string StatusString
    {
      get
      {
        return statusString;
      }
      set
      {
        if (statusString != value)
        {
          statusString = value;
          RaisePropertyChanged("StatusString");
        }
      }
    }

    public bool WasFixed { get; private set; }

    /// <summary>
    /// Connects to a Mike she model
    /// </summary>
    /// <param name="Mshe"></param>
    public bool LinkToMikeShe(Model Mshe)
    {
      _mshe = Mshe;
      if (Mshe.GridInfo.TryGetIndex(X, Y, out _col, out _row))
      {
        Column = _col;
        Row = _row;
        foreach (var sc in Screens)
        {
          sc.LinkToMshe(Mshe, this);
        }
        return true;
      }
      return false;
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
          ChangeDescriptionViewModel cv = new ChangeDescriptionViewModel(c);
          cv.Description = "X-coordinate changed on well: " + DisplayName;
          cv.IsDirty = true;
          cv.IsApplied = true;
          _well.X = value;
          RaisePropertyChanged("X");
          RaisePropertyChanged("MissingData");
          RaisePropertyChanged("HasFixableErrors");
          CVM.AddChange(cv, true);          
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
          ChangeDescriptionViewModel cv = new ChangeDescriptionViewModel(c);
          cv.Description = "Y-coordinate changed on well: " + DisplayName;
          cv.IsDirty = true;
          cv.IsApplied = true;
          _well.Y = value;
          RaisePropertyChanged("Y");
          RaisePropertyChanged("MissingData");
          RaisePropertyChanged("HasFixableErrors");
          CVM.AddChange(cv, true);
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
          ChangeDescriptionViewModel cv = new ChangeDescriptionViewModel(c);
          cv.Description = "Terrain level changed on well: " + DisplayName;
          cv.IsDirty = true;
          cv.IsApplied = true;
          _well.Terrain = value;
          RaisePropertyChanged("Terrain");
          CVM.AddChange(cv, true);
        }
      }
    }

    /// <summary>
    /// Gets the depth
    /// </summary>
    public double? Depth
    {
      get
      {
        return _well.Depth;
      }
      set
      {
        if (value != _well.Depth)
        {
          ChangeDescription c = CVM.ChangeController.ChangeDepthOnWell(_well, value.Value);
          ChangeDescriptionViewModel cv = new ChangeDescriptionViewModel(c);
          cv.Description = "Depth level changed on well: " + DisplayName;
          cv.IsDirty = true;
          cv.IsApplied = true;
          _well.Depth = value;
          RaisePropertyChanged("Depth");
          CVM.AddChange(cv, true);
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
          RaisePropertyChanged("IsUsedForExtraction");
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
          RaisePropertyChanged("Column");
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
          RaisePropertyChanged("Row");
        }
      }
    }


    public ScreenViewModel AddScreen()
    {
      Screen sc = new Screen(_well.Intakes.First());
      sc.Number = _well.Intakes.Max(var1 => var1.Screens.Max(var => var.Number)) + 1;
      ScreenViewModel svm = new ScreenViewModel(sc, CVM);
      Screens.Add(svm);
      return svm;
    }

    public void RemoveScreen(ScreenViewModel svm)
    {
      foreach (var i in _well.Intakes)
      {
        if (i.Screens.Contains(svm._screen))
          i.Screens.Remove(svm._screen);
      }
      Screens.Remove(svm);
    }


    #region Commands



    RelayCommand nextIntakeCommand;

    public ICommand NextIntakeCommand
    {
      get
      {
        if (nextIntakeCommand == null)
          nextIntakeCommand = new RelayCommand(() => NextIntake(), () => CanNextIntake);
        return nextIntakeCommand;
      }
    }

    private bool CanNextIntake
    {
      get
      {
        if (HeadObservations.Count > CurrentIntakeIndex + 1)
          return true;
        else
          return false;
      }
    }

    public void NextIntake()
    {
      CurrentIntakeIndex++;
      RaisePropertyChanged("SelectedObs");
    }


    RelayCommand previousIntakeCommand;

    public ICommand PreviousIntakeCommand
    {
      get
      {
        if (previousIntakeCommand == null)
          previousIntakeCommand = new RelayCommand(() => PreviousIntake(), () => CanPreviousIntake);
        return previousIntakeCommand;
      }
    }

    private bool CanPreviousIntake
    {
      get
      {
        if (CurrentIntakeIndex >0)
          return true;
        else
          return false;
      }
    }

    public void PreviousIntake()
    {
      CurrentIntakeIndex--;
      RaisePropertyChanged("SelectedObs");
    }


    #endregion
  }
}
