using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

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

      for (int i = 0; i < mshe.GridInfo.NumberOfLayers; i++)
      {
        MikeSheLayerViewModel msvm = new MikeSheLayerViewModel(mshe.GridInfo.NumberOfLayers -1-i, mshe.GridInfo.NumberOfLayers);
        msvm.DisplayName = mshe.Input.MIKESHE_FLOWMODEL.SaturatedZone.CompLayersSZ.Layer_2s[i].Name;
        Layers.Add(msvm);

        msvm.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(msvm_PropertyChanged);
      }
      Layers.Last().IsChalkLayer = true;


      Chalks = new ObservableCollection<string>();
      Chalks.Add("k");

      Chalks.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Chalks_CollectionChanged);

      Clays = new ObservableCollection<string>();
      Clays.Add("l");
      Clays.Add("ml");

      NotifyPropertyChanged("Layers");
      NotifyPropertyChanged("Chalks");
      NotifyPropertyChanged("Clays");

    }

    void Chalks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshChalk();
    }

    void msvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsChalkLayer")
      {
        RefreshChalk();
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

    public ObservableCollection<MoveToChalkViewModel> ScreensToMove { get; private set; }

    public IEnumerable<WellViewModel> wells;

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
                if (Chalks.Contains(l.RockSymbol.ToLower()))
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
                }
            }
        }
      }
      NotifyPropertyChanged("ScreensToMove");
    }
    
    public ObservableCollection<string> Chalks { get; private set; }

    public ObservableCollection<string> Clays {get;private set;}

    private double minLayThickness = 2;

    public double MinLayThickness
    {
      get { return minLayThickness; }
      set { minLayThickness = value; }
    }
    private double maxDistance = 10;

    public double MaxDistance
    {
      get { return maxDistance; }
      set { maxDistance = value; }
    }




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
