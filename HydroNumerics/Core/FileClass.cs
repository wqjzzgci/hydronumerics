using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  public class FileClass:NotifyPropertyChangedBase
  {

    public FileClass(string FileName)
    {
      this.FileName = FileName;
    }


    private string fileName;

    /// <summary>
    /// Gets the FileName
    /// </summary>
    public string FileName
    {
      get
      {
        return fileName;
      }
      protected set
      {
        if (fileName != value)
        {
          fileName = value;
          NotifyPropertyChanged("FileName");
        }
      }
    }

    /// <summary>
    /// Saves the file
    /// </summary>
    public virtual void Save()
    {

    }

    /// <summary>
    /// Saves the file to a new file name
    /// </summary>
    /// <param name="NewFileName"></param>
    public virtual void SaveAs(string NewFileName)
    {
      FileName = NewFileName;
      Save();
    }
  }
}
