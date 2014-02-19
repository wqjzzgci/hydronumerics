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
    private string _FileName;
    public string FileName
    {
      get { return _FileName; }
      set
      {
          _FileName = value;
          if (!File.Exists(_FileName))
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
