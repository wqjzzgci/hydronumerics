using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

using HydroNumerics.Core.WPF;

using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.View3d
{
  public class Sample: NotifyPropertyChangedBase
  {
    public Point3D point { get; set; }
    public string Compound { get; set; }
    public double Value { get; set; }
    public DateTime SampleTime { get; set; }
    public string Compartment { get; set; }
  }
}
