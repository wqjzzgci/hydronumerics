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
using System.Xml.Linq;

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
    private string SheFileName;
    private string Dfs3FileName;
    private int ItemNumber;
    private string WellsAsString;
    private string TimeStepsAsString;
    private bool PlotsMade = false;

    public MainWindow()
    {
      InitializeComponent();
      this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

      //Read the config-file
      var ConfigFile = XDocument.Load(Environment.GetCommandLineArgs()[1]).Element("Dfs3plotDfs0");
      SheFileName = ConfigFile.Element("SheFileName").Value;
      Dfs3FileName = ConfigFile.Element("Dfs3FileName").Value;
      ItemNumber = int.Parse(ConfigFile.Element("ItemNumber").Value);
      WellsAsString = ConfigFile.Element("WellNumbers").Value;
      TimeStepsAsString = ConfigFile.Element("TimeSteps").Value;

    }

    //When the window is loaded it makes the plots and output files and then closes
    void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      MakePlots();
      this.Close();
    }

    private void MakePlots()
    {
      if (!PlotsMade) //Only do this once
      {
        Model mShe = new Model(SheFileName);
        DFS3 dfs = new DFS3(Dfs3FileName);
        Item dfsI = dfs.Items[ItemNumber - 1];

        List<TimestampSeries> well_Concentration = new List<TimestampSeries>();

        int[] TimeSteps = ParseString(TimeStepsAsString, 0, dfs.NumberOfTimeSteps - 1);
        int[] WellNumbers = ParseString(WellsAsString, 0, mShe.ExtractionWells.Count - 1);

        List<MikeSheWell> wells = new List<MikeSheWell>();
        foreach (int j in WellNumbers)
          wells.Add(mShe.ExtractionWells[j]);

        foreach (int i in TimeSteps)
        {
          int k = 0;
          foreach (var w in wells)
          {
            if (i == TimeSteps[0])
              well_Concentration.Add(new TimestampSeries(w.ID, new Unit(dfsI.EumQuantity.UnitAbbreviation, 1, 0)));
            well_Concentration[k].Items.Add(new TimestampValue(dfs.TimeSteps[i], (dfs.GetData(i, ItemNumber)[w.Row, w.Column, w.Layer])));
            k++;
          }
        }

        //Sets the upper title
        Header.Content = dfsI.Name;

        //Sets the title of the y-axis
        var ytitle = new VerticalAxisTitle();
        ytitle.Content = dfsI.EumQuantity.ItemDescription + " [" + dfsI.EumQuantity.UnitAbbreviation + "]";
        TheChart.Children.Add(ytitle);

        int l = 0;
        //Loop the wells for plotting
        foreach (var w in wells)
        {
          //set the data source
          EnumerableDataSource<TimestampValue> ds = new EnumerableDataSource<TimestampValue>(well_Concentration[l].Items);
          ds.SetXMapping(var => dateAxis.ConvertToDouble(var.Time));
          ds.SetYMapping(var => var.Value);
          //create the graph
          var g = TheChart.AddLineGraph(ds, new Pen(Brushes.Black, 3), new PenDescription(w.ID));
         //create a filename
          string outfile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Dfs3FileName), "Well_No" + "_" + WellNumbers[l].ToString() + "_" + dfsI.EumQuantity.ItemDescription);
          //now save to file          
          this.UpdateLayout();
          
          MainWindow.SaveScreen(this, outfile + ".jpg", (int)ActualWidth, (int) ActualHeight);
          //remove the graph again
          TheChart.Children.Remove(g);

          //Now create the dfs0-file
          using (DFS0 dfs0 = new DFS0(outfile + ".dfs0", 1))
          {
            dfs0.FirstItem.Name = dfsI.Name;
            dfs0.FirstItem.EumItem = dfsI.EumItem;
            dfs0.FirstItem.EumUnit = dfsI.EumUnit;
            dfs0.FirstItem.ValueType = dfsI.ValueType;

            int t = 0;
            foreach (var v in well_Concentration[l].Items)
            {
              dfs0.InsertTimeStep(v.Time);
              dfs0.SetData(t, 1, v.Value);
              t++;
            }
          }

          //Now create the text-file
          using (StreamWriter sw = new StreamWriter(outfile + ".txt", false))
          {
            foreach (var v in well_Concentration[l].Items)
            {
              sw.WriteLine(v.Time + "; " + v.Value);
            }
          }
          l++;
        }
        mShe.Dispose();
        dfs.Dispose();
        PlotsMade = true;
      }
    }

   

    /// <summary>
    /// Saves the current screen to the file.
    /// </summary>
    /// <param name="FileName"></param>
    public static void SaveScreen(Visual visual, string FileName, int Width, int Height)
    {
      int dpi = 96;

      RenderTargetBitmap bmp = new RenderTargetBitmap(Width, Height, dpi, dpi, PixelFormats.Pbgra32);
      bmp.Render(visual);

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

    /// <summary>
    /// Split a string into ints. Splits on "," and ";". If string is empty an array with the values 0 ... MaxValue is returned
    /// </summary>
    /// <param name="val"></param>
    /// <param name="MaxValue"></param>
    /// <returns></returns>
    public static int[] ParseString(string val, int MinValue, int MaxValue)
    {
      string[] vals = val.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

      List<int> ToReturn = new List<int>();

      foreach (string s in vals)
      {
        string[] SeriesSplit = s.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);

        int val1 = int.Parse(SeriesSplit[0]);

        if (SeriesSplit.Count() == 2)
        {
          int val2 = int.Parse(SeriesSplit[1]);
          for (int i = val1; i <= val2; i++)
          {
            ToReturn.Add(i);
          }
        }
        else
          ToReturn.Add(val1);
      }


      if (ToReturn.Count() == 0)
      {
        for (int i = MinValue; i <= MaxValue; i++)
          ToReturn.Add(i);
      }
      return ToReturn.ToArray();
    }
  }
}

