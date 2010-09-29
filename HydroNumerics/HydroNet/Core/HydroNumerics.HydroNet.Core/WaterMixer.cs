using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace HydroNumerics.HydroNet.Core
{
  public class WaterMixer
  {


    /// <summary>
    /// Adds the WaterPackets and returns them as the most advanced type.
    /// </summary>
    /// <param name="Waters"></param>
    /// <returns></returns>
    public static IWaterPacket Mix(IEnumerable<IWaterPacket> Waters)
    {
      if (Waters.Count() == 0)
        return null;
      else if (Waters.Count()==1)
        return Waters.First();
      else
      {
      IWaterPacket[] warr = Waters.ToArray();
      IWaterPacket Water1 = warr[0];

      for (int i = 1; i < warr.Count(); i++)
      {
        if (warr[i].GetType().IsSubclassOf(Water1.GetType()))
        {
          warr[i].Add(Water1);
          Water1 = warr[i];
        }
        else
          Water1.Add(warr[i]);
      }

      return Water1;
      }
    }
  }
}
