using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using OpenMI.Standard;

namespace HydroNumerics.OpenMI.Sdk.Backbone
{
    public class OmiFileParser
    {
        private string linkableComponentClassName;
        private string assembleName;
        Dictionary<string, string> arguments;

        public OmiFileParser()
        {
            arguments = new Dictionary<string, string>();
            linkableComponentClassName = "No name defined for the Linkable component class";
            assembleName = "No name defined for the assembly";
        }

        /// <summary>
        /// class name of the class that implemnts the ILinkableComponent interface (fully qualified class name)
        /// </summary>
        public string LinkableComponentClassName
        {
            get { return linkableComponentClassName; }
            set { linkableComponentClassName = value; } 
        }

        /// <summary>
        /// path and name of the dll which holds the class that implements the ILinkableComponent interface
        /// </summary>
        public string AssemblyName 
        {
            get { return assembleName; }
            set { assembleName = value; } 
        }

        /// <summary>
        /// Arguments that are passed to the LinkableComponent when the Initialize method is invoked
        /// </summary>
        public Dictionary<string, string> Arguments
        {
            get { return arguments; }
        }

        public IArgument[] GetArgumentsAsIArgumentArray()
        {
            IArgument[] args = new Argument[arguments.Count];
            int i = 0;
            foreach (KeyValuePair<string, string> keyValuePair in arguments)
            {
                args[i] = new Argument(keyValuePair.Key, keyValuePair.Value, true, "No description");
                i++;
            }
            
            return args;
        }

        /// <summary>
        /// Write the Omi file
        /// </summary>
        /// <param name="filename">Path and filename for the Omi file</param>
        public void WriteOmiFile(string filename)
        {
            XmlTextWriter xmlTextWriter = new XmlTextWriter(filename, null);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.WriteStartDocument();
            xmlTextWriter.WriteStartElement("LinkableComponent");
            xmlTextWriter.WriteAttributeString("Type", linkableComponentClassName);
            xmlTextWriter.WriteAttributeString("Assembly", assembleName);
            xmlTextWriter.WriteStartElement("Arguments");

            foreach (KeyValuePair<string, string> kvp in arguments)
            {
                xmlTextWriter.WriteStartElement("Argument");
                xmlTextWriter.WriteAttributeString("Key",kvp.Key);
                xmlTextWriter.WriteAttributeString("ReadOnly", "true");
                xmlTextWriter.WriteAttributeString("Value", kvp.Value);
                
                xmlTextWriter.WriteEndElement();
            }

            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndElement();
            xmlTextWriter.WriteEndDocument();
            xmlTextWriter.Close();
        }

        /// <summary>
        /// Read a OMI file
        /// </summary>
        /// <param name="filename">Path and filename for the Omi file</param>
        public void ReadOmiFile(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            
            XmlReader reader = XmlReader.Create(filename, settings);
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "LinkableComponent")
                {
                    linkableComponentClassName = reader.GetAttribute("Type");
                    assembleName = reader.GetAttribute("Assembly");
                }

                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Argument")
                {
                    arguments.Add(reader.GetAttribute("Key"), reader.GetAttribute("Value"));
                }
            }

            reader.Close();
        }
    }
}
