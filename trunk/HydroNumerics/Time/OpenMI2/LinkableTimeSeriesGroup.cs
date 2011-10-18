using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard2;
using OpenMI.Standard2.TimeSpace;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.Time.Core;


namespace HydroNumerics.Time.OpenMI2
{
  public class LinkableTimeSeriesGroup : LinkableComponent
  {
    TimeSeriesGroup timeSeriesGroup;
    string filename;
    string outputFilename;

    

    public override IList<IBaseInput> Inputs { get;  protected set; }

    public override IList<IBaseOutput> Outputs { get; protected set; }

    public override List<IAdaptedOutputFactory> AdaptedOutputFactories { get; protected set; }

    public override void Initialize()
    {

      Dictionary<string, string> argumentsDictionary = new Dictionary<string, string>();

      //TODO: Check that you can assume that e.g. a configuration editor will populate the Argumenst, based on data from the OMI file
      foreach (IArgument argument in this.Arguments)
      {
        argumentsDictionary.Add(argument.Id, argument.ValueAsString);
      }
      filename = argumentsDictionary["Filename"];
      //outputFilename = argumentsDictionary["OutputFilename"];

      timeSeriesGroup = TimeSeriesGroupFactory.Create(filename);

      Outputs = new List<IBaseOutput>();


      foreach (var ts in timeSeriesGroup.Items)
      {
        Dimension dimention = new Dimension();
        dimention.SetPower(DimensionBase.AmountOfSubstance, ts.Unit.Dimension.AmountOfSubstance);
        dimention.SetPower(DimensionBase.Currency, ts.Unit.Dimension.Currency);
        dimention.SetPower(DimensionBase.ElectricCurrent, ts.Unit.Dimension.ElectricCurrent);
        dimention.SetPower(DimensionBase.Length, ts.Unit.Dimension.Length);
        dimention.SetPower(DimensionBase.LuminousIntensity, ts.Unit.Dimension.LuminousIntensity);
        dimention.SetPower(DimensionBase.Mass, ts.Unit.Dimension.Mass);
        dimention.SetPower(DimensionBase.Time, ts.Unit.Dimension.Time);

        Unit unit = new Unit(ts.Unit.ID);
        unit.Description = ts.Unit.Description;
        unit.ConversionFactorToSI = ts.Unit.ConversionFactorToSI;
        unit.OffSetToSI = ts.Unit.OffSetToSI;
        unit.Dimension = dimention;

        Quantity quantity = new Quantity();
        quantity.Caption = ts.Name;
        quantity.Description = ts.Description;
        quantity.Unit = unit;

        ElementSet elementSet = new ElementSet("IDBased");
        elementSet.Description = "IDBased";
        elementSet.ElementType = global::OpenMI.Standard2.TimeSpace.ElementType.IdBased;
        Element element = new Element();
        element.Caption = "IDBased";
        elementSet.AddElement(element);



        Output o = new Output(ts.Name, quantity, elementSet);

        o.TimeSet = new TimeSet();
        o.Values = new HydroNumerics.OpenMI.Sdk.Backbone.Generic.TimeSpaceValueSet<double>();

        if (ts is TimespanSeries)
        {
          foreach (var tsi in ((TimespanSeries)ts).Items)
          {
            o.TimeSet.Times.Add(new HydroNumerics.OpenMI.Sdk.Backbone.Time(ts.StartTime, ts.EndTime));
            o.Values.Values2D.Add(new List<double> { tsi.Value });
          }
        }
        else
        {
          foreach (var tsi in ((TimestampSeries)ts).Items)
          {
            o.TimeSet.Times.Add(new HydroNumerics.OpenMI.Sdk.Backbone.Time(ts.StartTime));
            o.Values.Values2D.Add(new List<double> { tsi.Value });
          }
        }

        Outputs.Add(o);
      }

      Caption = timeSeriesGroup.Name;
      Description = timeSeriesGroup.Name;



    }


    public override string[] Validate()
    {
      return new string[] { }; 
    }

    public override void Prepare()
    {
      
    }

    public override void Update(params IBaseOutput[] requiredOutput)
    {
      
    }

    public override void Finish()
    {
    }
  }
}
