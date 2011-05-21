using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

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
      ScreensToMove = new ObservableCollection<MoveToChalkViewModel>();
      ScreensToMoveWaterBodies = new ObservableCollection<MoveToChalkViewModel>();

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


    public IEnumerable<WellViewModel> wells;

    public ObservableCollection<MoveToChalkViewModel> ScreensToMove { get; private set; }
    public void RefreshChalk()
    {
      ScreensToMove.Clear();

      var ChalkLayer = Layers.Single(var => var.IsChalkLayer);

      if (wells != null)
      {
        foreach (var w in wells)
        {
          foreach (var i in w.Intakes)
            foreach (var s in i.Screens)
            {
              var lits = w.Lithology.Where(var => var.Bottom > s.DepthToTop & var.Top < s.DepthToBottom);

              foreach (var l in lits)
                if (Chalks.ContainsKey(l.RockSymbol.ToLower()))
                {
                  w.LinkToMikeShe(mshe);
                  double top = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, ChalkLayer.DfsLayerNumber];
                  double bottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, ChalkLayer.DfsLayerNumber];
                  if (bottom > s.TopAsKote || top < s.BottomAsKote)
                  {
                    MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                    mc.NewBottom = bottom;
                    mc.NewTop = top;
                    ScreensToMove.Add(mc);
                  }
                  break;
                }
            }
        }
      }
      NotifyPropertyChanged("ScreensToMove");
    }


    public ObservableCollection<MoveToChalkViewModel> ScreensToMoveWaterBodies { get; private set; }
    public void RefreshWaterbodies()
    {
      ScreensToMoveWaterBodies.Clear();

      var waterbodies = Layers.Where(var => var.IsGroundWaterBody);

      if (wells != null)
      {
        foreach (var w in wells)
        {
          foreach (var i in w.Intakes)
            foreach (var s in i.Screens)
            {
              var lits = w.Lithology.Where(var => var.Bottom > s.DepthToTop & var.Top < s.DepthToBottom);

              bool move = false;
              foreach (var l in lits)
                if (!Clays.ContainsKey(l.RockSymbol.ToLower()))
                  move = true;

              if (move)
              {
                w.LinkToMikeShe(mshe);
                int topl = mshe.GridInfo.GetLayer(w.Column, w.Row, s.TopAsKote.Value);
                int bottoml = mshe.GridInfo.GetLayer(w.Column, w.Row, s.BottomAsKote.Value);

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
                    down = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber] - s.BottomAsKote.Value;

                  if (up < maxDistance & up < down)//Move up
                  {
                    MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                    mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, upperdistance.DfsLayerNumber];
                    mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, upperdistance.DfsLayerNumber];
                    ScreensToMoveWaterBodies.Add(mc);
                  }
                  else if (down <maxDistance & down<up)
                  {
                    MoveToChalkViewModel mc = new MoveToChalkViewModel(w, s);
                    mc.NewBottom = mshe.GridInfo.LowerLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber];
                    mc.NewTop = mshe.GridInfo.UpperLevelOfComputationalLayers.Data[w.Row, w.Column, lowerdistance.DfsLayerNumber];
                    ScreensToMoveWaterBodies.Add(mc);
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
        return ScreensToMoveWaterBodies.Count != 0;
      }
    }

    private void ApplyWaterBody()
    {
      foreach (var v in ScreensToMoveWaterBodies)
        v.Move();
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
    }
    #endregion
  }
}
