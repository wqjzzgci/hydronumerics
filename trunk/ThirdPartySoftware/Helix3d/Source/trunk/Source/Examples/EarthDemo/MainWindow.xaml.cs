using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace EarthDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Current clouds
        // http://xplanet.sourceforge.net/clouds.php
        // Planet textures
        // http://planetpixelemporium.com/earth.html
        // http://blogs.msdn.com/b/pantal/archive/2007/08/03/details-on-the-3d-earth-rendering-sample.aspx
        // http://celestia.h-schmidt.net/earth-vt/
        // http://worldwindcentral.com/wiki/Add-on:Global_Clouds_(near_realtime_clouds)
        // http://www.unity3dx.com/index.php/products/earth-3d
        // http://en.wikipedia.org/wiki/DirectDraw_Surface
        // http://www.celestiamotherlode.net/catalog/earth.php
        // http://www.oera.net/How2/TextureMaps2.htm

        public static readonly DependencyProperty CloudsProperty =
            DependencyProperty.Register("Clouds", typeof (Material), typeof (MainWindow), new UIPropertyMetadata(null));

        private bool loadCurrent;
        private string url = "http://www.narrabri.atnf.csiro.au/operations/NASA/clouds_2048.jpg";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Clouds = MaterialHelper.CreateMaterial(Brushes.Transparent);
            
            // set this to true to download the latest cloud image (from the url specified above)
            loadCurrent = false;

            if (loadCurrent)
            {
                var client = new WebClient();
                client.DownloadDataCompleted += client_DownloadDataCompleted;
                client.DownloadDataAsync(new Uri(url));
            }
            else
            {
                SetDefaultClouds();
            }
            DataContext = this;
        }

        public Material Clouds
        {
            get { return (Material) GetValue(CloudsProperty); }
            set { SetValue(CloudsProperty, value); }
        }

        private void SetDefaultClouds()
        {
            Clouds = MaterialHelper.CreateImageMaterial("clouds.jpg", 0.5);
        }


        private void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                SetDefaultClouds();
                return;
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(e.Result);
            image.EndInit();

            Clouds = MaterialHelper.CreateImageMaterial(image, 0.5);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit();
        }
    }
}