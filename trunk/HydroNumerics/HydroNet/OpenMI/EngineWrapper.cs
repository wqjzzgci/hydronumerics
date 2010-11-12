using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.Geometry;
using HydroNumerics.HydroNet.Core;



using HydroNumerics.Core;

namespace HydroNumerics.HydroNet.OpenMI
{
    public class EngineWrapper : HydroNumerics.OpenMI.Sdk.Wrapper.IEngine
    {
        private HydroNumerics.HydroNet.Core.Model model;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.OutputExchangeItem> outputExchangeItems;
        private List<HydroNumerics.OpenMI.Sdk.Backbone.InputExchangeItem> inputExchangeItems;

        private Dictionary<string, GroundWaterBoundary> HeadBoundaries = new Dictionary<string, GroundWaterBoundary>();
        private Dictionary<string, GroundWaterBoundary> LeakageBoundaries = new Dictionary<string, GroundWaterBoundary>();
        private Dictionary<string, SinkSourceBoundary> FlowBoundaries = new Dictionary<string, SinkSourceBoundary>();

        string inputFilename;
        System.TimeSpan timestepLength;

        bool finishMethodWasInvoked;

        public EngineWrapper()
        {
            outputExchangeItems = new List<OutputExchangeItem>();
            inputExchangeItems = new List<InputExchangeItem>();
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

            
            // because the OpenMI configuration editor works makes paths relative to locations of OMI files or the configuration editor itselves, thigs may go wrong.
            // In order to overcome this problem, the full path for the input file is saves, so when the Finish method is invoked, the output is save to the input file again and not
            // a file at some random location :-). See below
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(inputFilename);
            inputFilename = fileInfo.FullName;

            model = HydroNumerics.HydroNet.Core.ModelFactory.GetModel(inputFilename);
            model.Initialize();
            //model = ModelFactory.GetModel(inputFilename);


            foreach (var wb in model._waterBodies)
            {
              //Build exchangeitems from groundwater boundaries
              foreach (var gwb in wb.GroundwaterBoundaries)
              {
                GroundWaterBoundary GWB = gwb as GroundWaterBoundary;
                if (GWB != null)
                {
                  ElementSet elementSet = new ElementSet();
                  elementSet.ID = GWB.Name;
                  elementSet.Description = "Location: " + GWB.Name;
                  elementSet.SpatialReference = new SpatialReference("Undefined");
                  Element element = new Element();
                  element.ID = GWB.Name;

                  elementSet.ElementType = ElementType.XYPolygon;
                  foreach (XYPoint xyPoint in ((XYPolygon)GWB.ContactGeometry).Points)
                  {
                    element.AddVertex(new Vertex(xyPoint.X, xyPoint.Y, 0));
                  }

                  elementSet.AddElement(element);

                  //Head
                  HydroNumerics.OpenMI.Sdk.Backbone.Dimension dimension = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
                  dimension.Length = 1;

                  HydroNumerics.OpenMI.Sdk.Backbone.Unit unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit();
                  unit.ID = "m";
                  unit.Description = "meter";
                  unit.ConversionFactorToSI = 1;
                  unit.OffSetToSI = 0;

                  Quantity quantity = new Quantity();
                  quantity.ID = "Head";
                  quantity.Description = "Head at:" + GWB.Name;
                  quantity.Dimension = dimension;
                  quantity.Unit = unit;
                  
                  InputExchangeItem inputExchangeItem = new InputExchangeItem();
                  inputExchangeItem.Quantity = quantity;
                  inputExchangeItem.ElementSet = elementSet;
                  inputExchangeItems.Add(inputExchangeItem);

                  HeadBoundaries.Add(GWB.Name, GWB);

                  //Leakage
                  Quantity leakage = new Quantity("Leakage");
                  leakage.Description = "Leakage at:" + GWB.Name;
                  var leakageDimension = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
                  leakageDimension.Length = 3;
                  leakageDimension.Time = -1;
                  leakage.Dimension = leakageDimension;
                  leakage.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("m3/s", 1, 0);

                  OutputExchangeItem outputitem = new OutputExchangeItem();
                  outputitem.Quantity = leakage;
                  outputitem.ElementSet = elementSet;
                  outputExchangeItems.Add(outputitem);
                  LeakageBoundaries.Add(GWB.Name, GWB);

                }
              }

              foreach (var S in wb.Sources)
              {
                ElementSet elementSet = new ElementSet();
                elementSet.ID = S.Name;
                elementSet.Description = "Location: " + S.Name;
                elementSet.SpatialReference = new SpatialReference("Undefined");
                Element element = new Element();
                element.ID = S.Name;

                elementSet.ElementType = ElementType.IDBased;


                //Flow
                Quantity leakage = new Quantity("Flow");
                leakage.Description = "Flow at:" + S.Name;
                var leakageDimension = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
                leakageDimension.Length = 3;
                leakageDimension.Time = -1;
                leakage.Dimension = leakageDimension;
                leakage.Unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit("m3/s", 1, 0);

                InputExchangeItem outputitem = new InputExchangeItem();
                outputitem.Quantity = leakage;
                outputitem.ElementSet = elementSet;
                inputExchangeItems.Add(outputitem);
                FlowBoundaries.Add(S.Name, (SinkSourceBoundary)S);
                


              }
            }

            

            //foreach (var exchangeItem in model.ExchangeItems)
            //{
            //    HydroNumerics.OpenMI.Sdk.Backbone.Dimension dimention = new HydroNumerics.OpenMI.Sdk.Backbone.Dimension();
            //    dimention.AmountOfSubstance = exchangeItem.Unit.Dimension.AmountOfSubstance;
            //    dimention.Currency = exchangeItem.Unit.Dimension.Currency;
            //    dimention.ElectricCurrent = exchangeItem.Unit.Dimension.ElectricCurrent;
            //    dimention.Length = exchangeItem.Unit.Dimension.AmountOfSubstance;
            //    dimention.LuminousIntensity = exchangeItem.Unit.Dimension.Length;
            //    dimention.Mass = exchangeItem.Unit.Dimension.LuminousIntensity;
            //    dimention.AmountOfSubstance = exchangeItem.Unit.Dimension.Mass;
            //    dimention.Time = exchangeItem.Unit.Dimension.Time;

            //    HydroNumerics.OpenMI.Sdk.Backbone.Unit unit = new HydroNumerics.OpenMI.Sdk.Backbone.Unit();
            //    unit.ID = exchangeItem.Unit.ID;
            //    unit.Description = exchangeItem.Unit.Description;
            //    unit.ConversionFactorToSI = exchangeItem.Unit.ConversionFactorToSI;
            //    unit.OffSetToSI = unit.OffSetToSI;
                                
            //    Quantity quantity = new Quantity();
            //    quantity.ID = exchangeItem.Quantity;
            //    quantity.Description = exchangeItem.Description;
            //    quantity.Dimension = dimention;
            //    quantity.Unit = unit;

            //    ElementSet elementSet = new ElementSet();
            //    elementSet.ID = exchangeItem.Location;
            //    elementSet.Description = "Location: " + exchangeItem.Location;
            //    elementSet.SpatialReference = new SpatialReference("Undefined");
            //    Element element = new Element();
            //    element.ID = exchangeItem.Location;

            //    if (exchangeItem.Geometry is XYPolygon)
            //    {
            //        elementSet.ElementType = ElementType.XYPolygon;
            //        foreach (XYPoint xyPoint in ((XYPolygon)exchangeItem.Geometry).Points)
            //        {
            //            element.AddVertex(new Vertex(xyPoint.X, xyPoint.Y, 0));
            //        }
            //    }
            //    else if (exchangeItem.Geometry is XYPoint)
            //    {
            //        throw new NotImplementedException();
            //    }
            //    else if (exchangeItem.Geometry is XYPolyline)
            //    {
            //        throw new NotImplementedException();
            //    }
            //    else
            //    {
            //        elementSet.ElementType = global::OpenMI.Standard.ElementType.IDBased;
            //    }
                
                
            //    elementSet.AddElement(element);


            //    if (exchangeItem.IsOutput)
            //    {
            //        OutputExchangeItem outputExchangeItem = new OutputExchangeItem();
            //        outputExchangeItem.Quantity = quantity;
            //        outputExchangeItem.ElementSet = elementSet;
            //        outputExchangeItems.Add(outputExchangeItem);
            //    }
            //    if (exchangeItem.IsInput)
            //    {
            //        InputExchangeItem inputExchangeItem = new InputExchangeItem();
            //        inputExchangeItem.Quantity = quantity;
            //        inputExchangeItem.ElementSet = elementSet;
            //        inputExchangeItems.Add(inputExchangeItem);
            //    }
            //}

        }

