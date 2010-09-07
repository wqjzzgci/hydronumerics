using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.Sim11
{
  public class Sim11File
  {
    private Input  _input;
    private PFSClass m11;

    public Sim11File(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
      m11 = new PFSClass(FileName);
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
