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



    public PlantViewModel(Plant Plant)
    {
      _plant = Plant;
    }
  }
}
