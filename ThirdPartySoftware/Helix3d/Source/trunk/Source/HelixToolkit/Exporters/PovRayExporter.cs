using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Export the visual tree to a PovRay input file
    /// http://www.povray.org
    /// </summary>
    public class PovRayExporter : Exporter
    {
        private readonly StreamWriter writer;

        public PovRayExporter(string path)
        {
            writer = new StreamWriter(path, false, Encoding.UTF8);
        }

        public override void Close()
        {
            writer.Close();
            base.Close();
        }
        
        protected override void ExportCamera(Camera camera)
        {
            base.ExportCamera(camera);
            // todo...
            // http://www.povray.org/documentation/view/3.6.1/17/
        }
        
        protected override void ExportLight(Light light, Transform3D inheritedTransform)
        {
            base.ExportLight(light, inheritedTransform);
            // todo...
            // http://www.povray.org/documentation/view/3.6.1/34/
        }

        protected override void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
                return;

            // http://www.povray.org/documentation/view/3.6.1/293/

            // todo: create textures/material properties from model.Material

            writer.WriteLine("mesh2 {");

            writer.WriteLine("  vertex_vectors");
            writer.WriteLine("  {");
            writer.WriteLine("    " + mesh.Positions.Count + ",");

            foreach (var pt in mesh.Positions)
            {
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "    {0} {1} {2},", pt.X, pt.Y, pt.Z));
            }
            writer.WriteLine("  }");

            writer.WriteLine("  face_indices");
            writer.WriteLine("  {");
            writer.WriteLine("    " + mesh.TriangleIndices.Count / 3 + ",");
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "    {0} {1} {2},", mesh.TriangleIndices[i], mesh.TriangleIndices[i + 1], mesh.TriangleIndices[i + 2]));
            }
            writer.WriteLine("  }");

            // todo: add transform from model.Transform and inheritedTransform
            // http://www.povray.org/documentation/view/3.6.1/49/

            writer.WriteLine("}"); // mesh2
        }

    }
}