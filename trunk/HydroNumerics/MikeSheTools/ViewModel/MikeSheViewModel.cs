using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

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

      for (int i = 0; i < mshe.GridInfo.NumberOfLayers; i++)
      {
        MikeSheLayerViewModel msvm = new MikeSheLayerViewModel(mshe.GridInfo.NumberOfLayers -1-i, mshe.GridInfo.NumberOfLayers);
        msvm.DisplayName = mshe.Input.MIKESHE_FLOWMODEL.SaturatedZone.CompLayersSZ.Layer_2s[i].Name;
        Layers.Add(msvm);
      }
      Layers.Last().IsChalkLayer = true;


      Chalks = new ObservableCollection<string>();
      Chalks.Add("k");

      Clays = new ObservableCollection<string>();
      Clays.Add("l");
      Clays.Add("ml");

      NotifyPropertyChanged("Layers");
      NotifyPropertyChanged("Chalks");
      NotifyPropertyChanged("Clays");

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

    public void SetWells(IEnumerable<WellViewModel> wells)
    {
      ScreensToMove = new ObservableCollection<MoveToChalkViewModel>();
      NotifyPropertyChanged("ScreensToMove");

      var ChalkLayer = Layers.Single(var=>var.IsChalkLayer);

      foreach (var w in wells)
      {
        foreach (var i in w.Intakes)
          foreach (var s in i.Screens)
          {
            var lits = w.Lithology.Where(var=>var.Bottom>s.DepthToTop & var.Top<s.DepthToBottom);

            foreach(var l in lits)
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
    

    public ObservableCollection<string> Chalks { get; private set; }

    public ObservableCollection<string> Clays {get;private set;}
  }
}
