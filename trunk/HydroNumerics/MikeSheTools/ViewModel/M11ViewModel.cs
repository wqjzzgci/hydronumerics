using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;

using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class M11ViewModel:BaseViewModel
  {
    private M11Setup _m11Model;
    private string _sim11FileName="";
    private string _nwk11FileName="";
    public DEMSourceConfiguration DEMConfig{get;private set;}

    public ObservableCollection<M11Branch> Branches{get;private set;}
    public ObservableCollection<CrossSection> SelectedCrossSections {get;private set;}


    public M11ViewModel()
    {
      DEMConfig = new DEMSourceConfiguration();
      HasChanges = false;
      
      _m11Model = new M11Setup();
      SelectedCrossSections = new ObservableCollection<CrossSection>();
      SelectedCrossSections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedCrossSections_CollectionChanged);

    }

    
    /// <summary>
    /// When ever a new cross section is selected. Try finding a height
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void SelectedCrossSections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      {
        if (e.NewItems != null)
        {
          foreach (var v in e.NewItems)
          {
            CrossSection cv = v as CrossSection;
            if (!cv.DEMHeight.HasValue)
            {
              double? val;
              if (DEMConfig.TryFindDemHeight(cv.MidStreamLocation, out val))
                cv.DEMHeight = val;
            }
          }
        }
      }
    }


    private CollectionView endBranches;

    /// <summary>
    /// Gets and sets EndBranches;
    /// </summary>
    public CollectionView EndBranches
    {
      get { return endBranches; }
      set
      {
        if (value != endBranches)
        {
          endBranches = value;
          NotifyPropertyChanged("EndBranches");
        }
      }
    }

    #region AdjustEndPoint
    RelayCommand adjustEndPointToZeroCommand;

    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand AdjustEndPointToZeroCommand
    {
      get
      {
        if (adjustEndPointToZeroCommand == null)
        {
          adjustEndPointToZeroCommand = new RelayCommand(param => this.SetEndPointToZero(), param => this.CanAdjustEndpoint);
        }
        return adjustEndPointToZeroCommand;
      }
    }


    private bool CanAdjustEndpoint
    {
      get
      {
        M11BranchViewModel mb = EndBranches.CurrentItem as M11BranchViewModel;

        if (mb == null)
          return false;

        return mb.EndPointElevation != 0;
      }
    }

    private void SetEndPointToZero()
    {
      M11BranchViewModel mb = EndBranches.CurrentItem as M11BranchViewModel;

      if (mb!=null)
      {
        mb.EndPointElevation = 0;
        NotifyPropertyChanged("CurrentBranch");
        HasChanges = true;
      }
    }

    #region AdjustLevel
    RelayCommand adjustLevelCommand;

    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand AdjustLevelCommand
    {
      get
      {
        if (adjustLevelCommand == null)
        {
          adjustLevelCommand = new RelayCommand(param => this.AdjustLevel(), param => this.CanAdjustLevel);
        }
        return adjustLevelCommand;
      }
    }



    private bool CanAdjustLevel
    {
      get
      {
        if (CurrentBranch == null)
          return false;

        double previousbottom = double.MaxValue;

        foreach(var xsec in CurrentBranch.Branch.CrossSections)
        {
          if (xsec.BottomLevel > previousbottom + 1e-8)
            return true;
          else
            previousbottom = xsec.BottomLevel;
        }

        return false;
      }
    }

    private void AdjustLevel()
    {
      double previousbottom = double.MinValue;

      foreach (var xsec in CurrentBranch.Branch.CrossSections.Reverse())
      {
        if (xsec.BottomLevel < previousbottom - 1e-8)
          xsec.BottomLevel = previousbottom;
        else
          previousbottom = xsec.BottomLevel;
      }
      NotifyPropertyChanged("CurrentBranch");
      HasChanges = true;

    }


    #endregion

        #endregion




    public void WriteToShape(string FilePrefix)
    {
      _m11Model.WriteToShape(FilePrefix);
    }

    RelayCommand saveCommand;

    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand SaveCommand
    {
      get
      {
        if (saveCommand == null)
        {
          saveCommand = new RelayCommand(param => this.SaveChanges(), param => HasChanges);
        }
        return saveCommand;
      }
    }

    public bool HasChanges { get; set; }

    private void SaveChanges()
    {
      _m11Model.Save();
      HasChanges = false;
    }

    /// <summary>
    /// Gets and sets the sim11 file name. Setting the filename will read in setup
    /// </summary>
    public string Sim11FileName
    {
      get
      {
        return _sim11FileName;
      }
      set
      {
        if (value != _sim11FileName)
        {
        _m11Model.ReadSetup(value);
          _sim11FileName = value;
          Branches = new ObservableCollection<M11Branch>(_m11Model.network.Branches);
          EndBranches = new CollectionView(Branches.Where(b => b.IsEndPoint).Select(b=>new M11BranchViewModel(b)));

          NotifyPropertyChanged("Sim11FileName");
        }
      }
    }

    public string Nwk11FileName
    {
      get { return _nwk11FileName;}
      set
      {
        if(value!=_nwk11FileName)
        {
          _m11Model.ReadNetwork(value);
          _nwk11FileName = value;
          NotifyPropertyChanged("Nwk11FileName");
        }
      }
    }

    private M11BranchViewModel _currentBranch;
    public M11BranchViewModel CurrentBranch
    {
      get { return _currentBranch; }
      set
      {
        if (value != _currentBranch)
        {
          _currentBranch = value;
          EndBranches.MoveCurrentTo(CurrentBranch);
          NotifyPropertyChanged("CurrentBranch");
        }
      }
    }

  }
}
