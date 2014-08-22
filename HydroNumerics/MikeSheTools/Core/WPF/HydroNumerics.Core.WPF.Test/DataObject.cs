using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core.WPF.Test
{
  public class DataObject:BaseViewModel
  {


    private double x;
    public double X
    {
      get
      {
        return x;
      }
      set
      {
        if (value != x)
        {
          x = value;
          RaisePropertyChanged("X");

        }
      }
    }


    private double y;
    public double Y
    {
      get
      {
        return y;
      }
      set
      {
        if (value != y)
        {
          y = value;
          RaisePropertyChanged("Y");

        }
      }
    }


  }
}
