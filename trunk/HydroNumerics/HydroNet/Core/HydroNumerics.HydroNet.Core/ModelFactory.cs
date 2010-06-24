using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;

namespace HydroNumerics.HydroNet.Core
{
  public class ModelFactory
  {

    private static List<Type> knownTypes;
    //Used by the serializer. Cannot be serialized it self
    public static List<Type> KnownTypes
    {
      get
      {
        if (knownTypes == null)
        {
          knownTypes = new List<Type>();

          knownTypes.Add(typeof(WaterPacket));
          knownTypes.Add(typeof(WaterWithChemicals));
          knownTypes.Add(typeof(IsotopeWater));
          knownTypes.Add(typeof(AbstractWaterBody));
          knownTypes.Add(typeof(Stream));
          knownTypes.Add(typeof(Lake));
          knownTypes.Add(typeof(EvaporationRateBoundary));
          knownTypes.Add(typeof(FlowBoundary));
          knownTypes.Add(typeof(GroundWaterBoundary));
          knownTypes.Add(typeof(BaseTimeSeries));
          knownTypes.Add(typeof(TimespanSeries));
          knownTypes.Add(typeof(TimestampSeries));

        }
        return knownTypes;
      }
    }

    public static Model GetModel(string FileName)
    {
      using (FileStream Fs = new FileStream(FileName, FileMode.Open))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(Model), KnownTypes, int.MaxValue, false, true, null);
        return (Model)ds.ReadObject(Fs);
      }
    }

    public static void SaveModel(string FileName, Model M)
    {
      using (FileStream Fs = new FileStream(FileName, FileMode.Create))
      {
        DataContractSerializer ds = new DataContractSerializer(typeof(Model), KnownTypes, int.MaxValue, false, true, null);
        ds.WriteObject(Fs, M);
      }

    }
  }
}
