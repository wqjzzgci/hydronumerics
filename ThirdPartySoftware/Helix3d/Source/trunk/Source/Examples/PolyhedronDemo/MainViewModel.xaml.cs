using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace PolyhedronDemo
{
    public enum ModelTypes { Tetrahedron, StellatedOctahedron }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            CreateModel();
        }

        private ModelTypes currentModelType;
        public ModelTypes CurrentModelType
        {
            get { return currentModelType; }
            set
            {
                currentModelType = value; RaisePropertyChanged("CurrentModelType");
                CreateModel();
            }
        }

        private void CreateModel()
        {
            var points = new Point3DCollection();
            var edges = new Int32Collection();
            var triangles = new Int32Collection();
            switch (CurrentModelType)
            {
                case ModelTypes.StellatedOctahedron:
                case ModelTypes.Tetrahedron:
                    points.Add(+1, +1, +1);
                    points.Add(-1, -1, 1);
                    points.Add(-1, +1, -1);
                    points.Add(+1, -1, -1);
                    edges.Add(0, 1, 1, 2, 2, 0, 0, 3, 1, 3, 2, 3);
                    triangles.Add(0, 1, 2, 0, 3, 1, 1, 3, 2, 2, 3, 0);
                    break;
            }
            switch (CurrentModelType)
            {
                case ModelTypes.StellatedOctahedron:
                    // http://en.wikipedia.org/wiki/Compound_of_two_tetrahedra
                    points.Add(-1, +1, +1);
                    points.Add(1, -1, 1);
                    points.Add(1, +1, -1);
                    points.Add(-1, -1, -1);
                    edges.Add(4, 5, 5, 6, 6, 4, 4, 7, 5, 7, 6, 7);
                    triangles.Add(4, 5, 6, 4, 7, 5, 5, 7, 6, 6, 7, 4);
                    break;
            }

            var m = new Model3DGroup();

            // Add the nodes
            var gm = new MeshBuilder();
            foreach (var p in points)
            {
                gm.AddSphere(p, 0.1);
            }
            m.Children.Add(new GeometryModel3D(gm.ToMesh(), Materials.Gold));

            // Add the triangles
            var tm = new MeshBuilder();
            for (int i = 0; i < triangles.Count; i += 3)
            {
                tm.AddTriangle(points[triangles[i]], points[triangles[i + 1]], points[triangles[i + 2]]);
            }
            m.Children.Add(new GeometryModel3D(tm.ToMesh(), Materials.Red) { BackMaterial = Materials.Blue });

            // Add the edges
            var em = new MeshBuilder();
            for (int i = 0; i < edges.Count; i += 2)
            {
                em.AddCylinder(points[edges[i]], points[edges[i + 1]], 0.08, 10);
            }
            m.Children.Add(new GeometryModel3D(em.ToMesh(), Materials.Gray));

            Model = m;
        }

        private Model3D model;
        public Model3D Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged("Model"); }
        }

        #region PropertyChanged Block
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion


    }
}