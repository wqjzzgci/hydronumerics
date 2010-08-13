using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.NWK11File
{
  public class NWK11File
  {
    private MIKE_11_Network_editor  _m11;
    private PFSClass m11;

    public NWK11File(string SheFileName)
    {
      FileName = Path.GetFullPath(SheFileName);
      m11 = new PFSClass(FileName);
      _m11 = new MIKE_11_Network_editor(m11.GetTarget(1));
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
      m11.DumpToPfsFile(FileName);
    }

    /// <summary>
    /// Access to the entries in the .she-file
    /// </summary>
    public MIKE_11_Network_editor MIKE_11_Network_editor
    {
      get { return _m11; }
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
