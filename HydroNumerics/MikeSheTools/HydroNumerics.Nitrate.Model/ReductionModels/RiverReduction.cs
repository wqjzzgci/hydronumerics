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
          Narrow,               //river with widths in interval [0m, 2.5m[
          Itermediate,          //river with widths in interval [2.5m, 12.0m[
          Large                 //river with widths in interval [12m, *[
      }

      public enum Season
      {
          Summer,
          Winter
      }

      public Dictionary<RiverReduction.StreamWidth, double> SummerVelocities;
      public Dictionary<RiverReduction.StreamWidth, double> WinterVelocities;
      public Dictionary<RiverReduction.StreamWidth, double> SummerDepths;
      public Dictionary<RiverReduction.StreamWidth, double> WinterDepths;
      

      public double StreamLengthFactor { get; set; } //The reach length is estimated to be 0.25 multiplied by the total length of the stream i the catchment polygon.
      public double ReductionEquationFactor { get; set; }
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
        //TODO: her skal der læses fra Shapefilen.
        throw new NotImplementedException();
    }

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
