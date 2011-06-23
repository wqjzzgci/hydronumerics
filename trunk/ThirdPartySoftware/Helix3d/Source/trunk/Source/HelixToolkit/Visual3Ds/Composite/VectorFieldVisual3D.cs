using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit
{
    // todo: this is under construction...
    public class VectorFieldVisual3D : ModelVisual3D
    {
        public Point3DCollection Positions { get; set; }
        public Vector3DCollection Directions { get; set; }

        public Brush Fill { get; set; }

        public double Diameter { get; set; }

        /// <summary>
        /// Length of head in diameters
        /// </summary>
        public double HeadLength { get; set; }
        public int ThetaDiv { get; set; }

        MeshGeometry3D _head;
        MeshGeometry3D _body;

        readonly ModelVisual3D _element;

        public VectorFieldVisual3D()
        {
            Positions = new Point3DCollection();
            Directions = new Vector3DCollection();
            Fill = Brushes.Blue;
            ThetaDiv = 37;
            Diameter = 1;
            HeadLength = 2;

            _element = new ModelVisual3D();
            Children.Add(_element);
        }

        void CreateGeometry()
        {

            double r = Diameter / 2;
            double l = HeadLength * Diameter;

            // arrowhead
            var pc = new PointCollection();
            pc.Add(new Point(-l, r));
            pc.Add(new Point(-l, r * 2));
            pc.Add(new Point(0, 0));

            var headBuilder = new MeshBuilder();
            headBuilder.AddRevolvedGeometry(pc, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), ThetaDiv);
            _head = headBuilder.ToMesh();
            _head.Freeze(); 
            

            // body
            pc = new PointCollection();
            pc.Add(new Point(0, 0));
            pc.Add(new Point(0, r));
            pc.Add(new Point(1, r));

            var bodyBuilder = new MeshBuilder();
            bodyBuilder.AddRevolvedGeometry(pc, new Point3D(0, 0, 0), new Vector3D(0, 0, 1), ThetaDiv);
            _body = bodyBuilder.ToMesh();
            _body.Freeze();
        }


        public void UpdateModel()
        {
            CreateGeometry();
            var c = new Model3DGroup();
            var mat = MaterialHelper.CreateMaterial(Fill);
            double l = HeadLength * Diameter;

            for (int i = 0; i < Positions.Count; i++)
            {
                var p = Positions[i];
                var d = Directions[i];
                var headModel = new GeometryModel3D
                                    {
                                        Geometry = _head,
                                        Material = mat,
                                        Transform = CreateHeadTransform(p + d, d)
                                    };
                c.Children.Add(headModel);

                var u = d;
                u.Normalize();
                var bodyModel = new GeometryModel3D
                                    {
                                        Geometry = _body,
                                        Material = mat,
                                        Transform = CreateBodyTransform(p, u*(1.0 - l/d.Length))
                                    };
                c.Children.Add(bodyModel);

            }

            _element.Content = c;
        }

        private static Transform3D CreateHeadTransform(Point3D p, Vector3D z)
        {
            z.Normalize();
            var x = z.FindAnyPerpendicular();
            x.Normalize();
            var y = Vector3D.CrossProduct(z, x);

            var mat = new Matrix3D(
                x.X, x.Y, x.Z, 0,
                y.X, y.Y, y.Z, 0,
                z.X, z.Y, z.Z, 0, p.X, p.Y, p.Z, 1);

            return new MatrixTransform3D(mat);
        }

        private static Transform3D CreateBodyTransform(Point3D p, Vector3D z)
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
