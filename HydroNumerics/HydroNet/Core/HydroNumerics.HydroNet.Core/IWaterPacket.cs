using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public interface IWaterPacket
  {
    /// <summary>
    /// Adds a volume of water
    /// </summary>
    /// <param name="Water"></param>
    void Add(IWaterPacket Water);
    /// <summary>
    /// Adds a volume of water as a rate.
    /// </summary>
    /// <param name="Water"></param>
    /// <param name="TS"></param>
    void Add(IWaterPacket Water, TimeSpan TS);
    void Evaporate(double Volume);
    IWaterPacket Substract(double Volume);
    double Volume { get; }
    int IDForComposition { set; }
    //JAG: ToDo jeg tror ikke man bør udstille en dictionary. Nu er det muligt ude fra at ændre på kompositionen
    Dictionary<int, double> Composition { get; }
    TimeSpan RelativeTimeTag { get;}
    TimeSpan WaterAge { get; }
    void MoveInTime(TimeSpan TimeStep);
    void ResetTime();
    void Tag(int ID);

    IWaterPacket DeepClone();
    IWaterPacket DeepClone(double Volume);
  }
}
