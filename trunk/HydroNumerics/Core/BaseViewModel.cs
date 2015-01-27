using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  [DataContract]
  public class BaseViewModel:GalaSoft.MvvmLight.ViewModelBase
  {
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
        RaisePropertyChanged("IsBusy");
      }
    }

    protected Task AsyncWithWait(Action method)
    {
      IsBusy = true;
      Task T = Task.Factory.StartNew(method);
      return T.ContinueWith((t) => IsBusy = false);
    }



    private string _DisplayName;
    /// <summary>
    /// Deprecated. Use name instead
    /// </summary>
    [DataMember]
    public string DisplayName
    {
      get 
      {
        if (string.IsNullOrEmpty(_DisplayName))
          return Name;
        else
          return _DisplayName; 
      }
      set
      {
        if (_DisplayName != value)
        {
          _DisplayName = value;
          RaisePropertyChanged("DisplayName");
        }
      }
    }
    


    private int iD;
    /// <summary>
    /// Gets and sets an ID-number
    /// </summary>
    [DataMember]
    public int ID
    {
      get
      {
        return iD;
      }
      set
      {
        if (iD != value)
        {
          iD = value;
          RaisePropertyChanged("ID");
        }
      }
    }

    private string name;

    /// <summary>
    /// Gets and sets a name
    /// </summary>
    [DataMember]
    public string Name
    {
      get
      {
        return name;
      }
      set
      {
        if (name != value)
        {
          name = value;
          RaisePropertyChanged("Name");
        }
      }
    }

    private string description;

    /// <summary>
    /// Gets and sets a description
    /// </summary>
    [DataMember]
    public string Description
    {
      get
      {
        return description;
      }
      set
      {
        if (description != value)
        {
          description = value;
          RaisePropertyChanged("Description");
        }
      }
    }


    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (!(obj is BaseViewModel))
        return false;
      if (ID == 0 & ((BaseViewModel)obj).ID == 0) //Id is not set.
      {
        if (string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(((BaseViewModel)obj).Name)) //Names are not set
          return base.Equals(obj); //Use base
        else if (string.IsNullOrEmpty(Name)) //Either one or the other have null names
          return false;
        else if (string.IsNullOrEmpty(((BaseViewModel)obj).Name))
          return false;
        else //Both have names
          return Name.Equals(((BaseViewModel)obj).Name); // use name 
      }

      return ID.Equals(((BaseViewModel)obj).ID); //Use ID
    }

    public override int GetHashCode()
    {
      if (ID == 0) //Id not set
      {
        if (string.IsNullOrWhiteSpace(Name)) //Name not set
          return base.GetHashCode(); //Use base
        return Name.GetHashCode(); // use name
      }
      return ID.GetHashCode(); //Use id
    }

    public override string ToString()
    {
      if (!String.IsNullOrEmpty(name))
        return name;
      else
        return ID.ToString();
    }



  }
}
