using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The BoxVisual3D renders a box aligned with the local X, Y and Z coordinate system.
    /// Use a transform to orient the box in other directions.
    /// </summary>
    public class BoxVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty BottomFaceProperty =
            DependencyProperty.Register("BottomFace", typeof(bool), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(true, GeometryChanged));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point3D), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(new Point3D(), GeometryChanged));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(1.0, GeometryChanged));

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(1.0, GeometryChanged));

        public static readonly DependencyProperty TopFaceProperty =
            DependencyProperty.Register("TopFace", typeof(bool), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(true, GeometryChanged));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(BoxVisual3D),
                                        new UIPropertyMetadata(1.0, GeometryChanged));

        public bool TopFace
        {
            get { return (bool)GetValue(TopFaceProperty); }
            set { SetValue(TopFaceProperty, value); }
        }

        public bool BottomFace
        {
            get { return (bool)GetValue(BottomFaceProperty); }
            set { SetValue(BottomFaceProperty, value); }
        }

        public Point3D Center
        {
            get { return (Point3D)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public double Length
        {
            get { return (double)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        protected override MeshGeometry3D Tessellate()
        {
            var b = new MeshBuilder();
            b.AddCubeFace(Center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), Length, Width, Height);
            b.AddCubeFace(Center, new Vector3D(1, 0, 0), new Vector3D(0, 0, -1), Length, Width, Height);
            b.AddCubeFace(Center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), Width, Length, Height);
            b.AddCubeFace(Center, new Vector3D(0, 1, 0), new Vector3D(0, 0, -1), Width, Length, Height);
            if (TopFace)
                b.AddCubeFace(Center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), Height, Length, Width);
            if (BottomFace)
                b.AddCubeFace(Center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), Height, Length, Width);
            return b.ToMesh();
        }
    }
}