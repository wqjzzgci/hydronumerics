using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using TDx.TDxInput;

namespace HelixToolkit.Input
{
    public enum SpaceNavigatorType
    {
        UnknownDevice = 0,
        SpaceNavigator = 6,
        SpaceExplorer = 4,
        SpaceTraveler = 25,
        SpacePilot = 29
    } ;

    public enum SpaceNavigatorZoomMode
    {
        InOut,
        UpDown
    }

    
    public class SpaceNavigatorDecorator : Decorator
    {
        public static readonly DependencyProperty IsConnectedProperty =
            DependencyProperty.Register("IsConnected", typeof (bool), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty IsPanEnabledProperty =
            DependencyProperty.Register("IsPanEnabled", typeof (bool), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(false));

        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("Type", typeof (SpaceNavigatorType), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(SpaceNavigatorType.UnknownDevice));

        public static readonly DependencyProperty NavigatorNameProperty =
            DependencyProperty.Register("NavigatorName", typeof (string), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(null));

        public static readonly DependencyProperty SensivityProperty =
            DependencyProperty.Register("Sensitivity", typeof (double), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(1.0));

        public static readonly DependencyProperty ZoomModeProperty =
            DependencyProperty.Register("ZoomMode", typeof (SpaceNavigatorZoomMode), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(SpaceNavigatorZoomMode.UpDown));

        public static readonly DependencyProperty ZoomSensivityProperty =
            DependencyProperty.Register("ZoomSensitivity", typeof (double), typeof (SpaceNavigatorDecorator),
                                        new UIPropertyMetadata(1.0));



        public CameraController CameraController
        {
            get { return (CameraController)GetValue(CameraControlProperty); }
            set { SetValue(CameraControlProperty, value); }
        }

        public static readonly DependencyProperty CameraControlProperty =
            DependencyProperty.Register("CameraController", typeof(CameraController), typeof(SpaceNavigatorDecorator), new UIPropertyMetadata(null));



        private CameraController Controller
        {
            get
            {
                // if CameraController is set, use it
                if (CameraController!=null)
                    return CameraController;

                // otherwise use the Child of the Decorator
                var view = Child as HelixView3D;
                return view == null ? null : view.CameraController;
            }
        }

        public double Sensitivity
        {
            get { return (double)GetValue(SensivityProperty); }
            set { SetValue(SensivityProperty, value); }
        }

        public double ZoomSensitivity
        {
            get { return (double)GetValue(SensivityProperty); }
            set { SetValue(SensivityProperty, value); }
        }


        public SpaceNavigatorZoomMode ZoomMode
        {
            get { return (SpaceNavigatorZoomMode)GetValue(ZoomModeProperty); }
            set { SetValue(ZoomModeProperty, value); }
        }


        public bool IsPanEnabled
        {
            get { return (bool)GetValue(IsPanEnabledProperty); }
            set { SetValue(IsPanEnabledProperty, value); }
        }

        public string NavigatorName
        {
            get { return (string)GetValue(NavigatorNameProperty); }
            set { SetValue(NavigatorNameProperty, value); }
        }

        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        public SpaceNavigatorType Type
        {
            get { return (SpaceNavigatorType)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        #region Private fields
        private Device _input;
        private Sensor _sensor;
        #endregion

        #region Events

        public static readonly RoutedEvent ConnectionChangedEvent =
            EventManager.RegisterRoutedEvent("ConnectionChanged",
                                             RoutingStrategy.Bubble, typeof (RoutedEventHandler),
                                             typeof (SpaceNavigatorDecorator));

        /// <summary>
        /// Event when a property has been changed
        /// </summary>
        public event RoutedEventHandler ConnectionChanged
        {
            add { AddHandler(ConnectionChangedEvent, value); }
            remove { RemoveHandler(ConnectionChangedEvent, value); }
        }

        protected virtual void RaiseConnectionChanged()
        {
            // e.Handled = true;
            var args = new RoutedEventArgs(ConnectionChangedEvent);
            RaiseEvent(args);
        }

        #endregion

        public SpaceNavigatorDecorator()
        {
            Connect();

            /* todo: try to start driver if not available
             Thread.Sleep(1000);
                        if (!IsConnected)
                        {
                            Disconnect();
                            Thread.Sleep(1000);
                            StartDriver();
                            Connect();
                        }*/
        }


        private void Connect()
        {
            try
            {
                _input = new Device();
                _sensor = _input.Sensor;
                _input.DeviceChange += input_DeviceChange;
                _sensor.SensorInput += Sensor_SensorInput;
                _input.Connect();
            }
            catch (COMException e)
            {
                Trace.WriteLine(e.Message);
            }
        }

        public void Disconnect()
        {
            if (_input != null)
                _input.Disconnect();
            _input = null;
            IsConnected = false;
        }
 
        // todo...
        /*
        private void StartDriver()
        {
            string exe = @"C:\Program Files\3Dconnexion\3Dconnexion 3DxSoftware\3DxWare\3dxsrv.exe";
            if (!File.Exists(exe))
                return;
            var p = Process.Start(exe, "-searchWarnDlg");
            Thread.Sleep(2000);
        }

        private void StopDriver()
        {
            string exe = @"C:\Program Files\3Dconnexion\3Dconnexion 3DxSoftware\3DxWare\3dxsrv.exe";
            if (!File.Exists(exe))
                return;
            var p = Process.Start(exe, "-shutdown");
        }
        */

        private void Sensor_SensorInput()
        {
            if (Controller == null) 
                return;
            
            Controller.AddRotateForce(Sensitivity*_sensor.Rotation.Y, Sensitivity*_sensor.Rotation.X);

            if (ZoomMode == SpaceNavigatorZoomMode.InOut)
                Controller.AddZoomForce(ZoomSensitivity*0.001*_input.Sensor.Translation.Z);

            if (ZoomMode == SpaceNavigatorZoomMode.UpDown)
            {
                Controller.AddZoomForce(ZoomSensitivity*0.001*_sensor.Translation.Y);
                if (IsPanEnabled)
                    Controller.AddPanForce(Sensitivity*0.03*_sensor.Translation.X,
                                              Sensitivity*0.03*_sensor.Translation.Z);
            }
        }

        private void input_DeviceChange(int reserved)
        {
            IsConnected = true;
            Type = (SpaceNavigatorType)_input.Type;
            NavigatorName = Type.ToString();
            RaiseConnectionChanged();
        }

    }
}