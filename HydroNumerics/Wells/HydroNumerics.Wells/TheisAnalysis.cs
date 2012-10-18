using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry;

namespace HydroNumerics.Wells
{
  public class TheisAnalysis
  {

    /// <summary>
    /// Calculates the theis drawdown. Pumpingrate is positive for injection. Pumpingrate and transmissivity time units should be seconds
    /// </summary>
    /// <param name="PumpingWell"></param>
    /// <param name="PumpingRate"></param>
    /// <param name="Storativity"></param>
    /// <param name="Transmissivity"></param>
    /// <param name="Time"></param>
    /// <param name="ObservationPoint"></param>
    /// <returns></returns>
    public double Drawdown(IXYPoint PumpingWell, double PumpingRate, double Storativity, double Transmissivity, TimeSpan Time, IXYPoint ObservationPoint)
    {
      double r = XYGeometryTools.CalculatePointToPointDistance(PumpingWell, ObservationPoint);
      return (PumpingRate / (4 * Math.PI * Transmissivity)) * W(r, Storativity, Transmissivity, Time.TotalSeconds);
    }



    public double W(double radius, double Storativity, double Transmissivity, double Time)
    {
      return W(Math.Pow(radius, 2) * Storativity / (4 * Transmissivity * Time));
    }


    public double W(double U)
    {
      return -0.577216 - Math.Log(U) + U - SeriesElements(U, 2) + SeriesElements(U, 3) - SeriesElements(U, 4);// +SeriesElements(U, 5) - SeriesElements(U, 6);
    }

    private double SeriesElements(double U, int exponent)
    {

      return Math.Pow(U, exponent) /(double) (exponent * fakultet(exponent));
    }

    private int fakultet(int n)
    {
      if (n == 0)
      {
        return 1;
      }
      else
      {
        return n * fakultet(n - 1);
      }
    }




  }
}
