using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Media.Media3D;
using System.Xml;

namespace HelixToolkit
{
    /// <summary>
    /// Export the visual tree to an X3D file
    /// http://en.wikipedia.org/wiki/X3D
    /// http://en.wikipedia.org/wiki/Web3D
    /// 
    /// Validation
    /// http://www.w3.org/People/mimasa/test/schemas/SCHEMA/x3d-3.0.xsd
    /// http://www.web3d.org/x3d/tools/schematron/X3dSchematron.html
    /// </summary>
    public class X3DExporter : Exporter
    {
        private XmlTextWriter writer;
        public Dictionary<string, string> Metadata { get; set; }

        public string Title { set { SetMetadata("title", value); } }

        private void SetMetadata(string key, string value)
        {
            if (Metadata.ContainsKey(key))
                Metadata[key] = value;
            else
            {
                Metadata.Add(key, value);
            }
        }

        public X3DExporter(string path)
        {
            Metadata = new Dictionary<string, string>();

            writer = new XmlTextWriter(path, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument(false);
            writer.WriteDocType("X3D", "ISO//Web3D//DTD X3D 3.0//EN", "http://www.web3d.org/specifications/x3d-3.1.dtd", null);
            writer.WriteStartElement("X3D");
            writer.WriteAttributeString("profile", "Immersive");
            writer.WriteAttributeString("version", "3.1");
            writer.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsd:noNamespaceSchemaLocation", "http://www.web3d.org/specifications/x3d-3.1.xsd");
        }

        protected override void ExportHeader()
        {
            // http://www.w3.org/TR/SVG/struct.html#SVGElement

            writer.WriteStartElement("head");
            foreach (var kvp in Metadata)
            {
                writer.WriteStartElement("meta");
                writer.WriteAttributeString("name", kvp.Key);
                writer.WriteAttributeString("value", kvp.Value);
                writer.WriteEndElement(); // meta                
            }

            writer.WriteEndElement(); // head
            writer.WriteStartElement("Scene");
        }

        public override void Close()
        {
            writer.WriteEndElement(); // Scene
            writer.WriteEndElement(); // X3D
            writer.WriteEndDocument();
            writer.Close();
            base.Close();
        }

        protected override void ExportModel(System.Windows.Media.Media3D.GeometryModel3D model, System.Windows.Media.Media3D.Transform3D inheritedTransform)
        {
            var mesh = model.Geometry as MeshGeometry3D;
            var indices = new StringBuilder();
            foreach (int i in mesh.TriangleIndices)
            {
                indices.Append(i + " ");
            }
            var points = new StringBuilder();
            foreach (var pt in mesh.Positions)
            {
                points.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} {2} ", pt.X, pt.Y, pt.Z);
            }

            writer.WriteStartElement("Transform");
            writer.WriteStartElement("Shape");
            writer.WriteStartElement("IndexedFaceSet");
            writer.WriteAttributeString("coordIndex", indices.ToString());
            writer.WriteStartElement("Coordinate");
            writer.WriteAttributeString("point", points.ToString());
            writer.WriteEndElement();
            writer.WriteEndElement(); // IndexedFaceSet
            writer.WriteStartElement("Appearance");

            writer.WriteStartElement("Material");
            writer.WriteAttributeString("diffuseColor", "0.8 0.8 0.2");
            writer.WriteAttributeString("specularColor", "0.5 0.5 0.5");
            writer.WriteEndElement();
            writer.WriteEndElement(); // Appearance

            writer.WriteEndElement(); // Shape
            writer.WriteEndElement(); // Transform
        }

        /* Example
        <?xml version="1.0" encoding="UTF-8"?>
        <!DOCTYPE X3D PUBLIC "ISO//Web3D//DTD X3D 3.0//EN" "http://www.web3d.org/specifications/x3d-3.0.dtd">
        <X3D profile='Immersive' version='3.0' xmlns:xsd='http://www.w3.org/2001/XMLSchema-instance' xsd:noNamespaceSchemaLocation=' http://www.web3d.org/specifications/x3d-3.0.xsd '>
        <head>
        <meta name='title' content='Pyramid.x3d'/> 
        <meta name='creator' content='MV4204 class'/> 
        <meta name='created' content='8 July 2000'/> 
        <meta name='modified' content='27 December 2003'/> 
        <meta name='description' content='Constructing a Pyramid geometric primitive in order to show use of points and coordinate indices.'/> 
        <meta name='identifier' content=' http://www.web3d.org/x3d/content/examples/Basic/course/Pyramid.x3d '/> 
        <meta name='generator' content='X3D-Edit 3.2, https://savage.nps.edu/X3D-Edit'/> 
        <meta name='license' content='../license.html'/>
        </head> 
        <Scene>
        <Viewpoint description='Pyramid' orientation='0 1 0 .2' position='4 0 25' fieldOfView='0.7854'/> 
        <Transform translation='0 -5 0'>
        <Shape>
        <IndexedFaceSet coordIndex='0 1 2 -1 1 3 2 -1 2 3 0 -1 3 1 0'>
        <Coordinate point='0 0 0 10 0 0 5 0 8.3 5 8.3 2.8'/>
        </IndexedFaceSet> 
        <Appearance>
        <Material diffuseColor='0.8 0.8 0.2' specularColor='0 0 0.5'/>
        </Appearance>
        </Shape>
        </Transform>
        </Scene>
        </X3D>
        */

    }
}