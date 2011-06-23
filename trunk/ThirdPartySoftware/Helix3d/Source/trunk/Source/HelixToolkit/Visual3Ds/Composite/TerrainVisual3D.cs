using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit
{
    /// <summary>
    /// Terrain model - reading model from .bt files
    /// The origin of  model will be centered around the midpoint of the model.
    /// Also support a ".btz" format - compressed with gzip. A compression method to convert from ".bt" to ".btz" can be found in the GZipHelper.
    /// No advanced LOD algorithm imported - this is for small terrains only...
    /// </summary>
    public class TerrainVisual3D : ModelVisual3D
    {
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(TerrainVisual3D), new UIPropertyMetadata(null, SourceChanged));

        protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((TerrainVisual3D)obj).UpdateModel();
        }

        private readonly ModelVisual3D visualChild;

        public TerrainVisual3D()
        {
            visualChild = new ModelVisual3D();
            Children.Add(visualChild);
        }

        void UpdateModel()
        {
            var r = new TerrainModel();
            r.Load(Source);
            //r.Texture = new SlopeDirectionTexture(0);
            r.Texture = new SlopeTexture(8);
            // r.Texture = new MapTexture(@"D:\tmp\CraterLake.png") { Left = r.Left, Right = r.Right, Top = r.Top, Bottom = r.Bottom };
            visualChild.Content = r.CreateModel(2);
        }
    }

    public class TerrainTexture
    {
        public Material Material { get; set; }
        public PointCollection TextureCoordinates { get; set; }

        public TerrainTexture()
        {
            Material = Materials.Green;
        }

        public virtual void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
        }
    }

    /// <summary>
    /// Terrain texture using a bitmap. Set the Left,Right,Bottom and Top coordinates to get the right alignment.
    /// </summary>
    public class MapTexture : TerrainTexture
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }

        public MapTexture(string source)
        {
            Material = MaterialHelper.CreateImageMaterial(source,1);
        }

        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var texcoords = new PointCollection();
            foreach (var p in mesh.Positions)
            {
                double x = p.X + model.Offset.X;
                double y = p.Y + model.Offset.Y;
                double u = (x - Left) / (Right - Left);
                double v = (y - Top) / (Bottom - Top);
                texcoords.Add(new Point(u, v));
            }
            TextureCoordinates = texcoords;
        }
    }

    /// <summary>
    /// Texture by the slope angle.
    /// </summary>
    public class SlopeTexture : TerrainTexture
    {
        public Brush Brush { get; set; }

        public SlopeTexture(int gradientSteps)
        {
            if (gradientSteps > 0)
                Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.BlueWhiteRed, gradientSteps);
            else
                Brush = GradientBrushes.BlueWhiteRed;
        }

        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var normals = MeshGeometryHelper.CalculateNormals(mesh);
            var texcoords = new PointCollection();
            var up = new Vector3D(0, 0, 1);
            for (int i = 0; i < normals.Count; i++)
            {
                double slope = Math.Acos(Vector3D.DotProduct(normals[i], up)) * 180 / Math.PI;
                double u = slope / 40;
                if (u > 1) u = 1;
                if (u < 0) u = 0;
                texcoords.Add(new Point(u, u));
            }
            TextureCoordinates = texcoords;
            Material = MaterialHelper.CreateMaterial(Brush);
        }
    }


    /// <summary>
    /// Texture by the direction of the steepest gradient.
    /// </summary>
    public class SlopeDirectionTexture : TerrainTexture
    {
        public Brush Brush { get; set; }

        public SlopeDirectionTexture(int gradientSteps)
        {
            if (gradientSteps > 0)
                Brush = BrushHelper.CreateSteppedGradientBrush(GradientBrushes.Hue, gradientSteps);
            else
                Brush = GradientBrushes.Hue;
        }
        public override void Calculate(TerrainModel model, MeshGeometry3D mesh)
        {
            var normals = MeshGeometryHelper.CalculateNormals(mesh);
            var texcoords = new PointCollection();
            for (int i = 0; i < normals.Count; i++)
            {
                double slopedir = Math.Atan2(normals[i].Y, normals[i].X) * 180 / Math.PI;
                if (slopedir < 0) slopedir += 360;
                double u = slopedir / 360;
                texcoords.Add(new Point(u, u));
            }
            TextureCoordinates = texcoords;
            Material = MaterialHelper.CreateMaterial(Brush);
        }
    }

    /// <summary>
    /// Read .bt files from disk, keeps the model data and creates the Model3D.
    /// The .btz format is a gzip compressed version of the .bt format.
    /// </summary>
    public class TerrainModel
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Top { get; set; }
        public double MinimumZ { get; set; }
        public double MaximumZ { get; set; }
        public double[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Point3D Offset { get; set; }
        public TerrainTexture Texture { get; set; }

        public void Load(string source)
        {
            var ext = Path.GetExtension(source).ToLower();
            switch (ext)
            {
                case ".btz": ReadBTZ(source);
                    break;
                case ".bt": ReadBT(source);
                    break;
            }
        }

        private void ReadBTZ(string source)
        {
            var infile = File.OpenRead(source);
            var deflateStream = new GZipStream(infile, CompressionMode.Decompress, true);

            ReadBT(deflateStream);
            deflateStream.Close();
            infile.Close();
        }

        private void ReadBT(string source)
        {
            var infile = File.OpenRead(source);
            ReadBT(infile);
            infile.Close();
        }

        /// <summary>
        /// Creates the 3D model of the terrain.
        /// </summary>
        /// <param name="lod">The level of detail.</param>
        /// <returns></returns>
        public GeometryModel3D CreateModel(int lod)
        {

            int ni = Height / lod;
            int nj = Width / lod;
            var pts = new Point3DCollection(ni * nj);

            double mx = (Left + Right) / 2;
            double my = (Top + Bottom) / 2;
            double mz = (MinimumZ + MaximumZ) / 2;

            Offset = new Point3D(mx, my, mz);

            for (int i = 0; i < ni; i++)
                for (int j = 0; j < nj; j++)
                {
                    double x = Left + (Right - Left) * j / (nj - 1);
                    double y = Top + (Bottom - Top) * i / (ni - 1);
                    double z = Data[i * lod * Width + j * lod];

                    x -= Offset.X;
                    y -= Offset.Y;
                    z -= Offset.Z;
                    pts.Add(new Point3D(x, y, z));
                }

            var mb = new MeshBuilder(false, false);
            mb.AddRectangularMesh(pts, nj);
            var mesh = mb.ToMesh();

            var material = Materials.Green;

            if (Texture != null)
            {
                Texture.Calculate(this, mesh);
                material = Texture.Material;
                mesh.TextureCoordinates = Texture.TextureCoordinates;
            }

            var model = new GeometryModel3D();
            model.Geometry = mesh;
            model.Material = material;
            model.BackMaterial = material;
            return model;
        }

        /// <summary>
        /// Reads a .bt (Binary terrain) file.
        /// http://www.vterrain.org/Implementation/Formats/BT.html
        /// </summary>
        /// <param name="s">The stream.</param>
        /// <returns></returns>
        public void ReadBT(Stream s)
        {
            var reader = new BinaryReader(s);

            var buffer = reader.ReadBytes(10);
            var enc = new ASCIIEncoding();
            var marker = enc.GetString(buffer);
            if (!marker.StartsWith("binterr"))
                throw new FileFormatException("Invalid marker.");
            var version = marker.Substring(7);

            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            short dataSize = reader.ReadInt16();
            bool isFloatingPoint = reader.ReadInt16() == 1;
            short horizontalUnits = reader.ReadInt16();
            short utmZone = reader.ReadInt16();
            short datum = reader.ReadInt16();
            Left = reader.ReadDouble();
            Right = reader.ReadDouble();
            Bottom = reader.ReadDouble();
            Top = reader.ReadDouble();
            short proj = reader.ReadInt16();
            float scale = reader.ReadSingle();
            var padding = reader.ReadBytes(190);

            int index = 0;
            Data = new double[Width * Height];
            MinimumZ = double.MaxValue;
            MaximumZ = double.MinValue;

            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    double z;

                    if (dataSize == 2)
                    {
                        z = reader.ReadUInt16();
                    }
                    else
                    {
                        z = isFloatingPoint ? reader.ReadSingle() : reader.ReadUInt32();
                    }
                    Data[index++] = z;
                    if (z < MinimumZ) MinimumZ = z;
                    if (z > MaximumZ) MaximumZ = z;
                }
            reader.Close();
        }
    }
}
