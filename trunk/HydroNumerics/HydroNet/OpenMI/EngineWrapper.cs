using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HydroNumerics.HydroNet.OpenMI
{
    public class EngineWrapper : HydroNumerics.OpenMI.Sdk.Wrapper.IEngine
    {
        private HydroNumerics.HydroNet.Core.Model model;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.OutputExchangeItem> outputExchangeItems;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.InputExchangeItem> inputExchangeItems;

        string inputFilename;
        string outputFilename;
        double timestepLength; // unit is seconds

        public EngineWrapper()
        {
            outputExchangeItems = new List<HydroNumerics.OpenMI.Sdk.Backbone.OutputExchangeItem>();
            inputExchangeItems = new List<HydroNumerics.OpenMI.Sdk.Backbone.InputExchangeItem>();
            model = new HydroNumerics.HydroNet.Core.Model();
        }


        #region IEngine Members

        public string GetModelID()
        {
            return "ModelID";
            //TODO: ask the HydroNet.Model about this 
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

        #endregion

        #region IRunEngine Members

        public void Initialize(System.Collections.Hashtable properties)
        {
            inputFilename = properties["inputFilename"].ToString();
            outputFilename = properties["outputFilename"].ToString();
            timestepLength = Convert.ToDouble(properties["TimeStepLength"].ToString());

            model.Open(inputFilename);
        }

        public void Finish()
        {
            model.Save(outputFilename);
        }

        public void Dispose()
        {
            //do nothing
        }

        public bool PerformTimeStep()
        {
            int seconds = (int) Math.Truncate(timestepLength);
            int miliseconds = (int)((timestepLength - seconds) * 1000);
            System.TimeSpan timeSpan = new TimeSpan(0, 0, 0, seconds, miliseconds);
            return true;
        }

        public global::OpenMI.Standard.ITime GetCurrentTime()
        {
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime (2000,01, 01));
            //TODO: above is a hack,... to be changed
        }

        public global::OpenMI.Standard.ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(2000, 01, 01));
            //TODO: above is a hack,... to be changed
        }

        public global::OpenMI.Standard.ITimeStamp GetEarliestNeededTime()
        {
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(2000, 01, 01));
            //TODO: above is a hack,... to be changed
        }

        public void SetValues(string quantityID, string elementSetID, global::OpenMI.Standard.IValueSet values)
        {
            throw new NotImplementedException();
        }

        public global::OpenMI.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            throw new NotImplementedException();
        }

        public double GetMissingValueDefinition()
        {
            return -99999.99;
        }

        public string GetComponentID()
        {
            return "HydroNet";
        }

        public string GetComponentDescription()
        {
            return "HydroNumerics.HydroNet description";
        }

        #endregion
    }
}
