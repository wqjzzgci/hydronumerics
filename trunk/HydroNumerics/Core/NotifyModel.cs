using System;
using System.Net;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace HydroNumerics.Core
{
  [DataContract]
  public class NotifyModel : INotifyPropertyChanged
  {


    /// <summary>
    /// Verifies that the property actually exists
    /// </summary>
    /// <param name="propertyName"></param>
    private void VerifyPropertyName(string propertyName)
    {
      if (TypeDescriptor.GetProperties(this)[propertyName] == null)
      {
        string msg = "Invalid property name: " + propertyName;
      }
    }


    private bool throwOnInvalidProperyName = false;
    /// <summary>
    /// Gets or sets a boolean to indicate whether the viewmodel should throw an exception when a property does not exist.
    /// </summary>
    protected bool ThrowOnInvalidPropertyName
    {
      get
      {
        return throwOnInvalidProperyName;
      }
      set
      {
        throwOnInvalidProperyName = value;
      }
    }

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(String propertyName)
    {
      if (ThrowOnInvalidPropertyName)
        VerifyPropertyName(propertyName);

      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}
