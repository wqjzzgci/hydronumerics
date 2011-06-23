using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;

namespace HelixToolkit
{
    /// <summary>
    /// Export the WPF3D visual tree to a Kerkythea xml file
    /// Kerkythea: http://www.kerkythea.net/joomla
    /// </summary>
    public class KerkytheaExporter : Exporter
    {
        #region RenderSettings enum

        public enum RenderSettings
        {
            RayTracer,
            PhotonMap,
            MetropolisLightTransport
        } ;

        #endregion

        private readonly HashSet<string> names = new HashSet<string>();
        private readonly XmlWriter writer;
        public Dictionary<Material, XmlDocument> RegisteredMaterials = new Dictionary<Material, XmlDocument>();

        /// <summary>
        /// Initializes a new instance of the <see cref="KerkytheaExporter"/> class.
        /// </summary>
        /// <param name="outputFileName">Name of the output file.</param>
        public KerkytheaExporter(string outputFileName)
        {
            Name = "My Scene";
            BackgroundColor = Colors.Black;
            ReflectionColor = Colors.Gray;
            Reflections = true;
            Shadows = true;
            SoftShadows = true;
            LightMultiplier = 3.0;
            Threads = 2;

            ShadowColor = Color.FromArgb(255, 100, 100, 100);
            RenderSetting = RenderSettings.RayTracer;
            Aperture = "Pinhole";
            FocusDistance = 1.0;
            LensSamples = 3;

            Width = 500;
            Height = 500;

            TexturePath = Path.GetDirectoryName(outputFileName);
            TextureWidth = 1024;
            TextureHeight = 1024;

            var settings = new XmlWriterSettings { Indent = true, };

            writer = XmlWriter.Create(outputFileName, settings);
        }

        public Color BackgroundColor { get; set; }
        public double LightMultiplier { get; set; }

        public bool Reflections { get; set; }
        public Color ReflectionColor { get; set; }

        public bool Shadows { get; set; }
        public bool SoftShadows { get; set; }
        public Color ShadowColor { get; set; }

