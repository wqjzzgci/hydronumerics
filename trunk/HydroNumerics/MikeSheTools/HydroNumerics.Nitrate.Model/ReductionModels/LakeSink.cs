using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;


namespace HydroNumerics.Nitrate.Model
{
  public class LakeSink : BaseModel, ISink
  {

    public LakeSink()
    {

    }



    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
      return CurrentMass * 0.1; ;
    }


    public void Initialize(DateTime Start, DateTime End, IEnumerable<Catchment> Catchments)
    {
      throw new NotImplementedException();
    }
  }
}
