using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit;
using Petzold.Media3D;

namespace PetzoldDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            //            view1.Camera = CameraHelper.CreateDefaultCamera();
            /*
          /  var teapotMesh = new TeapotMesh();
            var mvisual = new ModelVisual { MeshGenerator = teapotMesh, Material = Materials.Red, BackMaterial = Materials.Yellow };

            // change from Y up to Z up coordinate system
            mvisual.Transform = CreateWorldTransform(new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), new Vector3D(0, 1, 0));
            view1.Children.Add(mvisual);
             */
        }

        private Transform3D CreateWorldTransform(Vector3D v1, Vector3D v2, Vector3D v3)
        {
            return new MatrixTransform3D(new Matrix3D(
                v1.X, v2.X, v3.X, 0,
                v1.Y, v2.Y, v3.Y, 0,
                v1.Z, v2.Z, v3.Z, 0,
                0, 0, 0, 1));
        }
    }
}
