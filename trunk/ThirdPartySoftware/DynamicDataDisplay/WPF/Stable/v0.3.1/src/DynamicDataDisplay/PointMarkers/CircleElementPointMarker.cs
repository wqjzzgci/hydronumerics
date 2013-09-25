using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay.PointMarkers
{
    /// <summary>Adds Circle element at every point of graph</summary>
	public class CircleElementPointMarker : ShapeElementPointMarker {
        
        public override UIElement CreateMarker()
        {
            Ellipse result = new Ellipse();
            result.Width = Size;
            result.Height = Size;
            result.Stroke = Brush;
			result.Fill = Fill;
      result.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(result_MouseLeftButtonDown);
      result.MouseRightButtonDown += new System.Windows.Input.MouseButtonEventHandler(result_MouseRightButtonDown);
      result.IsHitTestVisible = true;
      Canvas.SetZIndex(result, 1000);
            if (!String.IsNullOrEmpty(ToolTipText))
            {
                ToolTip tt = new ToolTip();
                tt.Content = ToolTipText;
                result.ToolTip = tt;
            }
            return result;
        }

        void result_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
          throw new NotImplementedException();
        }


        void result_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
          
          throw new NotImplementedException();
        }

        public override void SetPosition(UIElement marker, Point screenPoint)
        {
            Canvas.SetLeft(marker, screenPoint.X - Size / 2);
            Canvas.SetTop(marker, screenPoint.Y - Size / 2);
        }
	}
}
