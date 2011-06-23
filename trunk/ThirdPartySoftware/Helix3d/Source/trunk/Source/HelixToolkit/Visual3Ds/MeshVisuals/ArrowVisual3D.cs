using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class ArrowVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof (double), typeof (ArrowVisual3D),
                                        new UIPropertyMetadata(1.0, GeometryChanged));

        //public static readonly DependencyProperty HeadLengthProperty =
        //    DependencyProperty.Register("HeadLength", typeof (double), typeof (ArrowVisual3D),
        //                                new UIPropertyMetadata(3.0, GeometryChanged));

        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register("Point1", typeof (Point3D), typeof (ArrowVisual3D),
                                        new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register("Point2", typeof (Point3D), typeof (ArrowVisual3D),
                                        new UIPropertyMetadata(new Point3D(0, 0, 10), GeometryChanged));

        public static readonly DependencyProperty ThetaDivProperty =
            DependencyProperty.Register("ThetaDiv", typeof (int), typeof (ArrowVisual3D),
                                        new UIPropertyMetadata(36, GeometryChanged));

        public double Diameter
        {
            get { return (double) GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        //public double HeadLength
        //{
        //    get { return (double) GetValue(HeadLengthProperty); }
        //    set { SetValue(HeadLengthProperty, value); }
        //}

        public int ThetaDiv
        {
            get { return (int) GetValue(ThetaDivProperty); }
            set { SetValue(ThetaDivProperty, value); }
        }

        public Point3D Point1
        {
            get { return (Point3D) GetValue(Point1Property); }
            set { SetValue(Point1Property, value); }
        }

        public Point3D Point2
        {
            get { return (Point3D) GetValue(Point2Property); }
            set { SetValue(Point2Property, value); }
        }

        public Point3D Origin
        {
            get { return Point1; }
            set { Point1 = value; }
        }

        public Vector3D Direction
        {
            get { return Point2 - Point1; }
            set { Point2 = Point1 + value; }
        }

        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(true, true);
            builder.AddArrow(Point1, Point2, Diameter, 3, ThetaDiv);
            return builder.ToMesh();
        }
    }
}