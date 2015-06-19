using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  public class FileTools
  {

    /// <summary>
    /// Moves a file and renames if it exist. Only renames 5 times. Both filenames must be full paths
    /// </summary>
    /// <param name="file"></param>
    /// <param name="NewFileName"></param>
    /// <returns></returns>
    public static int SafeMove(string file, string NewFileName)
    {
      int i = 0;

      Directory.CreateDirectory(Path.GetDirectoryName(NewFileName));

      while (File.Exists(NewFileName) & i < 5)
      {
        NewFileName += "_" + i;
        i++;
      }

      if (i < 5)
        File.Move(file, NewFileName);

      return i;
    }


    /// <summary>
    /// Returns true if file is locked
    /// </summary>
    /// <param name="FileName"></param>
    /// <returns></returns>
    public static bool IsFileLocked(string FileName)
    {
      FileStream stream = null;
      FileInfo file = new FileInfo(FileName);

      try
      {
        stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
      }
      catch (IOException)
      {
        //the file is unavailable because it is:
        //still being written to
        //or being processed by another thread
        //or does not exist (has already been processed)
        return true;
      }
      finally
      {
        if (stream != null)
          stream.Close();
      }

      //file is not locked
      return false;

    }

  }
}
