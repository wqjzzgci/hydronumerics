using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.Nitrate.Model
{
  public class ModelFactory
  {

    public static ISource GetSourceModel(string ModelID)
    {
      ISource NewModel = null;
      switch (ModelID)
      {
        case "Atmospheric":
          NewModel = new AtmosphericDeposition();
          break;
        case "GroundwaterSource":
          NewModel = new GroundWaterSource();
          break;
        case "PointSource":
          NewModel = new PointSource();
          break;
        case "OrganicN":
          NewModel = new OrganicN();
          break;
      }
      return NewModel;
    }

    public static ISink GetSinkModel(string ModelID)
    {
      ISink NewModel = null;
      switch (ModelID)
      {
        case "InternalLake":
          NewModel = new InternalLakeReduction();
          break;
        case "StreamReduction":
          NewModel = new StreamReduction();
          break;
      }
      return NewModel;
    }
  }
}
