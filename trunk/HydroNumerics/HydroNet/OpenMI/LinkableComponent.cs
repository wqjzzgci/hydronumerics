using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroNumerics.Core;
using HydroNumerics.OpenMI.Sdk.Backbone;
using HydroNumerics.OpenMI.Sdk.Wrapper;

namespace HydroNumerics.HydroNet.OpenMI
{
    public class LinkableComponent : LinkableEngine
    {
       

        protected override void SetEngineApiAccess()
        {
            this._engineApiAccess = new EngineWrapper();
            this.SendExtendedEventInfo = true;
        }

        public EngineWrapper TheEngineWrapper
        {
            get
            { return (EngineWrapper) this._engineApiAccess; }
        }

        public void WriteOmiFile(string omiFilename, string hydroNetInputFilename, double timestepLength)
        {
            OmiFileParser omiFileParser = new OmiFileParser();
            omiFileParser.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            omiFileParser.LinkableComponentClassName = "HydroNumerics.HydroNet.OpenMI.LinkableComponent";
            omiFileParser.Arguments.Add("InputFilename", hydroNetInputFilename);
            omiFileParser.Arguments.Add("TimestepLength", timestepLength.ToString());
            omiFileParser.WriteOmiFile(omiFilename);
        }

        public void WriteOmiFile(string hydroNetInputFilename, double timestepLength)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(hydroNetInputFilename);
            string extension = fileInfo.Extension;
            string omiFilename = fileInfo.FullName.Replace(fileInfo.Extension, ".omi");
            WriteOmiFile(omiFilename, hydroNetInputFilename, timestepLength);
        }
    }
}
