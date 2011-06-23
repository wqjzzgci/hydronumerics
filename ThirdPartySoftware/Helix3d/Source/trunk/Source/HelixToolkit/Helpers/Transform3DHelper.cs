using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public static class Transform3DHelper
    {
        public static Transform3D CombineTransform(Transform3D t1, Transform3D t2)
        {
            var g = new Transform3DGroup();
            g.Children.Add(t1);
            g.Children.Add(t2);
            return g;
        }

    }
}