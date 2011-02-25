using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class CellViewModel
  {
    public double Top { get; set; }
    public double Bottom { get; set; }
    public int DFSLayerNumber { get; set; }
    public int MsheLayerNumber { get; set; }
    public double HydraulicConductivity { get; set; }
  }
}
