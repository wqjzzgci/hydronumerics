using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit;
using Microsoft.Win32;

namespace ExportDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            view1.Loaded += ViewLoaded;
        }

        private void ViewLoaded(object sender, RoutedEventArgs e)
        {
            //  view1.ZoomToFit();
        }

        private void ExportKerkythea_Click(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();
            d.Filter = "Kerkythea files (*.xml)|*.xml";
            d.DefaultExt = ".xml";
            d.FileName = @"test.xml";
            //            if (!d.ShowDialog().Value) return;

            using (var exporter = new KerkytheaExporter(d.FileName))
            {
                var m1 = this.Resources["m1"] as Material;
                exporter.RegisterMaterial(m1, @"Materials\water.xml");
                exporter.Export(view1.Viewport);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ZoomToFit_Click(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit(500);
        }

        private void ExportXaml_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportObj_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportX3D_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void view1_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition(view1);
            var v = Viewport3DHelper.FindNearestVisual(view1.Viewport, p);
            // Left-clicking with control creates a bounding box around the object
            if (v != null && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var rect = Visual3DHelper.FindBounds(v, Transform3D.Identity);
                view1.Children.Add(new BoundingBoxVisual3D() { BoundingBox = rect });
                return;
            }

            // Left-clicking adds a blue sphere at the nearest hit point
            var pt = Viewport3DHelper.FindNearestPoint(view1.Viewport, p);
            if (pt.HasValue)
                view1.Children.Add(new SphereVisual3D() { Center = pt.Value, Radius = 0.03 });
        }

        private void ExportPOVRay_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportVRML_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
