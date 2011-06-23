using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class Target3D : ModelVisual3D
    {
        public Target3D()
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Children.Clear();
            double b = 0.3;
            double d = 0.1;
            Children.Add(new PipeVisual3D
                             {
                                 Point1 = new Point3D(b, 0, 0),
                                 Point2 = new Point3D(-b, 0, 0),
                                 Diameter = d,
                                 Fill = Brushes.Red
                             });
            Children.Add(new PipeVisual3D
                             {
                                 Point1 = new Point3D(0, b, 0),
                                 Point2 = new Point3D(0, -b, 0),
                                 Diameter = d,
                                 Fill = Brushes.Green
                             });
            Children.Add(new PipeVisual3D
                             {
                                 Point1 = new Point3D(0, 0, b),
                                 Point2 = new Point3D(0, 0, -b),
                                 Diameter = d,
                                 Fill = Brushes.Blue
                             });

            /*
            double a = 0.3;
            a = 0.5; 
            double hl = 0.4;
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(b,0,0), Point2 = new Point3D(a, 0, 0), Diameter = d, HeadLength = hl, Fill = Brushes.Red });
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(0,b,0), Point2 = new Point3D(0, a, 0), Diameter = d, HeadLength = hl, Fill = Brushes.Green });
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(0,0,b), Point2 = new Point3D(0, 0, a), Diameter = d, HeadLength = hl, Fill = Brushes.Blue });
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(-b,0,0), Point2 = new Point3D(-a, 0, 0), Diameter = d, HeadLength = hl, Fill = Brushes.Red });
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(0,-b,0), Point2 = new Point3D(0, -a, 0), Diameter = d, HeadLength = hl, Fill = Brushes.Green });
            Children.Add(new ArrowVisual3D() { Point1=new Point3D(0,0,-b), Point2 = new Point3D(0, 0, -a), Diameter = d, HeadLength = hl, Fill = Brushes.Blue });
            Children.Add(new SphereVisual3D() { Radius = 0.2, Fill = Brushes.Yellow });
            */
            /*            Children.Add(new Arrow3D() {Point2=new Point3D(1,0,0), Diameter=d, HeadLength=hl, Fill=Brushes.Red});
                        Children.Add(new Arrow3D() {Point2=new Point3D(0,1,0), Diameter=d, HeadLength=hl, Fill=Brushes.Green});
                        Children.Add(new Arrow3D() {Point2=new Point3D(0,0,1), Diameter=d, HeadLength=hl, Fill=Brushes.Blue});
                        Children.Add(new Arrow3D() {Point2=new Point3D(-1,0,0), Diameter=d, HeadLength=hl, Fill=Brushes.Red});
                        Children.Add(new Arrow3D() {Point2=new Point3D(0,-1,0), Diameter=d, HeadLength=hl, Fill=Brushes.Green});
                        Children.Add(new Arrow3D() {Point2=new Point3D(0,0,-1), Diameter=d, HeadLength=hl, Fill=Brushes.Blue});
                        Children.Add(new Sphere3D() {Radius=0.3, Fill=Brushes.Yellow});
             * */
        }
    }
}