using System;
using System.Runtime.InteropServices;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;
using DHI.Generic.MikeZero.DFS;

namespace HydroNumerics.MikeSheTools.DFS
{
  public class Item
  {
    internal IntPtr ItemPointer { get; private set; }
    private eumUnit _eumUnit = eumUnit.eumUUnitUndefined;
    private string eumUnitString;
    private string _name;
    private eumItem _eumitem = eumItem.eumIItemUndefined;
    private DFSBase _dfs;
    private DataValueType valueType;


    public int ItemNumber { get; private set; }

    internal Item(IntPtr ItemPointer, DFSBase DFS, int Number)
    {
      ItemNumber = Number;
      _dfs = DFS;
      int item_type = 0;
      int data_type = 0;
      int value_type =0;
      IntPtr name = new IntPtr();
      IntPtr Eum = new IntPtr();

      this.ItemPointer = ItemPointer;
      DfsDLLAccess.dfsGetItemInfo_(ItemPointer, out item_type, ref name, ref Eum, out data_type);
      DfsDLLAccess.dfsGetItemValueType(ItemPointer, out value_type);

      valueType = (DataValueType)value_type;
      _name = (Marshal.PtrToStringAnsi(name));
      eumUnitString = Marshal.PtrToStringAnsi(Eum);
      if(item_type!=0)
        _eumitem = (eumItem)item_type;
    }

    /// <summary>
    /// Gets and sets the name
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        if (value != _name)
        {
          _name = value;
          _dfs.WriteItemInfo(this);
        }
      }
    }

    /// <summary>
    /// Gets and sets the Eum Item
    /// </summary>
    public eumItem EumItem 
    {
      get
      {
        return _eumitem;
      }
      set
      {
        if (_eumitem != value)
        {
          _eumitem = value;
          _eumUnit =  PossibleUnits.First();
          _dfs.WriteItemInfo(this);
        }
      }
    }

    /// <summary>
    /// Gets a list of units possible for the selected EUM Item
    /// </summary>
    public eumUnit[] PossibleUnits
    {
      get
      {
        return EUMWrapper.GetItemAllowedUnits(EumItem); 
      }
    }

    /// <summary>
    /// Gets the eum quantity
    /// </summary>
    public eumQuantity EumQuantity
    {
      get
      {
        return new eumQuantity(EumItem, EumUnit);
      }
    }

    /// <summary>
    /// Gets and sets the eum unit.
    /// Note that possible units depend on the EUMItem. Setting to an impossible unit will set the unit to first possible type
    /// </summary>
    public eumUnit EumUnit
    {
      get
      {
        if (_eumUnit == eumUnit.eumUUnitUndefined)
        {
          int u = 0;
          bool w = EUMWrapper.GetUnitTag(this.eumUnitString, out u); //This call is very expensive.
          _eumUnit = (eumUnit)u;
        }
        return _eumUnit;
      }
      set
      {
        if (!PossibleUnits.Contains(value))
          _eumUnit = PossibleUnits.First();
        _eumUnit = value;
        _dfs.WriteItemInfo(this);
      }
    }

    /// <summary>
    /// Gets and sets the data value type
    /// </summary>
    public DataValueType ValueType
    {
      get
      {
        return valueType;
      }
      set
      {
        if (valueType != value)
        {
          valueType = value;
          DfsDLLAccess.dfsSetItemValueType(ItemPointer, (int)valueType);
        }
      }
    }


    public override string ToString()
    {
      return Name;
    }

  }
}
