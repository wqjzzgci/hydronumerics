using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class DfsFileFactory
  {

    public static DFSBase OpenFile(string FileName)
    {
      string FileType = Path.GetExtension(FileName).ToLower();
      DFSBase dfs;

      switch (FileType)
      {
        case ".dfs0":
          dfs = new DFS0(FileName);
          break;
        case ".dfs2":
          dfs = new DFS2(FileName);
          break;
        case ".dfs3":
          dfs = new DFS3(FileName);
          break;
        default:
          throw new Exception("Unsupported file format: " + FileType);
      }
      return dfs;
    }

    public static DFSBase CreateFile(string FileName, int NumberOfItems)
    {
      string FileType = Path.GetExtension(FileName).ToLower();
      DFSBase dfs;

      switch (FileType)
      {
        case ".dfs0":
          dfs = new DFS0(FileName, NumberOfItems);
          break;
        case ".dfs2":
          dfs = new DFS2(FileName, NumberOfItems);
          break;
        case ".dfs3":
          dfs = new DFS3(FileName, NumberOfItems);
          break;
        default:
          throw new Exception("Unsupported file format: " + FileType);
      }
      return dfs;

    }
  }
}
