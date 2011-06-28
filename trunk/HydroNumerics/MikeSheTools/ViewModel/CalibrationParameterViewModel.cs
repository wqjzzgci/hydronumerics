using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using HydroNumerics.MikeSheTools.Core;

using MathNet.Numerics.Distributions;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class CalibrationParameterViewModel : BaseViewModel, IDataErrorInfo, IComparable<CalibrationParameterViewModel>
  {
    private CalibrationParameter cp;
    private bool isUsedInCalibration = true;

    private string uniqueID;
    public string UniueID
    {
      get
      {
        return uniqueID;
      }
      set
      {
        uniqueID = value;
      }
    }

   



    public bool IsUsedInCalibration
    {
      get { return isUsedInCalibration; }
      set
      {
        isUsedInCalibration = value;
        NotifyPropertyChanged("IsUsedInCalibration");
      }
    }

    public CalibrationParameterViewModel(CalibrationParameter CP)
    {
      cp = CP;
      DisplayName = cp.ShortName;

      //Default boundaries one order of magnitude to each side
      MaxValue = Math.Pow(10, Math.Round(Math.Log10(CurrentValue)) + 1);
      MinValue = Math.Pow(10, Math.Round(Math.Log10(CurrentValue)) - 1);
    
    //statDistribution = new LogNormal(CurrentValue, CurrentValue / 10);
     
    }

    private IContinuousDistribution statDistribution;

    public IContinuousDistribution StatDistribution
    {
      get
      {
        return statDistribution;
      }
    }

    private double maxValue;
    public double MaxValue
    {
      get
      {
        return maxValue;
      }
      set
      {
        if (maxValue != value)
        {
          maxValue = value;
          NotifyPropertyChanged("MaxValue");
        }
      }
    }

    private double minValue;
    public double MinValue
    {
      get
      {
        return minValue;
      }
      set
      {
        if (minValue != value)
        {
          minValue = value;
          NotifyPropertyChanged("MinValue");
        }
      }
    }

    public double CurrentValue
    {
      get
      {
        return cp.CurrentValue;
      }
      set
      {
        if (cp.CurrentValue != value)
        {
          cp.CurrentValue = value;
          NotifyPropertyChanged("CurrentValue");
        }
      }
    }



    #region IDataErrorInfo Members

    public string Error
    {
      get { return null; }
    }

    public string this[string columnName]
    {
      get
      {
        string result = null;

        if (columnName == "MaxValue" || columnName == "MinValue")
        {
          if (MaxValue < MinValue)
          {
            result = "MaxValue has to larger than MinValue";
          }
        }
        return result;
      }
    }

    #endregion

    #region IComparable<CalibrationParameterViewModel> Members

    public int CompareTo(CalibrationParameterViewModel other)
    {
      return DisplayName.CompareTo(other.DisplayName);
    }

    #endregion
  }
}
