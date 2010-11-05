using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class WaterViewModel:BaseViewModel 
  {
    private IWaterPacket _water;

    public ObservableCollection<Tuple<string, double>> Chemicals { get; private set; }

    public WaterViewModel(IWaterPacket Water)
    {
      _water = Water;
      BuildChemicalView();
    }


    private void BuildChemicalView()
    {
      Chemicals = new ObservableCollection<Tuple<string, double>>();
      WaterPacket Wc = _water as WaterPacket;

      if (Wc != null)
      {
        foreach (var c in Wc.Chemicals)
        {
          Chemicals.Add(new Tuple<string,double>(c.Key.Name,Wc.GetConcentration(c.Key)));
        }
      }
    }

    /// <summary>
    /// Gets the volume
    /// </summary>
    public double Volume
    {
      get
      {
        return _water.Volume;
      }
    }

  }
}
