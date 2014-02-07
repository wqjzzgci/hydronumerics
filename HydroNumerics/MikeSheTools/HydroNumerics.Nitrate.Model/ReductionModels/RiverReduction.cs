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
          Winter
      }
      
      // -- global parameters ---
      //Parameter narrowStreamSummerDepth;
      XElement Parameters;
      double narrowStreamSummerDepth;
      
      
      // --- pr catchment ---
      double riverLengthRatio = 0.25; //The reach length is estimated to be 0.25 multiplied by the total length of the stream i the catchment polygon.

      XElement configuration;


    public RiverReduction(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);


      this.configuration = Configuration;

    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {


        Season season;
        if (CurrentTime.Month > 3 && CurrentTime.Month < 11)
        {
            season = Season.Summer;
        }
        else
        {
            season = Season.Winter;
        }

        double reduction = 0;


        reduction += CurrentMass * GetReductionFactor(StreamWidth.Narrow, season, c);
        reduction += CurrentMass * GetReductionFactor(StreamWidth.Itermediate, season, c);
        reduction += CurrentMass * GetReductionFactor(StreamWidth.Large, season, c);

        if (reduction > CurrentMass)
        {
            reduction = CurrentMass;
        }
        


        return reduction;
    }

    private double GetReductionFactor(StreamWidth streamWidth, Season season, Catchment catchment)
    {
        
        double travelTime = riverLengthRatio * GetStreamLength(streamWidth, catchment) / GetVelocity(streamWidth, season);
        double streamDepth = GetStreamDepth(streamWidth, season);
        return  74.61 * Math.Pow(streamDepth / (travelTime / (3600.0 * 24 * 365)), -0.344);
    }

    public double GetStreamDepth(StreamWidth streamWidth, Season season)
    {
       return double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == season.ToString() && var.Attribute("StreamWidth").Value == streamWidth.ToString()).Value);
    }

    public double GetVelocity(StreamWidth streamWidth, Season season)
    {
        return double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == season.ToString() && var.Attribute("StreamWidth").Value == streamWidth.ToString()).Value);
    }

    public double GetStreamLength(StreamWidth streamWidth, Catchment catchment)
    {
        //TODO: her skal der læses fra Shapefilen.
        throw new NotImplementedException();
    }


    public bool Update { get; set; }
  }
}
