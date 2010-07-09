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
      Client.GetTimeStampSeriesAsync();
    }

    void Client_GetTimeStampSeriesCompleted(object sender, HydroNumerics.Time.Web.TimeSeriesService.GetTimeStampSeriesCompletedEventArgs e)
    {
      TheChart.DataContext = e.Result;
      TheGrid.DataContext = e.Result.items;
    }
  }
}
