using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The CoordinateSystemVisual3D renders a coordinate system with arrows in the X, Y and Z directions.
    /// </summary>
    public class CoordinateSystemVisual3D : ModelVisual3D
    {
        public static readonly DependencyProperty ArrowLengthsProperty =
            DependencyProperty.Register("ArrowLengths", typeof (double), typeof (CoordinateSystemVisual3D),
                                        new UIPropertyMetadata(1.0, GeometryChanged));

        public CoordinateSystemVisual3D()
        {
            UpdateVisuals();
        }

        public double ArrowLengths
        {
            get { return (double) GetValue(ArrowLengthsProperty); }
            set { SetValue(ArrowLengthsProperty, value); }
        }

        protected static void GeometryChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((CoordinateSystemVisual3D) obj).UpdateVisuals();
        }


        private void UpdateVisuals()
        {
            Children.Clear();
            double l = ArrowLengths;
            double d = l*0.1;
            Children.Add(new ArrowVisual3D
                             {Point2 = new Point3D(l, 0, 0), Diameter = d, Fill = Brushes.Red});
            Children.Add(new ArrowVisual3D
                             {Point2 = new Point3D(0, l, 0), Diameter = d, Fill = Brushes.Green});
            Children.Add(new ArrowVisual3D
                             {Point2 = new Point3D(0, 0, l), Diameter = d, Fill = Brushes.Blue});
            Children.Add(new CubeVisual3D {SideLength = d, Fill = Brushes.Black});
        }
    }
}