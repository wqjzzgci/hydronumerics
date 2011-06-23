using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    // todo: this is under construction...
    public class SphereCollectionVisual3D : ModelVisual3D
    {
        private readonly ModelVisual3D _element;
        private MeshGeometry3D _sphere;

        public SphereCollectionVisual3D()
        {
            Positions = new Point3DCollection();
            Fill = Brushes.Blue;
            Diameter = 0.1;
            ThetaDiv = 37;
            PhiDiv = 19;

            _element = new ModelVisual3D();
            Children.Add(_element);
        }

        public Point3DCollection Positions { get; set; }

        public Brush Fill { get; set; }

        public double Diameter { get; set; }
        public int ThetaDiv { get; set; }
        public int PhiDiv { get; set; }


        private void CreateGeometry()
        {
            var builder = new MeshBuilder();
            builder.AddSphere(new Point3D(0, 0, 0), Diameter/2, ThetaDiv, PhiDiv);
            _sphere = builder.ToMesh();
            _sphere.Freeze();
        }


        public void UpdateModel()
        {
            CreateGeometry();
            var c = new Model3DGroup();
            Material mat = MaterialHelper.CreateMaterial(Fill);

            for (int i = 0; i < Positions.Count; i++)
            {
                Point3D p0 = Positions[i];
                var sphereModel = new GeometryModel3D();
                sphereModel.Geometry = _sphere;
                sphereModel.Material = mat;
                sphereModel.Transform = new TranslateTransform3D(p0.X, p0.Y, p0.Z);
                c.Children.Add(sphereModel);
            }

            _element.Content = c;
        }
    }
}