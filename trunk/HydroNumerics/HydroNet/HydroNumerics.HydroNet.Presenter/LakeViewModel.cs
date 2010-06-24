using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class LakeViewModel:WaterBodyViewModel,INotifyPropertyChanged
  {
    private Lake _lake;

    public LakeViewModel(Lake L)
      : base(L)
    {
      _lake = L;
    }


    /// <summary>
    /// Gets and sets the area of a lake.
    /// </summary>
    public double Area
    {
      get
      {
        return _lake.Area;
      }
      set
      {
        if (value != _lake.Area)
        {
          _lake.Area = value;
          NotifyPropertyChanged("Lake");
        }
      }
    }


  }
}
