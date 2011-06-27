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

      IXYPoint point = new XYPoint(689552.854744955, 6099503.7636487139);
      double dx = 5000;
      double dy = 5000;
      int utmzone = 32;

      im.Source = Map.GetImagery(point, dx, dy, utmzone);



    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      int dpi = 96;

      RenderTargetBitmap bmp = new RenderTargetBitmap((int)im.ActualWidth, (int)im.ActualHeight, dpi, dpi, PixelFormats.Pbgra32);
      bmp.Render(im);

      BitmapEncoder encoder;
      encoder = new JpegBitmapEncoder();

      encoder.Frames.Add(BitmapFrame.Create(bmp));
      using (System.IO.Stream stm = System.IO.File.Create(@"c:\temp\pict.jpg"))
      {
        encoder.Save(stm);
      }

    }
  }
}
