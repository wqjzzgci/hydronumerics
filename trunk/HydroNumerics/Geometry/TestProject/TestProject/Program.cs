using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Net;

namespace TestProject
{
  class Program
  {
    [STAThreadAttribute]
    static void Main(string[] args)
    {

      IXYPoint point = new XYPoint(715281.56, 6189341.78);
      double dx = 5000;
      double dy = 5000;
      int utmzone = 32;
      
      actual = Map.GetImagery(point, dx, dy, utmzone);

      actual.DownloadCompleted += new EventHandler(actual_DownloadCompleted);
      System.Windows.Controls.Image im = new System.Windows.Controls.Image();
      im.Source = actual;

      im.SourceUpdated += new EventHandler<System.Windows.Data.DataTransferEventArgs>(im_SourceUpdated);

      im.UpdateLayout();
      Console.WriteLine(DateTime.Now.Second);
      Console.ReadLine();

    }

    static void im_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
    {
      FileStream filestream = new FileStream(@"c:\temp\pict.jpg", FileMode.Create);
      JpegBitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(actual));
      encoder.Save(filestream);
      filestream.Dispose();
    }

    static BitmapImage actual;

    static void actual_DownloadCompleted(object sender, EventArgs e)
    {
      FileStream filestream = new FileStream(@"c:\temp\pict.jpg", FileMode.Create);
      JpegBitmapEncoder encoder = new JpegBitmapEncoder();
      encoder.Frames.Add(BitmapFrame.Create(actual));
      encoder.Save(filestream);
      filestream.Dispose();
    }
  }
}
