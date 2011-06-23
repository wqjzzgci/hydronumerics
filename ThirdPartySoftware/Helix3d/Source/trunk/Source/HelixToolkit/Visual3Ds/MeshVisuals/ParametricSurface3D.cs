using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Base class for surfaces evaluated on a rectangular mesh.
    /// Override the Evaluate method to define the points.
    /// </summary>
    public abstract class ParametricSurface3D : MeshElement3D
    {
        public static readonly DependencyProperty MeshSizeUProperty =
            DependencyProperty.Register("MeshSizeU", typeof (int), typeof (ParametricSurface3D),
                                        new UIPropertyMetadata(120));


        public static readonly DependencyProperty MeshSizeVProperty =
            DependencyProperty.Register("MeshSizeV", typeof (int), typeof (ParametricSurface3D),
                                        new UIPropertyMetadata(120));

        public int MeshSizeU
        {
            get { return (int) GetValue(MeshSizeUProperty); }
            set { SetValue(MeshSizeUProperty, value); }
        }

        public int MeshSizeV
        {
            get { return (int) GetValue(MeshSizeVProperty); }
            set { SetValue(MeshSizeVProperty, value); }
        }


        protected abstract Point3D Evaluate(double u, double v, out Point textureCoord);

        protected override MeshGeometry3D Tessellate()
        {
            var mesh = new MeshGeometry3D();

            int n = MeshSizeU;
            int m = MeshSizeV;
            var p = new Point3D[m*n];
            var tc = new Point[m*n];

            // todo: parallell...
            // Parallel.For(0, n, (i) =>
            for (int i = 0; i < n; i++)
            {
                double u = 1.0*i/(n - 1);

                for (int j = 0; j < m; j++)
                {
                    double v = 1.0*j/(m - 1);
                    int ij = i*m + j;
                    p[ij] = Evaluate(u, v, out tc[ij]);
                }
            }
            // );

            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    mesh.Positions.Add(p[idx]);
                    mesh.TextureCoordinates.Add(tc[idx]);
                    idx++;
                }
            }

            for (int i = 0; i + 1 < n; i++)
            {
                for (int j = 0; j + 1 < m; j++)
                {
                    int x0 = i*m;
                    int x1 = (i + 1)*m;
                    int y0 = j;
                    int y1 = j + 1;
                    AddTriangle(mesh, x0 + y0, x0 + y1, x1 + y0);
                    AddTriangle(mesh, x1 + y0, x0 + y1, x1 + y1);
                }
            }

            return mesh;
        }

        private void AddTriangle(MeshGeometry3D mesh, int i1, int i2, int i3)
        {
            var p1 = mesh.Positions[i1];
            if (!IsDefined(p1)) return;
            var p2 = mesh.Positions[i2];
            if (!IsDefined(p2)) return;
            var p3 = mesh.Positions[i3];
            if (!IsDefined(p3)) return;
            mesh.TriangleIndices.Add(i1);
            mesh.TriangleIndices.Add(i2);
            mesh.TriangleIndices.Add(i3);
        }

        private bool IsDefined(Point3D p1)
        {
            return !double.IsNaN(p1.X) && !double.IsNaN(p1.Y) && !double.IsNaN(p1.Z);
        }


    }
}