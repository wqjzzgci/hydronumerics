using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
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

    private bool isBusy = false;
    /// <summary>
    /// Returns true if the viewmodel is busy reading data
    /// </summary>
    public bool IsBusy
    {
      get { return isBusy; }
      set
      {
        isBusy = value;
        NotifyPropertyChanged("IsBusy");
      }
    }

    protected Task AsyncWithWait(Action method)
    {
      IsBusy = true;
      Task T = Task.Factory.StartNew(method);
      return T.ContinueWith((t) => IsBusy = false);
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

      //v  if (ThrowOnInvalidPropertyName)
        //  throw new Exception(msg);
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
