using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroNumerics.Core
{
  public class LogToFile
  {
    public string LogFile { get; set; }

    public LogToFile(string FileName)
    {

      LogFile = FileName;
    }


    public int? MaxNoLines { get; set; }

    public void AppendLine(string line)
    {
      try
      {

        string log;
        if (MaxNoLines.HasValue && File.Exists(LogFile))
        {

          using (StreamReader sr = new StreamReader(LogFile))
          {
            log = sr.ReadToEnd();
          }

          var lines = log.Split(new string[] { "\n" }, StringSplitOptions.None);

          if (lines.Count() > MaxNoLines.Value)
            lines = lines.Skip(lines.Count() - MaxNoLines.Value).ToArray();

          using (StreamWriter sw = new StreamWriter(LogFile))
          {
            foreach (var l in lines)
              sw.WriteLine(l);
            sw.WriteLine(DateTime.UtcNow.ToString() + ": " + line);
          }
        }
        else
        {
          using (StreamWriter sw = new StreamWriter(LogFile, true))
          {
            sw.WriteLine(DateTime.UtcNow.ToString() + ": " + line);
          }

        }
      }
      catch(Exception e)
      {
        //Could not write to file

      }
    }

  }
}
