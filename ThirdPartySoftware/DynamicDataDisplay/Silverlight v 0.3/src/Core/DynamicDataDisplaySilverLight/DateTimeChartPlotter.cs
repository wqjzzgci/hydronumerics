using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Microsoft.Research.DynamicDataDisplay
{
  public class DateTimeChartPlotter:ChartPlotter
  {

    public HorizontalDateTimeAxis DateTimeHorizontalAxis { get; set; }

    public DateTimeChartPlotter()
      : base()
    {
      
      Loaded += new RoutedEventHandler(DateTimeChartPlotter_Loaded);
    }

    public void DateTimeChartPlotter_Loaded(object sender, RoutedEventArgs e)
    {
     
      DateTimeHorizontalAxis = new HorizontalDateTimeAxis();
      this.HorizontalAxis = DateTimeHorizontalAxis;
    }



  }
}
