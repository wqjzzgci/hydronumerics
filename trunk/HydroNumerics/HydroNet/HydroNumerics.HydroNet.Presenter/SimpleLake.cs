using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class SimpleLake:WaterBodyViewModel 
  {
    private Lake _l;

    public SimpleLake(Lake L)
      : base(L)
    {
      _l = L;
    }


  
  }
}
