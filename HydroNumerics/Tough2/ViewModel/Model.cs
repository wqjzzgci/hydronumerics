using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public enum BlockPointer
  {
    Prelude,
    Iterating,
    FirstMatrixOutput,
    SecondMatrixOutput,
    MassBalance
  }

  public class Model:INotifyPropertyChanged
  {

    public Simulator simu { get; set; }

    private string _inputFileName;

    public ElementCollection Elements { get; private set; }
    public List<Connection> Connections { get; private set; }


    
    /// <summary>
    /// Gets and sets the input file name
    /// </summary>
    public string InputFileName
    {
      get
      {
        return _inputFileName;
      }
      set
      {
        if (value != _inputFileName)
        {
          _inputFileName = value;
          Load();
        }
      }
    }

    /// <summary>
    /// Gets the input file content as a string
    /// </summary>
    public string FileContent { get; set; }


    /// <summary>
    /// Gets the directory where the model is located
    /// </summary>
    public string ModelDirectory
    {
      get
      {
        return Path.GetDirectoryName(InputFileName);
      }
    }

    public Rocks Rocks { get;  set; }

    public OutputFileParser Results { get; private set; }

    public Model()
    {
      Results = new OutputFileParser(this);
      simu = new Simulator(this);
      Elements = new ElementCollection();
      Connections = new List<Connection>();

     

    }

    public Model(string InputFileName):this()
    {
      this.InputFileName = InputFileName;
    }

    /// <summary>
    /// Loads the mesh
    /// </summary>
    private void Load()
    {

      Open(Path.Combine(ModelDirectory, "mesh"));
     

      using (ReaderUtilities sr = new ReaderUtilities(InputFileName))
      {
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();

          if (line.StartsWith("ROCKS"))
          {
            Rocks = new Rocks();
            Rocks.ReadFromStream(sr);

            foreach (Element e in Elements)
            {
              e.rock = Rocks[e.Material-1];
            }
          }
          else if (line.StartsWith("INCON"))
          {
            while ((line = sr.ReadLine().TrimEnd()) != String.Empty)
            {
              if (line.StartsWith("+++"))
                break;

              string elname = line.Substring(0,5);
              
              if (Elements.Contains(elname))
              {
                Elements[elname].Porosity = ReaderUtilities.SplitIntoDoubles(line, 15, 15)[0];

                if (line.Length > 31)
                  Elements[elname].PrimaryVariablesIndex = int.Parse(line.Substring(31, 1));

                Elements[elname].PrimaryVaribles = ReaderUtilities.SplitIntoDoubles(sr.ReadLine(), 0, 20);
              }
            }
          }
          else if (line.StartsWith("PARAM"))
          {
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            sr.ReadLine();
            line = sr.ReadLine();
            var arr = ReaderUtilities.SplitIntoDoubles(line, 0, 20);

            foreach (var el in Elements)
            {
              if (el.PrimaryVaribles == null)
                el.PrimaryVaribles = arr.ToArray();
            }

          }
        }
        FileContent = sr.FileContent.ToString();
      }
      NotifyPropertyChanged("FileContent");
    }

    public string GetINCON()
    {
      StringBuilder outp = new StringBuilder();
      outp.AppendLine("INCON");

      foreach (var el in Elements)
      {
        outp.Append(el.Name + "           " + el.Porosity.ToString("0.00000000E+00"));
        if (el.PrimaryVariablesIndex.HasValue)
        {
          outp.AppendLine(" " +el.PrimaryVariablesIndex.ToString());
        }
        else
          outp.AppendLine();
        outp.AppendLine(ReaderUtilities.JoinIntoString(el.PrimaryVaribles,20));
      }

      return outp.ToString();
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

    public void SaveMesh()
    {
      StreamWriter SW = new StreamWriter(Path.Combine(ModelDirectory, "mesh"), false, Encoding.Default);
      Save(SW);
      SW.Dispose();
    }

    public void SaveOutput(string filename)
    {
      StreamWriter SW = new StreamWriter(Path.Combine(ModelDirectory, filename), false, Encoding.Default);
      SW.Write(simu.TotalOutput);
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


    public void OpenCoftFile()
    {
      Parser.COFT(Path.Combine(this.ModelDirectory, "COFT"), this);
      Parser.FOFT(Path.Combine(this.ModelDirectory, "FOFT"), this);
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
    


    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }
  }
}
