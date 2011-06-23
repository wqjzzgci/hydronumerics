using System;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Helix
    /// </summary>
    public class Helix3D
    {
        public double Turns { get; set; }
        public double Height { get; set; }
        public double BottomRadius { get; set; }
        public double TopRadius { get; set; }

        public Point3DCollection GetPoints(int n)
        {
            var points = new Point3DCollection();
            for (int i = 0; i < n; i++)
            {
                double u = (double)i / (n - 1);
                double b = Turns * 2 * Math.PI;
                double r = BottomRadius + (TopRadius - BottomRadius) * u;

                double x = r * Math.Cos(b * u);
                double y = r * Math.Sin(b * u);
                double z = u * Height;
                points.Add(new Point3D(x, y, z));
            }
            return points;
        }
    }
}