using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ClothDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public GeometryModel3D FlagModel { get; set; }
        public Flag Flag { get; private set; }

        private readonly Stopwatch watch;
        private readonly Thread integratorThread;

        public MainWindow()
        {
            InitializeComponent();
            
            CreateFlag();
            
            DataContext = this;
            Loaded += MainWindow_Loaded;
            
            watch = new Stopwatch();
            watch.Start();
            integratorThread = new Thread(IntegrationWorker);
            integratorThread.Start();
           
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            integratorThread.Abort();
        }

        private void IntegrationWorker()
        {
            while (true)
            {
                double dt = 1.0 * watch.ElapsedTicks / Stopwatch.Frequency;
                watch.Restart();
                Flag.Update(dt);                
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            Flag.Transfer();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit();
        }


        private void CreateFlag()
        {
            Flag = new Flag("FlagOfNorway.png");
            Flag.Init();

            FlagModel = new GeometryModel3D
                            {
                                Geometry = Flag.Mesh,
                                Material = Flag.Material,
                                BackMaterial = Flag.Material
                            };
        }
    }
}
