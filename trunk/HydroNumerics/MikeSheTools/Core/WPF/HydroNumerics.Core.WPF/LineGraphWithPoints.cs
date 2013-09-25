using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace HydroNumerics.Core.WPF
{
  public class LineGraphWithPoints : ExtendedLineGraph
  {
    protected List<FrameworkElement> Markers = new List<FrameworkElement>();


    public LineGraphWithPoints()
      : base()
    {
      points = new List<Point>();
      ShowMarker = true;
      ShowLine = true;
    }


    protected override void UpdateCore()
    {
      if (Viewport == null || Viewport.Output == new Rect(0, 0, 0, 0) || DataSource==null) return;

      foreach (var m in Markers)
        Plotter.MainCanvas.Children.Remove(m);

      points = GetPoints().ToList();

      if (points == null || points.Count == 0) return;

        var transform = GetTransform();
        var transformedpoints = transform.DataToScreen(points);


        for (int i = 0; i < transformedpoints.Count; i++)
        {

          //Update the points
            double canvasy = transformedpoints[i].Y - Markers[i].Height / 2;
            double canvasx = transformedpoints[i].X - Markers[i].Width / 2;
            Canvas.SetTop(Markers[i], canvasy);
            Canvas.SetLeft(Markers[i], canvasx);

            if (canvasy < Plotter.MainCanvas.ActualHeight & canvasy > 0 & canvasx < Plotter.MainCanvas.ActualWidth & canvasx > 0)
            {
              Plotter.MainCanvas.Children.Add(Markers[i]);
            }
        }
        ContentBounds = BoundsHelper.GetViewportBounds(points, transform.DataTransform);
       base.UpdateCore();
      
    }


    protected override void UpdateItems()
    {
      if ( Plotter != null)
      {
        points = new List<Point>();
        foreach (var m in Markers)
          Plotter.MainCanvas.Children.Remove(m);
        Markers.Clear();

        if (ItemsSource != null && ItemsSource.GetEnumerator().MoveNext())
        {
          foreach (var p in ItemsSource)
          {
            var visualItem = this.ItemTemplate.LoadContent() as FrameworkElement;
            visualItem.DataContext = p;
            double v = LineGraphWithPoints.GetYValue(visualItem as DependencyObject);
            double x = LineGraphWithPoints.GetXValue(visualItem as DependencyObject);
            Point pdata = new Point(x, v);
            points.Add(pdata);
            Markers.Add(visualItem);
          }
          //Set the datasource since that is called by other methods
          var es = new Microsoft.Research.DynamicDataDisplay.DataSources.EnumerableDataSource<FrameworkElement>(Markers);
          es.SetXMapping(p => LineGraphWithPoints.GetXValue(p as DependencyObject));
          es.SetYMapping(p => LineGraphWithPoints.GetYValue(p as DependencyObject));
          DataSource = es;

          if (Viewport != null & points.Count > 1)
            ((ChartPlotter)Viewport.Plotter).FitToView();
        }
      }
    }

    public static readonly DependencyProperty YValueProperty = DependencyProperty.RegisterAttached(
                        "YValue",                  //Name of the property
                        typeof(double),             //Type of the property
                        typeof(LineGraphWithPoints), new PropertyMetadata(-2.0) );   //Type of the provider of the registered attached property
                                                   //Callback invoked in case the property value has changed


    public static void SetYValue(DependencyObject obj, double yValue)
    {
      obj.SetValue(YValueProperty, yValue);
    }

    public static double GetYValue(DependencyObject obj)
    {
      return (double)obj.GetValue(YValueProperty);
    }



    public static readonly DependencyProperty XValueProperty = DependencyProperty.RegisterAttached(
                    "XValue",                  //Name of the property
                    typeof(double),             //Type of the property
                    typeof(LineGraphWithPoints));   //Type of the provider of the registered attached property
                                             //Callback invoked in case the property value has changed


    public static void SetXValue(DependencyObject obj, double xValue)
    {
      obj.SetValue(XValueProperty, xValue);
    }

    public static double GetXValue(DependencyObject obj)
    {
      return (double)obj.GetValue(XValueProperty);
    }




    public bool ShowMarker { get; set; }

    public bool ShowLine { get; set; }


    public override void OnPlotterDetaching(Plotter plotter)
    {
        foreach (var v in Markers)
          Plotter.MainCanvas.Children.Remove(v);
        base.OnPlotterDetaching(plotter);
    }

  }


}



