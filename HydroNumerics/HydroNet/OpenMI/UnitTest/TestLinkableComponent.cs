using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Geometry;
using HydroNumerics.Time.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.OpenMI.Sdk.Wrapper;

namespace HydroNumerics.HydroNet.OpenMI.UnitTest
{
    public class TestLinkableComponent : LinkableEngine
    {
        protected override void SetEngineApiAccess()
        {
            this._engineApiAccess = new TestEngine();
        }

        public TestEngine TestEngine
        {
            get
            {
                return (TestEngine)this._engineApiAccess;
            }
        }

        public void WriteOmiFile()
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            omiFileParser.LinkableComponentClassName = "HydroNumerics.HydroNet.OpenMI.UnitTest.TestLinkableComponent";
            //omiFileParser.Arguments.Add("InputFilename", hydroNetInputFilename);
            //omiFileParser.Arguments.Add("TimestepLength", timestepLength.ToString());
            omiFileParser.WriteOmiFile("TestLinkableComponent.omi");
        }
    }
 

    public class TestEngine : HydroNumerics.OpenMI.Sdk.Wrapper.IEngine
    {
        List<InputExchangeItem> inputExchangeItems;
        List<OutputExchangeItem> outputExchangeItems;
        HydroNumerics.OpenMI.Sdk.Backbone.RegularGrid grid;
        TimeSeriesGroup groundwaterHeads; //output
        TimeSeriesGroup infiltrations; //input
        int currentTimestep;
        int dt;
        int nx;
        int ny;
        int nt;
        DateTime simulationStart;

        #region IEngine Members

        public TimeSeriesGroup GroundwaterHeads
        {
            get
            {
                return groundwaterHeads;
            }
        }

        public TimeSeriesGroup Infiltrations
        {
            get
            {
                return infiltrations;
            }
        }

        public void Initialize(System.Collections.Hashtable properties)
        {
            currentTimestep = 0;
            simulationStart = new DateTime(2001,1,1);
            dt = 172800; //timestep length (2 days)
            nt = 80; // number of timesteps
            nx = 2; //number of grid cells in x-direction
            ny = 2; //number of grid cells in y-direction
            grid = new RegularGrid(10, 10, 1000, nx, ny, 0);
            groundwaterHeads = new TimeSeriesGroup();
            infiltrations = new TimeSeriesGroup();
            

            for (int i = 0; i < nx * ny; i++)
            {
                groundwaterHeads.Items.Add(new TimestampSeries("GwHead" + i.ToString(), simulationStart, nt, dt, TimestepUnit.Seconds, 3.3, new HydroNumerics.Core.Unit("meters",1.0,0.0)));
                infiltrations.Items.Add(new TimestampSeries("Infiltration" + i.ToString(), simulationStart, nt, dt, TimestepUnit.Seconds, -9999.9999, new HydroNumerics.Core.Unit("flow",1.0,0.0)));
            }

            Quantity infiltQuantity = new Quantity();
            infiltQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("level", 1.0,0.0); 
            infiltQuantity.ID = "Infiltration";
            infiltQuantity.Description = "infiltration";
            infiltQuantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
            InputExchangeItem infiltExItem = new InputExchangeItem();
            infiltExItem.Quantity = infiltQuantity;
            infiltExItem.ElementSet = grid;
            inputExchangeItems = new List<InputExchangeItem>();
            inputExchangeItems.Add(infiltExItem);
           
            Quantity headQuantity = new Quantity();
            headQuantity.Description = "Groundwater head";
            headQuantity.ID = "Head";
            headQuantity.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("meters",1,0);
            headQuantity.ValueType = global::OpenMI.Standard.ValueType.Scalar;
            OutputExchangeItem headOutItem = new OutputExchangeItem();
            headOutItem.Quantity = headQuantity;
            headOutItem.ElementSet = grid;
            outputExchangeItems = new List<OutputExchangeItem>();
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
            
            return new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(simulationStart), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(simulationStart.AddSeconds(dt*(nt-2))));
        }

        #endregion

        #region IRunEngine Members

        public void Dispose()
        {
           
        }

        public void Finish()
        {
            groundwaterHeads.Save("groundwaterheads.xts");
            infiltrations.Save("infiltrations.xts");
           
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
            DateTime ct = simulationStart.AddSeconds(currentTimestep * dt);
            return new TimeStamp(ct);
        }

        public global::OpenMI.Standard.ITimeStamp GetEarliestNeededTime()
        {
            return new TimeStamp(simulationStart);
        }

        public global::OpenMI.Standard.ITime GetInputTime(string QuantityID, string ElementSetID)
        {
            return GetCurrentTime();
        }

        public double GetMissingValueDefinition()
        {
            return -999.99;
        }

        public global::OpenMI.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {
            ScalarSet scalarSet = new ScalarSet(nx * ny, 0);
            for (int i = 0; i < nx * ny; i++)
            {
                scalarSet.data[i] = ((TimestampSeries)groundwaterHeads.Items[i]).Items[currentTimestep].Value;
            }
            return scalarSet;
        }



        public bool PerformTimeStep()
        {
            currentTimestep++;
            return true;
        }

        public void SetValues(string quantityID, string elementSetID, global::OpenMI.Standard.IValueSet values)
        {
            for (int i = 0; i < nx * ny; i++)
            {
                ((TimestampSeries)infiltrations.Items[i]).Items[currentTimestep].Value = ((ScalarSet)values).data[i];
            }
        }

        #endregion
    }
}
