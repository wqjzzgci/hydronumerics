using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// A visual for the specified light
    /// </summary>
    public class LightVisual3D : ModelVisual3D
    {
        public static readonly DependencyProperty LightProperty =
            DependencyProperty.Register("Light", typeof (Light), typeof (LightVisual3D),
                                        new UIPropertyMetadata(null, LightChanged));

        public Light Light
        {
            get { return (Light) GetValue(LightProperty); }
            set { SetValue(LightProperty, value); }
        }

        protected static void LightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((LightVisual3D) obj).UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Children.Clear();
            if (Light == null) return;

            var dl = Light as DirectionalLight;
            if (dl != null)
            {
                var arrow = new ArrowVisual3D();
                double distance = 10;
                double length = 5;
                arrow.Point1 = new Point3D() + dl.Direction*distance;
                arrow.Point2 = arrow.Point1 - dl.Direction*length;
                arrow.Diameter = 0.1*length;
                arrow.Fill = new SolidColorBrush(dl.Color);
                Children.Add(arrow);
            }

            var sl = Light as SpotLight;
            if (sl != null)
            {
                var sphere = new SphereVisual3D();
                sphere.Center = sl.Position;
                sphere.Fill = new SolidColorBrush(sl.Color);
                Children.Add(sphere);

                var arrow = new ArrowVisual3D();
                arrow.Point1 = sl.Position;
                arrow.Point2 = sl.Position + sl.Direction;
                arrow.Diameter = 0.1;
                Children.Add(arrow);
            }

            var pl = Light as PointLight;
            if (pl != null)
            {
                var sphere = new SphereVisual3D();
                sphere.Center = pl.Position;
                sphere.Fill = new SolidColorBrush(pl.Color);
                Children.Add(sphere);
            }

            var al = Light as AmbientLight;
        }
    }
}