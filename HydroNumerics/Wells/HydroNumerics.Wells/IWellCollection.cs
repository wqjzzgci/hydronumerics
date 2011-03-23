using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.Wells
{
  public class IWellCollection:KeyedCollection<string,IWell>
  {
    public IWellCollection() : base() { }

    protected override string GetKeyForItem(IWell item)
    {
      return item.ID;


      
    }

    public bool TryGetValue(string ID, out IWell well)
    {
      if (this.Contains(ID))
      {
        well = this[ID];
        return true;
      }
      else
      {
        well = null;
        return false;
      }
    }


  }
}
