using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class SafeFile:NotifyModel
  {

    private bool _CheckIfFileExists = true;
    public bool CheckIfFileExists
    {
      get { return _CheckIfFileExists; }
      set
      {
        if (_CheckIfFileExists != value)
        {
          _CheckIfFileExists = value;
          NotifyPropertyChanged("CheckIfFileExists");
        }
      }
    }
    

    private string _FileName;
    public string FileName
    {
      get { return _FileName; }
      set
      {
          _FileName = value;
          if (!File.Exists(_FileName) & CheckIfFileExists)
            throw new FileNotFoundException("File= " +_FileName);
          NotifyPropertyChanged("FileName");
      }
    }

    private List<double> _Parameters= new List<double>();
    /// <summary>
    /// Gets the list of parameters. These parameters are context specific
    /// </summary>
    public List<double> Parameters
    {
      get { return _Parameters; }
      set
      {
        if (_Parameters != value)
        {
          _Parameters = value;
          NotifyPropertyChanged("Parameters");
        }
      }
    }

    private List<bool> _Flags = new List<bool>();
    public List<bool> Flags
    {
      get { return _Flags; }
      set
      {
        if (_Flags != value)
        {
          _Flags = value;
          NotifyPropertyChanged("Flags");
        }
      }
    }
        
    

    private List<string> _ColumnNames = new List<string>();
    public List<string> ColumnNames
    {
      get { return _ColumnNames; }
      set
      {
        if (_ColumnNames != value)
        {
          _ColumnNames = value;
          NotifyPropertyChanged("ColumnNames");
        }
      }
    }

    public override bool Equals(object obj)
    {
      return FileName.Equals(((SafeFile)obj).FileName);
    }

    public override int GetHashCode()
    {
      return FileName.GetHashCode();
    }


  }
}
