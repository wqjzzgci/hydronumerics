using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;

namespace HydroNumerics.Time.Web
{
  public partial class MainPage : UserControl
  {
    TimeSeriesService.TimeSeriesServiceClient Client;
    public MainPage()
    {
      InitializeComponent();
      Client = new HydroNumerics.Time.Web.TimeSeriesService.TimeSeriesServiceClient();
      Client.GetTimeStampSeriesCompleted += new EventHandler<HydroNumerics.Time.Web.TimeSeriesService.GetTimeStampSeriesCompletedEventArgs>(Client_GetTimeStampSeriesCompleted);
      Client.GetTimeCompleted += new EventHandler<HydroNumerics.Time.Web.TimeSeriesService.GetTimeCompletedEventArgs>(Client_GetTimeCompleted);
      Client.GetTimeAsync();

      AddButton.Click += new RoutedEventHandler(AddButton_Click);
    }

    /// <summary>
    /// Pressing the add button hooks an event handler on the map double click event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AddButton_Click(object sender, RoutedEventArgs e)
    {
      TheMap.MouseDoubleClick += new EventHandler<MapMouseEventArgs>(TheMap_MouseDoubleClick);
    }

    void TheMap_MouseDoubleClick(object sender, MapMouseEventArgs e)
    {
      Pushpin p = new Pushpin();
      Location loc;
      if (TheMap.TryViewportPointToLocation(e.ViewportPoint, out loc))
      {
        
      }
      p.Location = loc;
      var ts = new HydroNumerics.Time.Web.TimeSeriesService.GeoXYPointTime();

      var val = new HydroNumerics.Time.Web.TimeSeriesService.TimestampValue();
      val.Time = DateTime.Now;
      val.Value = 0;
      ts.items = new System.Collections.ObjectModel.ObservableCollection<HydroNumerics.Time.Web.TimeSeriesService.TimestampValue>();
      ts.items.Add(val);

      p.Tag = ts;

      TheMap.Children.Add(p);
      TheMap.MouseDoubleClick -= TheMap_MouseDoubleClick;
      p_MouseLeftButtonUp(p, null);
    }

    void Client_GetTimeCompleted(object sender, HydroNumerics.Time.Web.TimeSeriesService.GetTimeCompletedEventArgs e)
    {
      TheChart.DataContext = e.Result.First();
      TheGrid.DataContext = e.Result.First().items;

      foreach (var v in e.Result)
      {
        Pushpin p = new Pushpin();
        p.Location = new Location(v.Geometry.X, v.Geometry.Y);
        p.Tag = v;
        p.MouseLeftButtonUp += new MouseButtonEventHandler(p_MouseLeftButtonUp);
        TheMap.Children.Add(p);

        }
    }

    void p_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      TheChart.DataContext = ((Pushpin)sender).Tag;
      TheGrid.DataContext = ((HydroNumerics.Time.Web.TimeSeriesService.GeoXYPointTime)((Pushpin)sender).Tag).items;
    }

    void Client_GetTimeStampSeriesCompleted(object sender, HydroNumerics.Time.Web.TimeSeriesService.GetTimeStampSeriesCompletedEventArgs e)
    {
      TheChart.DataContext = e.Result;
      TheGrid.DataContext = e.Result.items;
     
    }
  }
}
