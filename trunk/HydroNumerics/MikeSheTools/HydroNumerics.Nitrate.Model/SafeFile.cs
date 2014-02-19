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
    

  }
}