        public string GetComponentID()
        {
            return "HydroNet";
        }

        public string GetComponentDescription()
        {
            return "Conceptual model for trasport of water and solutes";
        }

        public string GetModelID()
        {
            return model.Name;
        }

        public string GetModelDescription()
        {
            return "No modeldescription available";
        }

        public global::OpenMI.Standard.ITimeSpan GetTimeHorizon()
        {
            //HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp startTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(model.CurrentTime);
            //HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp endTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(model.EndTime);
            //OpenMI times cannot handle the DateTime.MinValue and the DateTime.MaxValue so the timehorizon is simply made large (see below).
            HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp startTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(1900,1,1));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp endTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(new DateTime(2100,1,1));
            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan timeHorizon = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(startTime, endTime);
            return timeHorizon;
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
            model.Update(model.CurrentTime.Add(timestepLength));
            return true;
        }

        public void SetValues(string quantityID, string elementSetID, global::OpenMI.Standard.IValueSet values)
        {
            if (values is IScalarSet)
            {
              switch (quantityID)
              {
                case "Head":
                  HeadBoundaries[elementSetID].GroundwaterHead = ((IScalarSet)values).GetScalar(0);
                  break;
                case "Flow":
                  FlowBoundaries[elementSetID].OverrideFlowRate = ((IScalarSet)values).GetScalar(0);
                  break;
                default:
                  break;
              }
//                model.ExchangeItems.Single(var => var.Quantity == quantityID & var.Location == elementSetID).ExchangeValue = ((IScalarSet)values).GetScalar(0);
            }
            else
            {
                throw new Exception("The HydroNet model can only handle IScalarSet - not IVectorSet");
            }
        }

        public global::OpenMI.Standard.IValueSet GetValues(string QuantityID, string ElementSetID)
        {

          switch (QuantityID)
          {
            case "Leakage":
              return new ScalarSet(1, LeakageBoundaries[ElementSetID].CurrentFlowRate); 
            default:
              break;
          }


//            double x = model.ExchangeItems.Single(var => var.Quantity == QuantityID & var.Location == ElementSetID).ExchangeValue;
  //          ScalarSet scalarSet = new ScalarSet(1, x);
            return null;
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

        /// <summary>
        /// The wrapped HydroNet model. Intended to be used for Unit testing only.
        /// </summary>
        public Model HydroNetModel
        {
            get { return this.model; }
        }

        
    }
}
