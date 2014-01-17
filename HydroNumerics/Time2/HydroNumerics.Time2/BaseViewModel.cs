using System;
using System.Net;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HydroNumerics.Time2
{
  [DataContract]
  public class BaseViewModel: NotifyModel
  {      

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
          NotifyPropertyChanged("ID");
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
          NotifyPropertyChanged("Name");
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
          NotifyPropertyChanged("Description");
        }
      }
    }
    

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (!(obj is BaseViewModel))
        return false;
      if (ID == 0 & ((BaseViewModel)obj).ID == 0 && !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(((BaseViewModel)obj).Name))
        return Name.Equals(((BaseViewModel)obj).Name); //If id is not set use name instead
      return ID.Equals( ((BaseViewModel)  obj).ID);
    }

    public override int GetHashCode()
    {
      if (ID == 0)
      {
        if (string.IsNullOrWhiteSpace(Name))
          return base.GetHashCode();
        return Name.GetHashCode();
      }
      return ID.GetHashCode();
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
