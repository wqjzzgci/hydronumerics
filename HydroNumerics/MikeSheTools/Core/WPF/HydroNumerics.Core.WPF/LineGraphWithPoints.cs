using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
      ShowMarker = true;
    }


    protected override void UpdateCore()
    {
      if (Viewport == null || Viewport.Output == new Rect(0, 0, 0, 0) || DataSource==null) return;

      foreach (var m in Markers)
        Plotter.MainCanvas.Children.Remove(m);


      if (Markers.Count == 0) return;

        var transform = GetTransform();

        for (int i = 0; i < Markers.Count; i++)
        {
          double X = LineGraphWithPoints.GetXValue(Markers[i] as DependencyObject);
          double Y = LineGraphWithPoints.GetYValue(Markers[i] as DependencyObject);

          var p = transform.DataToScreen(new Point(X, Y));

          //Update the points
            double canvasy = p.Y - Markers[i].Height / 2;
            double canvasx = p.X - Markers[i].Width / 2;
            Canvas.SetTop(Markers[i], canvasy);
            Canvas.SetLeft(Markers[i], canvasx);

            if (canvasy < Plotter.MainCanvas.ActualHeight & canvasy > 0 & canvasx < Plotter.MainCanvas.ActualWidth & canvasx > 0)
            {
              Plotter.MainCanvas.Children.Add(Markers[i]);
            }
        }
        ContentBounds = BoundsHelper.GetViewportBounds(GetPoints(), transform.DataTransform);
        base.UpdateCore();
    }


    protected override void UpdateItems()
    {
      if ( Plotter != null)
      {
        foreach (var m in Markers)
          Plotter.MainCanvas.Children.Remove(m);
        Markers.Clear();

        if (ItemsSource != null && ItemsSource.GetEnumerator().MoveNext())
        {
          foreach (var p in ItemsSource)
          {
            var visualItem = this.ItemTemplate.LoadContent() as FrameworkElement;
            visualItem.DataContext = p;
            //Set the binding again to make it attach immediately
            visualItem.SetBinding(LineGraphWithPoints.XValueProperty, visualItem.GetBindingExpression(LineGraphWithPoints.XValueProperty).ParentBinding);
            visualItem.SetBinding(LineGraphWithPoints.YValueProperty, visualItem.GetBindingExpression(LineGraphWithPoints.YValueProperty).ParentBinding);

            Markers.Add(visualItem);
          }

          var es = new Microsoft.Research.DynamicDataDisplay.DataSources.EnumerableDataSource<DependencyObject>(Markers);
          es.SetXMapping(p => LineGraphWithPoints.GetXValue(p as DependencyObject));
          es.SetYMapping(p => LineGraphWithPoints.GetYValue(p as DependencyObject));
          DataSource = es;


          if (Viewport != null & Markers.Count > 1)
            ((ChartPlotter)Viewport.Plotter).FitToView();
        }
      }
    }

    public static readonly DependencyProperty YValueProperty = DependencyProperty.RegisterAttached(
                        "YValue",                  //Name of the property
                        typeof(double),             //Type of the property
                        typeof(LineGraphWithPoints));   //Type of the provider of the registered attached property

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


    public static void SetXValue(DependencyObject obj, double xValue)
    {
      obj.SetValue(XValueProperty, xValue);
    }

    public static double GetXValue(DependencyObject obj)
    {
      return (double)obj.GetValue(XValueProperty);
    }




    public bool ShowMarker { get; set; }


    public override void OnPlotterDetaching(Plotter plotter)
    {
        foreach (var v in Markers)
          Plotter.MainCanvas.Children.Remove(v);
        base.OnPlotterDetaching(plotter);
    }

  }


}



