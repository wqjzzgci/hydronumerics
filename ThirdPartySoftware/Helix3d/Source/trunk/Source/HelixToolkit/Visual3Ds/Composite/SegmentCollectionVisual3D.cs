using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit
{
    // todo: this is under construction...
    public class SegmentCollectionVisual3D : ModelVisual3D
    {
        public Point3DCollection Positions { get; set; }
        public Int32Collection SegmentIndices { get; set; }

        public int Segments { get { return SegmentIndices.Count / 2; } }

        public Brush Fill { get; set; }

        public double Diameter { get; set; }
        public int ThetaDiv { get; set; }

        MeshGeometry3D _tube;

        readonly ModelVisual3D _element;

        public SegmentCollectionVisual3D()
        {
            Positions = new Point3DCollection();
            SegmentIndices = new Int32Collection();
            Fill = Brushes.Blue;
            Diameter = 0.1;
            ThetaDiv = 37;

            _element = new ModelVisual3D();
            Children.Add(_element);
        }

        public void Add(Point3D p1, Point3D p2)
        {
            int i0 = Positions.Count;
            Positions.Add(p1);
            Positions.Add(p2);
            SegmentIndices.Add(i0);
            SegmentIndices.Add(i0 + 1);
        }

        void CreateGeometry()
        {

            double r = Diameter / 2;

            var pc = new PointCollection();
            pc.Add(new Point(0, 0));
            pc.Add(new Point(0, r));
            pc.Add(new Point(1, r));
            pc.Add(new Point(1, 0));

            var builder = new MeshBuilder();
            builder.AddRevolvedGeometry(pc, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), ThetaDiv);
            _tube= builder.ToMesh();
            _tube.Freeze();
        }


        public void UpdateModel()
        {
            CreateGeometry();
            var c = new Model3DGroup();
            var mat = MaterialHelper.CreateMaterial(Fill);

            for (int i = 0; i < Segments; i++)
            {
                var p0 = Positions[SegmentIndices[i * 2]];
                var p1 = Positions[SegmentIndices[i * 2 + 1]];
                var d = p1 - p0;
                var tubeModel = new GeometryModel3D
                                    {
                                        Geometry = _tube,
                                        Material = mat,
                                        Transform = CreateSegmentTransform(p0, d)
                                    };
                c.Children.Add(tubeModel);
            }

            _element.Content = c;
        }

        private static Transform3D CreateSegmentTransform(Point3D p, Vector3D z)
        {
            double length = z.Length;
            z.Normalize();
            var x = z.FindAnyPerpendicular();
            x.Normalize();
            var y = Vector3D.CrossProduct(z, x);

            var mat = new Matrix3D(x.X, x.Y, x.Z, 0, y.X, y.Y, y.Z, 0, z.X*length, z.Y*length, z.Z*length, 0, p.X, p.Y, p.Z, 1);
            return new MatrixTransform3D(mat);
        }

    }
}
