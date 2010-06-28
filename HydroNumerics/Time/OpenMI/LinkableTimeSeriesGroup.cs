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

        public override string ComponentDescription
        {
            get { throw new NotImplementedException(); }
        }

        public override string ComponentID
        {
            get { throw new NotImplementedException(); }
        }

        public override string ModelID
        {
            get { throw new NotImplementedException(); }
        }

        public override string ModelDescription
        {
            get { throw new NotImplementedException(); }
        }

        public override ITimeSpan TimeHorizon
        {
            get { throw new NotImplementedException(); }
        }
        
        public override void Prepare()
        {
            throw new NotImplementedException();
        }

         public override ITimeStamp EarliestInputTime
        {
            get { throw new NotImplementedException(); }
        }

        public override IValueSet GetValues(ITime time, string LinkID)
        {
            ILink link = this.GetLink(LinkID);
            //TODO: handle unit conversion also....

            if (time is ITimeStamp)
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp t = new TimeStamp((ITimeStamp)time);
                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(t.ToDateTime());
                ScalarSet scalarSet = new ScalarSet(new double[] { x });
                return scalarSet;
            }
            else
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp start = new TimeStamp(((ITimeSpan)time).Start);
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp end = new TimeStamp(((ITimeSpan)time).End);

                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(start.ToDateTime(), end.ToDateTime());
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
            throw new NotImplementedException();
        }

        public override string Validate()
        {
            throw new NotImplementedException();
        }

       

        

       
       

        public override void Finish()
        {
            throw new NotImplementedException();
        }
    }
}
