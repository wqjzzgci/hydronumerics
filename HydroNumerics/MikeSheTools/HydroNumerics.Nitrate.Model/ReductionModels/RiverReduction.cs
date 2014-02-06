using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

using HydroNumerics.Core;


namespace HydroNumerics.Nitrate.Model
{
  public class RiverReduction : BaseViewModel, IReductionModel
  {


      public enum StreamWidth
      {
          Narrow,               //river with widths in interval [0m, 2.5m[
          Itermediate,          //river with widths in interval [2.5m, 12.0m[
          Large                  //river with widths in interval [12m, *[
      }

      public enum Season
      {
          Summer,
          Vinter
      }
      
      // -- global parameters ---
      //Parameter narrowStreamSummerDepth;
      XElement Parameters;
      double narrowStreamSummerDepth;
      
      
      // --- pr catchment ---
      double riverLengthRatio = 0.25; //The reach length is estimated to be 0.25 multiplied by the total length of the stream i the catchment polygon.

      double StreamlengthNarrowRivers; // The total length of river with widths in interval [0m, 2.5m[ in the catchment 
      double StreamlengthMediumRivers; // The total length of river with widths in interval [2.5m, 12.0m[ in the catchment 
      double StreamlengthWideRivers;   // The total length of river with widths in interval [12m, *[ in the catchment 

      XElement configuration;


    public RiverReduction(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);


      this.configuration = Configuration;

      //narrowStreamSummerDepth = double.Parse(Configuration.Attribute("narrowStreamSummerDepth").Value);


    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {
        
        double depthInStream;
        double travelTime;

        //double reductionFactor = GetReductionFactor(depthInStream, travelTime);

        double reductionFactor = 1;
        return CurrentMass * reductionFactor;
    }

    private double GetReductionFactor(double depthInStream, double travelTime)
    {
        return  74.61 * Math.Pow(depthInStream / (travelTime / (3600.0 * 24 * 365)), -0.344);
    }

    public double GetStreamDepth(StreamWidth streamWidth, Season season)
    {
       return double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == season.ToString() && var.Attribute("StreamWidth").Value == streamWidth.ToString()).Value);
        //double streamDepth;

        //switch (streamType)
        //{
        //    case StreamType.Narrow:
        //        switch (season)
        //        {
        //            case Season.Summer:
        //                streamDepth = 0.17;
        //                break;
        //            case Season.Vinter:
        //                streamDepth = 0.21;
        //                break;
        //            default:
        //                break;
        //        }
        //        break;
        //    case StreamType.MediumWidth:
        //        break;
        //    case StreamType.Wide:
        //        break;
        //    default:
        //        break;
        //}
    }

    public bool Update { get; set; }
  }
}