        public RenderSettings RenderSetting { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public int Threads { get; set; }
        public string Name { get; set; }

        public double FocalLength { get; set; }
        public double FocusDistance { get; set; }
        public string Aperture { get; set; }
        public int LensSamples { get; set; }

        public string TexturePath { get; set; }
        public int TextureWidth { get; set; }
        public int TextureHeight { get; set; }

        public void RegisterMaterial(Material m, string filename)
        {
            var doc = new XmlDocument();
            doc.Load(filename);
            RegisteredMaterials.Add(m, doc);
        }

        protected override void ExportHeader()
        {
            writer.WriteStartDocument();

            writer.WriteStartElement("Root");
            writer.WriteAttributeString("Label", "Default Kernel");
            writer.WriteAttributeString("Name", "");
            writer.WriteAttributeString("Type", "Kernel");

            WriteStartObject("./Modellers/XML Modeller", "XML Modeller", "XML Modeller", "Modeller");
            WriteEndObject();

            WriteStartObject("./Image Handlers/Free Image Support", "Free Image Support", "Free Image Support",
                             "Image Handler");
            WriteParameter("Tone Mapping", "External");
            WriteParameter("Jpeg Quality", "Higher");
            WriteEndObject();

            WriteStartObject("./Direct Light Estimators/Refraction Enhanced", "Refraction Enhanced",
                             "Refraction Enhanced", "Direct Light Estimator");
            WriteParameter("Enabled", "Boolean", "1");
            WriteParameter("PseudoCaustics", "Boolean", "0");
            WriteParameter("PseudoTranslucencies", "Boolean", "0");
            WriteParameter("Area Light Evaluation", "Boolean", "1");
            WriteParameter("Optimized Area Lights", "Boolean", "1");
            WriteParameter("Accurate Soft Shadows", "Boolean", "0");
            WriteParameter("Antialiasing", "String", "High");
            WriteParameter("./Evaluation/Diffuse", "Boolean", "1");
            WriteParameter("./Evaluation/Specular", "Boolean", "1");
            WriteParameter("./Evaluation/Translucent", "Boolean", "1");
            WriteParameter("./Evaluation/Transmitted", "Boolean", "1");
            WriteEndObject();

            // add ray tracer module.
            for (int i = 0; i < Threads; i++)
            {
                WriteStandardRayTracer("#" + i);
            }
            WriteThreadedRaytracer(Threads);

            // add spatial subdivision module.
            WriteStartObject("./Environments/Octree Environment", "Octree Environment", "Octree Environment",
                             "Environment");
            WriteParameter("Max Objects per Cell", 20);
            WriteParameter("Instancing Switch", 1000000);
            WriteParameter("Caching Switch", 6000000);
            WriteEndObject();

            // add basic post filtering / tone mapping.
            WriteStartObject("./Filters/Simple Tone Mapping", "Simple Tone Mapping", "", "Filter");
            WriteParameter("Enabled", true);
            WriteParameter("Method", "Simple");
            WriteParameter("Exposure", 1.0);
            WriteParameter("Gamma", 1.0);
            WriteParameter("Dark Multiplier", 1.0);
            WriteParameter("Bright Multiplier", 1.0);
            WriteParameter("Reverse Correction", true);
            WriteParameter("Reverse Gamma", 2.2);
            WriteEndObject();

            // start of scene description.
            WriteStartObject("./Scenes/" + Name, "Default Scene", Name, "Scene");
        }

        public void WriteMetropolisLightTransport(string name)
        {
            WriteStartObject("./Ray Tracers/" + name, "Metropolis Light Transport", name, "Ray Tracer");
            WriteParameter("Max Ray Tracing Depth", 100);
            WriteParameter("Max Iterations", 10000);
            WriteParameter("Linear Lightflow", true);
            WriteParameter("Seed Paths", 50000);
            WriteParameter("Large Step Probability", 0.2);
            WriteParameter("Max Mutation Distance", 0.02);
            WriteParameter("Live Probability", 0.7);
            WriteParameter("Max Consecutive Rejections", 200);
            WriteParameter("Bidirectional", true);
            WriteParameter("Super Sampling", "3x3");
            WriteParameter("Image Filename", "temp.jpg");
            WriteParameter("Random Seed", "Automatic");
            WriteEndObject();
        }

        public void WriteStandardRayTracer(string name)
        {
            WriteStartObject("./Ray Tracers/" + name, "Standard Ray Tracer", name, "Ray Tracer");
            WriteParameter("Rasterization", "Auto");
            // WriteParameter("Antialiasing", "Extra Pass 3x3");
            WriteParameter("Antialiasing", "Production AA");
            WriteParameter("Antialiasing Filter", "Mitchell-Netravali 0.5 0.8");
            WriteParameter("Antialiasing Threshold", 0.3);
            WriteParameter("Texture Filtering", true);
            WriteParameter("Ambient Lighting", true);
            WriteParameter("Direct Lighting", true);
            WriteParameter("Sky Lighting", true);
            WriteParameter("Brightness Threshold", 0.002);
            WriteParameter("Max Ray Tracing Depth", 5);
            WriteParameter("Max Scatter Bounces", 5);
            WriteParameter("Max Dirac Bounces", 5);
            WriteParameter("Irradiance Precomputation", 4);
            WriteParameter("Irradiance Scale", Colors.White);
            WriteParameter("Linear Lightflow", true);
            WriteParameter("Max Iterations", 5);
            WriteParameter("Super Sampling", "None");
            WriteParameter("Image Filename", "temp.jpg");
            WriteParameter("./Sampling Criteria/Diffuse Samples", 1024);
            WriteParameter("./Sampling Criteria/Specular Samples", 32);
            WriteParameter("./Sampling Criteria/Dispersion Samples", true);
            WriteParameter("./Sampling Criteria/Trace Diffusers", false);
            WriteParameter("./Sampling Criteria/Trace Translucencies", false);
            WriteParameter("./Sampling Criteria/Trace Fuzzy Reflections", true);
            WriteParameter("./Sampling Criteria/Trace Fuzzy Refractions", true);
            WriteParameter("./Sampling Criteria/Trace Reflections", true);
            WriteParameter("./Sampling Criteria/Trace Refractions", true);
            WriteParameter("./Sampling Criteria/Random Generator", "Pure");
            WriteEndObject();
        }

        public void WriteThreadedRaytracer(int threads)
        {
            WriteStartObject("./Ray Tracers/Threaded Ray Tracer", "Threaded Ray Tracer", "Threaded Ray Tracer",
                             "Ray Tracer");
            for (int i = 0; i < threads; i++)
            {
                WriteParameter("Thread #" + i, "#" + i);
            }
            WriteParameter("Network Mode", "None");
            WriteParameter("Listening Port", 6200);
            WriteParameter("Host", "127.0.0.1");
            WriteEndObject();
        }

        public override void Close()
        {
            // end of scene description.
            writer.WriteFullEndElement();

            // it is necessary to describe the primary/active modules as there might exist more than one!
            WriteParameter("Mip Mapping", true);
            WriteParameter("./Interfaces/Active", "Null Interface");
            WriteParameter("./Modellers/Active", "XML Modeller");
            WriteParameter("./Image Handlers/Active", "Free Image Support");

            WriteParameter("./Ray Tracers/Active", "Threaded Ray Tracer");
            WriteParameter("./Irradiance Estimators/Active", "Null Irradiance Estimator");
            WriteParameter("./Direct Light Estimators/Active", "Refraction Enhanced");
            WriteParameter("./Environments/Active", "Octree Environment");
            WriteParameter("./Filters/Active", "Simple Tone Mapping");
            WriteParameter("./Scenes/Active", Name);
            WriteParameter("./Libraries/Active", "Material Librarian");

            // end of root element
            writer.WriteFullEndElement();

            writer.WriteEndDocument();
            writer.Close();
            base.Close();
        }

        protected override void ExportCamera(Camera c)
        {
            var pc = c as PerspectiveCamera;
            if (pc == null)
                throw new InvalidOperationException("Only perspective cameras are supported.");

            const string name = "Camera #1";
            WriteStartObject("./Cameras/" + name, "Pinhole Camera", name, "Camera");

            // FOV = 2 arctan (x / (2 f)), x is diagonal, f is focal length
            // f = x / 2 / Tan(FOV/2)
            // http://en.wikipedia.org/wiki/Angle_of_view
            // http://kmp.bdimitrov.de/technology/fov.html

            // PerspectiveCamera.FieldOfView: Horizontal field of view
            // Must multiply by ratio of Viewport Width/Height

            double ratio = Width / (double)Height;
            const double x = 40;
            double f = 0.5 * ratio * x / Math.Tan(0.5 * pc.FieldOfView / 180.0 * Math.PI);

            WriteParameter("Focal Length (mm)", f);
            WriteParameter("Film Height (mm)", x);
            WriteParameter("Resolution", String.Format(CultureInfo.InvariantCulture, "{0}x{1}", Width, Height));

            var t = CreateTransform(pc.Position, pc.LookDirection, pc.UpDirection);
            WriteTransform("Frame", t);

            WriteParameter("Focus Distance", FocusDistance);
            WriteParameter("f-number", Aperture);
            WriteParameter("Lens Samples", LensSamples);
            WriteParameter("Blades", 6);
            WriteParameter("Diaphragm", "Circular");
            WriteParameter("Projection", "Planar");

            WriteEndObject();
        }

        protected override void ExportLight(Light l, Transform3D t)
        {
            if (l is AmbientLight)
            {
                return;
            }
            string name = GetUniqueName(l, l.GetType().Name);

            var d = l as DirectionalLight;
            var s = l as SpotLight;
            var p = l as PointLight;

            WriteStartObject("./Lights/" + name, "Default Light", name, "Light");
            {
                string stype = "Projector Light";
                if (s != null)
                {
                    stype = "Spot Light";
                }
                if (p != null)
                {
                    stype = "Omni Light";
                }
                WriteStartObject(stype, stype, "", "Emittance");

                // emitter Radiance
                WriteStartObject("./Radiance/Constant Texture", "Constant Texture", "", "Texture");
                var c = Colors.White;
                WriteParameter("Color", c);
                WriteEndObject();

                // var v = new Vector3D(l.Color.R, l.Color.G, l.Color.B);
                // double lum = v.Length;


                WriteParameter("Attenuation", "None");

                // SpotLight (Spot Light)
                if (s != null)
                {
                    // todo : export the specular parameters
                    // s.ConstantAttenuation
                    // s.LinearAttenuation
                    // s.QuadraticAttenuation

                    WriteParameter("Fall Off", s.OuterConeAngle);
                    WriteParameter("Hot Spot", s.InnerConeAngle);
                }

                // DirectionalLight (Projector Light)
                if (d != null)
                {
                    WriteParameter("Width", 2.0);
                    WriteParameter("Height", 2.0);
                }
                // PointLight (Omni light)
                if (p != null)
                {
                    // todo: export pointlight parameters
                    // p.ConstantAttenuation
                    // p.LinearAttenuation
                    // p.QuadraticAttenuation
                    // p.Range // distance beyond which the light has no effect
                }

                WriteParameter("Focal Length", 1.0);

                WriteEndObject(); // stype

                WriteParameter("Enabled", true);
                WriteParameter("Shadow", Shadows);
                WriteParameter("Soft Shadow", SoftShadows);

                WriteParameter("Negative Light", false);
                WriteParameter("Global Photons", true);
                WriteParameter("Caustic Photons", true);
                WriteParameter("Multiplier", LightMultiplier);

                Matrix3D transform;
                var upVector = new Vector3D(0, 0, 1);
                if (s != null)
                {
                    transform = CreateTransform(s.Position, s.Direction, upVector);
                    WriteTransform("Frame", transform);
                }
                if (d != null)
                {
                    var origin = new Point3D(-1000 * d.Direction.X, -1000 * d.Direction.Y, -1000 * d.Direction.Z);
                    transform = CreateTransform(origin, d.Direction, upVector);
                    WriteTransform("Frame", transform);
                }
                if (p != null)
                {
                    var direction = new Vector3D(-p.Position.X, -p.Position.Y, -p.Position.Z);
                    transform = CreateTransform(p.Position, direction, upVector);
                    WriteTransform("Frame", transform);
                }
                WriteParameter("Focus Distance", 4.0);
                WriteParameter("Radius", 0.2);
                WriteParameter("Shadow Color", ShadowColor);
            }
            WriteEndObject();
        }


        /// <summary>
        /// Create transform from the original coordinate system to the system defined by translation origin
        /// </summary>
        private static Matrix3D CreateTransform(Point3D origin, Vector3D direction, Vector3D up)
        {
            var z = direction;
            var x = Vector3D.CrossProduct(direction, up);
            var y = up;

            x.Normalize();
            y.Normalize();
            z.Normalize();

            var m = new Matrix3D(x.X, y.X, z.X, 0, x.Y, y.Y, z.Y, 0, x.Z, y.Z, z.Z, 0, origin.X, origin.Y, origin.Z, 1);

            return m;
        }

        public void WriteTransform(string name, Matrix3D m)
        {
            string value = String.Format(CultureInfo.InvariantCulture,
                                         "{0:0.######} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######} {9:0.######} {10:0.######} {11:0.######}",
                                         m.M11, m.M12, m.M13, m.OffsetX,
                                         m.M21, m.M22, m.M23, m.OffsetY,
                                         m.M31, m.M32, m.M33, m.OffsetZ);

            WriteParameter(name, "Transform", value);
        }

        // Transposed version
        public void WriteTransformT(string name, Matrix3D m)
        {
            string value = String.Format(CultureInfo.InvariantCulture,
                                         "{0:0.######} {1:0.######} {2:0.######} {3:0.######} {4:0.######} {5:0.######} {6:0.######} {7:0.######} {8:0.######} {9:0.######} {10:0.######} {11:0.######}",
                                         m.M11, m.M21, m.M31, m.OffsetX,
                                         m.M12, m.M22, m.M32, m.OffsetY,
                                         m.M13, m.M23, m.M33, m.OffsetZ);

            WriteParameter(name, "Transform", value);
        }

        public void ExportMesh(MeshGeometry3D m)
        {
            WriteStartObject("Triangular Mesh", "Triangular Mesh", "", "Surface");

            writer.WriteStartElement("Parameter");
            {
                writer.WriteAttributeString("Name", "Vertex List");
                writer.WriteAttributeString("Type", "Point3D List");
                writer.WriteAttributeString("Value", m.Positions.Count.ToString());
                foreach (var p in m.Positions)
                {
                    writer.WriteStartElement("P");
                    writer.WriteAttributeString("xyz", ToKerkytheaString(p));
                    writer.WriteEndElement();
                }
            }
            writer.WriteFullEndElement();

            int triangles = m.TriangleIndices.Count / 3;

            // NORMALS
            // todo: write normal list per vertex instead of per triangle index
            if (m.Normals.Count > 0)
            {
                writer.WriteStartElement("Parameter");
                {
                    writer.WriteAttributeString("Name", "Normal List");
                    writer.WriteAttributeString("Type", "Point3D List");
                    writer.WriteAttributeString("Value", m.TriangleIndices.Count.ToString());
                    foreach (int index in m.TriangleIndices)
                    {
                        if (index >= m.Normals.Count)
                        {
                            continue;
                        }
                        var n = m.Normals[index];
                        writer.WriteStartElement("P");
                        writer.WriteAttributeString("xyz", ToKerkytheaString(n));
                        writer.WriteEndElement();
                    }
                }
                writer.WriteFullEndElement();
            }

            // TRIANGLE INDICES
            writer.WriteStartElement("Parameter");
            {
                writer.WriteAttributeString("Name", "Index List");
                writer.WriteAttributeString("Type", "Triangle Index List");
                writer.WriteAttributeString("Value", triangles.ToString());
                for (int a = 0; a < triangles; a++)
                {
                    int i = m.TriangleIndices[a * 3];
                    int j = m.TriangleIndices[a * 3 + 1];
                    int k = m.TriangleIndices[a * 3 + 2];
                    writer.WriteStartElement("F");
                    writer.WriteAttributeString("ijk", String.Format("{0} {1} {2}", i, j, k));
                    writer.WriteEndElement();
                }
            }
            writer.WriteFullEndElement();

            WriteParameter("Smooth", true);
            WriteParameter("AA Tolerance", 15.0);

            WriteEndObject();
        }

        protected override void ExportModel(GeometryModel3D g, Transform3D transform)
        {
            var mesh = g.Geometry as MeshGeometry3D;
            if (mesh == null)
            {
                return;
            }

            string name = GetUniqueName(g, g.GetType().Name);
            WriteStartObject("./Models/" + name, "Default Model", name, "Model");

            ExportMesh(mesh);

            if (g.Material != null)
            {
                ExportMaterial(g.Material);
            }

            var tg = new Transform3DGroup();
            tg.Children.Add(g.Transform);
            tg.Children.Add(transform);

            ExportMapChannel(mesh);

            WriteTransformT("Frame", tg.Value);

            WriteParameter("Enabled", true);
            WriteParameter("Visible", true);
            WriteParameter("Shadow Caster", true);
            WriteParameter("Shadow Receiver", true);
            WriteParameter("Caustics Transmitter", true);
            WriteParameter("Caustics Receiver", true);
            WriteParameter("Exit Blocker", false);

            WriteEndObject();
        }

        private void ExportMapChannel(MeshGeometry3D m)
        {
            writer.WriteStartElement("Parameter");
            {
                writer.WriteAttributeString("Name", "Map Channel");
                writer.WriteAttributeString("Type", "Point2D List");
                int n = m.TriangleIndices.Count;
                writer.WriteAttributeString("Value", n.ToString());
                foreach (int index in m.TriangleIndices)
                {
                    if (index >= m.TextureCoordinates.Count)
                    {
                        continue;
                    }
                    var uv = m.TextureCoordinates[index];
                    writer.WriteStartElement("P");
                    writer.WriteAttributeString("xy", ToKerkytheaString(uv));
                    writer.WriteEndElement();
                }
            }
            writer.WriteFullEndElement();
        }


        private Color GetSolidColor(Brush brush, Color fallbackColor)
        {
            var scb = brush as SolidColorBrush;
            if (scb != null)
            {
                return scb.Color;
            }
            return fallbackColor;
        }

        private void WriteWeight(string identifier, double weight)
        {
            WriteStartObject(identifier, "Weighted Texture", identifier, "Texture");
            WriteStartObject("Constant Texture", "Constant Texture", "", "Texture");
            WriteParameter("Color", Colors.White);
            WriteEndObject();
            WriteParameter("Weight #0", weight);
            WriteEndObject();
        }

        private void WriteDielectricMaterial(string identifier, Color? reflection, Color? refraction,
                                             double indexOfRefraction = 1.0, double dispersion = 0.0,
                                             string nkfile = null)
        {
            WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

            if (reflection.HasValue)
                WriteConstantTexture("Reflection", reflection.Value);
            if (refraction.HasValue)
                WriteConstantTexture("Refraction", refraction.Value);

            WriteParameter("Index of Refraction", indexOfRefraction);
            WriteParameter("Dispersion", dispersion);
            WriteParameter("N-K File", "");
            WriteEndObject();
        }

        private void WriteAshikhminMaterial(string identifier, Color? diffuse, Color? specular,
                                            Color? shininessXMap, Color? shininessYMap, Color? rotationMap,
                                            double shininessX = 100, double shininessY = 100,
                                            double rotation = 0, double indexOfRefraction = 1.0, string nkfile = null)
        {
            WriteStartObject(identifier, "Ashikhmin Material", identifier, "Material");

            if (diffuse.HasValue)
                WriteConstantTexture("Diffuse", diffuse.Value);
            if (specular.HasValue)
                WriteConstantTexture("Specular", specular.Value);
            if (shininessXMap.HasValue)
                WriteConstantTexture("Shininess X Map", shininessXMap.Value);
            if (shininessYMap.HasValue)
                WriteConstantTexture("Shininess Y Map", shininessYMap.Value);
            if (rotationMap.HasValue)
                WriteConstantTexture("RotationMap", rotationMap.Value);

            WriteParameter("Shininess X", shininessX);
            WriteParameter("Shininess Y", shininessY);
            WriteParameter("Rotation", rotation);
            WriteParameter("Attenuation", "Schlick");
            WriteParameter("Index of Refraction", indexOfRefraction);
            WriteParameter("N-K File", nkfile);
            WriteEndObject();
        }

        //<Object Identifier="#1" Label="Snell Material" Name="#1" Type="Material">
        //<Object Identifier="./Reflection/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="1 1 1"/>
        //</Object>
        //<Object Identifier="./Refraction/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="1 1 1"/>
        //</Object>
        //<Parameter Name="Fresnel" Type="Boolean" Value="1"/>
        //<Parameter Name="Dispersion" Type="Boolean" Value="0"/>
        //<Parameter Name="./Index of Refraction/Index" Type="Real" Value="1"/>
        //<Parameter Name="N-K File" Type="String" Value=""/>
        //</Object>

        //        <Object Identifier="#2" Label="Thin Glass Material" Name="#2" Type="Material">
        //<Object Identifier="./Reflectance/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="1 1 1"/>
        //</Object>

        //<Object Identifier="#4" Label="Ward Material" Name="#4" Type="Material">
        //<Object Identifier="./Diffuse/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="0 1 1"/>
        //</Object>
        //<Object Identifier="./Specular/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="0.501961 0.501961 0"/>
        //</Object>
        //<Parameter Name="Roughness X" Type="Real" Value="0.8"/>
        //<Parameter Name="Roughness Y" Type="Real" Value="0.8"/>
        //<Parameter Name="Specular Sampling" Type="Boolean" Value="0"/>
        //<Parameter Name="Rotation" Type="Real" Value="0"/>
        //</Object>

        //<Object Identifier="#5" Label="Lafortune Material" Name="#5" Type="Material">
        //<Object Identifier="./Diffuse/Constant Texture" Label="Constant Texture" Name="" Type="Texture">
        //<Parameter Name="Color" Type="RGB" Value="0 0.501961 0.501961"/>
        //</Object>
        //<Parameter Name="Index of Refraction" Type="Real" Value="1"/>
        //<Parameter Name="Specular Sampling" Type="Boolean" Value="0"/>
        //<Parameter Name="Specular Attenuation" Type="String" Value="Cosine"/>
        //</Object>

        private void WriteWhittedMaterial(string identifier, string texture, Color? diffuse, Color? specular,
                                          Color? refraction,
                                          double shininess = 128.0, double indexOfRefraction = 1.0)
        {
            WriteStartObject(identifier, "Whitted Material", identifier, "Material");

            if (texture != null)
                WriteBitmapTexture("Diffuse", texture);
            if (diffuse.HasValue)
                WriteConstantTexture("Diffuse", diffuse.Value);
            if (specular.HasValue)
                WriteConstantTexture("Specular", specular.Value);
            if (refraction.HasValue)
                WriteConstantTexture("Refraction", refraction.Value);

            WriteParameter("Shininess", shininess);
            WriteParameter("Transmitted Shininess", 128.0);
            WriteParameter("Index of Refraction", indexOfRefraction);
            WriteParameter("Specular Sampling", false);
            WriteParameter("Transmitted Sampling", false);
            WriteParameter("Specular Attenuation", "Cosine");
            WriteParameter("Transmitted Attenuation", "Cosine");

            WriteEndObject();
        }

        private void WriteConstantTexture(string name, Color color)
        {
            WriteStartObject("./" + name + "/Constant Texture", "Constant Texture", "", "Texture");
            WriteParameter("Color", color);
            WriteEndObject();
        }

        private void WriteBitmapTexture(string name, string filename)
        {
            if (!String.IsNullOrEmpty(filename))
            {
                WriteStartObject("./" + name + "/Bitmap Texture", "Bitmap Texture", "", "Texture");
                WriteParameter("Filename", filename);
                WriteParameter("Projection", "UV");
                WriteParameter("Offset X", 0.0);
                WriteParameter("Offset Y", 0.0);
                WriteParameter("Scale X", 1.0);
                WriteParameter("Scale Y", 1.0);
                WriteParameter("Rotation", 0.0);
                WriteParameter("Smooth", true);
                WriteParameter("Inverted", false);
                WriteParameter("Alpha Channel", false);
                WriteEndObject();
            }
        }

        private void ExportMaterial(string name, Material material, ICollection<double> weights)
        {
            var g = material as MaterialGroup;
            if (g != null)
            {
                foreach (var m in g.Children)
                {
                    ExportMaterial(name, m, weights);
                }
            }

            var d = material as DiffuseMaterial;
            if (d != null)
            {
                string texture = null;
                Color? color = null;
                double alpha = 1.0;
                if (d.Brush is SolidColorBrush)
                {
                    color = GetSolidColor(d.Brush, d.Color);
                    alpha = color.Value.A / 255.0;
                }
                else
                    texture = GetTexture(d.Brush, name);

                if (alpha > 0)
                {
                    WriteWhittedMaterial(string.Format("#{0}", weights.Count), texture, color, null, null);
                    weights.Add(alpha);
                }

                // The refractive part
                if (alpha < 1)
                {
                    WriteWhittedMaterial(string.Format("#{0}", weights.Count), null, null, null, Colors.White);
                    weights.Add(1 - alpha);
                }
            }

            var s = material as SpecularMaterial;
            if (s != null)
            {
                var color = GetSolidColor(s.Brush, s.Color);
                //    color = Color.FromArgb((byte)(color.A * factor), (byte)(color.R * factor), (byte)(color.G * factor), (byte)(color.B * factor));

                WriteWhittedMaterial(string.Format("#{0}", weights.Count), null, null, color, null, s.SpecularPower * 0.5);
                double weight = color.A / 255.0;
                weight *= 0.01;
                weights.Add(weight);
            }

            var e = material as EmissiveMaterial;
            if (e != null)
            {
                Trace.WriteLine("KerkytheaExporter: Emissive materials are not yet supported.");
                //Color color = GetSolidColor(e.Brush, d.Color);
                //WriteWhittedMaterial(string.Format("#{0}", weights.Count + 1), color, null, null);
                //    WriteStartObject("./Translucent/Constant Texture", "Constant Texture", "", "Texture");
                //    WriteParameter("Color", e.Color);
                //    WriteEndObject();
            }
        }

        /// <summary>
        /// Texture bitmaps are reused. This dictionary contains a map from brush to filename
        /// </summary>
        private readonly Dictionary<Brush, string> textureFiles = new Dictionary<Brush, string>();

        private string GetTexture(Brush brush, string name)
        {
            // reuse textures
            if (textureFiles.ContainsKey(brush))
                return textureFiles[brush];

            string filename = name + ".png";
            string path = Path.Combine(TexturePath, filename);
            RenderBrush(path, brush, TextureWidth, TextureHeight);

            textureFiles.Add(brush, filename);
            return filename;
        }

        private void ExportMaterial(Material material)
        {
            // If the material is registered, simply output the xml
            if (RegisteredMaterials.ContainsKey(material))
            {
                var doc = RegisteredMaterials[material];
                if (doc != null && doc.DocumentElement != null)
                    foreach (XmlNode e in doc.DocumentElement.ChildNodes)
                    {
                        e.WriteTo(writer);
                    }
                return;
            }

            string name = GetUniqueName(material, "Material");
            WriteStartObject(name, "Layered Material", name, "Material");

            var weights = new List<double>();

            ExportMaterial(name, material, weights);

            //if (Reflections)
            //{
            //    WriteConstantTexture("Reflection", ReflectionColor);
            //}

            for (int i = 0; i < weights.Count; i++)
            {
                WriteWeight("Weight #" + i, weights[i]);
            }


            /*
             switch (MaterialType)
             {
                 case MaterialTypes.Ashikhmin:
                     WriteParameter("Rotation", 0.0);
                     WriteParameter("Attenuation", "Schlick");
                     WriteParameter("Index of Refraction", 1.0);
                     WriteParameter("N-K File", "");
                     break;
                 case MaterialTypes.Diffusive: // Whitted material
                     WriteParameter("Shininess", 60.0);
                     WriteParameter("Transmitted Shininess", 128.0);
                     WriteParameter("Index of Refraction", 1.0);
                     WriteParameter("Specular Sampling", true);
                     WriteParameter("Transmitted Sampling", false);
                     WriteParameter("Specular Attenuation", "Cosine");
                     WriteParameter("Transmitted Attenuation", "Cosine");
                     break;
             }
             */
            WriteEndObject();
        }

        private string GetUniqueName(DependencyObject o, string defaultName)
        {
            var name = o.GetValue(FrameworkElement.NameProperty) as string;
            if (String.IsNullOrEmpty(name))
            {
                int n = 1;
                while (true)
                {
                    // name = defaultName + " #" + n;
                    name = defaultName + n;
                    if (!names.Contains(name))
                    {
                        break;
                    }
                    n++;
                }
            }
            names.Add(name);
            return name;
        }

        // Viewport3D
        //   ModelVisual3D : Visual3D
        //     GeometryModel3D
        //     DirectionalLight
        //     AmbientLight
        //     PointLight
        //     SpotLight
        //     Model3DGroup
        //       Model3DCollection
        //       GeometryModel3D
        //       Model3DGroup
        //   ModelUIElement3D : UIElement3D : Visual3D

        protected override void ExportViewport(Viewport3D v)
        {
            var ambient = Visual3DHelper.Find<AmbientLight>(v);

            // default global settings
            WriteStartObject("Default Global Settings", "Default Global Settings", "", "Global Settings");
            if (ambient != null)
            {
                WriteParameter("Ambient Light", ambient.Color);
            }

            WriteParameter("Background Color", BackgroundColor);
            WriteParameter("Compute Volume Transfer", false);
            WriteParameter("Transfer Recursion Depth", 1);
            WriteParameter("Background Type", "Sky Color");
            WriteParameter("Sky Intensity", 1.0);
            WriteParameter("Sky Frame", "Transform", "1 0 0 0 0 1 0 0 0 0 1 0 ");
            WriteParameter("Sun Direction", "0 0 1");
            WriteParameter("Sky Turbidity", 2.0);
            WriteParameter("Sky Luminance Gamma", 1.2);
            WriteParameter("Sky Chromaticity Gamma", 1.8);
            WriteParameter("Linear Lightflow", true);
            WriteParameter("Index of Refraction", 1.0);
            WriteParameter("Scatter Density", 0.1);
            WriteParameter("./Location/Latitude", 0.0);
            WriteParameter("./Location/Longitude", 0.0);
            WriteParameter("./Location/Timezone", 0);
            WriteParameter("./Location/Date", "0/0/2007");
            WriteParameter("./Location/Time", "12:0:0");
            WriteParameter("./Background Image/Filename", "[No Bitmap]");
            WriteParameter("./Background Image/Projection", "UV");
            WriteParameter("./Background Image/Offset X", 0.0);
            WriteParameter("./Background Image/Offset Y", 0.0);
            WriteParameter("./Background Image/Scale X", 1.0);
            WriteParameter("./Background Image/Scale Y", 1.0);
            WriteParameter("./Background Image/Rotation", 0.0);
            WriteParameter("./Background Image/Smooth", true);
            WriteParameter("./Background Image/Inverted", false);
            WriteParameter("./Background Image/Alpha Channel", false);
            WriteEndObject();

            // Visual3DHelper.Traverse<Light>(v.Children, ExportLight);
            // Visual3DHelper.Traverse<GeometryModel3D>(v.Children, ExportGeometryModel3D);
        }

        private static double NotNaN(double value, double defaultValue)
        {
            if (double.IsNaN(value))
            {
                return defaultValue;
            }
            return value;
        }

        private static string ToKerkytheaString(Point p)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} {1}", p.X, p.Y);
        }

