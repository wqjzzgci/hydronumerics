using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class IDObjectViewModel : BaseViewModel
  {
    IDObject _id;

    public IDObjectViewModel(IDObject NamedObject)
    {
      _id = NamedObject;
    }


    /// <summary>
    /// Gets and sets the Name of this object
    /// </summary>
    public string Name
    {
      get
      {
        return _id.Name;
      }
      set
      {
        if (value != _id.Name)
        {
          NotifyPropertyChanged("Name");
          _id.Name = value;
        }
      }
    }

    /// <summary>
    /// Gets and sets the ID of this object
    /// </summary>
    public int ID
    {
      get
      {
        return _id.ID;
      }
      set
      {
        if (value != _id.ID)
        {
          NotifyPropertyChanged("ID");
          _id.ID = value;
        }
      }
    }

  }

}

