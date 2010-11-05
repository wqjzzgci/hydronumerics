using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public enum ChemicalNames
  {
    Na = 0,
    Cl,
    IsotopeFraction,
    Radon
  }


  public sealed class ChemicalFactory
  {
    private static volatile ChemicalFactory instance;
    private static object syncRoot = new Object();
    private List<Chemical> _chemicals = new List<Chemical>();

    private ChemicalFactory()
    {
      Initialize();
      Chemicals = new Collection<Chemical>(_chemicals);
    }

    public static ChemicalFactory Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new ChemicalFactory();
          }
        }
        return instance;
      }
    }

    /// <summary>
    /// Gets the chemicals
    /// </summary>
    public Collection<Chemical> Chemicals { get; private set; }


    /// <summary>
    /// Reads in more chemicals from a file
    /// </summary>
    /// <param name="FileName"></param>
    public void ReadFile(string FileName)
    {
      using (FileStream fs = new FileStream(FileName, FileMode.Open))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(List<Chemical>));
        _chemicals.AddRange((List<Chemical>)ds.ReadObject(fs));
      }
    }

    /// <summary>
    /// Initializes the hard coded chemicals
    /// </summary>
    private void Initialize()
    {
      string[] Names = Enum.GetNames(typeof(ChemicalNames));
      _chemicals.Insert(0, new Chemical(Names[0], 32));
      _chemicals.Insert(1, new Chemical(Names[1], 13));
      _chemicals.Insert(2, new Chemical(Names[2], 1));
      _chemicals.Insert(3, new Chemical(Names[3], 1));
      _chemicals[3].IsVolatile = true;
    }

    /// <summary>
    /// Gets hard coded chemicals. Only used for testing
    /// </summary>
    /// <param name="ChemicalName"></param>
    /// <returns></returns>
    public Chemical GetChemical(ChemicalNames ChemicalName)
    {
      if (_chemicals == null)
        Initialize();

      return _chemicals[(int)ChemicalName];
    }
  }
}
