using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HydroNumerics.Geometry;


namespace HydroNumerics.HydroNet.ViewModel
{
  public class WFDLake
  {
    public DataRow Data { get; set; }
    public XYPolygon Polygon { get; set; }
    public BitmapImage Image { get; set; }
  }
}
