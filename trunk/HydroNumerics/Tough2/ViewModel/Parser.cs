using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Parser
  {

    public static IEnumerable<TSMassEntry> TSMASS(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        while (!sr.EndOfStream)
        {
          var split = sr.ReadLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
          var doubles = split.Select(var => double.Parse(var));
          yield return new TSMassEntry(TimeSpan.FromHours(doubles.First()), doubles.Skip(1).ToArray());
        }
      }
    }


    public static IEnumerable<TSBrtEntry> TSBRT(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        while (!sr.EndOfStream)
        {
          var split = sr.ReadLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
          var doubles = split.Select(var => double.Parse(var));
          yield return new TSBrtEntry(TimeSpan.FromHours(doubles.First()), doubles.Skip(1).ToArray());
        }
      }
    }

    public static void FOFT(string FileName, Model M)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        List<Element> Cons = null;
        int nelement = 0;
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          var split = line.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

          //First time we are here     
          if (Cons == null)
          {
            Cons = new List<Element>();
            nelement = (split.Count() - 2) / 6;
            for (int i = 0; i < nelement; i++)
            {
              Cons.Add(M.Elements[int.Parse(split[2 +i*6]) - 1]);
              Cons[i].TimeData  = new List<TSEntry>();
            }
          }
          TimeSpan Time = TimeSpan.FromSeconds(Double.Parse(split[1]));
          for (int i = 0; i < nelement; i++)
          {
            double[] vals = new double[5];
            for (int j = 0; j < 5; j++)
            {
              double d = 0;
              double.TryParse(split[3 + i * 6 + j], out d);
              vals[j] = d;
            }
            Cons[i].TimeData.Add(new TSEntry(Time, vals));
          }

        }
      }
    }

    /// <summary>
    /// Opens file with flow data for each timestep and attached data to the relevant connections
    /// </summary>
    /// <param name="FileName"></param>
    /// <param name="M"></param>
    public static void COFT(string FileName, Model M)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        List<Connection> Cons=null;
        int nconnect=0;
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          var split = line.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries);
          
          //First time we are here     
          if (Cons == null)
          {
            Cons = new List<Connection>();
            nconnect = (split.Count() - 2) / 5;
            for (int i = 0; i < nconnect; i++)
            {
              Cons.Add(M.Connections[int.Parse(split[2]) - 1]);
              Cons[i].Flow = new List<FlowDataEntry>();
            }
          }
          TimeSpan Time = TimeSpan.FromSeconds(Double.Parse(split[1]));
          for (int i = 0; i < nconnect; i++)
          {
            double[] vals = new double[4];
            for (int j = 0; j < 4; j++)
            {
              double d = 0;
              double.TryParse(split[3 + i*5+j], out d);
              vals[j] =d;
            }
            Cons[i].Flow.Add(new FlowDataEntry(Time,vals));
          }
        }
      }
    }
  }
}
