using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class JupiterViewModel:BaseViewModel
  {
    ObservableKeyedCollection<int, PlantViewModel> Plants = new ObservableKeyedCollection<int,PlantViewModel>(new Func<PlantViewModel,int>(var=>var.IDNumber));

   

  }
}
