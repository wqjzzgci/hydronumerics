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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.DataSources;

using HydroNumerics.Core;
using HydroNumerics.Wells;
using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Time.Core;

using DHI.Generic.MikeZero;

namespace Dfs3plotdfs0
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();      
      this.Activated += new EventHandler(MainWindow_Activated);
    }

    void MainWindow_Activated(object sender, EventArgs e)
    {
      MakePlots();
//      this.Close();
    }

    private string FileName =@"C:\Jacob\Work3\HydroNumerics\MikeSheTools\TestData\TestModelDemo.she";
    private string Dfs3FileName = @"C:\Jacob\Work3\HydroNumerics\MikeSheTools\TestData\TestModelDemo.she - Result Files\TestModelDemo_3DSZ.dfs3";
    private int ItemNumber =1;

    private bool PlotsMade = false;

    private void MakePlots()
    {
      if (!PlotsMade) //Only do this once
      {
        Model mShe = new Model(FileName);
        DFS3 dfs = new DFS3(Dfs3FileName);
        Item dfsI = dfs.Items[ItemNumber - 1];

        List<TimestampSeries> well_Concentration = new List<TimestampSeries>();

        for (int i = 0; i < dfs.NumberOfTimeSteps; i++)
        {
          for (int j = 0; j < mShe.ExtractionWells.Count; j++)
          {
            var w = mShe.ExtractionWells[j];
            if (i == 0)
              well_Concentration.Add(new TimestampSeries(w.ID, new Unit(dfsI.EumQuantity.UnitAbbreviation, 1, 0)));
            well_Concentration[j].Items.Add(new TimestampValue(dfs.TimeSteps[i], (dfs.GetData(i, ItemNumber)[w.Row, w.Column, w.Layer])));
          }
        }

        var ytitle = new VerticalAxisTitle();
        ytitle.Content = dfsI.EumQuantity.ItemDescription + " [" + dfsI.EumQuantity.UnitAbbreviation + "]";
        TheChart.Children.Add(ytitle);

        for (int j = 0; j < mShe.ExtractionWells.Count; j++)
        {
          var w = mShe.ExtractionWells[j];
          EnumerableDataSource<TimestampValue> ds = new EnumerableDataSource<TimestampValue>(well_Concentration[j].Items);
          ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          var g = TheChart.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription(w.ID));
          TheChart.FitToView();
          SaveScreen(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Dfs3FileName), "Well_No" + "_" + j.ToString()+"_" + dfsI.EumQuantity.ItemDescription + ".jpg"));
          TheChart.Children.Remove(g);
        }
      }
    }

    /// <summary>
    /// Saves the current screen to the file.
    /// </summary>
    /// <param name="FileName"></param>
    private void SaveScreen(string FileName)
    {
      this.UpdateLayout();
      int dpi = 96;
      
      RenderTargetBitmap bmp = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, dpi, dpi, PixelFormats.Pbgra32);
      bmp.Render(this);

      string Extension = System.IO.Path.GetExtension(FileName).ToLower();
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
      using (Stream stm = File.Create(FileName))
      {
        encoder.Save(stm);
      }
    }
    }
  }

