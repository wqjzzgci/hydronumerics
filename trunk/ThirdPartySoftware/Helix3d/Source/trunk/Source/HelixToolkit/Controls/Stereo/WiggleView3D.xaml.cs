using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HelixToolkit
{
    /// <summary>
    /// Interaction logic for WiggleView3D.xaml
    /// </summary>
    public partial class WiggleView3D : StereoControl
    {
        public static readonly DependencyProperty WiggleRateProperty =
            DependencyProperty.Register("WiggleRate", typeof (double), typeof (WiggleView3D),
                                        new UIPropertyMetadata(5.0, WiggleRateChanged));

        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Stopwatch watch = new Stopwatch();

        public WiggleView3D()
        {
            InitializeComponent();

            RightCamera = new PerspectiveCamera();
            BindViewports(View1, null, true, true);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
            UpdateTimer();
            watch.Start();
        }

        /// <summary>
        /// Wiggles per second
        /// </summary>
        public double WiggleRate
        {
            get { return (double) GetValue(WiggleRateProperty); }
            set { SetValue(WiggleRateProperty, value); }
        }

        protected static void WiggleRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WiggleView3D) d).UpdateTimer();
        }

        private void UpdateTimer()
        {
            timer.Interval = TimeSpan.FromSeconds(1.0/WiggleRate);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (watch.ElapsedMilliseconds > 1000/WiggleRate)
            {
                watch.Reset();
                watch.Start();
                Wiggle();
            }
        }

        private void Wiggle()
        {
            if (View1.Camera == LeftCamera)
                View1.Camera = RightCamera;
            else
                View1.Camera = LeftCamera;
        }
    }
}