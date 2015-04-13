using System;
namespace HydroNumerics.Core.Time
{
  public interface ITimeStampValue
  {
    DateTime Time { get; set; }
    double Value { get; set; }
  }
}
