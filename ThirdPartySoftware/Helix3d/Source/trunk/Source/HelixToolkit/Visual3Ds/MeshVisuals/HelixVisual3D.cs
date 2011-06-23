using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The HelixVisual3D renders a Helix.
    /// http://en.wikipedia.org/wiki/Helix
    /// A helix (pl: helixes or helices) is a type of space curve, i.e. a smooth curve in three-dimensional space. 
    /// It is characterised by the fact that the tangent line at any point makes a constant angle with a fixed line 
    /// called the axis. Examples of helixes are coil springs and the handrails of spiral staircases. A "filled-in" 
    /// helix – for example, a spiral ramp – is called a helicoid. Helices are important in biology, as the DNA
    /// molecule is formed as two intertwined helices, and many proteins have helical substructures, known as alpha 
    /// helices. The word helix comes from the Greek word ἕλιξ.
    /// </summary>
    public class HelixVisual3D : ParametricSurface3D
    {
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register("Length", typeof (double), typeof (HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register("Phase", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(0.0, GeometryChanged));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        public static readonly DependencyProperty TurnsProperty =
            DependencyProperty.Register("Turns", typeof(double), typeof(HelixVisual3D), new UIPropertyMetadata(1.0, GeometryChanged));

        public double Radius
        {
            get { return (double) GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public double Turns
        {
            get { return (double) GetValue(TurnsProperty); }
            set { SetValue(TurnsProperty, value); }
        }


        public double Length
        {
            get { return (double) GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        public double Phase
        {
            get { return (double) GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }

        protected override Point3D Evaluate(double u, double v, out Point texCoord)
        {
            double color = u;
            v *= 2*Math.PI;

            double b = Turns*2*Math.PI;
            double r = Radius;
            double p = Phase;

            double x = Math.Cos(b*u + p)*(1 + r*Math.Cos(v)) + Math.Cos(b*u + p);
            double y = Math.Sin(b*u + p)*(1 + r*Math.Cos(v)) + Math.Sin(b*u + p);
            double z = u*Length + r*Math.Sin(v);

            texCoord = new Point(color, 0);
            return new Point3D(x, y, z);
        }
    }
}