using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Collections;
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
    private List<FrameworkElement> Markers = new List<FrameworkElement>();


    public LineGraphWithPoints()
      : base()
    {
      points = new List<Point>();
//      DataSource = new Microsoft.Research.DynamicDataDisplay.DataSources.EnumerableDataSource<Point>(points);

      MarkerHeight = 10;
      MarkerWidth = 10;
      ShowInPlotter = true;
      ShowMarker = true;

      
      
    }

    public LineGraphWithPoints(IPointDataSource pointSource, string description)
      : base(pointSource, description)
    {
      MarkerHeight = 10;
      MarkerWidth = 10;
      ShowInPlotter = true;
      ShowMarker = true;
    }

    public LineGraphWithPoints(IPointDataSource pointSource, LineGraphSettings settings)
      : base(pointSource, settings)
    {
      MarkerHeight = 10;
      MarkerWidth = 10;
      ShowInPlotter = true;
      ShowMarker = true;
    }


    public double MarkerWidth
    {
      get { return (double)GetValue(MarkerWidthProperty); }
      set
      {

        SetValue(MarkerWidthProperty, value);
      }
    }

    public static readonly DependencyProperty MarkerWidthProperty =
        DependencyProperty.Register(
          "MarkerWidth",
          typeof(double),
          typeof(LineGraphWithPoints),
          null
        );


    public double MarkerHeight
    {
      get { return (double)GetValue(MarkerHeightProperty); }
      set
      {

        SetValue(MarkerHeightProperty, value);
      }
    }

    public static readonly DependencyProperty MarkerHeightProperty =
        DependencyProperty.Register(
          "MarkerHeight",
          typeof(double),
          typeof(LineGraphWithPoints),
          null
        );


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
      source.UpdateCore();
      if (source.Viewport != null)
        source.Viewport.FitToView();

    }


    public override void OnPlotterAttached(Plotter plotter)
    {
      DataContext = plotter.DataContext;
      base.OnPlotterAttached(plotter);
    }

    List<Point> points;

    protected override void UpdateCore()
    {

      if (ItemsSource == null || !ItemsSource.GetEnumerator().MoveNext() || Viewport == null || Viewport.Output == new Rect(0, 0, 0, 0)) return;

      if (path.Clip == null) updateClippingRect();

      segments.Clear();
      foreach (var m in Markers)
        Plotter.MainCanvas.Children.Remove(m);

      if (ShowInPlotter)
      {
        if (!Plotter.MainCanvas.Children.Contains(path))
          Plotter.MainCanvas.Children.Add(path);

        var transform = GetTransform();
        var transformedpoints = transform.DataToScreen(points);

        figure.StartPoint = transformedpoints[0];
        for (int i = 0; i < transformedpoints.Count; i++)
        {
          LineSegment segment = new LineSegment();
          segment.Point = transformedpoints[i];
          segments.Add(segment);

          //Update the points
          if (ShowMarker)
          {
            double canvasy = transformedpoints[i].Y - Markers[i].Height / 2;
            double canvasx = transformedpoints[i].X - Markers[i].Width / 2;
            Canvas.SetTop(Markers[i], canvasy);
            Canvas.SetLeft(Markers[i], canvasx);

            if (canvasy < Plotter.MainCanvas.ActualHeight & canvasy > 0 & canvasx < Plotter.MainCanvas.ActualWidth & canvasx > 0)
            {
              Plotter.MainCanvas.Children.Add(Markers[i]);
            }
          }
        }
        ContentBounds = BoundsHelper.GetViewportBounds(points, transform.DataTransform);
        figure.Segments = segments;
      }
      else
      {
        if (Plotter.MainCanvas.Children.Contains(path))
          Plotter.MainCanvas.Children.Remove(path);
      }
    }

    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set {
        SetValue(ItemsSourceProperty, value);
      }
    }

    public DataTemplate ItemTemplate
    {
      get { return (DataTemplate)GetValue(ItemTemplateProperty); }
      set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LineGraphWithPoints),
    new PropertyMetadata((s, e) => ((LineGraphWithPoints)s).UpdateItems()));

    public static readonly DependencyProperty ItemTemplateProperty =
    DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LineGraphWithPoints),
    new PropertyMetadata((s, e) => ((LineGraphWithPoints)s).UpdateItems()));

    HorizontalDateTimeAxis HorAxis;


    private void UpdateItems()
    {
      if (ItemsSource != null & Plotter !=null)
      {
        HorAxis = (Plotter as ChartPlotter).HorizontalAxis as HorizontalDateTimeAxis;


        points = new List<Point>();
        foreach (var m in Markers)
          Plotter.MainCanvas.Children.Remove(m);
        Markers.Clear();

        foreach (var p in ItemsSource)
        {
          var visualItem = this.ItemTemplate.LoadContent() as FrameworkElement;
          visualItem.DataContext = p;
          double v = LineGraphWithPoints.GetYValue(visualItem as DependencyObject);
          DateTime d = LineGraphWithPoints.GetDateValue(visualItem as DependencyObject);
          Point pdata = new Point(HorAxis.ConvertToDouble(d), v);
          points.Add(pdata);
          Markers.Add(visualItem);
        }
        var es = new Microsoft.Research.DynamicDataDisplay.DataSources.EnumerableDataSource<Point>(points);
        es.SetXMapping(p => p.X);
        es.SetYMapping(p => p.Y);

        DataSource = es;

      }

      if (Viewport != null)
        Viewport.FitToView();

    }

    



     public static readonly DependencyProperty YValueProperty = DependencyProperty.RegisterAttached(
                         "YValue",                  //Name of the property
                         typeof( double ),             //Type of the property
                         typeof( LineGraphWithPoints ),   //Type of the provider of the registered attached property
                         null );                           //Callback invoked in case the property value has changed
 
 
                public static void SetYValue( DependencyObject obj, double yValue )
                {
                         obj.SetValue( YValueProperty, yValue );
                }
 
                public static double GetYValue( DependencyObject obj )
                {
                         return ( double )obj.GetValue( YValueProperty );
                }


   
         public static readonly DependencyProperty DateValueProperty = DependencyProperty.RegisterAttached(
                         "DateValue",                  //Name of the property
                         typeof( DateTime ),             //Type of the property
                         typeof( LineGraphWithPoints ),   //Type of the provider of the registered attached property
                         null );                           //Callback invoked in case the property value has changed
 
 
                public static void SetDateValue( DependencyObject obj, DateTime yValue )
                {
                         obj.SetValue( DateValueProperty, yValue );
                }
 
                public static DateTime GetDateValue( DependencyObject obj )
                {
                         return ( DateTime )obj.GetValue( DateValueProperty );
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



