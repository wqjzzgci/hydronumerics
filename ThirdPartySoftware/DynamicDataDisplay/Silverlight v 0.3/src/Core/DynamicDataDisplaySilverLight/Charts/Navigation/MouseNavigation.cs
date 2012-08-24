using System;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

namespace Microsoft.Research.DynamicDataDisplay.Navigation
{
    /// <summary>Provides common methods of mouse navigation around viewport</summary>
    public class MouseNavigation : NavigationBase
    {
        private Rectangle zoomingRect = new Rectangle();
        private bool isPanning = false;
        private bool isZoomRectCreting = false;
        private bool isCtrlPressed = false;
        private bool isMouseLeftWhileAction = false;
        private Point zoomingRectStartPointInScreen;
        private Point panningStartPointInViewport;
        private Point panningEndPointInViewport;
        private DateTime lastClick = DateTime.MinValue;
        private const double wheelZoomSpeed = 1.2;

        private void relocateVisisbleAfterPanning()
        {
            Rect vis = Viewport.Visible;
            double shiftX = (panningStartPointInViewport.X - panningEndPointInViewport.X);
            double shiftY = (panningStartPointInViewport.Y - panningEndPointInViewport.Y);
            vis.X += shiftX;
            vis.Y += shiftY;
            Viewport.Visible = vis;
        }


        public override void OnPlotterDetaching(Plotter plotter)
        {
            plotter.CentralGrid.MouseLeftButtonDown -= CentralGrid_MouseLeftButtonDown;
            plotter.CentralGrid.MouseLeftButtonUp -= CentralGrid_MouseLeftButtonUp;

          
          plotter.CentralGrid.MouseMove -= CentralGrid_MouseMove;
            plotter.CentralGrid.MouseLeave -= CentralGrid_MouseLeave;
            plotter.CentralGrid.MouseEnter -= CentralGrid_MouseEnter;
            plotter.CentralGrid.MouseWheel -= CentralGrid_MouseWheel;

            base.OnPlotterDetaching(plotter);
        }


        public override void OnPlotterAttached(Plotter plotter)
        {
            base.OnPlotterAttached(plotter);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(5);



            plotter.CentralGrid.MouseLeftButtonDown += new MouseButtonEventHandler(CentralGrid_MouseLeftButtonDown);
            plotter.CentralGrid.MouseLeftButtonUp += new MouseButtonEventHandler(CentralGrid_MouseLeftButtonUp);

            plotter.CentralGrid.MouseMove += new MouseEventHandler(CentralGrid_MouseMove);
            plotter.CentralGrid.MouseWheel += new MouseWheelEventHandler(CentralGrid_MouseWheel);
            plotter.CentralGrid.MouseLeave += new MouseEventHandler(CentralGrid_MouseLeave);
            //plotter.CentralGrid.MouseEnter += new MouseEventHandler(CentralGrid_MouseEnter);
            plotter.KeyDown += new KeyEventHandler(plotter_KeyDown);
            plotter.KeyUp += new KeyEventHandler(plotter_KeyUp);

            zoomingRect.Stroke = new SolidColorBrush(Colors.LightGray);
            Color fillColor = new Color();
            fillColor.A = 40;
            fillColor.R = 0x80;
            fillColor.G = 0x80;
            fillColor.B = 0x80;
            zoomingRect.RadiusX = 2;
            zoomingRect.RadiusY = 2;

            zoomingRect.Fill = new SolidColorBrush(fillColor);
        }

        void timer_Tick(object sender, EventArgs e)
        {
          Nowmousemove();
        }



        void CentralGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
          double zoomspeed;
          if (e.Delta > 0)
            zoomspeed = 0.5;
          else
            zoomspeed = 2;

          var p = e.GetPosition(Viewport);

          var v = e.GetPosition(Viewport).X / Viewport.Output.Width;
          var v2 =  (Viewport.Output.Height - e.GetPosition(Viewport).Y) / Viewport.Output.Height;

          Point zoomTo = new Point(Viewport.Visible.Width * v + Viewport.Visible.X, Viewport.Visible.Height * v2 + Viewport.Visible.Y);
          Viewport.Visible = Viewport.Visible.ZoomX(zoomTo, zoomspeed);

          e.Handled = true;

        }
        
