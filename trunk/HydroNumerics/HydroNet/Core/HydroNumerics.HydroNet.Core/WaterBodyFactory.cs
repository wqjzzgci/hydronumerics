using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterBodyFactory
  {
    public static int LakeIDCounter=0;
    public static int StreamIDCounter=100000;

    public Stream GetNewStream(IWaterPacket InitialWater)
    {
      Stream S = new Stream(InitialWater);
      S.ID = StreamIDCounter;
      StreamIDCounter++;
      return S;
    }

    public Lake GetNetLake(IWaterPacket InitialWater)
    {
      Lake S = new Lake(InitialWater);
      S.ID = StreamIDCounter;
      StreamIDCounter++;
      return S;
    }

    public Lake GetNetLake(double Volume)
    {
      Lake S = new Lake(Volume);
      S.ID = LakeIDCounter;
      LakeIDCounter++;
      return S;
    }
  }
}
