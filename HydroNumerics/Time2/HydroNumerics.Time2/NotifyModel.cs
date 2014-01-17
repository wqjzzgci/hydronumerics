using System;
using System.Net;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace HydroNumerics.Time2
{
  [DataContract]
  public class NotifyModel : INotifyPropertyChanged
  {

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }
    #endregion
  }
}
