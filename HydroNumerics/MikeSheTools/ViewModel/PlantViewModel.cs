using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.JupiterTools;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class PlantViewModel:BaseViewModel 
  {
    private Plant _plant;

    public Plant JupPlant
    {
      get
      {
        return _plant;
      }
    }

    public int IDNumber
    {
      get
      {
        return _plant.IDNumber;
      }
    }

    public string Address
    {
      get
      {
        return _plant.Address;
      }
    }



    public PlantViewModel(Plant Plant)
    {
      _plant = Plant;
      
    }
  }
}
