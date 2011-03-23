using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace GridTools
{
  public class ConfigurationReader
  {

    public static Configuration GetConfiguration(string FileName)
    {
      using (FileStream Fs = new FileStream(FileName, FileMode.Open))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(Configuration), null, int.MaxValue, false, false, null);
        return (Configuration)ds.ReadObject(Fs);
      }
    }

    public static void SaveConfiguration(string FileName, Configuration M)
    {
      using (FileStream Fs = new FileStream(FileName, FileMode.Create))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(Configuration), null, int.MaxValue, false, false, null);
        ds.WriteObject(Fs, M);
      }
    }

  }
}
