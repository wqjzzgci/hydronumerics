using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MathNet.Numerics.Distributions;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public static class DistributionExtension
  {

    public static IEnumerable<double> LatinHyperCubeSample(this IContinuousDistribution ContinuousDistribution, int divisions)
    {
      for (int i=0;i<divisions;i++)
        yield return ContinuousDistribution.Sample();

    }
  }
}
