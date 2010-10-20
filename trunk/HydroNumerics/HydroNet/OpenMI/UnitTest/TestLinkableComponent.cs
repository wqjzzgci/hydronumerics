using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    /// <summary>
    /// asdf
    /// </summary>
    public class TestLinkableComponent : HydroNumerics.OpenMI.Sdk.Wrapper.IEngine
    {
        List<InputExchangeItem> inputExchangeItems;
        List<OutputExchangeItem> outputExchangeItems;
        HydroNumerics.OpenMI.Sdk.Backbone.RegularGrid grid;
        TimeSeriesGroup groundwaterHeads; //output
        TimeSeriesGroup infiltrations; //input
        int currentTimestep;

        #region IEngine Members

        public void Initialize(System.Collections.Hashtable properties)
        {
            currentTimestep = 0;
            DateTime simulationStart = new DateTime(2010,1,1);
            int dt = 3600; //timestep length
            int nt = 100; // number of timesteps
            int nx = 2; //number of grid cells in x-direction
            int ny = 2; //number of grid cells in y-direction
            grid = new RegularGrid(10, 10, 20, nx, ny, 0);
            groundwaterHeads = new TimeSeriesGroup();
            infiltrations = new TimeSeriesGroup();
            

            for (int i = 0; i < nx * ny; i++)
            {
                groundwaterHeads.Items.Add(new TimestampSeries("GwHead" + i.ToString(), simulationStart, nt, dt, TimestepUnit.Seconds, 2.34, new HydroNumerics.Core.Unit("meters",1.0,0.0)));
                infiltrations.Items.Add(new TimestampSeries("Infiltration" + i.ToString(), simulationStart, nt, dt, TimestepUnit.Seconds, 2.34, new HydroNumerics.Core.Unit("flow",1.0,0.0)));
            }

            Quantity infiltQuantity = new Quantity();
            infiltQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("level", 1.0,0.0); 
            infiltQuantity.ID = "Infiltration";
            infiltQuantity.Description = "infiltration";
            infiltQuantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
            InputExchangeItem infiltExItem = new InputExchangeItem();
            infiltExItem.ElementSet = grid;
            inputExchangeItems.Add(infiltExItem);
           
            Quantity headQuantity = new Quantity();
            headQuantity.Description = "Groundwater head";
            headQuantity.ID = "Head";
            headQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("meters",1,0);
            headQuantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
            OutputExchangeItem headOutItem = new OutputExchangeItem();
            headOutItem.Quantity = headQuantity;
            headOutItem.ElementSet = grid;
            outputExchangeItems.Add(headOutItem);
        }
        
        public InputExchangeItem GetInputExchangeItem(int exchangeItemIndex)
        {
            return inputExchangeItems[exchangeItemIndex];
        }

        public int GetInputExchangeItemCount()
        {
            return inputExchangeItems.Count;
        }

        public string GetModelDescription()
        {
            return "DummyGroundwaterModel";
        }

        public string GetModelID()
        {
            return "DGW";
        }

        public OutputExchangeItem GetOutputExchangeItem(int exchangeItemIndex)
        {
            return outputExchangeItems[exchangeItemIndex];
        }

        public int GetOutputExchangeItemCount()
        {
            return outputExchangeItems.Count;
        }

        public global::OpenMI.Standard.ITimeSpan GetTimeHorizon()
        {
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(DateTime.MinValue), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(DateTime.MaxValue));
        }

        #endregion

        #region IRunEngine Members

        public void Dispose()
        {
           
        }

        public void Finish()
        {
           
        }

        public string GetComponentDescription()
        {
            return "Dummy groundwatermodel";
        }

        public string GetComponentID()
        {
            return "DGWM";
        }

        public global::OpenMI.Standard.ITime GetCurrentTime()
        {
            throw new NotImplementedException();
        }

        public global::OpenMI.Standard.ITimeStamp GetEarliestNeededTime()
        {
            throw new NotImplementedException();
        }

        public global::OpenMI.Standard.ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            throw new NotImplementedException();
        }

        public double GetMissingValueDefinition()
        {
            throw new NotImplementedException();
        }

        public global::OpenMI.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            throw new NotImplementedException();
        }

       

        public bool PerformTimeStep()
        {
            throw new NotImplementedException();
        }

        public void SetValues(string quantityID, string elementSetID, global::OpenMI.Standard.IValueSet values)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
