using System.Windows;
using System.Windows.Media;

// http://linas.org/gle/
// http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864

namespace HelixToolkit
{
    /// <summary>
    /// The TubeVisual3D renders a tube along the specified Path.
    /// </summary>
    public class TubeVisual3D : ExtrudedVisual3D
    {
        public static readonly DependencyProperty DiameterProperty =
            DependencyProperty.Register("Diameter", typeof (double), typeof (TubeVisual3D),
                                        new UIPropertyMetadata(1.0, SectionChanged));

        public static readonly DependencyProperty ThetaDivProperty =
            DependencyProperty.Register("ThetaDiv", typeof (int), typeof (TubeVisual3D),
                                        new UIPropertyMetadata(36, SectionChanged));

        public TubeVisual3D()
        {
            UpdateSection();
        }

        public int ThetaDiv
        {
            get { return (int) GetValue(ThetaDivProperty); }
            set { SetValue(ThetaDivProperty, value); }
        }


        public double Diameter
        {
            get { return (double) GetValue(DiameterProperty); }
            set { SetValue(DiameterProperty, value); }
        }

        protected static void SectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TubeVisual3D) d).UpdateSection();
        }

        private void UpdateSection()
        {
            var pc = new PointCollection();
            PointCollection circle = MeshBuilder.GetCircle(ThetaDiv);
            double r = Diameter/2;
            for (int j = 0; j < ThetaDiv; j++)
            {
                pc.Add(new Point(circle[j].X*r, circle[j].Y*r));
            }
            Section = pc;
        }
    }
}