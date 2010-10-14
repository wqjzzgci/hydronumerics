using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.Misc
{
  public class DBFEntry
  {
    public string name;
    public ShapeLib.DBFFieldType _dbfType;
    public Type _dotNetType;
    public int _index;
    public int _width = 0;
    public int _decimals = 0;
  }
}
