using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace UIElementDemo
{
    public class DemoElement3D : UIElement3D
    {
        public DemoElement3D()
        {
            var gm = new GeometryModel3D();
            var mb = new MeshBuilder();
            mb.AddSphere(new Point3D(0, 0, 0), 2, 100, 50);
            gm.Geometry = mb.ToMesh();
            gm.Material = Materials.Blue;

            Visual3DModel = gm;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            var gm = Visual3DModel as GeometryModel3D;
            gm.Material = Materials.Red;
        }
    }
}