        void plotter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl)
            {
                isCtrlPressed = false;
                isZoomRectCreting = false;
                if (Plotter.MainCanvas.Children.Contains(zoomingRect)) Plotter.MainCanvas.Children.Remove(zoomingRect);
            }
        }

        void plotter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Ctrl)
            {

                isCtrlPressed = true;
            }
        }


        void CentralGrid_MouseMove(object sender, MouseEventArgs e)
        {
          MousePos = e;
          timer.Stop();
         
          timer.Start();

        }

        private System.Windows.Threading.DispatcherTimer timer;
        private MouseEventArgs MousePos;

      
      
      private void Nowmousemove()
      {
        timer.Stop();
          if (!isPanning && !isZoomRectCreting) return;
            else if (isZoomRectCreting)
            {
              Point currentMousePosition = MousePos.GetPosition(Plotter.CentralGrid);
                if (currentMousePosition.X > zoomingRectStartPointInScreen.X)
                {
                    zoomingRect.Width = currentMousePosition.X - zoomingRectStartPointInScreen.X;
                }
                else
                {
                    Canvas.SetLeft(zoomingRect, currentMousePosition.X);
                    zoomingRect.Width = zoomingRectStartPointInScreen.X - currentMousePosition.X;
                }
                if (currentMousePosition.Y > zoomingRectStartPointInScreen.Y)
                {
                    zoomingRect.Height = currentMousePosition.Y - zoomingRectStartPointInScreen.Y;
                }
                else
                {
                    Canvas.SetTop(zoomingRect, currentMousePosition.Y);
                    zoomingRect.Height = zoomingRectStartPointInScreen.Y - currentMousePosition.Y;
                }
            }
            else
            {
              panningEndPointInViewport = MousePos.GetPosition(Plotter.CentralGrid).ScreenToViewport(Viewport.Transform);
                relocateVisisbleAfterPanning();
            }
        }

        void CentralGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                relocateVisisbleAfterPanning();
                isMouseLeftWhileAction = true;
                isPanning = false;
            }
            else if (isZoomRectCreting)
            {
                isZoomRectCreting = false;
                isCtrlPressed = false;
                if (Plotter.MainCanvas.Children.Contains(zoomingRect)) Plotter.MainCanvas.Children.Remove(zoomingRect);
            }
        }

        void CentralGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Plotter.Focus();
            if (!isMouseLeftWhileAction) return;            
        }


        void CentralGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlPressed)
            {
                Plotter.MainCanvas.Children.Remove(zoomingRect);
                Point currPosution = e.GetPosition(Plotter.CentralGrid);
                Point p1 = zoomingRectStartPointInScreen.ScreenToViewport(Viewport.Transform);
                Point p2 = new Point();
                if (currPosution.X < zoomingRectStartPointInScreen.X)
                    p2.X = zoomingRectStartPointInScreen.X - zoomingRect.Width;
                else
                    p2.X = currPosution.X;
                if (currPosution.Y < zoomingRectStartPointInScreen.Y)
                    p2.Y = zoomingRectStartPointInScreen.Y - zoomingRect.Height;
                else
                    p2.Y = currPosution.Y;
                p2 = p2.ScreenToViewport(Viewport.Transform);
                Rect newVisible = new Rect(p1, p2);
                Viewport.Visible = newVisible;
            }
            else
                if (isPanning)
                {
                    //Point panningEndPointInScreen;
                    //panningEndPointInScreen = e.GetPosition(Plotter.CentralGrid);
                    //panningEndPointInViewport = panningEndPointInScreen.ScreenToViewport(Viewport.Transform);
                    //relocateVisisble();
                    isPanning = false;
                    Plotter.CentralGrid.ReleaseMouseCapture();
                }
        }

        void CentralGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isCtrlPressed)
            {
                zoomingRectStartPointInScreen = e.GetPosition(Plotter.CentralGrid);
                if (!Plotter.MainCanvas.Children.Contains(zoomingRect)) Plotter.MainCanvas.Children.Add(zoomingRect);
                Canvas.SetZIndex(zoomingRect, Int16.MaxValue - 3);
                Canvas.SetLeft(zoomingRect, zoomingRectStartPointInScreen.X);
                Canvas.SetTop(zoomingRect, zoomingRectStartPointInScreen.Y);
                zoomingRect.Width = 0;
                zoomingRect.Height = 0;
                isZoomRectCreting = true;
            }
            else
            {
                Point panningStartPointInScreen;
                DateTime d = DateTime.Now;
                if ((d - lastClick).TotalMilliseconds < 200)
                {
                    Viewport.FitToView();
                    return;
                }
                isPanning = true;
                panningStartPointInScreen = e.GetPosition(Plotter.CentralGrid);
                panningStartPointInViewport = panningStartPointInScreen.ScreenToViewport(Viewport.Transform);
                Viewport.AutoFitToView = false;
                Plotter.CentralGrid.CaptureMouse();
                lastClick = DateTime.Now;
            }
            //MessageBox.Show("vis loc "+Viewport.Visible.X+" "+Viewport.Visible.Y+" size "+Viewport.Visible.Width+" "+Viewport.Visible.Height);
            //MessageBox.Show("Coords screen "+panningStartPointInScreen.X+" "+panningStartPointInScreen.Y+" viewport "+panningStartPointInViewport.X+" "+panningStartPointInViewport.Y);
        }


    }
}
