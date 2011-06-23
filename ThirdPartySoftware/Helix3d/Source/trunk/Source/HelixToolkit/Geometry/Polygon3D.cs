using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// 3D Polygon
    /// </summary>
    public class Polygon3D
    {
        internal Point3DCollection points;

        public Polygon3D()
        {            
        }

        public Polygon3D(Point3DCollection pts)
        {
            points = pts;
        }

        public Point3DCollection Points
        {
            get { return points ?? (points = new Point3DCollection()); }
            set { points = value; }
        }

        // http://en.wikipedia.org/wiki/Polygon_triangulation
        // http://en.wikipedia.org/wiki/Monotone_polygon
        // http://www.codeproject.com/KB/recipes/hgrd.aspx LGPL
        // http://www.springerlink.com/content/g805787811vr1v9v/

        public bool IsPlanar()
        {
            Vector3D v1 = Points[1] - Points[0];
            var normal = new Vector3D();
            for (int i = 2; i < Points.Count; i++)
            {
                var n = Vector3D.CrossProduct(v1, Points[i] - Points[0]);
                n.Normalize();
                if (i == 2)
                    normal = n;
                else
                    if (Math.Abs(Vector3D.DotProduct(n, normal) - 1) > 1e-8)
                    {
                        return false;
                    }
            }
            return true;
        }

        public Vector3D GetNormal()
        {
            if (Points.Count < 3)
                throw new InvalidOperationException("At least three points required in the polygon to find a normal.");
            Vector3D v1 = Points[1] - Points[0];
            for (int i = 2; i < Points.Count; i++)
            {
                var n = Vector3D.CrossProduct(v1, Points[i] - Points[0]);
                if (n.LengthSquared > 1e-8)
                {
                    n.Normalize();
                    return n;
                }
            }
            throw new InvalidOperationException("Invalid polygon.");
        }

        public Polygon Flatten()
        {
            // http://forums.xna.com/forums/p/16529/86802.aspx
            // http://stackoverflow.com/questions/1023948/rotate-normal-vector-onto-axis-plane
            var up = GetNormal();
            up.Normalize();
            var right = Vector3D.CrossProduct(up, Math.Abs(up.X) > Math.Abs(up.Z) ? new Vector3D(0, 0, 1) : new Vector3D(1, 0, 0));
            var backward = Vector3D.CrossProduct(right, up);
            var m = new Matrix3D(backward.X, right.X, up.X, 0, backward.Y, right.Y, up.Y, 0, backward.Z, right.Z, up.Z, 0, 0, 0, 0, 1);
            
            // make first point origin
            var offs = m.Transform(Points[0]);
            m.OffsetX = -offs.X;
            m.OffsetY = -offs.Y;

            var polygon = new Polygon { Points = new PointCollection(Points.Count) };
            foreach (Point3D p in Points)
            {
                var pp = m.Transform(p);
                polygon.Points.Add(new Point(pp.X, pp.Y));
            }
            return polygon;
        }
    }
}