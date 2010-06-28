using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.Time.Core;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.Time.OpenMI
{
    public class LinkableTimeSeriesGroup : LinkableComponent
    {
        TimeSeriesGroup tsGroup;

        public override void Initialize(IArgument[] properties)
        {
            string filename = properties[0].Value;
            tsGroup = TimeSeriesGroupFactory.Create(filename);

            foreach (BaseTimeSeries baseTimeSeries in tsGroup.Items)
            {
                HydroNumerics.OpenMI.Sdk.Backbone.Dimension dimention = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
                dimention.AmountOfSubstance = baseTimeSeries.Unit.Dimension.AmountOfSubstance;
                dimention.Currency = baseTimeSeries.Unit.Dimension.Currency;
                dimention.ElectricCurrent = baseTimeSeries.Unit.Dimension.ElectricCurrent;
                dimention.Length = baseTimeSeries.Unit.Dimension.Length;
                dimention.LuminousIntensity = baseTimeSeries.Unit.Dimension.LuminousIntensity;
                dimention.Mass = baseTimeSeries.Unit.Dimension.Mass;
                dimention.Time = baseTimeSeries.Unit.Dimension.Time;

                HydroNumerics.OpenMI.Sdk.Backbone.Unit unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit();
                unit.ID = baseTimeSeries.Unit.ID;
                unit.Description = baseTimeSeries.Description;
                unit.ConversionFactorToSI = baseTimeSeries.Unit.ConversionFactorToSI;
                unit.OffSetToSI = baseTimeSeries.Unit.OffSetToSI;

                Quantity quantity = new Quantity();
                quantity.ID = baseTimeSeries.Name;
                quantity.Description = baseTimeSeries.Description;
                quantity.Dimension = dimention;
                quantity.Unit = unit;

                ElementSet elementSet = new ElementSet();
                elementSet.ID = "IDBased";
                elementSet.Description = "IDBased";
                elementSet.ElementType = global::OpenMI.Standard.ElementType.IDBased;
                elementSet.SpatialReference = new SpatialReference("Undefined");
                Element element = new Element();
                element.ID = "IDBased";
                elementSet.AddElement(element);

                OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
                outputExchangeItem.Quantity = quantity;
                outputExchangeItem.ElementSet = elementSet;
                this.AddOutputExchangeItem(outputExchangeItem);
            }
        }

        public override string ComponentID
        {
            get { return "HydroNumerics.Time.TimeSeriesGroup"; }
        }
        
        public override string ComponentDescription
        {
            get { return "HydroNumerics timeseries group"; }
        }

        public override string ModelID
        {
            get { return tsGroup.Name; }
        }

        public override string ModelDescription
        {
            get { return "   "; }
        }

        public override ITimeSpan TimeHorizon
        {
           
            get 
            {
                DateTime startTime = DateTime.MinValue;
                DateTime endTime = DateTime.MaxValue;

                foreach (BaseTimeSeries ts in tsGroup.Items)
                {
                    if (ts is TimestampSeries)
                    {
                        if (((TimestampSeries)ts).Items[0].Time > startTime)
                        {
                            startTime = ((TimestampSeries)ts).Items[0].Time;
                        }
                        if (((TimestampSeries)ts).Items.Last().Time < endTime)
                        {
                            endTime = ((TimestampSeries)ts).Items.Last().Time;
                        }
                    }
                    else
                    {
                        if (((TimespanSeries)ts).Items[0].StartTime > startTime)
                        {
                            startTime = ((TimespanSeries)ts).Items[0].StartTime;
                        }

                        if (((TimespanSeries)ts).Items.Last().EndTime < endTime)
                        {
                            endTime = ((TimespanSeries)ts).Items.Last().EndTime;
                        }
                    }
                   
                }
                return new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(startTime), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(endTime));
            }
        }
        
        public override void Prepare()
        {
            //nothing is needed here
        }

         public override ITimeStamp EarliestInputTime
        {
            get { return TimeHorizon.Start; }
        }

        public override IValueSet GetValues(ITime time, string LinkID)
        {
            ILink link = this.GetLink(LinkID);
            HydroNumerics.Core.Unit toUnit = new HydroNumerics.Core.Unit();
            toUnit.ConversionFactorToSI = link.TargetQuantity.Unit.ConversionFactorToSI;
            toUnit.OffSetToSI = link.TargetQuantity.Unit.OffSetToSI;

            if (time is ITimeStamp)
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp t = new TimeStamp((ITimeStamp)time);

                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(t.ToDateTime(), toUnit);
                ScalarSet scalarSet = new ScalarSet(new double[] { x });
                return scalarSet;
            }
            else
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp start = new TimeStamp(((ITimeSpan)time).Start);
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp end = new TimeStamp(((ITimeSpan)time).End);

                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(start.ToDateTime(), end.ToDateTime(), toUnit);
                ScalarSet scalarSet = new ScalarSet(new double[] { x });
                return scalarSet;
            }
        }

        public override EventType GetPublishedEventType(int providedEventTypeIndex)
        {
            throw new NotImplementedException();
        }

        public override int GetPublishedEventTypeCount()
        {
            return 0;
        }

        public override string Validate()
        {
            return "";
        }

        public override void Finish()
        {
            //no implementation is needed here.
        }
    }
}
