using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.SheFile
{
  public class InputFile
  {
    private MIKESHE_FLOWMODEL _mshe;
    private PFSClass she1;
    
    public InputFile(string SheFileName)
    {
      FileName = Path.GetFullPath(SheFileName);
      she1 = new PFSClass(FileName);
      _mshe = new MIKESHE_FLOWMODEL( she1.GetTarget(1) );
    }

    /// <summary>
    /// Saves the .she-file to a new name
    /// </summary>
    /// <param name="SheFileName"></param>
    public void SaveAs(string SheFileName)
    {
      FileName = Path.GetFullPath(SheFileName);
      Save();
    }

    /// <summary>
    /// Saves the .she file
    /// </summary>
    public void Save()
    {
      using (StreamWriter sw = new StreamWriter(FileName))
      {
        sw.Write(she1.ToString());
      }
    }

    /// <summary>
    /// Access to the entries in the .she-file
    /// </summary>
    public MIKESHE_FLOWMODEL MIKESHE_FLOWMODEL
    {
      get { return _mshe; }
    }

    /// <summary>
    /// Gets the name of the .she file
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }

  }
}
