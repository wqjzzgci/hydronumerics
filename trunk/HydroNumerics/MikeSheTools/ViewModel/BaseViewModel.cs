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

    public BaseViewModel()
    {
      ThrowOnInvalidPropertyName = true;
    }

    protected void NotifyPropertyChanged(String propertyName)
    {
      VerifyPropertyName(propertyName);
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    /// <summary>
    /// Verifies that the property actually exists
    /// </summary>
    /// <param name="propertyName"></param>
    public void VerifyPropertyName(string propertyName)
    {
      if (TypeDescriptor.GetProperties(this)[propertyName] == null)
      {
        string msg = "Invalid property name: " + propertyName;

        if (ThrowOnInvalidPropertyName)
          throw new Exception(msg);
      }
    }

    /// <summary>
    /// Gets or sets a boolean to indicate whether the viewmodel should throw an exception when a property does not exist.
    /// </summary>
    public bool ThrowOnInvalidPropertyName { get; set; }

    /// <summary>
    /// Gets and sets the displayname
    /// </summary>
    public string DisplayName { get; set; }


    #endregion

  }
}
