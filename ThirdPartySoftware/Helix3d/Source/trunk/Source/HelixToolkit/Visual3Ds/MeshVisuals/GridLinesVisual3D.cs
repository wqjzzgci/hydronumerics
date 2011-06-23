using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class GridLinesVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point3D), typeof(GridLinesVisual3D),
                                        new UIPropertyMetadata(new Point3D(), GeometryChanged));

        public static readonly DependencyProperty DistanceProperty =
            DependencyProperty.Register("MinorDistance", typeof(double), typeof(GridLinesVisual3D),
                                        new PropertyMetadata(2.5, GeometryChanged));

        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof(double), typeof(GridLinesVisual3D),
                                        new PropertyMetadata(200.0, GeometryChanged));

        public static readonly DependencyProperty LengthDirectionProperty =
            DependencyProperty.Register("LengthDirection", typeof(Vector3D), typeof(GridLinesVisual3D),
                                        new UIPropertyMetadata(new Vector3D(1, 0, 0), GeometryChanged));

        public static readonly DependencyProperty MajorDistanceProperty =
            DependencyProperty.Register("MajorDistance", typeof(double), typeof(GridLinesVisual3D),
                                        new PropertyMetadata(10.0, GeometryChanged));

        public static readonly DependencyProperty NormalProperty =
            DependencyProperty.Register("Normal", typeof(Vector3D), typeof(GridLinesVisual3D),
                                        new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register("Thickness", typeof(double), typeof(GridLinesVisual3D),
                                        new PropertyMetadata(0.08, GeometryChanged));

        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(GridLinesVisual3D),
                                        new PropertyMetadata(200.0, GeometryChanged));

        private Vector3D lengthDirection;
        private Vector3D widthDirection;

        public Point3D Center
        {
            get { return (Point3D)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public Vector3D Normal
        {
            get { return (Vector3D)GetValue(NormalProperty); }
            set { SetValue(NormalProperty, value); }
        }

        public Vector3D LengthDirection
        {
            get { return (Vector3D)GetValue(LengthDirectionProperty); }
            set { SetValue(LengthDirectionProperty, value); }
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

        public double MinorDistance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }

        public double MajorDistance
        {
            get { return (double)GetValue(MajorDistanceProperty); }
            set { SetValue(MajorDistanceProperty, value); }
        }

        public double Thickness
        {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        public GridLinesVisual3D()
        {
            Fill = Brushes.Gray;
        }

        protected override MeshGeometry3D Tessellate()
        {
            lengthDirection = LengthDirection;
            lengthDirection.Normalize();
            widthDirection = Vector3D.CrossProduct(Normal, lengthDirection);
            widthDirection.Normalize();

            var mesh = new MeshBuilder(true,false);
            double minX = -Width / 2;
            double minY = -Length / 2;
            double maxX = Width / 2;
            double maxY = Length / 2;

            double x = minX;
            double eps = MinorDistance / 10;
            while (x <= maxX + eps)
            {
                double t = Thickness;
                if (IsMultipleOf(x, MajorDistance))
                    t *= 2;
                AddLineX(mesh, x, minY, maxY, t);
                x += MinorDistance;
            }

            double y = minY;
            while (y <= maxY + eps)
            {
                double t = Thickness;
                if (IsMultipleOf(y, MajorDistance))
                    t *= 2;
                AddLineY(mesh, y, minX, maxX, t);
                y += MinorDistance;
            }
            var m=mesh.ToMesh();
            m.Freeze();
            return m;
        }

        private static bool IsMultipleOf(double y, double d)
        {
            double y2 = d * (int)(y / d);
            return Math.Abs(y - y2) < 1e-3;
        }

        private Point3D GetPoint(double x, double y)
        {
            return Center + widthDirection * x + lengthDirection * y;
        }

        private void AddLineY(MeshBuilder mesh, double y, double minX, double maxX, double thickness)
        {
            int i0 = mesh.Positions.Count;
            mesh.Positions.Add(GetPoint(minX, y + thickness / 2));
            mesh.Positions.Add(GetPoint(maxX, y + thickness / 2));
            mesh.Positions.Add(GetPoint(maxX, y - thickness / 2));
            mesh.Positions.Add(GetPoint(minX, y - thickness / 2));
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.TriangleIndices.Add(i0);
            mesh.TriangleIndices.Add(i0 + 1);
            mesh.TriangleIndices.Add(i0 + 2);
            mesh.TriangleIndices.Add(i0 + 2);
            mesh.TriangleIndices.Add(i0 + 3);
            mesh.TriangleIndices.Add(i0);
        }

        private void AddLineX(MeshBuilder mesh, double x, double minY, double maxY, double thickness)
        {
            int i0 = mesh.Positions.Count;
            mesh.Positions.Add(GetPoint(x - thickness / 2, minY));
            mesh.Positions.Add(GetPoint(x - thickness / 2, maxY));
            mesh.Positions.Add(GetPoint(x + thickness / 2, maxY));
            mesh.Positions.Add(GetPoint(x + thickness / 2, minY));
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.Normals.Add(Normal);
            mesh.TriangleIndices.Add(i0);
            mesh.TriangleIndices.Add(i0 + 1);
            mesh.TriangleIndices.Add(i0 + 2);
            mesh.TriangleIndices.Add(i0 + 2);
            mesh.TriangleIndices.Add(i0 + 3);
            mesh.TriangleIndices.Add(i0);
        }
    }
}