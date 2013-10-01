using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;

using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.MikeSheTools.PFS.Sim11;
using HydroNumerics.Core.WPF;

using DHI.Generic;
using DHI.Mike1D.CrossSections;

namespace HydroNumerics.MikeSheTools.Mike11
{
  public class M11Setup:BaseViewModel
  {
    public Network network { get; private set; }

    private CrossSectionCollection csc;
    private Dictionary<M11Branch, ObservableCollection<M11Branch>> SubNetworks = new Dictionary<M11Branch, ObservableCollection<M11Branch>>();

    public M11Setup()
    {
      network = new Network();
      SelectedCrossSections = new ObservableCollection<CrossSection>();
    }


    private DEMSourceConfiguration demConfig = new DEMSourceConfiguration();

    /// <summary>
    /// Gets and sets DEMConfig;
    /// </summary>
    public DEMSourceConfiguration DEMConfig
    {
      get { return demConfig; }
      set
      {
        if (value != demConfig)
        {
          demConfig = value;
          NotifyPropertyChanged("DEMConfig");
        }
      }
    }


    private string sim11FileName;

    /// <summary>
    /// Gets and sets Sim11FileName;
    /// </summary>
    public string Sim11FileName
    {
      get { return sim11FileName; }
      set
      {
        if (value != sim11FileName)
        {
          sim11FileName = value;
          NotifyPropertyChanged("Sim11FileName");
        }
      }
    }

    public ObservableCollection<CrossSection> SelectedCrossSections { get; set; }

    private M11Branch _currentBranch;
    public M11Branch CurrentBranch
    {
      get { return _currentBranch; }
      set
      {
        if (value != _currentBranch)
        {
          _currentBranch = value;
          CurrentSubNetwork = SubNetworks[_currentBranch.SubNetWorkEndpoint];
          FindXSecsThatNeedAdjustment();
          NotifyPropertyChanged("CurrentBranch");
          GetHeightsOnxsec(_currentBranch.CrossSections.ToList());
        }
      }
    }




    private ObservableCollection<M11Branch> currentSubNetwork;

    /// <summary>
    /// Gets and sets CurrentSubNetwork;
    /// </summary>
    public ObservableCollection<M11Branch> CurrentSubNetwork
    {
      get { return currentSubNetwork; }
      set
      {
        if (value != currentSubNetwork)
        {
          currentSubNetwork = value;
          NotifyPropertyChanged("CurrentSubNetwork");
        }
      }
    }


    
    
    private ObservableCollection<M11Branch> endBranches;

    public ObservableCollection<M11Branch> EndBranches
    {
      get
      {
        if (endBranches == null & network.Branches != null)
        {
          endBranches = new ObservableCollection<M11Branch>(network.Branches.Where(b => b.IsEndPoint));
        }
        return endBranches;
      }
    }



    /// <summary>
    /// Reads the setup from a sim11 file
    /// </summary>
    /// <param name="Sim11FileName"></param>
    public void ReadSetup(string Sim11FileName)
    {
      ReadSetup(Sim11FileName, false);
    }


    /// <summary>
    /// Reads the setup from a sim11 file
    /// </summary>
    /// <param name="Sim11FileName"></param>
    public void ReadSetup(string Sim11FileName, bool SkipCrossSections)
    {
      Sim11File sm11 = new Sim11File(Sim11FileName);
      this.Sim11FileName = Sim11FileName;
      ReadNetwork(sm11.FileNames.NWK11FileName);
      if (!SkipCrossSections)
        ReadCrossSections(sm11.FileNames.XNS11FileName);

      foreach (var b in EndBranches)
      {
        ObservableCollection<M11Branch> upstreamnet = new ObservableCollection<M11Branch>();
        upstreamnet.Add(b);
        b.SubNetWorkEndpoint = b;
        RecursiveAdd(b, upstreamnet);
        SubNetworks.Add(b, upstreamnet);
      }
    }


    private void RecursiveAdd(M11Branch b, ObservableCollection<M11Branch> subnet)
    {
      foreach (var u in b.UpstreamBranches)
      {
        u.SubNetWorkEndpoint = b.SubNetWorkEndpoint;
        subnet.Add(u);
        RecursiveAdd(u, subnet);
      }
    }

