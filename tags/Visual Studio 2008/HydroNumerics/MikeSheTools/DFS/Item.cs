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
    private eumUnit _eumUnit;
    private string eumUnitString;
    private string _name;
    private eumItem _eumitem;
    private DFSBase _dfs;

    public int ItemNumber { get; private set; }

    internal Item(IntPtr ItemPointer, DFSBase DFS, int Number)
    {
      ItemNumber = Number;
      _dfs = DFS;
      int item_type = 0;
      int data_type = 0;
      IntPtr name = new IntPtr();
      IntPtr Eum = new IntPtr();

      this.ItemPointer = ItemPointer;
      Status = DFSBase.dfsGetItemInfo_(ItemPointer, ref item_type, ref name, ref Eum, ref data_type);
      _name = (Marshal.PtrToStringAnsi(name));
      eumUnitString = Marshal.PtrToStringAnsi(Eum);
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



    private int _status;

    internal int Status
    {
      get { return _status; }
      set {
        _status = value;
        if (_status != 0)
        {
          string error = "fjel";
        }
      }
    }

    public override string ToString()
    {
      return Name;
    }

  }
}
