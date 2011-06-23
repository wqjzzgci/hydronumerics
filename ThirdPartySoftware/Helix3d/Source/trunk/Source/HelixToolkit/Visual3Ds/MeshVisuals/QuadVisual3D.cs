using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// A quadrilateral defined by the four corner points.
    /// 
    /// Point4          Point3
    ///   +---------------+
    ///   |               |
    ///   |               |
    ///   +---------------+
    /// Point1          Point2
    /// 
    /// The texture coordinates are
    /// (0,0)           (1,0)
    ///   +---------------+
    ///   |               |
    ///   |               |
    ///   +---------------+
    /// (0,1)          (1,1)
    /// </summary>
    public class QuadVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty Point1Property =
            DependencyProperty.Register("Point1", typeof(Point3D), typeof(QuadVisual3D),
                                        new UIPropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        public static readonly DependencyProperty Point2Property =
            DependencyProperty.Register("Point2", typeof(Point3D), typeof(QuadVisual3D),
                                        new UIPropertyMetadata(new Point3D(1, 0, 0), GeometryChanged));

        public static readonly DependencyProperty Point3Property =
            DependencyProperty.Register("Point3", typeof(Point3D), typeof(QuadVisual3D),
                                        new UIPropertyMetadata(new Point3D(1, 1, 0), GeometryChanged));

        public static readonly DependencyProperty Point4Property =
            DependencyProperty.Register("Point4", typeof(Point3D), typeof(QuadVisual3D),
                                        new UIPropertyMetadata(new Point3D(0, 1, 0), GeometryChanged));

        public Point3D Point1
        {
            get { return (Point3D)GetValue(Point1Property); }
            set { SetValue(Point1Property, value); }
        }

        public Point3D Point2
        {
            get { return (Point3D)GetValue(Point2Property); }
            set { SetValue(Point2Property, value); }
        }

        public Point3D Point3
        {
            get { return (Point3D)GetValue(Point3Property); }
            set { SetValue(Point3Property, value); }
        }

        public Point3D Point4
        {
            get { return (Point3D)GetValue(Point4Property); }
            set { SetValue(Point4Property, value); }
        }

        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder();
            builder.AddQuad(Point1, Point2, Point3, Point4, new Point(0, 1), new Point(1, 1), new Point(1, 0), new Point(0, 0));
            return builder.ToMesh();
        }
    }
}