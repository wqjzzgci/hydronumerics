using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class Plane3D
    {
        internal Point3D position;
        public Point3D Position
        {
            get { return position; }
            set { position = value; }
        }

        internal Vector3D normal;
        public Vector3D Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        // public Vector3D Up { get; set; }
        // public double Width { get; set; }
        // public double Height {get;set;}

        public Plane3D()
        {
            //Width=50;
            //Height=50;
        }

        public Plane3D(Point3D p0, Vector3D n)
        {
            Position = p0;
            Normal = n;
        }

        public static Point3D InPlane =new Point3D(double.NaN,double.NaN,double.NegativeInfinity);
        public static Point3D NoIntersection = new Point3D(double.NaN, double.NaN, double.PositiveInfinity);

        /// <summary>
        /// Finds the intersection between the plane and the line (la,lb).
        /// </summary>
        /// <param name="la">The la.</param>
        /// <param name="lb">The lb.</param>
        /// <returns></returns>
        public Point3D LineIntersection(Point3D la, Point3D lb)
        {
            // http://en.wikipedia.org/wiki/Line-plane_intersection
            var l = lb - la;
            double a=Vector3D.DotProduct(position - la, normal);
            double b=Vector3D.DotProduct(l, normal);
            if (a==0 && b==0)
                return InPlane;
            if (b == 0)
                return NoIntersection;
            return la + (a/b)*l;
        }

        //public void SetYZ(double x, int dir)
        //{
        //    Position = new Point3D(x, 0, Height / 2);
        //    Normal = new Vector3D(dir, 0, 0);
        //    Up = new Vector3D(0, 0, 1);
        //}

        //public void SetXY(double z, int dir)
        //{
        //    Position = new Point3D(0, 0, z);
        //    Normal = new Vector3D(0, 0, dir);
        //    Up = new Vector3D(1, 0, 0);
        //}

        //public void SetXZ(double y, int dir)
        //{
        //    Position = new Point3D(0, y, 0);
        //    Normal = new Vector3D(0, dir, 0);
        //    Up = new Vector3D(1, 0, 0);
        //}

        //public Point3D[] GetCornerPoints()
        //{
        //    var pts = new Point3D[4];
        //    Vector3D right = Vector3D.CrossProduct(Normal, Up);
        //    pts[0] = Position + (-right * 0.5 * Width - Up * 0.5 * Height);
        //    pts[1] = Position + (right * 0.5 * Width - Up * 0.5 * Height);
        //    pts[2] = Position + (right * 0.5 * Width + Up * 0.5 * Height);
        //    pts[3] = Position + (-right * 0.5 * Width + Up * 0.5 * Height);
        //    return pts;
        //}

    }
}
