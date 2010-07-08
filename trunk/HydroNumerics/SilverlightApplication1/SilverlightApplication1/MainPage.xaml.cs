using System;
using System.IO;
using System.Text;
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

using Expression.Blend.SampleData.SampleDataSource;

using System.Collections.ObjectModel;


using System.Windows.Controls.DataVisualization.Charting;

namespace SilverlightApplication1
{
  public partial class MainPage : UserControl
  {

    ServiceReference4.Service1Client  ssc;
    TimeService.TimeSeriesServiceClient tid;

    public MainPage()
    {
      InitializeComponent();

      Mymap.MouseDoubleClick += new EventHandler<Microsoft.Maps.MapControl.MapMouseEventArgs>(Mymap_MouseDoubleClick);

      ServiceReference3.Service1Client sc = new SilverlightApplication1.ServiceReference3.Service1Client();
      sc.DoWorkCompleted += new EventHandler<SilverlightApplication1.ServiceReference3.DoWorkCompletedEventArgs>(sc_DoWorkCompleted);

      MapPolyline line = new MapPolyline();
      line.Locations = new LocationCollection() { 
        new Location(55.715094, 12.51892), 
        new Location(55.715094, 12.1892), 
        new Location(55.615094, 12.1892), };

      MapPolygon polygon = new MapPolygon();
      polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
      line.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
      line.StrokeThickness = 5;
      polygon.Opacity = 0.7;
      polygon.Locations = new LocationCollection() { 
        new Location(55.715094, 12.51892), 
        new Location(55.715094, 12.1892), 
        new Location(55.615094, 12.1892), 
        new Location(55.615094, 12.51892) };



      Mymap.Children.Add(line);

      polygon.MouseLeftButtonUp += new MouseButtonEventHandler(polygon_MouseLeftButtonUp);

      var data = new SampleDataSource();

      RechargeData.ItemsSource = data.Collection;

      tid = new SilverlightApplication1.TimeService.TimeSeriesServiceClient();
      tid.DoWorkCompleted += new EventHandler<SilverlightApplication1.TimeService.DoWorkCompletedEventArgs>(tid_DoWorkCompleted);

      tid.DoWorkAsync();

      //Uri address = new Uri(Application.Current.Host.Source, "../Service1.svc");

      //ssc = new SilverlightApplication1.ServiceReference4.Service1Client("CustomBinding_Service1", address.AbsolutePath);
      //ssc = new SilverlightApplication1.ServiceReference4.Service1Client();
      //ssc.DoWorkCompleted+=new EventHandler<SilverlightApplication1.ServiceReference4.DoWorkCompletedEventArgs>(ssc_DoWorkCompleted);

      //sc.DoWorkAsync();

      //ServiceReference2.WellWrapperClient we = new SilverlightApplication1.ServiceReference2.WellWrapperClient();
      //we.GetXCompleted += new EventHandler<SilverlightApplication1.ServiceReference2.GetXCompletedEventArgs>(we_GetXCompleted);
      //we.GetXAsync();
      //sc.CloseAsync();

    }

    void tid_DoWorkCompleted(object sender, SilverlightApplication1.TimeService.DoWorkCompletedEventArgs e)
    {
      string k= e.Result.name;
      RechargeData.ItemsSource = e.Result.items;
      Line1 = new LineSeries();
      MyChart.Series.Add(Line1);
      Line1.IndependentValueBinding = new System.Windows.Data.Binding();
      Line1.DependentValueBinding = new System.Windows.Data.Binding();
      Line1.DependentValueBinding.ElementName = "val";
      Line1.IndependentValueBinding.ElementName = "startTime.Month";
      Line1.DataContext = e.Result.items;
  
     

    }

    void ssc_DoWorkCompleted(object sender, SilverlightApplication1.ServiceReference4.DoWorkCompletedEventArgs e)
    {
      HeightLabel.Content = "højden i punktet er: " + e.Result;
    }


    void polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
    }

    void Mymap_MouseDoubleClick(object sender, Microsoft.Maps.MapControl.MapMouseEventArgs e)
    {

//      Location loc;
//      if (Mymap.TryViewportPointToLocation(e.ViewportPoint,out loc))
//      {
//        HeightLabel.Content = ssc.State.ToString();
//        ssc.DoWorkAsync(loc.Latitude, loc.Longitude);
        
////        ssc.CloseAsync();
//        MyChart.Title = loc.Latitude.ToString() + loc.Longitude.ToString();
//      }
    }

    void sc_DoWorkCompleted(object sender, SilverlightApplication1.ServiceReference3.DoWorkCompletedEventArgs e)
    {
      MyChart.Title = e.Result;
    }

    void sc_DoWorkCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
    }

    void we_GetXCompleted(object sender, SilverlightApplication1.ServiceReference2.GetXCompletedEventArgs e)
    {
      ServiceReference2.JupiterWell w = e.Result;
      MyChart.Title = w.ID + " intake= " + w.IntakesA[0].IDNumber;


    }

    void s_GetDataCompleted(object sender, SilverlightApplication1.ServiceReference1.GetDataCompletedEventArgs e)
    {
      ObservableCollection<string> data = e.Result;
      MyChart.Title = data[1];
    }

  }
}
