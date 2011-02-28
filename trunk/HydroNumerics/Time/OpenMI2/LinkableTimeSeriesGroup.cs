using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMI.Standard2;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.Time.Core;




namespace HydroNumerics.Time.OpenMI2
{
    public class LinkableTimeSeriesGroup : LinkableComponent
    {
        TimeSeriesGroup timeSeriesGroup;
        string filename;
        string outputFilename;

        public override IList<IBaseInput> Inputs
        {
            get { throw new NotImplementedException(); }
        }

        public override IList<IBaseOutput> Outputs
        {
            get { throw new NotImplementedException(); }
        }

        public override List<IAdaptedOutputFactory> AdaptedOutputFactories
        {
            get { throw new NotImplementedException(); }
        }

        public override void Initialize()
        {
            Dictionary<string, string> argumentsDictionary = new Dictionary<string, string>();
            
            //TODO: Check that you can assume that e.g. a configuration editor will populate the Argumenst, based on data from the OMI file
            foreach (IArgument argument in this.Arguments)
            {
                argumentsDictionary.Add(argument.Id, argument.ValueAsString);
            }
            filename = argumentsDictionary["Filename"];
            outputFilename = argumentsDictionary["OutputFilename"];

            timeSeriesGroup = TimeSeriesGroupFactory.Create(filename);

            
        }

        public override string[] Validate()
        {
            throw new NotImplementedException();
        }

        public override void Prepare()
        {
            throw new NotImplementedException();
        }

        public override void Update(params IBaseOutput[] requiredOutput)
        {
            throw new NotImplementedException();
        }

        public override void Finish()
        {
            throw new NotImplementedException();
        }
    }
}
