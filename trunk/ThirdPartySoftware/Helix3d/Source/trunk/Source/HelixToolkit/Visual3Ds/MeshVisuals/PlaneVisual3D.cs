using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// A plane defined by origin and normal, length and width
    /// </summary>
    public class PlaneVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty DivLengthProperty =
            DependencyProperty.Register("DivLength", typeof(int), typeof(PlaneVisual3D), new UIPropertyMetadata(10));

        public static readonly DependencyProperty DivWidthProperty =
            DependencyProperty.Register("DivWidth", typeof(int), typeof(PlaneVisual3D), new UIPropertyMetadata(10));

        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register("LengthDirection", typeof(Vector3D), typeof(PlaneVisual3D),
                                        new PropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(PlaneVisual3D),
                                        new PropertyMetadata(10.0, GeometryChanged));

        public static readonly DependencyProperty NormalProperty =
            DependencyProperty.Register("Normal", typeof(Vector3D), typeof(PlaneVisual3D),
                                        new PropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.Register("Origin", typeof(Point3D), typeof(PlaneVisual3D),
                                        new PropertyMetadata(new Point3D(0, 0, 0), GeometryChanged));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(PlaneVisual3D),
                                        new PropertyMetadata(10.0, GeometryChanged));

        /// <summary>
        /// Gets or sets the center point of the plane.
        /// </summary>
        /// <value>The origin.</value>
        public Point3D Origin
        {
            get { return (Point3D)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        /// <summary>
        /// Gets or sets the normal vector of the plane.
        /// </summary>
        /// <value>The normal.</value>
        public Vector3D Normal
        {
            get { return (Vector3D)GetValue(NormalProperty); }
            set { SetValue(NormalProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length direction.
        /// </summary>
        /// <value>The length direction.</value>
        public Vector3D LengthDirection
        {
            get { return (Vector3D)GetValue(LengthDirectionProperty); }
            set { SetValue(LengthDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }


        /// <summary>
        /// Gets or sets the number of divisions in the 'length' direction.
        /// </summary>
        /// <value>The number of divisions.</value>
        public int DivLength
        {
            get { return (int)GetValue(DivLengthProperty); }
            set { SetValue(DivLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the number of divisions in the 'width' direction.
        /// </summary>
        /// <value>The number of divisions.</value>
        public int DivWidth
        {
            get { return (int)GetValue(DivWidthProperty); }
            set { SetValue(DivWidthProperty, value); }
        }

        protected override MeshGeometry3D Tessellate()
        {

            Vector3D u = LengthDirection;
            Vector3D w = Normal;
            Vector3D v = Vector3D.CrossProduct(w, u);
            u = Vector3D.CrossProduct(v, w);

            u.Normalize();
            v.Normalize();
            w.Normalize();

            double le = Length;
            double wi = Width;

            var pts = new Point3DCollection();
            for (int i = 0; i < DivLength; i++)
            {
                double fi = -0.5 + (double)i / (DivLength - 1);
                for (int j = 0; j < DivWidth; j++)
                {
                    double fj = -0.5 + (double)j / (DivWidth - 1);
                    pts.Add(Origin + u * le * fi + v * wi * fj);
                }
            }

            var builder = new MeshBuilder();
            builder.AddRectangularMesh(pts, DivWidth);

            return builder.ToMesh();
        }
    }
}