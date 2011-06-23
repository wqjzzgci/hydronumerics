using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The HelixView3D contains a camera controller, coordinate view and a view cube
    /// </summary>
    [ContentProperty("Children"), Localizability(LocalizationCategory.NeverLocalize)]
    public class HelixView3D : ItemsControl, IHelixView3D
    {

        public double RotationSensitivity
        {
            get { return (double)GetValue(RotationSensitivityProperty); }
            set { SetValue(RotationSensitivityProperty, value); }
        }

        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register("RotationSensitivity", typeof(double), typeof(HelixView3D), new UIPropertyMetadata(1.0));


        public static readonly DependencyProperty OrthographicProperty =
            DependencyProperty.Register("Orthographic", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false, OrthographicChanged));

        public Vector3D ModelUpDirection
        {
            get { return (Vector3D)GetValue(ModelUpDirectionProperty); }
            set { SetValue(ModelUpDirectionProperty, value); }
        }

        public static readonly DependencyProperty ModelUpDirectionProperty =
            DependencyProperty.Register("ModelUpDirection", typeof(Vector3D), typeof(HelixView3D),
            new FrameworkPropertyMetadata(new Vector3D(0, 0, 1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ModelUpDirectionChanged));

        private static void ModelUpDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public bool ZoomToFitWhenLoaded
        {
            get { return (bool)GetValue(ZoomToFitWhenLoadedProperty); }
            set { SetValue(ZoomToFitWhenLoadedProperty, value); }
        }

        public static readonly DependencyProperty ZoomToFitWhenLoadedProperty =
            DependencyProperty.Register("ZoomToFitWhenLoaded", typeof(bool), typeof(HelixView3D), new UIPropertyMetadata(false));

        public bool IsHitTestVisible
        {
            get { return (bool)GetValue(IsHitTestVisibleProperty); }
            set { SetValue(IsHitTestVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsHitTestVisibleProperty =
            DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(HelixView3D), new UIPropertyMetadata(false, HitTestVisibleChanged));

        private static void HitTestVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (HelixView3D)d;
            if (v.viewport != null)
                v.viewport.IsHitTestVisible = v.IsHitTestVisible;
        }


        private readonly OrthographicCamera orthographicCamera;
        private readonly PerspectiveCamera perspectiveCamera;

        static HelixView3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(HelixView3D),
                new FrameworkPropertyMetadata(typeof(HelixView3D)));
            ClipToBoundsProperty.OverrideMetadata(
                typeof(HelixView3D),
                new FrameworkPropertyMetadata(true, ClipToBoundsChanged));
        }

        private static void ClipToBoundsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (HelixView3D)d;
            if (v.viewport != null)
                v.viewport.ClipToBounds = v.ClipToBounds;
        }

        public HelixView3D()
        {
            // The Viewport3D must be created here since the Children collection is attached directly
            viewport = new Viewport3D { IsHitTestVisible = this.IsHitTestVisible, ClipToBounds = this.ClipToBounds };

            // headlight
            lights = new Model3DGroup();
            viewport.Children.Add(new ModelVisual3D { Content = lights });

            perspectiveCamera = new PerspectiveCamera();
            CameraHelper.Reset(perspectiveCamera);
            orthographicCamera = new OrthographicCamera();
            CameraHelper.Reset(orthographicCamera);
            // perspectiveCamera.Changed += Camera_Changed;
            // orthographicCamera.Changed += Camera_Changed;

            // Set the current camera
            OnOrthographicChanged();

            // http://blogs.msdn.com/wpfsdk/archive/2007/01/15/maximizing-wpf-3d-performance-on-tier-2-hardware.aspx
            // RenderOptions.EdgeMode?

            // start a watch for FPS calculations
            fpsWatch.Start();

            // Using the rendering event to calculate FPS
            // todo: is this giving correct numbers??
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            Loaded += HelixView3D_Loaded;
        }

        private void HelixView3D_Loaded(object sender, RoutedEventArgs e)
        {
            if (ZoomToFitWhenLoaded)
                ZoomToFit();
        }

        #region Viewport/Children/Camera access properties

        public Visual3DCollection Children
        {
            get { return viewport.Children; }
        }

        public Viewport3D Viewport
        {
            get { return viewport; }
        }

        private Camera currentCamera;

        /// <summary>
        /// Access to the Camera
        /// </summary>
        public ProjectionCamera Camera
        {
            get { return Viewport.Camera as ProjectionCamera; }
            set
            {
                if (currentCamera != null)
                    currentCamera.Changed -= Camera_Changed;
                Viewport.Camera = value;

                currentCamera = Viewport.Camera;
                currentCamera.Changed += Camera_Changed;
            }
        }

        /// <summary>
        /// Access to the camera controller
        /// </summary>
        public CameraController CameraController
        {
            get { return cameraController; }
        }

        public Model3DGroup Lights
        {
            get { return lights; }
        }

        #endregion

        #region Template initialization

        #region Custom control PARTs

        private const string PART_ADORNER_LAYER = "PART_AdornerLayer";
        private const string PART_COORDINATE_VIEW = "PART_CoordinateView";
        private const string PART_VIEW_CUBE_VIEW = "PART_ViewCubeView";
        private const string PART_VIEW_CUBE = "PART_ViewCube";
        private const string PART_CAMERA_CONTROLLER = "PART_CameraController";
        private readonly Model3DGroup lights;
        private readonly Viewport3D viewport;
        private AdornerDecorator adornerLayer;
        private CameraController cameraController;
        private Model3DGroup coordinateSystemLights;
        private Viewport3D coordinateView;
        private ViewCubeVisual3D viewCube;
        private Model3DGroup viewCubeLights;
        private Viewport3D viewCubeView;

        #endregion

        public override void OnApplyTemplate()
        {
            if (adornerLayer == null)
            {
                adornerLayer = Template.FindName(PART_ADORNER_LAYER, this) as AdornerDecorator;
                if (adornerLayer != null)
                    adornerLayer.Child = viewport;
            }
            Debug.Assert(adornerLayer != null, String.Format("{0} is missing from the template.", PART_ADORNER_LAYER));

            if (cameraController == null)
            {
                cameraController = Template.FindName(PART_CAMERA_CONTROLLER, this) as CameraController;
                if (cameraController != null)
                {
                    cameraController.Viewport = Viewport;
                    cameraController.SelectionChanged += CameraControllerSelectionChanged;
                }
            }
            Debug.Assert(cameraController != null,
                         String.Format("{0} is missing from the template.", PART_CAMERA_CONTROLLER));

            if (coordinateView == null)
            {
                coordinateView = Template.FindName(PART_COORDINATE_VIEW, this) as Viewport3D;

                coordinateSystemLights = new Model3DGroup();
                coordinateSystemLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
                coordinateSystemLights.Children.Add(new AmbientLight(Colors.DarkGray));
                if (coordinateView != null)
                {
                    coordinateView.Camera = new PerspectiveCamera();
                    coordinateView.Children.Add(new ModelVisual3D { Content = coordinateSystemLights });
                }
            }
            Debug.Assert(coordinateView != null, String.Format("{0} is missing from the template.", PART_COORDINATE_VIEW));

            if (viewCubeView == null)
            {
                viewCubeView = Template.FindName(PART_VIEW_CUBE_VIEW, this) as Viewport3D;

                viewCubeLights = new Model3DGroup();
                // _viewCubeLights.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 1, 1)));
                viewCubeLights.Children.Add(new AmbientLight(Colors.White));
                if (viewCubeView != null)
                {
                    viewCubeView.Camera = new PerspectiveCamera();
                    viewCubeView.Children.Add(new ModelVisual3D { Content = viewCubeLights });
                    viewCubeView.MouseEnter += ViewCubeViewMouseEnter;
                    viewCubeView.MouseLeave += ViewCubeViewMouseLeave;
                }
                viewCube = Template.FindName(PART_VIEW_CUBE, this) as ViewCubeVisual3D;
                if (viewCube != null)
                    viewCube.Viewport = Viewport;
            }
            Debug.Assert(coordinateView != null, String.Format("{0} is missing from the template.", PART_COORDINATE_VIEW));

            // update the coordinateview camera
            OnCameraChanged();

            // add the default headlight
            OnHeadlightChanged();
            base.OnApplyTemplate();
        }

        #endregion

        #region SelectionChanged event

        #region Delegates

        public delegate void SelectionChangedType(DependencyObject visual);

        #endregion

        public event SelectionChangedType SelectionChanged;

        private void OnSelectionChanged(DependencyObject visual)
        {
            if (SelectionChanged != null)
                SelectionChanged(visual);
        }

        private void CameraControllerSelectionChanged(DependencyObject visual)
        {
            OnSelectionChanged(visual);
        }

        #endregion

        #region Annotation properties

        #region Title/SubTitle

        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(string), typeof(HelixView3D), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SubTitleProperty =
            DependencyProperty.Register("SubTitle", typeof(string), typeof(HelixView3D), new UIPropertyMetadata(null));

        public static readonly DependencyProperty SubTitleSizeProperty =
            DependencyProperty.Register("SubTitleSize", typeof(double), typeof(HelixView3D),
                                        new UIPropertyMetadata(12.0));

        public static readonly DependencyProperty TextBrushProperty =
            DependencyProperty.Register("TextBrush", typeof(Brush), typeof(HelixView3D),
                                        new UIPropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty TitleBackgroundProperty =
            DependencyProperty.Register("TitleBackground", typeof(Brush), typeof(HelixView3D),
                                        new UIPropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty TitleFontFamilyProperty =
            DependencyProperty.Register("TitleFontFamily", typeof(FontFamily), typeof(HelixView3D),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(HelixView3D), new UIPropertyMetadata(null));

        public static readonly DependencyProperty TitleSizeProperty =
            DependencyProperty.Register("TitleSize", typeof(double), typeof(HelixView3D), new UIPropertyMetadata(16.0));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string SubTitle
        {
            get { return (string)GetValue(SubTitleProperty); }
            set { SetValue(SubTitleProperty, value); }
        }

        public string Status
        {
            get { return (string)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        public Brush TitleBackground
        {
            get { return (Brush)GetValue(TitleBackgroundProperty); }
            set { SetValue(TitleBackgroundProperty, value); }
        }

        public Brush TextBrush
        {
            get { return (Brush)GetValue(TextBrushProperty); }
            set { SetValue(TextBrushProperty, value); }
        }

        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }

        public double TitleSize
        {
            get { return (double)GetValue(TitleSizeProperty); }
            set { SetValue(TitleSizeProperty, value); }
        }

        public double SubTitleSize
        {
            get { return (double)GetValue(SubTitleSizeProperty); }
            set { SetValue(SubTitleSizeProperty, value); }
        }

        #endregion

        #region FPS calculation

        // todo: remove the FPS calculations since they are incorrect...??

        // fps stopwatch
        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.Register("FrameRate", typeof(int), typeof(HelixView3D));

        public static readonly DependencyProperty FrameRateTextProperty =
            DependencyProperty.Register("FrameRateText", typeof(string), typeof(HelixView3D),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty ShowFrameRateProperty =
            DependencyProperty.Register("ShowFrameRate", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty FieldOfViewTextProperty =
            DependencyProperty.Register("FieldOfViewText", typeof(string), typeof(HelixView3D),
                                        new UIPropertyMetadata(null));


        public static readonly DependencyProperty ShowFieldOfViewProperty =
            DependencyProperty.Register("ShowFieldOfView", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false));


        private readonly Stopwatch fpsWatch = new Stopwatch();

        // Frame counter for FPS calculation
        private int frames;

        public string FieldOfViewText
        {
            get { return (string)GetValue(FieldOfViewTextProperty); }
            set { SetValue(FieldOfViewTextProperty, value); }
        }

        public bool ShowFieldOfView
        {
            get { return (bool)GetValue(ShowFieldOfViewProperty); }
            set { SetValue(ShowFieldOfViewProperty, value); }
        }

        public string FrameRateText
        {
            get { return (string)GetValue(FrameRateTextProperty); }
            set { SetValue(FrameRateTextProperty, value); }
        }

        public bool ShowFrameRate
        {
            get { return (bool)GetValue(ShowFrameRateProperty); }
            set { SetValue(ShowFrameRateProperty, value); }
        }

        public int FrameRate
        {
            get { return (int)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        #endregion

        #region ViewCube

        public static readonly DependencyProperty ShowViewCubeProperty =
            DependencyProperty.Register("ShowViewCube", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty ViewCubeOpacityProperty =
            DependencyProperty.Register("ViewCubeOpacity", typeof(double), typeof(HelixView3D),
                                        new UIPropertyMetadata(0.5));

        public bool ShowViewCube
        {
            get { return (bool)GetValue(ShowViewCubeProperty); }
            set { SetValue(ShowViewCubeProperty, value); }
        }

        /// <summary>
        /// Opacity of the ViewCube when inactive
        /// </summary>
        public double ViewCubeOpacity
        {
            get { return (double)GetValue(ViewCubeOpacityProperty); }
            set { SetValue(ViewCubeOpacityProperty, value); }
        }

        #endregion

        #region ShowCoordinateSystem

        public static readonly DependencyProperty ShowCoordinateSystemProperty =
            DependencyProperty.Register("ShowCoordinateSystem", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false));

        public bool ShowCoordinateSystem
        {
            get { return (bool)GetValue(ShowCoordinateSystemProperty); }
            set { SetValue(ShowCoordinateSystemProperty, value); }
        }

        #endregion

        #endregion

        #region Camera properties

        public static readonly DependencyProperty CameraModeProperty =
            DependencyProperty.Register("CameraMode", typeof(CameraMode), typeof(HelixView3D),
                                        new UIPropertyMetadata(CameraMode.Inspect));

        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register("CameraRotationMode", typeof(CameraRotationMode), typeof(HelixView3D),
                                        new UIPropertyMetadata(CameraRotationMode.TwoAxis));

        public static readonly DependencyProperty InfiniteSpinProperty =
            DependencyProperty.Register("InfiniteSpin", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false));

        #region CameraInertiaFactor dependency property

        public static readonly DependencyProperty CameraInertiaFactorProperty =
            DependencyProperty.Register("CameraInertiaFactor", typeof(double), typeof(HelixView3D),
                                        new UIPropertyMetadata(0.93));

        public double CameraInertiaFactor
        {
            get { return (double)GetValue(CameraInertiaFactorProperty); }
            set { SetValue(CameraInertiaFactorProperty, value); }
        }

        #endregion

        public bool InfiniteSpin
        {
            get { return (bool)GetValue(InfiniteSpinProperty); }
            set { SetValue(InfiniteSpinProperty, value); }
        }

        /// <summary>
        /// Select rotation by two-axis or virtual trackball
        /// </summary>
        public CameraRotationMode CameraRotationMode
        {
            get { return (CameraRotationMode)GetValue(CameraRotationModeProperty); }
            set { SetValue(CameraRotationModeProperty, value); }
        }

        /// <summary>
        /// Selected <see cref="CameraMode"/>
        /// </summary>
        public CameraMode CameraMode
        {
            get { return (CameraMode)GetValue(CameraModeProperty); }
            set { SetValue(CameraModeProperty, value); }
        }

        public bool ShowCameraTarget
        {
            get { return (bool)GetValue(ShowCameraTargetProperty); }
            set { SetValue(ShowCameraTargetProperty, value); }
        }

        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register("ShowCameraTarget", typeof(bool), typeof(HelixView3D), new UIPropertyMetadata(true));


        #endregion

        #region Headlight

        public static readonly DependencyProperty EnableHeadLightProperty =
            DependencyProperty.Register("IsHeadLightEnabled", typeof(bool), typeof(HelixView3D),
                                        new UIPropertyMetadata(false, HeadlightChanged));

        private readonly DirectionalLight headLight = new DirectionalLight { Color = Colors.White };

        public bool IsHeadLightEnabled
        {
            get { return (bool)GetValue(EnableHeadLightProperty); }
            set { SetValue(EnableHeadLightProperty, value); }
        }

        private static void HeadlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixView3D)d).OnHeadlightChanged();
        }

        public void OnHeadlightChanged()
        {
            if (lights == null)
                return;

            if (IsHeadLightEnabled && !lights.Children.Contains(headLight))
                lights.Children.Add(headLight);
            if (!IsHeadLightEnabled && lights.Children.Contains(headLight))
                lights.Children.Remove(headLight);
        }

        #endregion

        #region Camera changed event

        public static readonly RoutedEvent CameraChangedEvent =
            EventManager.RegisterRoutedEvent("CameraChanged",
                                             RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HelixView3D));

        /// <summary>
        /// Event when a property has been changed
        /// </summary>
        public event RoutedEventHandler CameraChanged
        {
            add { AddHandler(CameraChangedEvent, value); }
            remove { RemoveHandler(CameraChangedEvent, value); }
        }

        protected virtual void RaiseCameraChangedEvent()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(CameraChangedEvent);
            RaiseEvent(args);
        }


        private void Camera_Changed(object sender, EventArgs e)
        {
            // Raise notification
            RaiseCameraChangedEvent();
            // Update the CoordinateView camera and the headlight direction
            OnCameraChanged();
        }

        public virtual void OnCameraChanged()
        {
            // update the camera of the coordinate system
            if (coordinateView != null)
                CameraHelper.CopyDirectionOnly(Camera, coordinateView.Camera as PerspectiveCamera, 30);

            // update the camera of the view cube
            if (viewCubeView != null)
                CameraHelper.CopyDirectionOnly(Camera, viewCubeView.Camera as PerspectiveCamera, 20);

            // update the headlight and coordinate system light
            if (Camera != null)
            {
                if (headLight != null)
                {
                    headLight.Direction = Camera.LookDirection;
                }
                if (coordinateSystemLights != null)
                {
                    var cshl = coordinateSystemLights.Children[0] as DirectionalLight;
                    if (cshl != null)
                        cshl.Direction = Camera.LookDirection;
                }
            }

            var pCamera = Camera as PerspectiveCamera;
            FieldOfViewText = pCamera != null ? String.Format("FOV ∠ {0:0}°", pCamera.FieldOfView) : null;
        }

        #endregion

        #region View cube mouse enter/leave opacity animation

        private void ViewCubeViewMouseLeave(object sender, MouseEventArgs e)
        {
            AnimateOpacity(viewCubeView, ViewCubeOpacity, 200);
        }

        private void ViewCubeViewMouseEnter(object sender, MouseEventArgs e)
        {
            AnimateOpacity(viewCubeView, 1.0, 200);
        }

        private static void AnimateOpacity(UIElement obj, double toOpacity, double animationTime)
        {
            var a = new DoubleAnimation(toOpacity,
                                        new Duration(TimeSpan.FromMilliseconds(animationTime))) { AccelerationRatio = 0.3, DecelerationRatio = 0.5 };
            obj.BeginAnimation(OpacityProperty, a);
        }

        #endregion

        public bool Orthographic
        {
            get { return (bool)GetValue(OrthographicProperty); }
            set { SetValue(OrthographicProperty, value); }
        }

        #region IHelixView3D Members

        public void Export(string fileName)
        {
            Viewport3DHelper.Export(Viewport, fileName);
        }

        public void Copy()
        {
            Viewport3DHelper.Copy(Viewport, Viewport.ActualWidth * 2, Viewport.ActualHeight * 2, Brushes.White);
        }

        public void CopyXaml()
        {
            Clipboard.SetText(XamlHelper.GetXaml(Viewport.Children));
        }

        public void ZoomToFit(double animationTime = 0)
        {
            var bounds = Visual3DHelper.FindBounds(Children);

            if (bounds.IsEmpty)
                return;

            ZoomToFit(bounds, animationTime);
        }

        public void ZoomToFit(Rect3D bounds, double animationTime = 0)
        {
            var diagonal = new Vector3D(bounds.SizeX, bounds.SizeY, bounds.SizeZ);
            var center = bounds.Location + diagonal * 0.5;
            double radius = diagonal.Length * 0.5;
            ZoomToFit(center, radius, animationTime);
        }

        #endregion

        private static void OrthographicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((HelixView3D)d).OnOrthographicChanged();
        }

        private void OnOrthographicChanged()
        {
            if (Orthographic)
                Camera = orthographicCamera;
            else
                Camera = perspectiveCamera;
        }


        public void LookAt(Point3D p)
        {
            LookAt(p, 0);
        }

        public void LookAt(Point3D p, double animationTime)
        {
            Debug.Assert(CameraController != null, "CameraController not defined");
            CameraController.LookAt(p, animationTime);
        }

        public void LookAt(Point3D p, double distance, double animationTime)
        {
            Debug.Assert(CameraController != null, "CameraController not defined");
            CameraController.LookAt(p, distance, animationTime);
        }

        public void LookAt(Point3D p, Vector3D direction, double animationTime)
        {
            Debug.Assert(CameraController != null, "CameraController not defined");
            CameraController.LookAt(p, direction, animationTime);
        }

        public void SetView(Point3D newPosition, Vector3D newDirection, Vector3D newUpDirection, double animationTime)
        {
            CameraHelper.AnimateTo(Camera, newPosition, newDirection, newUpDirection, animationTime);
        }

        public void Add(Visual3D v)
        {
            if (!Viewport.Children.Contains(v))
                Viewport.Children.Add(v);
        }

        public void Remove(Visual3D v)
        {
            if (Viewport.Children.Contains(v))
                Viewport.Children.Remove(v);
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            frames++;
            if (fpsWatch.ElapsedMilliseconds > 500)
            {
                FrameRate = (int)(frames / (0.001 * fpsWatch.ElapsedMilliseconds));
                FrameRateText = FrameRate + " FPS";
                frames = 0;
                fpsWatch.Restart();
            }
        }

        private void ZoomToFit(Point3D center, double radius, double animationTime = 0)
        {
            // var target = Camera.Position + Camera.LookDirection;
            if (Camera is PerspectiveCamera)
            {
                var pcam = Camera as PerspectiveCamera;
                double disth = radius / Math.Tan(0.5 * pcam.FieldOfView * Math.PI / 180);
                double vfov = pcam.FieldOfView / ActualWidth * ActualHeight;
                double distv = radius / Math.Tan(0.5 * vfov * Math.PI / 180);

                double dist = Math.Max(disth, distv);
                var dir = Camera.LookDirection;
                dir.Normalize();
                LookAt(center, dir * dist, animationTime);
            }
            if (Camera is OrthographicCamera)
            {
                LookAt(center, animationTime);
                double newWidth = radius * 2;

                if (ActualWidth > ActualHeight)
                    newWidth = radius * 2 * ActualWidth / ActualHeight;

                CameraHelper.AnimateWidth(Camera as OrthographicCamera, newWidth, animationTime);
            }
        }


        public void ExportToKerkythea(string fileName)
        {
            var exporter = new KerkytheaExporter(fileName);
            var scb = (Background as SolidColorBrush);
            if (scb != null)
                exporter.BackgroundColor = scb.Color;
            exporter.Reflections = true;
            exporter.Shadows = true;
            exporter.SoftShadows = true;
            exporter.Width = (int)Viewport.ActualWidth;
            exporter.Height = (int)Viewport.ActualHeight;
            exporter.Export(Viewport);
            exporter.Close();
        }


        public Visual3D FindNearestVisual(Point pt)
        {
            return Viewport3DHelper.FindNearestVisual(Viewport, pt);
        }

        public Point3D? FindNearestPoint(Point pt)
        {
            return Viewport3DHelper.FindNearestPoint(Viewport, pt);
        }

        public bool FindNearest(Point pt, out Point3D pos, out Vector3D normal, out DependencyObject obj)
        {
            return Viewport3DHelper.FindNearest(Viewport, pt, out pos, out normal, out obj);
        }

    }

    public interface IHelixView3D
    {
        Viewport3D Viewport { get; }
        ProjectionCamera Camera { get; }
        CameraController CameraController { get; }
        Model3DGroup Lights { get; }

        void Export(string fileName);

        void Copy();

        void CopyXaml();

        void ZoomToFit(double animationTime);
    }
}