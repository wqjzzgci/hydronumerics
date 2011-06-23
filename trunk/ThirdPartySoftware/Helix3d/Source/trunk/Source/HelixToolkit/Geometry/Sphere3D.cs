using System;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class Sphere3D
    {
        public Point3D Center { get; set; }
        public double Diameter { get; set; }

        // http://en.wikipedia.org/wiki/Sphere

        public double Area()
        {
            return Math.PI * Diameter * Diameter;
        }

        public double Volume()
        {
            return Math.PI * Diameter * Diameter * Diameter / 6;
        }

        public bool RayIntersection(Ray3D ray, out Point3D[] result)
        {
            throw new NotImplementedException();
            // http://www.devmaster.net/wiki/Ray-sphere_intersection
        }
    }
}