using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class SourceBoundaryViewModel:IDObjectViewModel
  {
    public string BoundaryType { get; private set; }

    private AbstractBoundary _boundary;

    public SourceBoundaryViewModel(AbstractBoundary Boundary, string boundaryType)
      : base(Boundary)
    {
      BoundaryType = boundaryType;
      _boundary = Boundary;
    }

    public TimestampSeries TimeValues
    {
      get
      {
        SinkSourceBoundary sb = _boundary as SinkSourceBoundary;
        if (sb != null)
        {
          return sb.TimeValues2;
        }
        else
          return null;
      }    
    }



  }
}
