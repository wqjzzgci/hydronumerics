using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Tough2.ViewModel
{
  public class Mesh : FileClass
  {

    private ElementCollection elements;
    /// <summary>
    /// Gets the elements
    /// </summary>
    public ElementCollection Elements
    {
      get
      {
        if (elements == null)
          ReadMesh();
        return elements;
      }
    }

    private List<Connection> connections;
    /// <summary>
    /// Gets the connections
    /// </summary>
    public List<Connection> Connections
    {
      get
      {
        if (connections == null)
          ReadMesh();
        return connections;
      }
    }

    /// <summary>
    /// Mesh constructor. 
    /// </summary>
    /// <param name="FileName"></param>
    public Mesh(string FileName)
      : base(FileName)
    {
    }

    public override void Save()
    {
      using (StreamWriter SW = new StreamWriter(FileName, false, Encoding.Default))
      {
        SW.WriteLine("ELEME");
        foreach (Element E in Elements)
          SW.WriteLine(E.ToString());

        SW.WriteLine();
        SW.WriteLine("CONNE");
        foreach (Connection C in Connections)
          SW.WriteLine(C.ToString());
        SW.WriteLine();
        SW.WriteLine();

      }
    }




    /// <summary>
    /// Reads in Mesh information from a stream. Does not look back in the stream
    /// </summary>
    /// <param name="StreamWithMeshInfo"></param>
    private void ReadMesh()
    {

      elements = new ElementCollection();
      connections = new List<Connection>();

      using (StreamReader StreamWithMeshInfo = new StreamReader(FileName))
      {
        while (!StreamWithMeshInfo.EndOfStream)
        {
          string word = StreamWithMeshInfo.ReadLine().Trim().ToUpper();

          if (word.StartsWith("ELEME"))
          {
            word = StreamWithMeshInfo.ReadLine().Trim();
            while (word != string.Empty)
            {
              Elements.Add(new Element(word));
              word = StreamWithMeshInfo.ReadLine().Trim();
            }
          }
          else if (word.StartsWith("CONNE"))
          {
            word = StreamWithMeshInfo.ReadLine().Trim();
            while (word != string.Empty & word != "+++")
            {
              Connection C = new Connection(word, Elements);
              Connections.Add(C);
              word = StreamWithMeshInfo.ReadLine().Trim();
            }
          }
        }
      }
    }
  }
}

