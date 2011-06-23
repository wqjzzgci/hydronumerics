using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace UIElementDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var c = new ContainerUIElement3D();
            var e = new ModelUIElement3D();
            var gm = new GeometryModel3D();
            var mb = new MeshBuilder();
            mb.AddSphere(new Point3D(0, 0, 0), 2, 100, 50);
            gm.Geometry = mb.ToMesh();
            gm.Material = Materials.Red;
            e.Model = gm;
            e.Transform = new TranslateTransform3D(5, 0, 0);
            e.MouseDown += (sender, args) => gm.Material = Materials.Yellow;
            c.Children.Add(e);
            view1.Children.Add(c);

            //e.Visibility = Visibility.Hidden;
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Send, new ThreadStart(delegate
            //{
            //    e.Visibility = Visibility.Visible;
            //}));
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            view1.Export("test.xml");
        }

        private void ZoomToFit_Click(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit(500);
        }
    }
}