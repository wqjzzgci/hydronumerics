using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;

using System.Linq;
using System.Text;

using HydroNumerics.Core;
using HydroNumerics.Core.WPF;

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

  public class Model:BaseViewModel
  {

    public Simulator simu { get; set; }

    
    /// <summary>
    /// Gets the collection of elements
    /// </summary>
    public ElementCollection Elements
    {
      get
      {
        return mesh.Elements;
      }
    }
    /// <summary>
    /// Gets the list of connections
    /// </summary>
    public List<Connection> Connections
    {
      get
      {
        return mesh.Connections;
      }
    }

    public Mesh mesh { get; private set; }



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
        return Path.GetDirectoryName(FileName);
      }
    }

    public Rocks Rocks { get;  set; }

    public OutputFileParser Results { get; private set; }

    string FileName;

    public Model(string InputFileName)
    {
      FileName = InputFileName;
      Results = new OutputFileParser(this);
      simu = new Simulator(this);
      mesh = new Mesh(Path.Combine(ModelDirectory, "mesh"));
      Load();
      Results.ReadOutputFile(Path.Combine(ModelDirectory, "ud.txt"));
    }

    /// <summary>
    /// Loads the mesh
    /// </summary>
    private void Load()
    {
      //now read input file
      using (ReaderUtilities sr = new ReaderUtilities(FileName))
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
          else if (line.StartsWith("TSXCD"))
          {
            var arr = sr.ReadLine().Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            var ints = arr.Select(var => int.Parse(var));

            foreach (var i in ints.Skip(1))
            {
              if (i<=Elements.Count)
                detailedTimeSeries.Add(Elements[i - 1]);
            }
          }
        }
        FileContent = sr.FileContent.ToString();
      }
      NotifyPropertyChanged("FileContent");
      NotifyPropertyChanged("DetailedTimeSeries");
    }


    private List<Element> detailedTimeSeries = new List<Element>();
    private bool detailedTimeSeriesLoaded = false;
    /// <summary>
    /// Gets the list of detailed time series. Only T2VOC. Should be combined with the others.
    /// </summary>
    public List<Element> DetailedTimeSeries
    {
      get
      {
        if (!detailedTimeSeriesLoaded & detailedTimeSeries.Count>0)
        {
          for (int i = 0; i < detailedTimeSeries.Count; i++)
          {
            string file = Path.Combine(ModelDirectory, "TSBRT" + (i + 1).ToString() + ".txt");
            if (File.Exists(file)) 
              detailedTimeSeries[i].DetailedTimeSeries = Parser.TSBRT(file);
          }
          detailedTimeSeriesLoaded = true;
        }
        return detailedTimeSeries;
      }
    }


    private List<TSMassEntry> massBalance;
    public IEnumerable<TSMassEntry> T2VOCMassBalance
    {
      get
      {
        if (massBalance == null)
        {
          massBalance = new List<TSMassEntry>();
          string file = Path.Combine(ModelDirectory, "TSMASS.txt");
            if (File.Exists(file))
              massBalance.AddRange(Parser.TSMASS(file));
            
        }
        return massBalance;
      }
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


    public void SaveMesh()
    {
      using (StreamWriter SW = new StreamWriter(Path.Combine(ModelDirectory, "mesh"), false, Encoding.Default))
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

    public void SaveOutput(string filename)
    {
      StreamWriter SW = new StreamWriter(Path.Combine(ModelDirectory, filename), false, Encoding.Default);
      SW.Write(simu.TotalOutput);
      SW.Dispose();
    }


    public void OpenCoftFile()
    {
      Parser.COFT(Path.Combine(this.ModelDirectory, "COFT"), this);
      Parser.FOFT(Path.Combine(this.ModelDirectory, "FOFT"), this);
    }

    
   
   
    
  }
}
