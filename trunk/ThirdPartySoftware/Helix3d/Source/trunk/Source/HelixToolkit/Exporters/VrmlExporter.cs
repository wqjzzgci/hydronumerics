using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Export the visual tree to a VRML97 (2.0) file
    /// http://en.wikipedia.org/wiki/Vrml
    /// http://en.wikipedia.org/wiki/Web3D
    ///
    /// VRML plugin/browser detector:
    /// http://cic.nist.gov/vrml/vbdetect.html
    ///
    /// Links
    /// http://openvrml.org/
    /// </summary>
    public class VrmlExporter : Exporter
    {
        private readonly StreamWriter writer;

        public VrmlExporter(string path, string title=null)
        {
            writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine("# VRML V2.0 utf8");
            if (title!=null)
              writer.WriteLine("# "+title);
        }

        public override void Close()
        {
            writer.Close();
            base.Close();
        }

        protected override void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
                return;

            // writer.WriteLine("Transform {");
            // todo: add transform from model.Transform and inheritedTransform
            
            writer.WriteLine("Shape {");
            
            writer.WriteLine("  appearance Appearance {");
            // todo: set material properties from model.Material
            
            writer.WriteLine("    material Material {");
            writer.WriteLine("      diffuseColor 0.8 0.8 0.2");
            writer.WriteLine("      specularColor 0.5 0.5 0.5");
            writer.WriteLine("    }");
            writer.WriteLine("  }"); // Appearance

            writer.WriteLine("  geometry IndexedFaceSet {");
            writer.WriteLine("    coord Coordinate {");
            writer.WriteLine("      point [");
            
            foreach (var pt in mesh.Positions)
            {           
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} {1} {2},", pt.X, pt.Y, pt.Z));
            }
            writer.WriteLine("      ]");
            writer.WriteLine("    }");
            
            writer.WriteLine("    coordIndex [");
            for (int i=0;i<mesh.TriangleIndices.Count;i+=3)
            {
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0} {1} {2},", mesh.TriangleIndices[i], mesh.TriangleIndices[i+1], mesh.TriangleIndices[i+2]));
            }
            writer.WriteLine("    ]");
            writer.WriteLine("  }"); // IndexedFaceSet
            

            writer.WriteLine("}"); // Shape
            // writer.WriteLine("}"); // Transform
        }

       

    }
}