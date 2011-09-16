using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class MikeSheLayerViewModel:NotifyPropertyChangedBase
  {

    public MikeSheLayerViewModel(int dfsNumber, int TotalNumberOfLayers)
    {
      DfsLayerNumber = dfsNumber;
      MikeSheLayerNumber = TotalNumberOfLayers - dfsNumber;
    }

    private bool isChalkLayer = false;
    public bool IsChalkLayer
    {
      get
      {
        return isChalkLayer;
      }
      set
      {
        if (isChalkLayer != value)
        {
          isChalkLayer = value;
          NotifyPropertyChanged("IsChalkLayer");
          if (isChalkLayer)
            IsGroundWaterBody = true;
        }
      }
    }

    private bool isGroundWaterBody = true;
    public bool IsGroundWaterBody
    {
      get
      {
        return isGroundWaterBody;
      }
      set
      {
        if (isGroundWaterBody != value)
        {
          isGroundWaterBody = value;
          NotifyPropertyChanged("IsGroundWaterBody");
        }
      }
    }


    public int MikeSheLayerNumber { get; private set; }

    public int DfsLayerNumber { get; private set; }


  }
}
