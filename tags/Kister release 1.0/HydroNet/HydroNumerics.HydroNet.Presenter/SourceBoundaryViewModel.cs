using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class SourceBoundaryViewModel:IDObjectViewModel
  {
    public string BoundaryType { get; private set; }

    public SourceBoundaryViewModel(AbstractBoundary Boundary, string boundaryType)
      : base(Boundary)
    {
      BoundaryType = boundaryType;
    }



  }
}
