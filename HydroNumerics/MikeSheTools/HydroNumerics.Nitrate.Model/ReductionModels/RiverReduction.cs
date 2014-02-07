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

      public double StreamLengthFactor { get; private set; } //The reach length is estimated to be 0.25 multiplied by the total length of the stream i the catchment polygon.
      public double ReductionEquationFactor { get; private set; }
      public double ReductionEquationPower { get; private set; }

      XElement configuration;


    public RiverReduction(XElement Configuration)
    {
      Name = Configuration.Attribute("Type").Value;
      Update = bool.Parse(Configuration.Element("Update").Value);
      StreamLengthFactor = double.Parse(Configuration.Element("StreamLengthFactor").Value);
      ReductionEquationFactor = double.Parse(Configuration.Element("ReductionEquation").Attribute("Factor").Value);
      ReductionEquationPower = double.Parse(Configuration.Element("ReductionEquation").Attribute("Power").Value);

      this.configuration = Configuration;

    }


    public double GetReduction(Catchment c, double CurrentMass, DateTime CurrentTime)
    {

        Season season = GetSeason(CurrentTime);

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
        double travelTime = StreamLengthFactor * GetStreamLength(streamWidth, catchment) / GetVelocity(streamWidth, season);
        double streamDepth = GetStreamDepth(streamWidth, season);
        return  ReductionEquationFactor * Math.Pow(streamDepth / (travelTime / (3600.0 * 24 * 365)), ReductionEquationPower);
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
