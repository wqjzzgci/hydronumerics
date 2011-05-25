using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Mesh
  {
    public ElementCollection Elements { get; private set; }
    public List<Connection> Connections { get; private set; }


    public Mesh()
    {
      Elements = new ElementCollection();
      Connections = new List<Connection>();
    }

    /// <summary>
    /// Saves to a file. Overwrites without warning if the file exists.
    /// </summary>
    /// <param name="FileName"></param>
    public void Save(string FileName)
    {
      StreamWriter SW = new StreamWriter(FileName, false, Encoding.Default);
      Save(SW);
      SW.Dispose();
    }

    /// <summary>
    /// Saves to a stream
    /// </summary>
    /// <param name="Stream"></param>
    public void Save(StreamWriter Stream)
    {
      Stream.WriteLine("ELEME");
      foreach (Element E in Elements)
        Stream.WriteLine(E.ToString());

      Stream.WriteLine();
      Stream.WriteLine("CONNE");
      foreach (Connection C in Connections)
        Stream.WriteLine(C.ToString());
      Stream.WriteLine();
      Stream.WriteLine();
    }

    /// <summary>
    /// Reads in Mesh information from a File
    /// </summary>
    /// <param name="FileName"></param>
    public void Open(string FileName)
    {
      StreamReader Sr = new StreamReader(FileName);
      Open(Sr);
      Sr.Dispose();
    }

    /// <summary>
    /// Reads in Mesh information from a stream. Does not look back in the stream
    /// </summary>
    /// <param name="StreamWithMeshInfo"></param>
    public void Open(StreamReader StreamWithMeshInfo)
    {
      while (!StreamWithMeshInfo.EndOfStream)
      {
        string word = StreamWithMeshInfo.ReadLine().Trim().ToUpper();

        switch (word)
        {
          case "ELEME":
            word = StreamWithMeshInfo.ReadLine().Trim();
            while (word != string.Empty)
            {
              Elements.Add(new Element(word));
              word = StreamWithMeshInfo.ReadLine().Trim();
            }
            break;
          case "CONNE":
            word = StreamWithMeshInfo.ReadLine().Trim();
            while (word != string.Empty & word != "+++")
            {
              Connection C = new Connection(word, Elements);
              Connections.Add(C);
              word = StreamWithMeshInfo.ReadLine().Trim();
            }
            break;
          default:
            break;
        }
      }
    }
  }
}
