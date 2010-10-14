using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

namespace HydroNumerics.Core
{
  public enum NamedUnits
  {
    meter = 0,
    cubicmeter,
    cubicmeterpersecond
  }


  public sealed class UnitFactory
  {
    private static volatile UnitFactory instance;
    private static object syncRoot = new Object();
    private List<Unit> _units = new List<Unit>();

    private UnitFactory()
    {
      Initialize();
      Units = new Collection<Unit>(_units);
    }

    public static UnitFactory Instance
    {
      get
      {
        if (instance == null)
        {
          lock (syncRoot)
          {
            if (instance == null)
              instance = new UnitFactory();
          }
        }
        return instance;
      }
    }

    /// <summary>
    /// Gets the chemicals
    /// </summary>
    public Collection<Unit> Units { get; private set; }


    /// <summary>
    /// Reads in more units from a file
    /// </summary>
    /// <param name="FileName"></param>
    public void ReadFile(string FileName)
    {
      using (FileStream fs = new FileStream(FileName, FileMode.Open))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(List<Unit>));
        _units.AddRange((List<Unit>)ds.ReadObject(fs));
      }
    }

    /// <summary>
    /// Initializes the hard coded chemicals
    /// </summary>
    private void Initialize()
    {
      string[] Names = Enum.GetNames(typeof(NamedUnits));
      _units.Insert(0, new Unit("m", 1, 0, Names[0], new Dimension(1, 0, 0, 0, 0, 0, 0, 0)));
      _units.Insert(0, new Unit("m3", 1, 0, Names[1], new Dimension(3, 0, 0, 0, 0, 0, 0, 0)));
      _units.Insert(0, new Unit("m3/s", 1, 0, Names[2], new Dimension(3, 0, -1, 0, 0, 0, 0, 0)));
    }

    /// <summary>
    /// Gets hard coded chemicals. Only used for testing
    /// </summary>
    /// <param name="ChemicalName"></param>
    /// <returns></returns>
    public Unit GetUnit(NamedUnits Name)
    {
      if (_units == null)
        Initialize();

      return _units[(int)Name];
    }
  }
}
