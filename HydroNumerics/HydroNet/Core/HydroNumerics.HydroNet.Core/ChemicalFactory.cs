using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public enum Chemicals
  {
    Na=0,
    Cl
  }

  public class ChemicalFactory
  {
    private static Chemical[] _chemicals;

    private static void Initialize()
    {
      string[] Names= Enum.GetNames(typeof(Chemicals));
      _chemicals = new Chemical[Names.Count()];
      _chemicals[0] = new Chemical(Names[0], 32);
      _chemicals[1] = new Chemical(Names[1], 13);
    }

    public static Chemical GetChemical(Chemicals ChemicalName)
    {
      if (_chemicals == null)
        Initialize();

      return _chemicals[(int)ChemicalName];
    }
  }
}
