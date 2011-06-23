using System.Windows;

namespace HelixToolkit
{
    /// <summary>
    /// A Cube visual.
    /// </summary>
    public class CubeVisual3D : BoxVisual3D
    {
        public static readonly DependencyProperty SideLengthProperty =
            DependencyProperty.Register("SideLength", typeof (double), typeof (CubeVisual3D),
                                        new UIPropertyMetadata(1.0, SideLengthChanged));

        public double SideLength
        {
            get { return (double) GetValue(SideLengthProperty); }
            set { SetValue(SideLengthProperty, value); }
        }

        private static void SideLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (CubeVisual3D) d;
            // todo: turn of tesselate
            c.Length = c.Height = c.Width = c.SideLength;
            // todo: do tesselate
        }
    }
}