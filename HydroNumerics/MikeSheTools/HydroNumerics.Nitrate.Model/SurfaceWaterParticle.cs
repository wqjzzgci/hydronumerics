using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;

namespace HydroNumerics.Nitrate.Model
{
  public class SurfaceWaterParticle:BaseViewModel
  {
    public double CurrentMass { get; set; }

    public Dictionary<ReductionProcess, double> MassHistory { get; set; }
    public List<ReductionProcess> History { get; set; }

    public void Reduce(ReductionProcess Rp, double NewMass)
    {
      History.Add(Rp);
      MassHistory.Add(Rp, NewMass);
      CurrentMass = NewMass;
    }
  }
}
