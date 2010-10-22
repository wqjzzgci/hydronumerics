﻿using System;
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

    /// <summary>
    /// Gets and sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets and sets the Eum Item
    /// </summary>
    public eumItem EumItem {get; set;}

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
        if (_eumUnit == null)
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
      }
    }


    internal Item(IntPtr ItemPointer)
    {
      int item_type = 0;
      int data_type = 0;
      IntPtr name = new IntPtr();
      IntPtr Eum = new IntPtr();

      this.ItemPointer = ItemPointer;
      Status = DFSBase.dfsGetItemInfo_(ItemPointer, ref item_type, ref name, ref Eum, ref data_type);
      Name = (Marshal.PtrToStringAnsi(name));
      eumUnitString = Marshal.PtrToStringAnsi(Eum);    
      EumItem = (eumItem)item_type;
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

  }
}
