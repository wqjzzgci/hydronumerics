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
        protected PFSClass _pfsClass;

        public PFSFile(string Sim11FileName)
    {
      FileName = Path.GetFullPath(Sim11FileName);
      _pfsClass = new PFSClass(FileName);
      _pfsClass.SourceDirectory = @"C:\Program Files\DHI\2011\bin";
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
      DHI.Mike1D.PFS.PFSFile file = new DHI.Mike1D.PFS.PFSFile(_pfsClass.DumpToPfs());

      file.Write(FileName);
      file.Close();

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
