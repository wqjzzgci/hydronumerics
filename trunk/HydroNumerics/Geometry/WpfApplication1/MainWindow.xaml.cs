using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HydroNumerics.Geometry.Net;
using HydroNumerics.Geometry;

namespace WpfApplication1
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      IXYPoint point = new XYPoint(715281.56, 6189341.78);
      double dx = 100;
      double dy = 100;
      int utmzone = 32;

      im.Source = Map.GetImagery(point, dx, dy, utmzone);

    }
  }
}
