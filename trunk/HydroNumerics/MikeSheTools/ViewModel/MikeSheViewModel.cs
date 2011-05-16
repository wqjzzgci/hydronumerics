using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Core;

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


      Chalks = new ObservableCollection<string>();
      Chalks.Add("K");

      Clays = new ObservableCollection<string>();
      Clays.Add("L");
      Clays.Add("Ml");

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


    public ObservableCollection<string> Chalks { get; private set; }

    public ObservableCollection<string> Clays {get;private set;}
  }
}
