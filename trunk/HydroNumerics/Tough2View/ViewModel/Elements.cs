using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HydroNumerics.Tough2.ViewModel
{
  public class ElementCollection:KeyedCollection<string,Element>
  {
        // The parameterless constructor of the base class creates a 
    // KeyedCollection with an internal dictionary. For this code 
    // example, no other constructors are exposed.
    //
    public ElementCollection() : base() { }

    // This is the only method that absolutely must be overridden,
    // because without it the KeyedCollection cannot extract the
    // keys from the items. The input parameter type is the 
    // second generic type argument, in this case OrderItem, and 
    // the return value type is the first generic type argument,
    // in this case int.
    //
    protected override string GetKeyForItem(Element item)
    {
        // In this example, the key is the part number.
        return item.Name;
    }

  }
}
