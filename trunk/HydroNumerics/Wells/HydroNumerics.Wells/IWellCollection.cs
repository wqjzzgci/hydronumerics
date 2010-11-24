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

  }
}
