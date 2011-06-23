using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Panorama cube / skybox
    /// </summary>
    public class PanoramaCube3D : ModelVisual3D
    {
        public static readonly DependencyProperty AutoCenterProperty =
            DependencyProperty.Register("AutoCenter", typeof (bool), typeof (PanoramaCube3D),
                                        new UIPropertyMetadata(true));


        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof (string), typeof (PanoramaCube3D),
                                        new UIPropertyMetadata(null, SourceChanged));

        public static readonly DependencyProperty ShowSeamsProperty =
            DependencyProperty.Register("ShowSeams", typeof (bool), typeof (PanoramaCube3D),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof (double), typeof (PanoramaCube3D),
                                        new UIPropertyMetadata(100.0, GeometryChanged));

        private readonly ModelVisual3D visualChild;

        public PanoramaCube3D()
        {
            visualChild = new ModelVisual3D();
            Children.Add(visualChild);
        }

        public bool AutoCenter
        {
            get { return (bool) GetValue(AutoCenterProperty); }
            set { SetValue(AutoCenterProperty, value); }
        }

        public string Source
        {
            get { return (string) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool ShowSeams
        {
            get { return (bool) GetValue(ShowSeamsProperty); }
            set { SetValue(ShowSeamsProperty, value); }
        }

        public double Size
        {
            get { return (double) GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        private static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PanoramaCube3D) d).UpdateModel();
        }

        protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((PanoramaCube3D) obj).UpdateModel();
        }

        private void UpdateModel()
        {
            string directory = Path.GetDirectoryName(Source);
            string prefix = Path.GetFileName(Source);
            if (string.IsNullOrEmpty(prefix))
                prefix = "cube";

            var group = new Model3DGroup();
            string path = Path.Combine(directory, prefix + "_f.jpg");
            AddCubeSide(group, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), path);
            path = Path.Combine(directory, prefix + "_l.jpg");
            AddCubeSide(group, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), path);
            path = Path.Combine(directory, prefix + "_r.jpg");
            AddCubeSide(group, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), path);
            path = Path.Combine(directory, prefix + "_b.jpg");
            AddCubeSide(group, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), path);
            path = Path.Combine(directory, prefix + "_u.jpg");
            AddCubeSide(group, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), path);
            path = Path.Combine(directory, prefix + "_d.jpg");
            AddCubeSide(group, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), path);

            visualChild.Content = group;
        }

        private GeometryModel3D AddCubeSide(Model3DGroup group, Vector3D normal, Vector3D up, string fileName)
        {
            string fullPath = Path.GetFullPath(fileName);

            if (!File.Exists(fullPath)) return null;
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fullPath);
            // image.CacheOption = BitmapCacheOption.Default;
            // image.CreateOptions = BitmapCreateOptions.None;
            image.EndInit();
            // image.DownloadCompleted += new EventHandler(image_DownloadCompleted);
            Brush brush = new ImageBrush(image);
            Material material = new DiffuseMaterial(brush);

            var mesh = new MeshGeometry3D();
            Vector3D right = Vector3D.CrossProduct(normal, up);
            var origin = new Point3D(0, 0, 0);
            double f = ShowSeams ? 0.995 : 1;
            f *= Size;
            Vector3D n = normal * Size;

            right *= f;
            up *= f;
            Point3D p1 = origin + n - up - right;
            Point3D p2 = origin + n - up + right;
            Point3D p3 = origin + n + up + right;
            Point3D p4 = origin + n + up - right;
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);
            mesh.Positions.Add(p4);
            mesh.TextureCoordinates.Add(new Point(0, 1));
            mesh.TextureCoordinates.Add(new Point(1, 1));
            mesh.TextureCoordinates.Add(new Point(1, 0));
            mesh.TextureCoordinates.Add(new Point(0, 0));
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            var model = new GeometryModel3D(mesh, material);
            group.Children.Add(model);
            return model;
        }
    }
}