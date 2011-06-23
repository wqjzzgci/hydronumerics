using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Export the WPF3D visual tree to an OBJ file
    /// http://en.wikipedia.org/wiki/Obj
    /// http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// http://www.eg-models.de/formats/Format_Obj.html
    /// </summary>
    public class ObjExporter : Exporter
    {
        private readonly StreamWriter writer;
        private readonly StreamWriter mwriter;
        private readonly string directory;

        public ObjExporter(string outputFileName)
            : this(outputFileName, null)
        {
        }

        public ObjExporter(string outputFileName, string comment)
        {
            var fullPath = Path.GetFullPath(outputFileName);
            var mtlPath = Path.ChangeExtension(outputFileName, ".mtl");
            string mtlFilename = Path.GetFileName(mtlPath);
            directory = Path.GetDirectoryName(fullPath);

            writer = new StreamWriter(outputFileName);
            mwriter = new StreamWriter(mtlPath);

            if (!String.IsNullOrEmpty(comment))
                writer.WriteLine(String.Format("# {0}", comment));

            writer.WriteLine("mtllib " + mtlFilename);
        }

        public override void Close()
        {
            writer.Close();
            mwriter.Close();
            base.Close();
        }

        private int objectNo = 1;
        private int groupNo = 1;
        private int matNo = 1;

        protected override void ExportModel(GeometryModel3D model, Transform3D transform)
        {
            writer.WriteLine(String.Format("o object{0}", objectNo++));
            writer.WriteLine(String.Format("g group{0}", groupNo++));
            string matName = String.Format("mat{0}", matNo++);
            writer.WriteLine(String.Format("usemtl {0}", matName));
            ExportMaterial(matName, model.Material, model.BackMaterial);
            var mesh = model.Geometry as MeshGeometry3D;
            ExportMesh(mesh, Transform3DHelper.CombineTransform(transform, model.Transform));
        }

        private void ExportMaterial(string matName, Material material, Material backMaterial)
        {
            mwriter.WriteLine(String.Format("newmtl {0}", matName));
            var dm = material as DiffuseMaterial;
            var sm = material as SpecularMaterial;
            var mg = material as MaterialGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    if (m is DiffuseMaterial)
                        dm = m as DiffuseMaterial;
                    if (m is SpecularMaterial)
                        sm = m as SpecularMaterial;
                }
            }
            if (dm != null)
            {
                mwriter.WriteLine(String.Format("Ka {0}", ToColorString(dm.AmbientColor)));
                var scb = dm.Brush as SolidColorBrush;
                if (scb != null)
                {
                    mwriter.WriteLine(String.Format("Kd {0}", ToColorString(scb.Color)));
                    mwriter.WriteLine(String.Format(CultureInfo.InvariantCulture, "d {0:F4}", scb.Color.A / 255.0));
                }
                else
                {
                    var textureFilename = matName + ".png";
                    var texturePath = Path.Combine(directory, textureFilename);
                    // create .png bitmap file for the brush
                    RenderBrush(texturePath, dm.Brush, 1024, 1024);
                    mwriter.WriteLine(String.Format("map_Ka {0}", textureFilename));
                }
            }
            if (sm != null)
            {
                var scb = sm.Brush as SolidColorBrush;
                if (scb != null)
                    mwriter.WriteLine(String.Format("Ks {0}", ToColorString(scb.Color)));
                // todo: Shininess conversion to SpecularPower?
                mwriter.WriteLine(String.Format(CultureInfo.InvariantCulture, "Ns {0:F4}", sm.SpecularPower));
            }
        }

        private string ToColorString(Color color)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:F4} {1:F4} {2:F4}", color.R / 255.0, color.G / 255.0,
                                 color.B / 255.0);
        }

        public void ExportMesh(MeshGeometry3D m, Transform3D t)
        {

            int triangles = m.TriangleIndices.Count / 3;

            foreach (var v in m.Positions)
            {
                var p = t.Transform(v);
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "v {0} {1} {2}", p.X, p.Y, p.Z));
            }

            foreach (var vt in m.TextureCoordinates)
            {
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "vt {0} {1}", vt.X, vt.Y));
            }

            foreach (var vn in m.Normals)
            {
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "vn {0} {1} {2}", vn.X, vn.Y, vn.Z));
            }


            for (int i = 0; i < m.TriangleIndices.Count; i += 3)
            {
                writer.WriteLine("f {0} {1} {2}",
                    m.TriangleIndices[i] + 1,
                    m.TriangleIndices[i + 1] + 1,
                    m.TriangleIndices[i + 2] + 1);

            }

            // todo: texture/normal info...
            //            _writer.WriteLine("f {0}/{1}/{2} {3}/{4}/{5} {6}/{7}/{8}")

        }
    }


}