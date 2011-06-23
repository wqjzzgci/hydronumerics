using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public enum MouseAction
    {
        None,
        Pan,
        Zoom,
        Rotate,
        ShowContextMenu,
        ResetCamera,
        ChangeLookAt,
        Select
    }

    // http://en.wikipedia.org/wiki/Virtual_camera_system

    /// <summary>
    /// Inspect/Examine: orbits around a point (fixed target position, move closer target when zooming)
    /// WalkAround: walk around (fixed camera position, move in cameradirection when zooming)
    /// FixedPosition: fixed camera position, change FOV when zooming
    /// </summary>
    public enum CameraMode
    {
        Inspect,
        WalkAround,
        FixedPosition
    }

    /// <summary>
    /// TwoAxis: constrained to two axes of rotation
    /// VirtualTrackball: free rotation
    /// </summary>
    public enum CameraRotationMode
    {
        TwoAxis,
        VirtualTrackball
    } ;

    public class CameraController : Border
    {
        #region Delegates

        public delegate void SelectionChangedType(DependencyObject visual);

        #endregion

        private const double MinimumFoV = 3;
        private const double MaximumFoV = 160;

        public static readonly DependencyProperty RotationSensitivityProperty =
            DependencyProperty.Register("RotationSensitivity", typeof (double), typeof (CameraController),
                                        new UIPropertyMetadata(1.0));


        public static readonly DependencyProperty CameraProperty =
            DependencyProperty.Register("Camera", typeof (ProjectionCamera), typeof (CameraController),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty FixedMouseDownPointProperty =
            DependencyProperty.Register("FixedMouseDownPoint", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty InertiaFactorProperty =
            DependencyProperty.Register("InertiaFactor", typeof (double), typeof (CameraController),
                                        new UIPropertyMetadata(0.9));

        public static readonly DependencyProperty InfiniteSpinProperty =
            DependencyProperty.Register("InfiniteSpin", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register("IsPanEnabled", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty IsZoomEnabledProperty =
            DependencyProperty.Register("IsZoomEnabled", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty ShowCameraTargetProperty =
            DependencyProperty.Register("ShowCameraTarget", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty SpinReleaseTimeProperty =
            DependencyProperty.Register("SpinReleaseTime", typeof (int), typeof (CameraController),
                                        new UIPropertyMetadata(200));

        public static readonly DependencyProperty TargetModelProperty =
            DependencyProperty.Register("TargetModel", typeof (ModelVisual3D), typeof (CameraController),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty CameraModeProperty =
            DependencyProperty.Register("CameraMode", typeof (CameraMode), typeof (CameraController),
                                        new UIPropertyMetadata(CameraMode.Inspect));

        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register("CameraRotationMode", typeof (CameraRotationMode), typeof (CameraController),
                                        new UIPropertyMetadata(CameraRotationMode.TwoAxis));

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register("Enabled", typeof (bool), typeof (CameraController),
                                        new UIPropertyMetadata(true));

        public static readonly DependencyProperty EventSurfaceProperty =
            DependencyProperty.Register("EventSurface", typeof (FrameworkElement), typeof (CameraController),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register("Viewport", typeof (Viewport3D), typeof (CameraController),
                                        new PropertyMetadata(null, ViewportChanged));

        public static readonly DependencyProperty UpDirectionProperty =
            DependencyProperty.Register("ModelUpDirection", typeof (Vector3D), typeof (CameraController),
                                        new UIPropertyMetadata(new Vector3D(0, 0, 1)));


        private readonly Stopwatch spinWatch = new Stopwatch();
        private readonly Stopwatch watch = new Stopwatch();
        public SelectionChangedType SelectionChanged;
        private bool isSpinning;
        private Point3D? lastPoint3D;
        private Point lastPosition;
        private long lastTick;
        private Point mouseDownPosition;
        private Vector3D panSpeed;
        private bool panning;
        private bool rotating;
        private Vector rotationSpeed;
        private Vector spinningSpeed;
        private Adorner targetAdorner;
        private double zoomSpeed;
        private bool zooming;
        private Vector3D fixRelative;
        private bool isFixed;
        private Point3D? mouseDownPoint3D;

        public CameraController()
        {
            ControlRightButtonAction = MouseAction.Zoom;
            ShiftRightButtonAction = MouseAction.Pan;
            MiddleButtonAction = MouseAction.Pan;
            RightButtonAction = MouseAction.Rotate;
            MiddleDoubleClickAction = MouseAction.ResetCamera;
            RightDoubleClickAction = MouseAction.ChangeLookAt;
            LeftButtonAction = MouseAction.Select;

            Background = Brushes.Transparent;
            EventSurface = this;
            SubscribeEvents();

            watch.Start();
            lastTick = watch.ElapsedTicks;
        }

        public Vector3D ModelUpDirection
        {
            get { return (Vector3D) GetValue(UpDirectionProperty); }
            set { SetValue(UpDirectionProperty, value); }
        }

        public double RotationSensitivity
        {
            get { return (double) GetValue(RotationSensitivityProperty); }
            set { SetValue(RotationSensitivityProperty, value); }
        }

        public double InertiaFactor
        {
            get { return (double) GetValue(InertiaFactorProperty); }
            set { SetValue(InertiaFactorProperty, value); }
        }

        /// <summary>
        /// Max time for mousebutton relesae to activate spin
        /// </summary>
        public int SpinReleaseTime
        {
            get { return (int) GetValue(SpinReleaseTimeProperty); }
            set { SetValue(SpinReleaseTimeProperty, value); }
        }


        public bool InfiniteSpin
        {
            get { return (bool) GetValue(InfiniteSpinProperty); }
            set { SetValue(InfiniteSpinProperty, value); }
        }


        public bool IsPanEnabled
        {
            get { return (bool) GetValue(IsPanEnabledProperty); }
            set { SetValue(IsPanEnabledProperty, value); }
        }

        public bool IsZoomEnabled
        {
            get { return (bool) GetValue(IsZoomEnabledProperty); }
            set { SetValue(IsZoomEnabledProperty, value); }
        }

        /// <summary>
        /// Show a 3D model at the target position when manipulating the camera
        /// </summary>
        public bool ShowCameraTarget
        {
            get { return (bool) GetValue(ShowCameraTargetProperty); }
            set { SetValue(ShowCameraTargetProperty, value); }
        }

        /// <summary>
        /// Children to show at the camera target position
        /// </summary>
        public ModelVisual3D TargetModel
        {
            get { return (ModelVisual3D) GetValue(TargetModelProperty); }
            set { SetValue(TargetModelProperty, value); }
        }

        /// <summary>
        /// Keep the point (3D) where rotation/zoom started at the same screen position(2D)
        /// </summary>
        public bool FixedMouseDownPoint
        {
            get { return (bool) GetValue(FixedMouseDownPointProperty); }
            set { SetValue(FixedMouseDownPointProperty, value); }
        }

        private bool IsPerspectiveCamera
        {
            get { return ActualCamera is PerspectiveCamera; }
        }

        private bool IsOrthographicCamera
        {
            get { return ActualCamera is OrthographicCamera; }
        }

        private PerspectiveCamera PerspectiveCamera
        {
            get { return ActualCamera as PerspectiveCamera; }
        }

        private OrthographicCamera OrthographicCamera
        {
            get { return ActualCamera as OrthographicCamera; }
        }

        public ProjectionCamera Camera
        {
            get { return (ProjectionCamera) GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        private Vector3D LookDirection
        {
            get { return ActualCamera.LookDirection; }
            set { ActualCamera.LookDirection = value; }
        }

        private Vector3D UpDirection
        {
            get { return ActualCamera.UpDirection; }
            set { ActualCamera.UpDirection = value; }
        }

        private Point3D Position
        {
            get { return ActualCamera.Position; }
            set { ActualCamera.Position = value; }
        }

        public Point3D CameraTarget
        {
            get { return Position + LookDirection; }
        }

        public MouseAction LeftDoubleClickAction { get; set; }
        public MouseAction MiddleDoubleClickAction { get; set; }
        public MouseAction RightDoubleClickAction { get; set; }

        public MouseAction LeftButtonAction { get; set; }
        public MouseAction ShiftLeftButtonAction { get; set; }
        public MouseAction ControlLeftButtonAction { get; set; }

        public MouseAction MiddleButtonAction { get; set; }
        public MouseAction ShiftMiddleButtonAction { get; set; }
        public MouseAction ControlMiddleButtonAction { get; set; }

        public MouseAction RightButtonAction { get; set; }
        public MouseAction ShiftRightButtonAction { get; set; }
        public MouseAction ControlRightButtonAction { get; set; }

        public double ZoomSensitivity { get; set; }
        public double RotateSensitivity { get; set; }
        public double PanSensitivity { get; set; }

        public CameraRotationMode CameraRotationMode
        {
            get { return (CameraRotationMode) GetValue(CameraRotationModeProperty); }
            set { SetValue(CameraRotationModeProperty, value); }
        }

        public CameraMode CameraMode
        {
            get { return (CameraMode) GetValue(CameraModeProperty); }
            set { SetValue(CameraModeProperty, value); }
        }

        /// <summary>
        /// The element that receives mouse events
        /// </summary>
        public FrameworkElement EventSurface
        {
            get { return (FrameworkElement) GetValue(EventSurfaceProperty); }
            set { SetValue(EventSurfaceProperty, value); }
        }

        public Viewport3D Viewport
        {
            get { return (Viewport3D) GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        public bool Enabled
        {
            get { return (bool) GetValue(EnabledProperty); }
            set { SetValue(EnabledProperty, value); }
        }

        public bool IsActive
        {
            get { return Enabled && Viewport != null && ActualCamera != null; }
        }

        private void OnSelectionChanged(DependencyObject visual)
        {
            if (SelectionChanged != null)
                SelectionChanged(visual);
        }

        private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CameraController) d).OnViewportChanged();
        }

        private void OnViewportChanged()
        {
        }

        public ProjectionCamera ActualCamera
        {
            get
            {
                if (Camera != null)
                    return Camera;
                if (Viewport != null)
                    return Viewport.Camera as ProjectionCamera;
                return null;
            }
        }

        private void SubscribeEvents()
        {
            EventSurface.MouseMove += MouseMoveHandler;
            EventSurface.MouseDown += MouseDownHandler;
            EventSurface.MouseUp += MouseUpHandler;
            EventSurface.MouseWheel += OnMouseWheel;
            //EventSurface.KeyDown += OnKeyDown;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        // todo
        private void UnSubscribeEvents()
        {
            EventSurface.MouseMove -= MouseMoveHandler;
            EventSurface.MouseDown -= MouseDownHandler;
            EventSurface.MouseUp -= MouseUpHandler;
            EventSurface.MouseWheel -= OnMouseWheel;
            //EventSurface.KeyDown -= OnKeyDown;
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Time in seconds
            double time = 1.0*(watch.ElapsedTicks - lastTick)/Stopwatch.Frequency;
            lastTick = watch.ElapsedTicks;
            OnTimeStep(time);
        }

        private void OnTimeStep(double time)
        {
            // should be independent of time
            double factor = Math.Pow(InertiaFactor, time/0.012);
            factor = Clamp(factor, 0.2, 1);

            if (isSpinning && spinningSpeed.LengthSquared > 0)
            {
                Rotate(spinningSpeed.X*time, spinningSpeed.Y*time);

                if (!InfiniteSpin)
                    spinningSpeed *= factor;
                spinWatch.Reset();
                spinWatch.Start();
            }

            if (rotationSpeed.LengthSquared > 0.1)
            {
                Rotate(rotationSpeed.X*time, rotationSpeed.Y*time);
                rotationSpeed *= factor;
            }

            if (Math.Abs(panSpeed.LengthSquared) > 0.0001)
            {
                Pan(panSpeed*time);
                panSpeed *= factor;
            }
            if (Math.Abs(zoomSpeed) > 0.1)
            {
                Zoom(zoomSpeed*time);
                zoomSpeed *= factor;
            }
        }

        private double Clamp(double factor, double min, double max)
        {
            if (factor < min)
                return min;
            if (factor > max)
                return max;
            return factor;
        }

        // todo
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Left:
                    AddRotateForce(-50, 0);
                    break;
                case Key.Right:
                    AddRotateForce(50, 0);
                    break;
            }
        }

        private void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;
            if (Viewport == null)
                throw new NullReferenceException("Viewport");

            var element = (UIElement) sender;
            if (element.IsMouseCaptured)
                return;

            mouseDownPosition = e.GetPosition(this);
            fixRelative = new Vector3D();

            // reset camera
            if (CheckButton(e, MouseAction.ResetCamera))
                ResetCamera();

            Point3D point;
            Vector3D normal;
            DependencyObject visual;
            if (Viewport3DHelper.FindNearest(Viewport, mouseDownPosition, out point, out normal, out visual))
                mouseDownPoint3D = point;
            else
                mouseDownPoint3D = null;

            lastPoint3D = UnProject(mouseDownPosition, CameraTarget, LookDirection);

            // select object
            if (CheckButton(e, MouseAction.Select) && visual != null)
                OnSelectionChanged(visual);

            zooming = CheckButton(e, MouseAction.Zoom);
            panning = CheckButton(e, MouseAction.Pan);
            rotating = CheckButton(e, MouseAction.Rotate);
            isFixed = false;

            // change the 'lookat' point
            if (mouseDownPoint3D != null && CheckButton(e, MouseAction.ChangeLookAt))
            {
                LookAt(mouseDownPoint3D.Value, 0);
                rotating = true;
            }

            if (zooming || panning || rotating)
            {
                bool rightWinKey = (Keyboard.IsKeyDown(Key.RWin));
                if (FixedMouseDownPoint || rightWinKey)
                {
                    if (!panning && mouseDownPoint3D != null)
                    {
                        fixRelative = mouseDownPoint3D.Value - CameraTarget;
                        ShowTargetAdorner(mouseDownPosition);
                        isFixed = true;
                    }
                }
                else
                {
                    // show the adorner in the middle
                    ShowTargetAdorner(new Point(Viewport.ActualWidth/2, Viewport.ActualHeight/2));
                }

                /*
                Position += _fixRelative;
                ShowTargetModel();
                Position -= _fixRelative;
                */

                if (zooming || panning || rotating)
                {
                    e.Handled = true;
                    element.CaptureMouse();
                }

                spinWatch.Reset();
                spinWatch.Start();

                // ProjectToTrackball(EventSurface.ActualWidth, EventSurface.ActualHeight, _mouseDownPosition);
                lastPosition = mouseDownPosition;
            }

            isSpinning = false;
        }

        private bool CheckButton(MouseButtonEventArgs e, MouseAction a)
        {
            bool control = (Keyboard.IsKeyDown(Key.LeftCtrl));
            bool shift = (Keyboard.IsKeyDown(Key.LeftShift));
            bool doubleClick = e.ClickCount == 2;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (control)
                    return a == ControlLeftButtonAction;
                if (shift)
                    return a == ShiftLeftButtonAction;
                if (doubleClick)
                    return a == LeftDoubleClickAction;
                return a == LeftButtonAction;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (control)
                    return a == ControlMiddleButtonAction;
                if (shift)
                    return a == ShiftMiddleButtonAction;
                if (doubleClick)
                    return a == MiddleDoubleClickAction;
                return a == MiddleButtonAction;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                if (control)
                    return a == ControlRightButtonAction;
                if (shift)
                    return a == ShiftRightButtonAction;
                if (doubleClick)
                    return a == RightDoubleClickAction;
                return a == RightButtonAction;
            }

            return false;
        }


        // From 3dtools
        private static Vector3D ProjectToTrackball(double width, double height, Point point)
        {
            double x = point.X/(width/2); // Scale so bounds map to [0,0] - [2,2]
            double y = point.Y/(height/2);

            x = x - 1; // Translate 0,0 to the center
            y = 1 - y; // Flip so +Y is up instead of down

            double z2 = 1 - x*x - y*y; // z^2 = 1 - x^2 - y^2
            double z = z2 > 0 ? Math.Sqrt(z2) : 0;

            return new Vector3D(x, z, y);
        }

        private void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!IsActive) return;

            var element = (UIElement) sender;
            if (element.IsMouseCaptured)
            {
                Point point = e.MouseDevice.GetPosition(element);

                // move target point to mouse down point (3D)
                // camera will be positioned back later
                Position += fixRelative;

                Point3D? thisPoint3D = UnProject(point, CameraTarget, LookDirection);
                Vector3D delta3D = lastPoint3D.Value - thisPoint3D.Value;

                Vector delta = point - lastPosition;
                lastPosition = point;

                // var thisTrack3D = ProjectToTrackball(EventSurface.ActualWidth, EventSurface.ActualHeight, point);


                if (rotating)
                {
                    Rotate(delta.X, delta.Y);
                }

                if (zooming)
                    Zoom(delta.Y*0.01);
                if (panning)
                    Pan(delta3D);

                UpdateTargetModel();

                lastPoint3D = UnProject(point, CameraTarget, LookDirection);


                Position -= fixRelative;

                if (isFixed)
                {
                    // todo:
                    // reposition the camera so mouse down point (3D) matches the mousedown position (2D)
                    if (mouseDownPoint3D != null)
                        Pan(mouseDownPoint3D.Value, mouseDownPosition);
                }

                e.Handled = true;
            }
        }

        private void Pan(Point3D point3D, Point position)
        {
            Point3D? nowPoint3D = UnProject(position, point3D, LookDirection);
            Pan(point3D - nowPoint3D.Value);
            Point newPosition = Project(point3D);
            Debug.Assert(newPosition == position);
        }

        private void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (!Enabled) return;

            var element = (UIElement) sender;

            if (spinWatch.ElapsedMilliseconds < SpinReleaseTime)
            {
                if (rotating)
                {
                    spinningSpeed = 4*(lastPosition - mouseDownPosition)
                                     *((double) SpinReleaseTime/spinWatch.ElapsedMilliseconds);
                    spinWatch.Reset();
                    spinWatch.Start();
                    isSpinning = true;
                }
            }
            rotating = false;
            zooming = false;
            panning = false;

            if (element.IsMouseCaptured)
            {
                e.Handled = true;
                HideTargetModel();
                HideTargetAdorner();
                element.ReleaseMouseCapture();
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            //Zoom(-e.Delta * 0.001);
            AddZoomForce(-e.Delta*0.001);
        }

        public void ResetCamera()
        {
            CameraHelper.Reset(ActualCamera);
        }

        /// <summary>
        /// Changes the field of view and tries to keep the scale fixed.
        /// </summary>
        /// <param name="delta">The relative change in fov.</param>
        public void ChangeFov(double delta)
        {
            if (!IsPerspectiveCamera)
                return;
            double fov = PerspectiveCamera.FieldOfView;
            double d = LookDirection.Length;
            double r = d*Math.Tan(0.5*fov/180*Math.PI);

            fov *= (1 + delta*0.5);
            if (fov < MinimumFoV) fov = MinimumFoV;
            if (fov > MaximumFoV) fov = MaximumFoV;
            PerspectiveCamera.FieldOfView = fov;
            double d2 = r/Math.Tan(0.5*fov/180*Math.PI);
            Vector3D dir = LookDirection;
            dir.Normalize();
            dir *= d2;
            Point3D target = Position + LookDirection;
            Position = target - dir;
            LookDirection = dir;
        }

        public void Zoom(double delta)
        {
            if (!IsZoomEnabled)
                return;
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            if (shift && IsPerspectiveCamera)
                ChangeFov(delta);
            else
                ChangeCameraDistance(delta);
        }

        /// <summary>
        /// Changes the camera distance.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void ChangeCameraDistance(double delta)
        {
            bool alt = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);
            CameraMode cm = CameraMode;
            if (alt)
                cm = (CameraMode) (((int) CameraMode + 1)%2);

            if (delta < -0.5)
                delta = -0.5;

            if (IsPerspectiveCamera)
            {
                switch (cm)
                {
                    case CameraMode.Inspect:
                        Point3D target = Position + LookDirection;
                        LookDirection *= (1 + delta);
                        Position = target - LookDirection;
                        break;
                    case CameraMode.WalkAround:
                        Position -= LookDirection*delta;
                        break;
                    case CameraMode.FixedPosition:
                        double fov = PerspectiveCamera.FieldOfView;
                        fov *= (1 + delta);
                        if (fov < MinimumFoV) fov = MinimumFoV;
                        if (fov > MaximumFoV) fov = MaximumFoV;
                        PerspectiveCamera.FieldOfView = fov;
                        break;
                }
            }
            if (IsOrthographicCamera)
            {
                switch (cm)
                {
                        // todo...
                    case CameraMode.WalkAround:
                    case CameraMode.Inspect:
                    case CameraMode.FixedPosition:
                        OrthographicCamera.Width *= 1 + delta;
                        break;
                }
            }
        }

        public void Pan(Vector3D delta)
        {
            if (!IsPanEnabled)
                return;
            if (CameraMode == CameraMode.FixedPosition)
                return;
            Position += delta;
        }


        public void Rotate(double dx, double dy)
        {
            Point3D target = CameraTarget;

            // toggle rotation mode if the user presses alt
            bool alt = (Keyboard.IsKeyDown(Key.LeftAlt));

            if ((CameraRotationMode == CameraRotationMode.VirtualTrackball) != alt)
            {
                RotateRoam(dx, dy);
                //    Track(thisTrack3D, lastTrack3D);
                //    lastTrack3D = thisTrack3D;
            }
            else
            {
                RotateTwoAxes(dx, dy);
            }

            if (Math.Abs(UpDirection.Length - 1) > 1e-8)
                UpDirection.Normalize();

            if (IsFixedPosition())
                Position = target - LookDirection;
        }

        public bool IsFixedPosition()
        {
            bool leftWinKey = Keyboard.IsKeyDown(Key.LWin);

            // fix the camera position if user presses left Windows key
            if (leftWinKey)
                return CameraMode != CameraMode.Inspect;

            return CameraMode == CameraMode.Inspect;
        }

        // http://www.codeplex.com/3DTools/Thread/View.aspx?ThreadId=22310
        private void RotateRoam(double dx, double dy)
        {
            double dist = LookDirection.Length;

            Vector3D camZ = LookDirection;
            camZ.Normalize();
            Vector3D camX = -Vector3D.CrossProduct(camZ, UpDirection);
            camX.Normalize();
            Vector3D camY = Vector3D.CrossProduct(camZ, camX);
            camY.Normalize();

            double d = 0.5*RotationSensitivity;

            var aarY = new AxisAngleRotation3D(camY, -dx*d);
            var aarX = new AxisAngleRotation3D(camX, dy*d);

            var rotY = new RotateTransform3D(aarY);
            var rotX = new RotateTransform3D(aarX);

            camZ = camZ*rotY.Value*rotX.Value;
            camZ.Normalize();
            camY = camY*rotX.Value*rotY.Value;
            camY.Normalize();

            Vector3D newLookDir = camZ*dist;
            Vector3D newUpDir = camY;


            Vector3D right = Vector3D.CrossProduct(newLookDir, newUpDir);
            right.Normalize();
            Vector3D modUpDir = Vector3D.CrossProduct(right, newLookDir);
            modUpDir.Normalize();
            if ((newUpDir - modUpDir).Length > 1e-8)
                newUpDir = modUpDir;

            LookDirection = newLookDir;
            UpDirection = newUpDir;
        }

        public void RotateTwoAxes(double dx, double dy)
        {
            Vector3D up = ModelUpDirection; // new Vector3D(0, 0, 1);
            Vector3D dir = LookDirection;
            dir.Normalize();

            Vector3D right = Vector3D.CrossProduct(dir, UpDirection);
            right.Normalize();

            double d = -0.5;
            if (CameraMode == CameraMode.WalkAround)
                d = 0.1;
            d *= RotationSensitivity;

            var q1 = new Quaternion(up, d*dx);
            var q2 = new Quaternion(right, d*dy);
            Quaternion q = q1*q2;

            var m = new Matrix3D();
            m.Rotate(q);

            Vector3D newLookDir = m.Transform(LookDirection);
            Vector3D newUpDir = m.Transform(UpDirection);

            right = Vector3D.CrossProduct(newLookDir, newUpDir);
            right.Normalize();
            Vector3D modUpDir = Vector3D.CrossProduct(right, newLookDir);
            modUpDir.Normalize();
            if ((newUpDir - modUpDir).Length > 1e-8)
                newUpDir = modUpDir;

            LookDirection = newLookDir;
            UpDirection = newUpDir;
        }

        public void AddPanForce(double dx, double dy)
        {
            AddPanForce(FindPanVector(dx, dy));
        }

        private Vector3D FindPanVector(double dx, double dy)
        {
            Vector3D axis1 = Vector3D.CrossProduct(LookDirection, UpDirection);
            Vector3D axis2 = Vector3D.CrossProduct(axis1, LookDirection);
            axis1.Normalize();
            axis2.Normalize();
            double l = LookDirection.Length;
            double f = l*0.001;
            Vector3D move = -axis1*f*dx + axis2*f*dy; // this should be dependent on distance to target?           
            return move;
        }

        public void AddPanForce(Vector3D pan)
        {
            panSpeed += pan*40;
        }

        public void AddRotateForce(double dx, double dy)
        {
            rotationSpeed.X += dx*40;
            rotationSpeed.Y += dy*40;
        }

        public void AddZoomForce(double dx)
        {
            zoomSpeed += dx*8;
        }

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates)
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Ray3D GetRay(Point position)
        {
            Point3D point1, point2;
            bool ok = Viewport3DHelper.Point2DtoPoint3D(Viewport, position, out point1, out point2);
            if (!ok)
                return null;

            return new Ray3D {Origin = point1, Direction = point2 - point1};
        }

        /// <summary>
        /// Unproject a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="position">plane position</param>
        /// <param name="normal">plane normal</param>
        /// <returns></returns>
        public Point3D? UnProject(Point p, Point3D position, Vector3D normal)
        {
            Ray3D ray = GetRay(p);
            if (ray == null)
                return null;
            return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// Calculate the screen position of a 3D point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Point Project(Point3D p)
        {
            return Viewport3DHelper.Point3DtoPoint2D(Viewport, p);
        }

        /// <summary>
        /// Set the camera target point
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="animationTime">The animation time.</param>
        public void LookAt(Point3D target, double animationTime)
        {
            LookAt(target, LookDirection, animationTime);
        }

        /// <summary>
        /// Set the camera target point
        /// </summary>
        /// <param name="target">The target point.</param>
        /// <param name="distance">The distance to the camera.</param>
        /// <param name="animationTime">The animation time.</param>
        public void LookAt(Point3D target, double distance, double animationTime)
        {
            Vector3D d = LookDirection;
            d.Normalize();
            LookAt(target, d*distance, animationTime);
        }

        /// <summary>
        /// Set the camera target point
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="newDirection">The new direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public void LookAt(Point3D target, Vector3D newDirection, double animationTime)
        {
            Point3D newPosition = target - newDirection;

            if (IsPerspectiveCamera)
                CameraHelper.AnimateTo(PerspectiveCamera, newPosition, newDirection, UpDirection, animationTime);
            if (IsOrthographicCamera)
                CameraHelper.AnimateTo(OrthographicCamera, newPosition, newDirection, UpDirection, animationTime);
        }

        private void UpdateTargetModel()
        {
            Point3D target = Position + LookDirection;
            if (TargetModel != null)
                TargetModel.Transform = new TranslateTransform3D(target.X, target.Y, target.Z);
        }

        private void ShowTargetModel()
        {
            if (TargetModel != null && ShowCameraTarget)
            {
                if (!Viewport.Children.Contains(TargetModel))
                    Viewport.Children.Insert(0, TargetModel);
                UpdateTargetModel();
            }
        }

        private void HideTargetModel()
        {
            if (TargetModel != null)
            {
                if (Viewport.Children.Contains(TargetModel))
                    Viewport.Children.Remove(TargetModel);
            }
        }

        private void ShowTargetAdorner(Point position)
        {
            if (!ShowCameraTarget)
                return;
            if (targetAdorner != null)
                return;
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
            targetAdorner = new TargetSymbolAdorner(Viewport, position);
            myAdornerLayer.Add(targetAdorner);
        }

        private void HideTargetAdorner()
        {
            AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(Viewport);
            if (targetAdorner != null)
                myAdornerLayer.Remove(targetAdorner);
            targetAdorner = null;
        }
    }
}