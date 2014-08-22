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
        case "SmallLakesSink":
          NewModel = new SmallLakesSink();
          break;
        case "StreamSink":
          NewModel = new StreamSink();
          break;
        case "LakeSink":
          NewModel = new LakeSink();
          break;
        case "ConstructedWetland":
          NewModel = new ConstructedWetlandSink();
          break;
        case "ConceptualSourceReducer":
          NewModel = new ConceptualSourceReducer();
          break;
      }
      return NewModel;
    }
  }
}