    /// <summary>
    /// Reads the network from a NWK11-file
    /// </summary>
    /// <param name="Nwk11FileName"></param>
    public void ReadNetwork(string Nwk11FileName)
    {
      network.Load(Nwk11FileName);

      endBranches = null;
      NotifyPropertyChanged("EndBranches");
    }

    /// <summary>
    /// Reads the cross sections from an .xns-file
    /// </summary>
    /// <param name="xnsFile"></param>
    public void ReadCrossSections(string xnsFile)
    {
      //This is necessary because it fails if DHI.CrossSection.Dll tries to load UFS.dll
      DFS0 d = new DFS0(@"v");
      d.Dispose();

      csc = new CrossSectionCollection();
      csc.Connection.FilePath = xnsFile;
      csc.Connection.Bridge = csc.Connection.AvailableBridges[0];
      csc.Connection.Open(false);


      //Now loop the cross sections
      foreach (var cs in csc.CrossSections)
      {
        //Create a HydroNumerics.MikeSheTools.Mike11.CrossSection from the M11-CrossSection
        CrossSection MyCs = new CrossSection(cs);
        
        CombineNetworkAndCrossSections(MyCs);
      }
    }

    /// <summary>
    /// Writes out some shapefiles with the setup.
    /// </summary>
    /// <param name="FilePrefix"></param>
    public void WriteToShape(string FilePrefix)
    {
      string dir = Path.GetDirectoryName(Path.GetFullPath(FilePrefix));
      network.WriteToShape(Path.Combine(dir,Path.GetFileNameWithoutExtension(FilePrefix)));
    }

    public void Save()
    {
      if (csc != null)
        csc.Connection.Save();

      HasChanges = false;
    }



    private bool hasChanges = false;

    /// <summary>
    /// Gets and sets HasChanges;
    /// </summary>
    public bool HasChanges
    {
      get { return hasChanges; }
      set
      {
        if (value != hasChanges)
        {
          hasChanges = value;
          NotifyPropertyChanged("HasChanges");
        }
      }
    }

    RelayCommand adjustDatumCommand;
    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand AdjustDatumCommand
    {
      get
      {
        if (adjustDatumCommand == null)
        {
          adjustDatumCommand = new RelayCommand(param => ApplyMove(param), param => CanApplyMove(param));
        }
        return adjustDatumCommand;
      }
    }

    private bool CanApplyMove(object tomove)
    {
      System.Collections.IList items = (System.Collections.IList)tomove;
      if (items == null || items.Count==0)
        return false;

      var collection = items.Cast<CrossSection>();
      return collection.Any(c => c.HeightDifference != 0);
    }

    private void ApplyMove(object tomove)
    {
      System.Collections.IList items = (System.Collections.IList)tomove;
      var collection = items.Cast<CrossSection>();

      foreach (var v in collection)
      {
        if (v.DEMHeight.HasValue)
        {
          v.MaxHeightMrk1and3 = v.DEMHeight.Value;
          HasChanges = true;
        }
      }
    }



    RelayCommand getHeightsCommand;
    /// <summary>
    /// Gets the command that saves the extration files
    /// </summary>
    public ICommand GetHeightsCommand
    {
      get
      {
        if (getHeightsCommand == null)
        {
          getHeightsCommand = new RelayCommand(param => GetHeights(param), param => CanGetHeights(param));
        }
        return getHeightsCommand;
      }
    }

    private bool CanGetHeights(object tomove)
    {
      System.Collections.IList items = (System.Collections.IList)tomove;
      if (items == null)
        return false;
      return items.Count > 0;
    }

    private void GetHeights(object tomove)
    {
      System.Collections.IList items = (System.Collections.IList)tomove;
      var collection = items.Cast<M11Branch>().SelectMany(b=>b.CrossSections).ToList();
      GetHeightsOnxsec(collection);
    }

