using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The BoundingBoxVisual3D shows a wireframe for the specified Rect3D.
    /// </summary>
    public class BoundingBoxVisual3D : ModelVisual3D
    {
        public static readonly DependencyProperty BoundingBoxProperty =
            DependencyProperty.Register("BoundingBox", typeof (Rect3D), typeof (BoundingBoxVisual3D),
                                        new UIPropertyMetadata(new Rect3D(), BoxChanged));

        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof (double), typeof (BoundingBoxVisual3D),
                                        new UIPropertyMetadata(0.1, BoxChanged));

        public Rect3D BoundingBox
        {
            get { return (Rect3D) GetValue(BoundingBoxProperty); }
            set { SetValue(BoundingBoxProperty, value); }
        }

        public double Diameter
        {
            get { return (double) GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        private static void BoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BoundingBoxVisual3D) d).UpdateBox();
        }


        private void UpdateBox()
        {
            Children.Clear();
            if (BoundingBox.IsEmpty)
                return;

            Rect3D bb = BoundingBox;

            var p0 = new Point3D(bb.X, bb.Y, bb.Z);
            var p1 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z);
            var p2 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z);
            var p3 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z);
            var p4 = new Point3D(bb.X, bb.Y, bb.Z + bb.SizeZ);
            var p5 = new Point3D(bb.X, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p6 = new Point3D(bb.X + bb.SizeX, bb.Y + bb.SizeY, bb.Z + bb.SizeZ);
            var p7 = new Point3D(bb.X + bb.SizeX, bb.Y, bb.Z + bb.SizeZ);

            AddEdge(p0, p1);
            AddEdge(p1, p2);
            AddEdge(p2, p3);
            AddEdge(p3, p0);

            AddEdge(p4, p5);
            AddEdge(p5, p6);
            AddEdge(p6, p7);
            AddEdge(p7, p4);

            AddEdge(p0, p4);
            AddEdge(p1, p5);
            AddEdge(p2, p6);
            AddEdge(p3, p7);
        }

        private void AddEdge(Point3D p1, Point3D p5)
        {
            var fv = new PipeVisual3D();
            fv.Diameter = Diameter;
            fv.ThetaDiv = 10;
            fv.Fill = new SolidColorBrush(Colors.Yellow);
            fv.Point1 = p1;
            fv.Point2 = p5;
            Children.Add(fv);
        }
    }
}