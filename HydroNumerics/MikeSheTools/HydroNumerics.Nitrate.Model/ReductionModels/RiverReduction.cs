using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Data;

using HydroNumerics.Core;


namespace HydroNumerics.Nitrate.Model
{
  public class RiverReduction : BaseViewModel, IReductionModel
  {


      public enum StreamWidth
      {
          /// <summary>
          /// River with widths in interval [0m, 2.5m[
          /// </summary>
          Narrow,              
          /// <summary>
          /// River with widths in interval [2.5m, 12.0m[
          /// </summary>
          Itermediate,         
          /// <summary>
          /// River with widths in interval [12m, *[
          /// </summary>
          Large                
      }

      public enum Season
      {
          /// <summary>
          /// Apr, May, Jun, Jul, Aug, Sep or Oct
          /// </summary>
          Summer,
          /// <summary>
          /// Nov, Dec, Jan, Feb, Mar
          /// </summary>
          Winter
      }

      /// <summary>
      /// Corresponding values of stream width catagory (Narrow, Intermediate og Large) (Defined by enum: RiverReduction.StreamWidth) and average velocity in the streams in the summer period
      /// </summary>
      public Dictionary<RiverReduction.StreamWidth, double> SummerVelocities;
      /// <summary>
      /// Corresponding values of stream width catagory (Narrow, Intermediate og Large) (Defined by enum: RiverReduction.StreamWidth) and average velocity in the streams in the Winter period
      /// </summary>
      public Dictionary<RiverReduction.StreamWidth, double> WinterVelocities;
      /// <summary>
      /// Corresponding values of stream width catagory (Narrow, Intermediate og Large) (Defined by enum: RiverReduction.StreamWidth) and average water depth in the streams in the summer period
      /// </summary>
      public Dictionary<RiverReduction.StreamWidth, double> SummerDepths;
      /// <summary>
      /// Corresponding values of stream width catagory (Narrow, Intermediate og Large) (Defined by enum: RiverReduction.StreamWidth) and average water depth in the streams in the Winter period
      /// </summary>
      public Dictionary<RiverReduction.StreamWidth, double> WinterDepths;
      /// <summary>
      /// The accumulated length of rivers in the catchment is multiplied by the StreamLenghtFactor.
      /// The reason for having this factor is that water from the catchment enters the streams over the total length of the streams,
      /// but water entering the streams near the outlet from the catchment will travel shorter than water entering at the top of the streams.
      /// The StreamLength factor will compensate for this. 
      /// </summary>
      public double StreamLengthFactor { get; set; } //The reach length is estimated to be 0.25 multiplied by the total length of the stream i the catchment polygon.
      /// <summary>
      /// The factor used in the reduction equation.
      /// </summary>
      public double ReductionEquationFactor { get; set; }
      /// <summary>
      /// The power used in the reduction equation
      /// </summary>
      public double ReductionEquationPower { get; set; }

     
      public RiverReduction()
      {
          SummerVelocities = new Dictionary<StreamWidth, double>();
          WinterVelocities = new Dictionary<StreamWidth, double>();
          SummerDepths = new Dictionary<StreamWidth, double>();
          WinterDepths = new Dictionary<StreamWidth, double>();

      }