        private static string ToKerkytheaString(Point3D p)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.######} {1:0.######} {2:0.######}", NotNaN(p.X, 1),
                                 NotNaN(p.Y, 0), NotNaN(p.Z, 0));
        }

        private static string ToKerkytheaString(Vector3D p)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.######} {1:0.######} {2:0.######}", NotNaN(p.X, 1),
                                 NotNaN(p.Y, 0), NotNaN(p.Z, 0));
        }

        private static string ToKerkytheaString(Color c)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0:0.######} {1:0.######} {2:0.######}", c.R / 255.0,
                                 c.G / 255.0, c.B / 255.0);
        }

        #region helper methods

        private void WriteParameter(string name, string type, string value)
        {
            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("Name", name);
            writer.WriteAttributeString("Type", type);
            writer.WriteAttributeString("Value", value);
            writer.WriteEndElement();
        }

        private void WriteParameter(string p, string value)
        {
            WriteParameter(p, "String", value);
        }

        private void WriteParameter(string p, Color color)
        {
            WriteParameter(p, "RGB", ToKerkytheaString(color));
        }

        private void WriteParameter(string p, bool flag)
        {
            WriteParameter(p, "Boolean", flag ? "1" : "0");
        }

        private void WriteParameter(string p, double value)
        {
            WriteParameter(p, "Real", value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteParameter(string p, int value)
        {
            WriteParameter(p, "Integer", value.ToString(CultureInfo.InvariantCulture));
        }

        protected void WriteObject(string identifier, string label, string name, string type)
        {
            WriteStartObject(identifier, label, name, type);
            WriteEndObject();
        }

        protected void WriteStartObject(string identifier, string label, string name, string type)
        {
            writer.WriteStartElement("Object");
            writer.WriteAttributeString("Identifier", identifier);
            writer.WriteAttributeString("Label", label);
            writer.WriteAttributeString("Name", name);
            writer.WriteAttributeString("Type", type);
        }

        protected void WriteEndObject()
        {
            writer.WriteFullEndElement();
        }

        #endregion
    }
}