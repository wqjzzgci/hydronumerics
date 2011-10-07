using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Wells;

namespace HydroNumerics.MikeSheTools.LayerStatistics
{
  public class LsIntake:Intake
  {

    public LsIntake(IWell well, int IntakeNumber):base()
    {
      this.well = well;
      this.IDNumber = IntakeNumber;
    }

    public List<Observation> Observations = new List<Observation>();
  }
}
