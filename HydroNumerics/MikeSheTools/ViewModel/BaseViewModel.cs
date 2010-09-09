using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class BaseViewModel:INotifyPropertyChanged
  {
    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }


    #endregion

  }
}