    public RiverReduction(XElement configuration) : this()
    {
      Name = configuration.Attribute("Type").Value;
      Update = bool.Parse(configuration.Element("Update").Value);
      StreamLengthFactor = double.Parse(configuration.Element("StreamLengthFactor").Value);
      ReductionEquationFactor = double.Parse(configuration.Element("ReductionEquation").Attribute("Factor").Value);
      ReductionEquationPower = double.Parse(configuration.Element("ReductionEquation").Attribute("Power").Value);

      WinterDepths.Add(RiverReduction.StreamWidth.Narrow, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Narrow.ToString()).Value));
      WinterDepths.Add(RiverReduction.StreamWidth.Itermediate, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Itermediate.ToString()).Value));
      WinterDepths.Add(RiverReduction.StreamWidth.Large, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Large.ToString()).Value));

      SummerDepths.Add(RiverReduction.StreamWidth.Narrow, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Narrow.ToString()).Value));
      SummerDepths.Add(RiverReduction.StreamWidth.Itermediate, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Itermediate.ToString()).Value));
      SummerDepths.Add(RiverReduction.StreamWidth.Large, double.Parse(configuration.Elements("Depths").Elements("Depth").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Large.ToString()).Value));

      WinterVelocities.Add(RiverReduction.StreamWidth.Narrow, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Narrow.ToString()).Value));
      WinterVelocities.Add(RiverReduction.StreamWidth.Itermediate, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Itermediate.ToString()).Value));
      WinterVelocities.Add(RiverReduction.StreamWidth.Large, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Winter.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Large.ToString()).Value));

      SummerVelocities.Add(RiverReduction.StreamWidth.Narrow, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Narrow.ToString()).Value));
      SummerVelocities.Add(RiverReduction.StreamWidth.Itermediate, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Itermediate.ToString()).Value));
      SummerVelocities.Add(RiverReduction.StreamWidth.Large, double.Parse(configuration.Elements("Velocities").Elements("Velocity").First(var => var.Attribute("Season").Value == Season.Summer.ToString() && var.Attribute("StreamWidth").Value == StreamWidth.Large.ToString()).Value));

    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {

        Season season = GetSeason(CurrentTime);

        double reduction = 0;
        //TODO: Spørgsmål: Skal reduktionen beregnes som vist nedenfor eller skal der tages en fraktion af den nitratmængder der er tilbage efter hver reduktion
        //F.eks. Skal det små vandløb reduceres først, dernæst opdatere CurrentMass, så reducere mellem størrelse vandløbene, osv ??

        reduction += CurrentMass * GetReductionFactor(StreamWidth.Narrow, season, c);
        reduction += CurrentMass * GetReductionFactor(StreamWidth.Itermediate, season, c);
        reduction += CurrentMass * GetReductionFactor(StreamWidth.Large, season, c);

        if (reduction > CurrentMass)
        {
            reduction = CurrentMass;
        }

        return reduction;
    }

    public double GetReductionFactor(StreamWidth streamWidth, Season season, Catchment catchment)
    {
        double travelTime = StreamLengthFactor * GetStreamLength(streamWidth, catchment) / GetVelocity(streamWidth, season);
        double streamDepth = GetStreamDepth(streamWidth, season);
        return  ReductionEquationFactor * Math.Pow(streamDepth / (travelTime / (3600.0 * 24 * 365)), ReductionEquationPower);
    }

    public double GetStreamDepth(StreamWidth streamWidth, Season season)
    {
        if (season == Season.Winter)
        {
            return WinterDepths[streamWidth];
        }
        else if (season == Season.Summer)
        {
            return SummerDepths[streamWidth];
        }
        else
        {
            throw new Exception();
        }
    }


    public double GetVelocity(StreamWidth streamWidth, Season season)
    {
        if (season == Season.Winter)
        {
            return WinterVelocities[streamWidth];
        }
        else if (season == Season.Summer)
        {
            return SummerVelocities[streamWidth];
        }
        else
        {
            throw new Exception();
        }

    }

    public double GetStreamLength(StreamWidth streamWidth, Catchment catchment)
    {
        //TODO: her skal der læses fra Shapefilen, eller også skal disse værdier kunne hentes via Catchment objected...
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets the season. Summer defined as Apr, May, Jun, Jul, Aug, Sep, Oct. Winter as the reamining months
    /// </summary>
    /// <param name="dateTime">The date for which the season is requested</param>
    /// <returns>The season (Summer or Winter)</returns>
      public Season GetSeason(DateTime dateTime)
    {
        Season season;
        if (dateTime.Month > 3 && dateTime.Month < 11)
        {
            season = Season.Summer;
        }
        else
        {
            season = Season.Winter;
        }
        return season;
    }


    public bool Update { get; set; }
  }
}
