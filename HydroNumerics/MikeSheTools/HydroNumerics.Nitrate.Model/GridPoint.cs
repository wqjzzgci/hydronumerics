using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;

namespace HydroNumerics.Nitrate.Model
{
  public class GridPoint:BaseViewModel
  {

    public int GridID { get;  set; }
    public int DMIGridID { get; set; }
    public int LandUseID { get; set; }
    public int SoilID { get; set; }
    public int DKModelID { get; set; }

    public int GridX { get; set; }
    public int GridY { get; set; }


    public XYPoint Point { get; set; }

  }
}
