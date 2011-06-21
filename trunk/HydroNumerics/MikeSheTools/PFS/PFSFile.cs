using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS
{
  public class PFSFile
  {

    private PFSClass pfsClass;
    protected PFSClass _pfsClass
    {
      get
      {
        if (pfsClass == null)
          pfsClass = new PFSClass(FileName);
        return pfsClass;
      }
    }


    public PFSFile(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
    }

    /// <summary>
    /// Saves the .she-file to a new name
    /// </summary>
    /// <param name="SheFileName"></param>
    public void SaveAs(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
      Save();
    }

    /// <summary>
    /// Saves the .pfs file
    /// </summary>
    public void Save()
    {
      using (StreamWriter sw = new StreamWriter(FileName, false, Encoding.Default))
      {
        sw.Write(_pfsClass.ToString());
      }

    }

    /// <summary>
    /// Gets the name of the .pfs file
    /// </summary>
    public string FileName
    {
      get;
      private set;
    }

  }
}
