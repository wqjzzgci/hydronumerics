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
    public class LinkableTimeSeriesGroup : LinkableComponent, IDiscreteTimes
    {
        TimeSeriesGroup tsGroup;
        bool initializeWasInvoked;
        List<EventType> publishedEventTypes;
        bool isBusy;
        string filename;
        string outputFilename;

        public LinkableTimeSeriesGroup():base()
        {
            initializeWasInvoked = false;
            publishedEventTypes = new List<EventType>();
            publishedEventTypes.Add(EventType.SourceAfterGetValuesCall);
            publishedEventTypes.Add(EventType.SourceBeforeGetValuesReturn);
            publishedEventTypes.Add(EventType.Informative);
            publishedEventTypes.Add(EventType.TargetBeforeGetValuesCall);
            publishedEventTypes.Add(EventType.TargetAfterGetValuesReturn);

            isBusy = false;
        }

        public override void Initialize(IArgument[] properties)
        {
            SendEvent(new Event(EventType.Informative, this, "Started initialization.."));
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            for (int i = 0; i < properties.Length; i++)
            {
                arguments.Add(properties[i].Key, properties[i].Value);
            }
            filename = arguments["Filename"];
            outputFilename = arguments["OutputFilename"];

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
                unit.Description = baseTimeSeries.Unit.Description;
                unit.ConversionFactorToSI = baseTimeSeries.Unit.ConversionFactorToSI;
                unit.OffSetToSI = baseTimeSeries.Unit.OffSetToSI;

                TsQuantity quantity = new TsQuantity();
                quantity.ID = baseTimeSeries.Name;
                quantity.Description = baseTimeSeries.Description;
                quantity.Dimension = dimention;
                quantity.Unit = unit;
                quantity.BaseTimeSeries = baseTimeSeries;

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

                InputExchangeItem inputExchangeItem = new InputExchangeItem();
                inputExchangeItem.Quantity = quantity;
                inputExchangeItem.ElementSet = elementSet;
                this.AddInputExchangeItem(inputExchangeItem);
            }

            initializeWasInvoked = true;
            SendEvent(new Event(EventType.Informative, this, "Completed initialization"));
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
            get 
            {
                if (!initializeWasInvoked)
                {
                    throw new Exception("property \"ModelID\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
                }
                return tsGroup.Name; 
            }
        }

        public override string ModelDescription
        {
            get { return "   "; }
        }

        public override ITimeSpan TimeHorizon
        {
           
            get 
            {
                if (!initializeWasInvoked)
                {
                    throw new Exception("property \"TimeHorizon\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
                }

                return new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(tsGroup.Overlap.Start), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(tsGroup.Overlap.End));
            }
        }
        
        public override void Prepare()
        {
            //nothing is needed here
        }

         public override ITimeStamp EarliestInputTime
        {
            get 
            {
                if (!initializeWasInvoked)
                {
                    throw new Exception("property \"EarliestInputTime\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
                }

                return TimeHorizon.Start; 
            }
        }

        public override IValueSet GetValues(ITime time, string LinkID)
        {
            if (!initializeWasInvoked)
            {
                throw new Exception("Method \"GetValues\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
            }

            ILink link = this.GetLink(LinkID);
            SendSourceAfterGetValuesCallEvent(time, link);

            // -- handle incoming links (where this component is the target)
            if (!isBusy) //avoiding deadlocks 
            {
                isBusy = true;
                foreach (ILink acceptingLink in _acceptingLinks)
                {
                    if (((TsQuantity)acceptingLink.TargetQuantity).BaseTimeSeries is TimestampSeries)
                    {
                        TimestampSeries timestampSeries = (TimestampSeries)((TsQuantity)acceptingLink.TargetQuantity).BaseTimeSeries;
                        foreach (TimestampValue timestampValue in timestampSeries.Items)
                        {
                            TimeStamp getValuesTime = new TimeStamp(timestampValue.Time);
                            SendTargetBeforeGetValuesCall(getValuesTime, acceptingLink);
                            IValueSet valueSet = acceptingLink.SourceComponent.GetValues(getValuesTime, acceptingLink.ID);
                            timestampValue.Value = ((IScalarSet)valueSet).GetScalar(0);
                            SendTargetAfterGetValuesReturn(timestampValue.Value, acceptingLink);
                        }
                    }
                    else if (((TsQuantity)acceptingLink.TargetQuantity).BaseTimeSeries is TimespanSeries)
                    {
                        TimespanSeries timespanSeries = (TimespanSeries)((TsQuantity)acceptingLink.TargetQuantity).BaseTimeSeries;
                        foreach (TimespanValue timespanValue in timespanSeries.Items)
                        {
                            HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan timeSpan = new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(timespanValue.StartTime), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(timespanValue.EndTime));
                            SendTargetBeforeGetValuesCall(timeSpan, acceptingLink);
                            IValueSet valueSet = acceptingLink.SourceComponent.GetValues(timeSpan, acceptingLink.ID);
                            timespanValue.Value = ((IScalarSet)valueSet).GetScalar(0);
                            SendTargetAfterGetValuesReturn(timespanValue.Value, acceptingLink);
                        }
                    }
                    else
                    {
                        throw new Exception("Unexpected exception : Undefined timeseries type (occured in HydroNumerics.Time.OpenMI.LinkableTimeSeriesGroup class)");
                    }
                }
                isBusy = false;
            }

            // -- handle outgoing links (where this component is the source)
            HydroNumerics.Core.Unit toUnit = new HydroNumerics.Core.Unit();
            toUnit.ConversionFactorToSI = link.TargetQuantity.Unit.ConversionFactorToSI;
            toUnit.OffSetToSI = link.TargetQuantity.Unit.OffSetToSI;

            if (time is ITimeStamp)
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp t = new TimeStamp((ITimeStamp)time);

                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(t.ToDateTime(), toUnit);
                ScalarSet scalarSet = new ScalarSet(new double[] { x });
                SendSourceBeforeGetValuesReturn(x, link);
                return scalarSet;
            }
            else
            {
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp start = new TimeStamp(((ITimeSpan)time).Start);
                HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp end = new TimeStamp(((ITimeSpan)time).End);

                double x = ((TsQuantity)link.SourceQuantity).BaseTimeSeries.GetValue(start.ToDateTime(), end.ToDateTime(), toUnit);
                ScalarSet scalarSet = new ScalarSet(new double[] { x });
                SendSourceBeforeGetValuesReturn(x, link);
                return scalarSet;
            }
        }

        public override EventType GetPublishedEventType(int providedEventTypeIndex)
        {
            return publishedEventTypes[providedEventTypeIndex];
        }

        public override int GetPublishedEventTypeCount()
        {
            return publishedEventTypes.Count;
        }

        public override string Validate()
        {
            return "";
        }

        public override void Finish()
        {
            this.TimeSeriesGroup.Save(outputFilename);
            WriteOmiFile(outputFilename); //to enable the output to be used in OpenMI also
        }

        /// <summary>
        /// Write a xml OMI file which can be used e.g. for loading into the OpenMI configuration editor.
        /// The omi file will have the same extension as the timeseries file but with the extension omi.
        /// A output filename is also defined. When the Timeseries group is used under OpenMI changes will be
        /// saved to a new xts file with the same name as the original file, but with .out.xts added the the end
        /// of the filename
        /// </summary>
        /// <param name="timeSeriesGroupInputFilename"></param>
        public void WriteOmiFile(string timeSeriesGroupInputFilename)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(timeSeriesGroupInputFilename);
            string extension = fileInfo.Extension;
            string omiFilename = fileInfo.FullName.Replace(fileInfo.Extension, ".omi");
            string outputFilename = fileInfo.FullName.Replace(fileInfo.Extension, ".out.xts");
            WriteOmiFile(omiFilename, timeSeriesGroupInputFilename, outputFilename);
        }

        public void WriteOmiFile(string omiFilename, string timeSeriesGroupInputFilename, string timeSeriesGroupOutputFilename)
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            omiFileParser.LinkableComponentClassName = "HydroNumerics.Time.OpenMI.LinkableTimeSeriesGroup";
            omiFileParser.Arguments.Add("Filename", timeSeriesGroupInputFilename);
            omiFileParser.Arguments.Add("OutputFilename", timeSeriesGroupOutputFilename);
            omiFileParser.WriteOmiFile(omiFilename);
        }

        #region IDiscreteTimes Members

        public bool HasDiscreteTimes(IQuantity quantity, IElementSet elementSet)
        {
            return true;
        }

        public int GetDiscreteTimesCount(IQuantity quantity, IElementSet elementSet)
        {
            if (!initializeWasInvoked)
            {
                throw new Exception("Method \"GetDiscreteTimesCount\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
            }
            
            if (((TsQuantity)quantity).BaseTimeSeries is TimestampSeries)
            {
                return ((TimestampSeries)((TsQuantity)quantity).BaseTimeSeries).Items.Count;
            }
            else if (((TsQuantity)quantity).BaseTimeSeries is TimespanSeries)
            {
                return ((TimespanSeries)((TsQuantity)quantity).BaseTimeSeries).Items.Count;
            }
            else
            {
                throw new Exception("undefined time series type");
            }
        }

        public ITime GetDiscreteTime(IQuantity quantity, IElementSet elementSet, int discreteTimeIndex)
        {
            if (!initializeWasInvoked)
            {
                throw new Exception("Method \"GetDiscreteTime\" in LinkableTimeSeriesGroup class was invoked before the Initialize method was invoked");
            }
            
            if (((TsQuantity)quantity).BaseTimeSeries is TimestampSeries)
            {
                DateTime time = ((TimestampSeries)((TsQuantity)quantity).BaseTimeSeries).Items[discreteTimeIndex].Time;
                return new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(time);
            }
            else if (((TsQuantity)quantity).BaseTimeSeries is TimespanSeries)
            {
                DateTime startTime = ((TimespanSeries)((TsQuantity)quantity).BaseTimeSeries).Items[discreteTimeIndex].StartTime;
                DateTime endTime = ((TimespanSeries)((TsQuantity)quantity).BaseTimeSeries).Items[discreteTimeIndex].EndTime;
                return new HydroNumerics.OpenMI.Sdk.Backbone.TimeSpan(new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(startTime), new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(endTime));
            }
            else
            {
                throw new Exception("undefined time series type");
            }
        }

        #endregion

        private void SendSourceAfterGetValuesCallEvent(ITime time, ILink link)
        {
            Event aEvent = new Event();
            aEvent.Type = EventType.SourceAfterGetValuesCall;
            aEvent.Sender = this;
            aEvent.Description = ModelID + " (" + ComponentID + ") was requested for values for \"" + link.SourceQuantity.ID + "\" at time: " + ITimeToString(time);
            
            SendEvent(aEvent);
        }

        private void SendSourceBeforeGetValuesReturn(double returnValue, ILink link)
        {
            Event aEvent = new Event();
            aEvent.Type = EventType.SourceBeforeGetValuesReturn;
            aEvent.Sender = this;
            aEvent.Description = ModelID + "returned the value: " + returnValue.ToString() + " for \"" + link.SourceQuantity.ID + "\"";
            SendEvent(aEvent);
        }

        private void SendTargetBeforeGetValuesCall(ITime time, ILink link)
        {
            Event aEvent = new Event();
            aEvent.Type = EventType.TargetBeforeGetValuesCall;
            aEvent.Sender = this;
            aEvent.Description = ModelID + " requested value for \"" + link.TargetQuantity.ID + "\" at time: " + ITimeToString(time);
            SendEvent(aEvent);
        }

        private void SendTargetAfterGetValuesReturn(double returnedValue, ILink link)
        {
            Event aEvent = new Event();
            aEvent.Type = EventType.TargetAfterGetValuesReturn;
            aEvent.Sender = this;
            aEvent.Description = ModelID + " recieved value " + returnedValue.ToString() + " for \"" + link.TargetQuantity.ID +"\""; 
            SendEvent(aEvent);
        }

        private string ITimeToString(ITime time)
        {
            if (time is ITimeSpan)
            {
                DateTime startTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(((ITimeSpan)time).Start).ToDateTime();
                DateTime endTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp(((ITimeSpan)time).End).ToDateTime();
                return("(" + startTime.ToLongDateString()+":"+ startTime.ToLongTimeString() + ", " + endTime.ToLongDateString() +":" + endTime.ToLongTimeString());
            }
            else
            {
                DateTime dateTime = new HydroNumerics.OpenMI.Sdk.Backbone.TimeStamp((ITimeStamp)time).ToDateTime();
                return("(" + dateTime.ToLongDateString() +":" +dateTime.ToLongTimeString() + ")");
            }

        }

        public TimeSeriesGroup TimeSeriesGroup 
        {
            get
            {
                return this.tsGroup;
            }
        }
    }
}
