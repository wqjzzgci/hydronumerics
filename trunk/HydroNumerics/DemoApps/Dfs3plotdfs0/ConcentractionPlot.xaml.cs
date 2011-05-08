using System;
using System.IO;
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
using System.Windows.Shapes;

namespace Dfs3plotdfs0
{
  /// <summary>
  /// Interaction logic for ConcentractionPlot.xaml
  /// </summary>
  public partial class ConcentractionPlot : Window
  {
    public ConcentractionPlot()
    {
      InitializeComponent();
      Button_Click(null, null);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      RenderTargetBitmap bmp = new RenderTargetBitmap((int)100, (int)100, 96, 96, PixelFormats.Pbgra32);
      bmp.Render(this);

      string file = @"c:\temp\test.jpg";

      string Extension = System.IO.Path.GetExtension(file).ToLower();

      BitmapEncoder encoder;
      if (Extension == ".gif")
        encoder = new GifBitmapEncoder();
      else if (Extension == ".png")
        encoder = new PngBitmapEncoder();
      else if (Extension == ".jpg")
        encoder = new JpegBitmapEncoder();
      else
        return;

      encoder.Frames.Add(BitmapFrame.Create(bmp));

      using (Stream stm = File.Create(file))
      {
        encoder.Save(stm);
      }
    }
  }
}
