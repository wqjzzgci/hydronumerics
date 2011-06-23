using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Base control handling stereo cameras
    /// todo: keyboard shortcut 'x' to change cross/parallel viewing   
    /// </summary>
    public class StereoControl : ContentControl
    {
        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register("Camera", typeof (PerspectiveCamera), typeof (StereoControl),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register("CameraRotationMode", typeof (CameraRotationMode), typeof (StereoControl),
                                        new UIPropertyMetadata(CameraRotationMode.TwoAxis));

        public static readonly DependencyProperty CopyDirectionVectorProperty =
            DependencyProperty.Register("CopyDirectionVector", typeof (bool), typeof (StereoControl),
                                        new UIPropertyMetadata(true, StereoViewChanged));

        public static readonly DependencyProperty CopyUpVectorProperty =
            DependencyProperty.Register("CopyUpVector", typeof (bool), typeof (StereoControl),
                                        new UIPropertyMetadata(false, StereoViewChanged));

        public static readonly DependencyProperty StereoBaseProperty =
            DependencyProperty.Register("StereoBase", typeof (double), typeof (StereoControl),
                                        new UIPropertyMetadata(0.12, StereoViewChanged));

        public static readonly DependencyProperty CrossViewingProperty =
            DependencyProperty.Register("CrossViewing", typeof (bool), typeof (StereoControl),
                                        new UIPropertyMetadata(false));

        private Viewport3D viewport;

        static StereoControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (StereoControl),
                                                     new FrameworkPropertyMetadata(typeof (StereoControl)));
        }

        public StereoControl()
        {
            viewport = new Viewport3D();
            Camera = CameraHelper.CreateDefaultCamera();
            Camera.Changed += Camera_Changed;
            Children = new ObservableCollection<Visual3D>();
        }

        public CameraRotationMode CameraRotationMode
        {
            get { return (CameraRotationMode) GetValue(CameraRotationModeProperty); }
            set { SetValue(CameraRotationModeProperty, value); }
        }

        public bool CrossViewing
        {
            get { return (bool) GetValue(CrossViewingProperty); }
            set { SetValue(CrossViewingProperty, value); }
        }

/*        void StereoControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                CrossViewing = !CrossViewing;
            }
        }
        */

        public PerspectiveCamera Camera
        {
            get { return (PerspectiveCamera) GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }


        public bool CopyUpVector
        {
            get { return (bool) GetValue(CopyUpVectorProperty); }
            set { SetValue(CopyUpVectorProperty, value); }
        }


        public bool CopyDirectionVector
        {
            get { return (bool) GetValue(CopyDirectionVectorProperty); }
            set { SetValue(CopyDirectionVectorProperty, value); }
        }

        public double StereoBase
        {
            get { return (double) GetValue(StereoBaseProperty); }
            set { SetValue(StereoBaseProperty, value); }
        }

        public Viewport3D LeftViewport { get; set; }
        public Viewport3D RightViewport { get; set; }
        public PerspectiveCamera LeftCamera { get; set; }
        public PerspectiveCamera RightCamera { get; set; }

        public ObservableCollection<Visual3D> Children { get; private set; }

        protected static void StereoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (StereoControl) d;
            v.UpdateCameras();
        }

        public void BindViewports(Viewport3D left, Viewport3D right)
        {
            BindViewports(left, right, true, true);
        }

        public void BindViewports(Viewport3D left, Viewport3D right, bool createLights, bool createCamera)
        {
            LeftViewport = left;
            RightViewport = right;

            Children.CollectionChanged += Children_CollectionChanged;

            if (createLights)
            {
                Children.Add(new DefaultLightsVisual3D());
            }

            if (createCamera)
            {
                if (LeftViewport.Camera == null)
                    LeftViewport.Camera = CameraHelper.CreateDefaultCamera();
                else
                    CameraHelper.Reset(LeftViewport.Camera as PerspectiveCamera);
                if (RightViewport != null && RightViewport.Camera == null)
                    RightViewport.Camera = new PerspectiveCamera();
            }

            LeftCamera = LeftViewport.Camera as PerspectiveCamera;
            if (RightViewport != null)
                RightCamera = RightViewport.Camera as PerspectiveCamera;
            UpdateCameras();
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // todo: update left and right collections here
        }

        public void Clear()
        {
            Children.Clear();
            SynchronizeStereoModel();
        }

        /*
        /// <summary>
        /// Add the visual to the left view, and create a clone for the right view
        /// </summary>
        /// <param name="visual"></param>
        public void Add(Visual3D visual)
        {
            if (LeftViewport != null)
                LeftViewport.Children.Add(visual);
            if (RightViewport != null)
            {
                // todo: Visual geometry is not created yet

                // var clone = new ClonedVisual3D(visual);
                var clone = StereoHelper.CreateClone(visual);
                if (clone != null)
                {
                    var el = clone as UIElement3D;
                    if (el != null)
                        el.InvalidateModel();

                    RightViewport.Children.Add(clone);
                }
            }
        }
        */

        // todo: does not work properly yet
        public void SynchronizeStereoModel()
        {
            LeftViewport.Children.Clear();
            if (RightViewport != null)
                RightViewport.Children.Clear();
            foreach (Visual3D v in Children)
            {
                LeftViewport.Children.Add(v);
                if (RightViewport != null)
                {
                    Visual3D clone = StereoHelper.CreateClone(v);
                    if (clone != null)
                    {
                        RightViewport.Children.Add(clone);
                    }
                }
            }
        }

        private void Camera_Changed(object sender, EventArgs e)
        {
            UpdateCameras();
        }

        public void UpdateCameras()
        {
            StereoHelper.UpdateStereoCameras(Camera, LeftCamera, RightCamera, StereoBase, CrossViewing, CopyUpVector, CopyDirectionVector);
        }

        public void ExportKerkythea(string leftFileName, string rightFileName)
        {
            var scb = Background as SolidColorBrush;

            var leftExporter = new KerkytheaExporter(leftFileName);
            if (scb != null)
                leftExporter.BackgroundColor = scb.Color;
            leftExporter.Reflections = true;
            leftExporter.Shadows = true;
            leftExporter.SoftShadows = true;
            leftExporter.Width = (int) LeftViewport.ActualWidth;
            leftExporter.Height = (int) LeftViewport.ActualHeight;
            leftExporter.Export(LeftViewport);
            leftExporter.Close();

            var rightExporter = new KerkytheaExporter(rightFileName);
            if (scb != null)
                rightExporter.BackgroundColor = scb.Color;
            rightExporter.Reflections = true;
            rightExporter.Shadows = true;
            rightExporter.SoftShadows = true;
            rightExporter.Width = (int) RightViewport.ActualWidth;
            rightExporter.Height = (int) RightViewport.ActualHeight;
            rightExporter.Export(RightViewport);
            rightExporter.Close();
        }
    }
}