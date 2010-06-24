using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;



using HydroNumerics.Core;

namespace HydroNumerics.HydroNet.OpenMI
{
    public class EngineWrapper : HydroNumerics.OpenMI.Sdk.Wrapper.IEngine
    {
        private HydroNumerics.HydroNet.Core.Model model;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.OutputExchangeItem> outputExchangeItems;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.InputExchangeItem> inputExchangeItems;

        string inputFilename;
        System.TimeSpan timestepLength;

        bool finishMethodWasInvoked;

        public EngineWrapper()
        {
            outputExchangeItems = new List<OutputExchangeItem>();
            inputExchangeItems = new List<InputExchangeItem>();
            model = new HydroNumerics.HydroNet.Core.Model();
            finishMethodWasInvoked = false;
        }

        public void Initialize(System.Collections.Hashtable properties)
        {

            if (!properties.Contains("InputFilename"))
            {
                throw new Exception("Missing key \"InputFilename\" in parameter to method HydroNumerics.HydroNet.OpenMI.EngineWrapper.Initialize(...)");
            }

            if (!properties.Contains("TimestepLength"))
            {
                throw new Exception("Missing key \"TimestepLength\" in parameter to method HydroNumerics.HydroNet.OpenMI.EngineWrapper.Initialize(...)");
            }

            inputFilename = (string)properties["InputFilename"];

            double dt = Convert.ToDouble((string)properties["TimestepLength"]);
            timestepLength = System.TimeSpan.FromSeconds(dt);

            model.Open(inputFilename);
            //model.Initialize

            foreach (HydroNumerics.Core.ExchangeItem exchangeItem in model.ExchangeItems)
            {
                HydroNumerics.OpenMI.Sdk.Backbone.Dimension dimention = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
                dimention.AmountOfSubstance = exchangeItem.Unit.Dimension.AmountOfSubstance;
                dimention.Currency = exchangeItem.Unit.Dimension.Currency;
                dimention.ElectricCurrent = exchangeItem.Unit.Dimension.ElectricCurrent;
                dimention.Length = exchangeItem.Unit.Dimension.AmountOfSubstance;
                dimention.LuminousIntensity = exchangeItem.Unit.Dimension.Length;
                dimention.Mass = exchangeItem.Unit.Dimension.LuminousIntensity;
                dimention.AmountOfSubstance = exchangeItem.Unit.Dimension.Mass;
                dimention.Time = exchangeItem.Unit.Dimension.Time;

                HydroNumerics.OpenMI.Sdk.Backbone.Unit unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit();
                unit.ID = exchangeItem.Unit.ID;
                unit.Description = exchangeItem.Unit.Description;
                unit.ConversionFactorToSI = exchangeItem.Unit.ConversionFactorToSI;
                unit.OffSetToSI = unit.OffSetToSI;
                                
                Quantity quantity = new Quantity();
                quantity.ID = exchangeItem.Quantity;
                quantity.Description = exchangeItem.Description;
                quantity.Dimension = dimention;
                quantity.Unit = unit;

                ElementSet elementSet = new ElementSet();
                elementSet.ID = exchangeItem.Location;
                elementSet.Description = "No description";
                elementSet.ElementType = global::OpenMI.Standard.ElementType.IDBased;
                elementSet.SpatialReference = new SpatialReference("Undefined");
                Element element = new Element();
                element.ID = exchangeItem.Location;
                elementSet.AddElement(element);


                if (exchangeItem.IsOutput)
                {
                    OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
                    outputExchangeItem.Quantity = quantity;
                    outputExchangeItem.ElementSet = elementSet;
                    outputExchangeItems.Add(outputExchangeItem);
                }
                if (exchangeItem.IsInput)
                {
                    InputExchangeItem inputExchangeItem = new InputExchangeItem();
                    inputExchangeItem.Quantity = quantity;
                    inputExchangeItem.ElementSet = elementSet;
                    inputExchangeItems.Add(inputExchangeItem);
                }
            }

        }

        public string GetComponentID()
        {
            return "HydroNet";
        }

        public string GetComponentDescription()
        {
            return "HydroNumerics.HydroNet description";
        }

        public string GetModelID()
        {
            return model.Name;
        }

        public string GetModelDescription()
        {
            return "ModelDescription";
            //TODO: ask the HydroNet.Model about this 
        }

        public global::OpenMI.Standard.ITimeSpan GetTimeHorizon()
        {
            HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp startTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(2000, 01, 01));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp endTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(2020, 01, 01));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan timeHorizon = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(startTime, endTime);
            return timeHorizon;
            //TODO: this is a dummy implemtation, to be corrected later....
        }

        public int GetInputExchangeItemCount()
        {
            return inputExchangeItems.Count;
        }

        public int GetOutputExchangeItemCount()
        {
            return outputExchangeItems.Count;
        }

        public HydroNumerics.OpenMI.Sdk.Backbone.OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
        {
            return outputExchangeItems[exchangeItemIndex];
        }

        public HydroNumerics.OpenMI.Sdk.Backbone.InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
        {
            return inputExchangeItems[exchangeItemIndex];
        }

        public global::OpenMI.Standard.ITime GetCurrentTime()
        {
             return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(model.CurrentTime);
        }

        public global::OpenMI.Standard.ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            return GetCurrentTime();
        }

        public global::OpenMI.Standard.ITimeStamp GetEarliestNeededTime()
        {
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(model.CurrentTime);
        }

        public bool PerformTimeStep()
        {
            //TODO: make sure that the state is defined...
            model.MoveInTime(timestepLength);
            return true;
        }

        public void SetValues(string quantityID, string elementSetID, global::OpenMI.Standard.IValueSet values)
        {
            if (values is IScalarSet)
            {
                model.ExchangeItems.Single(var => var.Quantity == quantityID & var.Location == elementSetID).ExchangeValue = ((IScalarSet)values).GetScalar(0);
            }
            else
            {
                throw new Exception("The HydroNet model can only handle IScalarSet - not IVectorSet");
            }
        }

        public global::OpenMI.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            double x = model.ExchangeItems.Single(var => var.Quantity == QuantityID & var.Location == ElementSetID).ExchangeValue;
            ScalarSet scalarSet = new ScalarSet(1, x);
            return scalarSet;
        }

        public double GetMissingValueDefinition()
        {
            return -99999.99;
        }

       
        public void Finish()
        {
            model.Save(inputFilename);
            finishMethodWasInvoked = true;
        }

        public void Dispose()
        {
            if (!finishMethodWasInvoked)
            {
                Finish();
            }
        }

        
    }
}