    private void GetHeightsOnxsec(List<CrossSection> collection)
    {
      var heights = DEMConfig.FindManyHeights(collection.Where(v => !v.DEMHeight.HasValue).Select(c => c.MidStreamLocation as HydroNumerics.Geometry.XYPoint));
      for (int i = 0; i < heights.Count; i++)
      {
        collection[i].DEMHeight = heights[i];
      }


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
          saveCommand = new RelayCommand(param => this.Save(), param => HasChanges);
        }
        return saveCommand;
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
        if (CurrentBranch == null)
          return false;
        if (CurrentBranch.IsEndPoint)
          return CurrentBranch.EndPointElevation != 0;
        else
          return CurrentBranch.ConnectionBottomLevelOffset.Value < 0;
      }
    }

    private void SetEndPointToZero()
    {
      if (CurrentBranch.IsEndPoint)
      {
        CurrentBranch.EndPointElevation = 0;
      }
      else
      {
        CurrentBranch.EndPointElevation += CurrentBranch.ConnectionBottomLevelOffset.Value;
      }
      FindXSecsThatNeedAdjustment();
      NotifyPropertyChanged("CurrentBranch");
      HasChanges = true;

    }

    #region AdjustLevel

    RelayCommand adjustLevelUpCommand;

    /// <summary>
    /// 
    /// </summary>
    public ICommand AdjustLevelUpCommand
    {
      get
      {
        if (adjustLevelUpCommand == null)
        {
          adjustLevelUpCommand = new RelayCommand(param => this.AdjustLevelUp(), param => this.CanAdjustLevelUp);
        }
        return adjustLevelUpCommand;
      }
    }


    RelayCommand adjustLevelDownCommand;

    /// <summary>
    /// 
    /// </summary>
    public ICommand AdjustLevelDownCommand
    {
      get
      {
        if (adjustLevelDownCommand == null)
        {
          adjustLevelDownCommand = new RelayCommand(param => this.AdjustLevelDown(), param => this.CanAdjustLevelDown);
        }
        return adjustLevelDownCommand;
      }
    }



    private bool CanAdjustLevelUp
    {
      get
      {
        if (CurrentBranch == null)
          return false;       
        return CurrentBranch.SelectedCrossSections.Any(s=>CurrentBranch.CrossSections.IndexOf(s)!=0);
      }
    }

    private bool CanAdjustLevelDown
    {
      get
      {
        if (CurrentBranch == null)
          return false;
        return CurrentBranch.SelectedCrossSections.Any(s => CurrentBranch.CrossSections.IndexOf(s) != CurrentBranch.CrossSections.Count-1);
      }
    }



    private void FindXSecsThatNeedAdjustment()
    {
      CurrentBranch.SelectedCrossSections.Clear();
      double previousbottom = double.MaxValue;
      foreach (var xsec in CurrentBranch.CrossSections)
      {
        if (xsec.BottomLevel > previousbottom + 1e-8)
          CurrentBranch.SelectedCrossSections.Add(xsec);
        previousbottom = xsec.BottomLevel;
      }
    }

    private void AdjustLevelUp()
    {
      foreach (var xsec in CurrentBranch.SelectedCrossSections)
      {
        int index = CurrentBranch.CrossSections.IndexOf(xsec);
        xsec.BottomLevel = CurrentBranch.CrossSections[index -1].BottomLevel;
      }
      FindXSecsThatNeedAdjustment();
      NotifyPropertyChanged("CurrentBranch");
      HasChanges = true;
    }


    private void AdjustLevelDown()
    {
      foreach (var xsec in CurrentBranch.SelectedCrossSections)
      {
        int index = CurrentBranch.CrossSections.IndexOf(xsec);
        if (index!=CurrentBranch.CrossSections.Count-1)
          xsec.BottomLevel = CurrentBranch.CrossSections[index + 1].BottomLevel;
      }
      FindXSecsThatNeedAdjustment();
      NotifyPropertyChanged("CurrentBranch");
      HasChanges = true;
    }




    #endregion

    #endregion



    /// <summary>
    /// Adds the cross section to the correct branch
    /// </summary>
    /// <param name="MyCs"></param>
    private void CombineNetworkAndCrossSections(CrossSection MyCs)
    {
      if (network != null)
      {
        //Finds the branch with the correct name, topoid, and chainage interval
        M11Branch mb = network.Branches.FirstOrDefault(var => var.Name.ToUpper() == MyCs.BranchName.ToUpper() & var.TopoID.ToUpper() == MyCs.TopoID.ToUpper() & var.ChainageStart <= MyCs.Chainage & var.ChainageEnd >= MyCs.Chainage);
        if (mb != null)
          mb.AddCrossection(MyCs);
      } 
    }

  }
}
