using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The DefaultLightsVisual3D adds a default 3-point directional light setup to the visual3d tree.
    /// </summary>
    public class DefaultLightsVisual3D : ModelVisual3D
    {
        /// <summary>
        /// A <see cref="ModelVisual3D"/> consisting of a 3-point directional light setup.
        /// </summary>
        public DefaultLightsVisual3D()
        {
            var lightGroup = new Model3DGroup();
            // http://www.3drender.com/light/3point.html

            //lights.Children.Add(new PointLight(Color.FromRgb(180, 180, 180), new Point3D(-2, -3, 8)));
            //lights.Children.Add(new PointLight(Color.FromRgb(180, 180, 180), new Point3D(4, 7, 12)));

            // key light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(180, 180, 180), new Vector3D(-1, -1, -1)));
            // fill light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(120, 120, 120), new Vector3D(1, -1, -0.1)));
            // rim/back light
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(60, 60, 60), new Vector3D(0.1, 1, -1)));
            // and a little bit from below
            lightGroup.Children.Add(new DirectionalLight(Color.FromRgb(50, 50, 50), new Vector3D(0.1, 0.1, 1)));

            lightGroup.Children.Add(new AmbientLight(Color.FromRgb(30, 30, 30)));

            Content = lightGroup;
        }
    }
}