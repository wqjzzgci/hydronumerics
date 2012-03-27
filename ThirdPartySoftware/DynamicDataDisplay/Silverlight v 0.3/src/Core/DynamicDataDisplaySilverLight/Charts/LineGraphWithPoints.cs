using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;

namespace Microsoft.Research.DynamicDataDisplay
{
  public class LineGraphWithPoints : LineGraph
  {
    private List<Ellipse> Markers = new List<Ellipse>();

    private bool firsttime = true;


    public LineGraphWithPoints()
      : base()
    {
      ShowInPlotter = true;
      ShowMarker = true;
    }

    public LineGraphWithPoints(IPointDataSource pointSource, string description)
      : base(pointSource, description)
    {
      ShowInPlotter = true;
      ShowMarker = true;
    }

    public LineGraphWithPoints(IPointDataSource pointSource, LineGraphSettings settings)
      : base(pointSource, settings)
    {
      ShowInPlotter = true;
      ShowMarker = true;
    }


    public bool ShowInPlotter
    {
      get { return (bool)GetValue(ShowInPlotterProperty); }
      set { 
        
        SetValue(ShowInPlotterProperty, value);
      }
    }

    public static readonly DependencyProperty ShowInPlotterProperty =
        DependencyProperty.Register(
          "ShowInPlotter",
          typeof(bool),
          typeof(LineGraphWithPoints),
          new PropertyMetadata(OnShowInPlotterPropertyChanged)
        );

    private static void OnShowInPlotterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      LineGraphWithPoints source = d as LineGraphWithPoints;
      source.ClearGraph();
    }


    public void ClearGraph()
    {
      
      if (ShowInPlotter)
      {
        UpdateCore();
        if (Viewport != null)
          Viewport.FitToView();

      }
      else
      {
        foreach (var v in Markers)
          Plotter.MainCanvas.Children.Remove(v);
        Markers.Clear();
        Plotter.MainCanvas.Children.Remove(path);
        if (Viewport != null)
          Viewport.FitToView();

      }
    }


    protected override void UpdateCore()
    {
      if (DataSource == null || Viewport == null || Viewport.Output == new Rect(0, 0, 0, 0) || !ShowInPlotter) return;

      if (path.Clip == null) updateClippingRect();


      if (!Plotter.MainCanvas.Children.Contains(path))
        Plotter.MainCanvas.Children.Add(path);

      Rect output = Viewport.Output;
      var transform = GetTransform();

      IEnumerable<Point> points = GetPoints();

      ContentBounds = BoundsHelper.GetViewportBounds(points, transform.DataTransform);

      List<Point> transformedPoints = transform.DataToScreen(points);
      filteredPoints = new FakePointList(FilterPoints(transformedPoints),
          output.Left, output.Right, output.Top, output.Bottom);
      if (filteredPoints.Count != 0)
      {
        segments.Clear();

        foreach (var v in Markers)
          Plotter.MainCanvas.Children.Remove(v);
        if (firsttime | filteredPoints.Count != Markers.Count)
          Markers.Clear();

        for (int i = 0; i < filteredPoints.Count; i++)
        {
          LineSegment segment = new LineSegment();
          segment.Point = filteredPoints[i];
          segments.Add(segment);

          //Update the points
          if (ShowMarker)
          {
            if (firsttime | filteredPoints.Count != Markers.Count)
            {
              Ellipse e = new Ellipse();
              e.Width = 10;
              e.Height = 10;
              e.Fill = new SolidColorBrush(this.LineColor);
              e.Opacity = 0.65;
              Markers.Add(e);
              firsttime = false;
            }

            double canvasy = filteredPoints[i].Y - Markers[i].Height / 2;
            double canvasx = filteredPoints[i].X - Markers[i].Width / 2;

            if (canvasy < Plotter.MainCanvas.ActualHeight & canvasy > 0 & canvasx < Plotter.MainCanvas.ActualWidth & canvasx > 0)
            {
              Canvas.SetTop(Markers[i], canvasy);
              Canvas.SetLeft(Markers[i], canvasx);
              Plotter.MainCanvas.Children.Add(Markers[i]);
            }
          }
        }
        figure.StartPoint = filteredPoints.StartPoint;
      }
    }


    public bool ShowMarker { get; set; }

    public override void OnPlotterDetaching(Plotter plotter)
    {
      if (parentPlotter != null)
      {
        foreach (var v in Markers)
          Plotter.MainCanvas.Children.Remove(v);
        base.OnPlotterDetaching(plotter);
      }
    }

  }


}



