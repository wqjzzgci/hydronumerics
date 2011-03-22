using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.JupiterTools
{
  public class IPlantCollection:KeyedCollection<int,Plant>
  {
    public IPlantCollection() : base() { }

    protected override int GetKeyForItem(Plant item)
    {
      return item.IDNumber;
     
    }

    public bool TryGetValue(int ID, out Plant plant)
    {
      if (this.Contains(ID))
      {
        plant = this[ID];
        return true;
      }
      else
      {
        plant = null;
        return false;
      }
    }
  }
}
