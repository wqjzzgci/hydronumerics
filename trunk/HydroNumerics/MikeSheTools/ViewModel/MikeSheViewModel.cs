using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;
using System.Threading.Tasks;

using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;

using MathNet.Numerics.LinearAlgebra;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class MikeSheViewModel:BaseViewModel
  {

    public Model mshe { get; private set; }

    public MikeSheViewModel(Model Mshe)
    {
      mshe = Mshe;
      Layers = new ObservableCollection<MikeSheLayerViewModel>();
      ScreensToMove = new List<MoveToChalkViewModel>();
      ScreensAboveTerrain = new List<MoveToChalkViewModel>();
      ScreensBelowBottom = new List<MoveToChalkViewModel>();

      for (int i = 0; i < mshe.GridInfo.NumberOfLayers; i++)
      {
        MikeSheLayerViewModel msvm = new MikeSheLayerViewModel(i, mshe.GridInfo.NumberOfLayers);
        msvm.DisplayName = mshe.Input.MIKESHE_FLOWMODEL.SaturatedZone.CompLayersSZ.Layer_2s[mshe.GridInfo.NumberOfLayers - 1 - i].Name;
        Layers.Add(msvm);
      }
      Layers.First().IsChalkLayer = true;

      Chalks = new SortedDictionary<string, string>();
      Clays = new SortedDictionary<string, string>();
      var doc = XDocument.Load("LithologyGroups.xml").Element("LithologyGroups");

      foreach (var el in doc.Element("Chalks").Elements())
        Chalks.Add(el.Value, "");

      foreach (var el in doc.Element("Clays").Elements())
        Clays.Add(el.Value, "");
 
      foreach(var msvm in Layers)
        msvm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(msvm_PropertyChanged);

      NotifyPropertyChanged("Layers");
    }


    int c = 0;
    void msvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsChalkLayer")
      {
        if (c == 1)
        {
          c = 0;
          RefreshChalk();
        }
        else
        { c++; }
      }
      if (e.PropertyName == "IsGroundWaterBody")
      {
        RefreshWaterbodies();
      }
    }

    /// <summary>
    /// Gets the collection of layers
    /// </summary>
    public ObservableCollection<MikeSheLayerViewModel> Layers { get; private set; }

    /// <summary>
    /// Gets the she file name
    /// </summary>
    public string SheFileName
    {
      get
      {
        return mshe.Files.SheFile;
      }
    }

    /// <summary>
    /// Gets the minimum thickness for the computational layers
    /// </summary>
    public double MinimumLayerThickness
    {
      get
      {
        return mshe.Input.MIKESHE_FLOWMODEL.SaturatedZone.MimimumLayerThickness;
      }
    }

    private IEnumerable<WellViewModel> wells;

    public void SetWells(IEnumerable<WellViewModel> Wells)
    {
      wells = Wells;
      Refresh();
    }

    public List<MoveToChalkViewModel> ScreensAboveTerrain { get; private set; }
    public List<MoveToChalkViewModel> ScreensBelowBottom { get; private set; }

    private void RefreshBelowTerrain()
    {
      ScreensAboveTerrain.Clear();
      ScreensBelowBottom.Clear();
      if (wells != null)
      {
        foreach (var w in wells)
        {
          if (w.Row >= 0 & w.Column >= 0)
          {
            foreach (var s in w.Screens)
            {
              if (s.AboveModelTerrain)
              {
                MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                mc.NewTop = mshe.GridInfo.SurfaceTopography.Data[w.Row, w.Column];
                mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column,mshe.GridInfo.NumberOfLayers - 1];
                mc.NewLayer = mshe.GridInfo.NumberOfLayers-1;
                ScreensAboveTerrain.Add(mc);
              }
              if (s.BelowModelBottom)
              {
                MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, 0];
                mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, 0];
                mc.NewLayer = 0;
                ScreensBelowBottom.Add(mc);
              }
            }
          }
        }
      }
      NotifyPropertyChanged("ScreensBelowBottom");
      NotifyPropertyChanged("ScreensAboveTerrain");
    }


    public List<MoveToChalkViewModel> ScreensToMove { get; private set; }
    private void RefreshChalk()
    {
      ScreensToMove.Clear();

      var ChalkLayer = Layers.Single(var => var.IsChalkLayer);

      if (wells != null)
      {
        foreach (var w in wells)
        {
          if (w.Row >= 0 & w.Column >= 0)
          {
              foreach (var s in w.Screens)
              {
                var lits = w.Lithology.Where(var => var.Bottom > s.DepthToTop & var.Top < s.DepthToBottom);

                foreach (var l in lits)
                  if (Chalks.ContainsKey(l.RockSymbol.ToLower()))
                  {
                    bool move= false;
                    if (s.NewMsheLayer.HasValue)
                    {
                      if (s.NewMsheLayer.Value != ChalkLayer.DfsLayerNumber)
                        move = true;
                    }
                    else if (ChalkLayer.DfsLayerNumber < s.MsheBottomLayer || ChalkLayer.DfsLayerNumber > s.MsheTopLayer)
                      move =true;

                    if(move)
                    {
                      MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                      mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, ChalkLayer.DfsLayerNumber];
                      mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, ChalkLayer.DfsLayerNumber];
                      mc.NewLayer = ChalkLayer.DfsLayerNumber;
                      ScreensToMove.Add(mc);
                    }
                    break;
                  }
              }
          }
        }
      }
      NotifyPropertyChanged("ScreensToMove");
    }

    private List<MoveToChalkViewModel> screensToMoveWayerBodies = new List<MoveToChalkViewModel>();
    public ObservableCollection<MoveToChalkViewModel> ScreensToMoveWaterBodies
    {
      get
      {
        return new ObservableCollection<MoveToChalkViewModel>(screensToMoveWayerBodies);
      }
    }

    private void RefreshWaterbodies()
    {
      AsyncWithWait(()=>RefreshWaterBodiesMethod()).ContinueWith((t)=>NotifyPropertyChanged("ScreensToMoveWaterBodies"));
    }

    private void RefreshWaterBodiesMethod()
    {
      screensToMoveWayerBodies.Clear();
      var waterbodies = Layers.Where(var => var.IsGroundWaterBody);

      if (wells != null)
      {
        foreach (var w in wells)
        {
            foreach (var s in w.Screens)
            {
              var lits = w.Lithology.Where(var => var.Bottom > s.DepthToTop & var.Top < s.DepthToBottom);

              bool move = false;
              foreach (var l in lits)
                if (!Clays.ContainsKey(l.RockSymbol.ToLower()))
                  move = true;

              if (move & w.Column>= 0 & w.Row>=0)
              {
                int topl = s.MsheTopLayer;
                int bottoml = s.MsheBottomLayer;
                if (s.NewMsheLayer.HasValue)
                {
                  topl = s.NewMsheLayer.Value;
                  bottoml = s.NewMsheLayer.Value;
                }

                if (topl == -1)
                  topl = mshe.GridInfo.NumberOfLayers - 1;
                else if (topl == -2)
                  topl = 0;

                if (bottoml == -1)
                  bottoml = mshe.GridInfo.NumberOfLayers - 1;
                else if (bottoml == -2)
                  bottoml = 0;

                for (int j = bottoml; j <= topl; j++)
                {
                  if (Layers[j].IsGroundWaterBody)
                    move = false;
                }

                if (move)
                {
                  var upperdistance = Layers.FirstOrDefault(var => var.DfsLayerNumber > topl & mshe.GridInfo.ThicknessOfComputationalLayers.Data[w.Row, w.Column, var.DfsLayerNumber] > minLayThickness);
                  
                  double up = double.MaxValue;
                  if (upperdistance!=null)
                   up = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, upperdistance.DfsLayerNumber] - s.TopAsKote.Value;

                  var lowerdistance = Layers.FirstOrDefault(var => var.DfsLayerNumber < bottoml & mshe.GridInfo.ThicknessOfComputationalLayers.Data[w.Row, w.Column, var.DfsLayerNumber] > minLayThickness);
                  double down = double.MaxValue;
                  if (lowerdistance!=null)
                    down =-( mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber] - s.BottomAsKote.Value);

                  if (up < maxDistance & up < down)//Move up
                  {
                    MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                    mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, upperdistance.DfsLayerNumber];
                    mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, upperdistance.DfsLayerNumber];
                    mc.NewLayer = upperdistance.DfsLayerNumber;
                    screensToMoveWayerBodies.Add(mc);
                  }
                  else if (down <maxDistance & down<up)
                  {
                    MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                    mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber];
                    mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber];
                    mc.NewLayer = lowerdistance.DfsLayerNumber;
                    screensToMoveWayerBodies.Add(mc);
                  }
                }
              }
            }
        }
        NotifyPropertyChanged("ScreensToMoveWaterBodies");
      }
    }

   
    public SortedDictionary<string,string> Chalks { get; private set; }

    public SortedDictionary<string, string> Clays { get; private set; }

    private double minLayThickness = 2;

    public double MinLayThickness
    {
      get { return minLayThickness; }
      set 
      {
        if (minLayThickness != value)
        {
          minLayThickness = value;
          RefreshWaterbodies();
        }
      }
    }
    private double maxDistance = 10;

    public double MaxDistance
    {
      get { return maxDistance; }
      set
      {
        if (maxDistance != value)
        {
          maxDistance = value;
          RefreshWaterbodies();
        }
      }
    }


    #region ApplyWaterBody
    RelayCommand applyWaterBodyCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand ApplyWaterBodyCommand
    {
      get
      {
        if (applyWaterBodyCommand == null)
        {
          applyWaterBodyCommand = new RelayCommand(param => this.ApplyWaterBody(), param => this.CanApplyWaterBody);
        }
        return applyWaterBodyCommand;
      }
    }

    private bool CanApplyWaterBody
    {
      get
      {
        return screensToMoveWayerBodies.Count != 0;
      }
    }

    private void Refresh()
    {
      RefreshWaterbodies();
      RefreshChalk();
      RefreshBelowTerrain();
    }

    private void ApplyWaterBody()
    {
      foreach (var v in screensToMoveWayerBodies)
        v.Move();
      Refresh();
    }
    #endregion

    #region ApplyChalk
    RelayCommand applyChalkCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand ApplyChalkCommand
    {
      get
      {
        if (applyChalkCommand == null)
        {
          applyChalkCommand = new RelayCommand(param => this.ApplyChalk(), param => this.CanApplyChalk);
        }
        return applyChalkCommand;
      }
    }

    private bool CanApplyChalk
    {
      get
      {
        return ScreensToMove.Count != 0;
      }
    }

    private void ApplyChalk()
    {
      foreach (var v in ScreensToMove)
        v.Move();
      Refresh();
    }
    #endregion

    #region ApplyMoveDown
    RelayCommand applyMoveDownCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand ApplyMoveDownCommand
    {
      get
      {
        if (applyMoveDownCommand == null)
        {
          applyMoveDownCommand = new RelayCommand(param => this.ApplyMoveDown(), param => this.CanApplyMoveDown);
        }
        return applyMoveDownCommand;
      }
    }

    private bool CanApplyMoveDown
    {
      get
      {
        return this.ScreensAboveTerrain.Count != 0;
      }
    }

    private void ApplyMoveDown()
    {
      foreach (var v in ScreensAboveTerrain)
        v.Move();
      Refresh();
    }
    #endregion

    #region ApplyMoveUp
    RelayCommand applyMoveUpCommand;

    /// <summary>
    /// Gets the command that loads the Mike she
    /// </summary>
    public ICommand ApplyMoveUpCommand
    {
      get
      {
        if (applyMoveUpCommand == null)
        {
          applyMoveUpCommand = new RelayCommand(param => this.ApplyMoveUp(), param => this.CanApplyMoveUp);
        }
        return applyMoveUpCommand;
      }
    }

    private bool CanApplyMoveUp
    {
      get
      {
        return ScreensBelowBottom.Count != 0;
      }
    }

    private void ApplyMoveUp()
    {
      foreach (var v in ScreensBelowBottom)
        v.Move();
      Refresh();
    }
    #endregion
  
  
  }
}
