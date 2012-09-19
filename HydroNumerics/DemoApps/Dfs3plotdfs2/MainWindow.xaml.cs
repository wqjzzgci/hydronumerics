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

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Geometry.ASCII;

using MathNet.Numerics.LinearAlgebra;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Common;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;

using Dfs3plotdfs0;

namespace Dfs3plotdfs2
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private string SheFileName;
    private string Dfs3FileName;
    private int ItemNumber;
    private string LayersAsString;
    private string TimeStepsAsString;
    private bool PlotsMade = false;


    public MainWindow()
    {
      InitializeComponent();
      this.Activated += new EventHandler(MainWindow_Activated);

      //Read the config-file
      var ConfigFile = XDocument.Load(Environment.GetCommandLineArgs()[1]).Element("Dfs3plotDfs2");
      SheFileName = ConfigFile.Element("SheFileName").Value;
      Dfs3FileName = ConfigFile.Element("Dfs3FileName").Value;
      ItemNumber = int.Parse(ConfigFile.Element("ItemNumber").Value);
      TimeStepsAsString = ConfigFile.Element("TimeSteps").Value;
      LayersAsString = ConfigFile.Element("Layers").Value;
    }

    //When the window is activated it makes the plots and output files and then closes
    void MainWindow_Activated(object sender, EventArgs e)
    {
      MakePlots();
      //this.Close();
    }

    private void MakePlots()
    {
      if (!PlotsMade) //Only do this once
      {
        Model mShe = new Model(SheFileName);
        DFS3 dfs = new DFS3(Dfs3FileName);
        Item dfsI = dfs.Items[ItemNumber - 1];
        string BaseFileName = System.IO.Path.ChangeExtension(Dfs3FileName, "");

        int[] TimeSteps = Dfs3plotdfs0.MainWindow.ParseString(TimeStepsAsString, 0, dfs.NumberOfTimeSteps - 1);
        int[] Layers = Dfs3plotdfs0.MainWindow.ParseString(LayersAsString, 0, dfs.NumberOfLayers - 1);

        //Set graph headers
        Header.Content = dfsI.Name;
        Unit.Content = dfsI.EumQuantity.UnitAbbreviation;

        //Give plot the same scale as the dfs grid
        plotter.Width = plotter.Height * ((double)dfs.NumberOfColumns) / (double)dfs.NumberOfRows;

        //Plot the extraction wells
        EnumerableDataSource<MikeSheWell> ds = new EnumerableDataSource<MikeSheWell>(mShe.ExtractionWells);
        ds.SetXMapping(var => var.X);
        ds.SetYMapping(var => var.Y);
        var point = new Microsoft.Research.DynamicDataDisplay.PointMarkers.CirclePointMarker();
        point.Size = 10;
        point.Pen = new Pen(Brushes.Black, 3);
        plotter.AddLineGraph(ds, null, point, null);

        //Now loop, first on time steps then on layers
        foreach (int T in TimeSteps)
        {
          foreach (int L in Layers)
          {
            Header2.Content = "Time: " + dfs.TimeSteps[T].ToShortDateString() + ", Layer: " + L; 
            var M = dfs.GetData(T, ItemNumber)[L];
            NaiveColorMap nc = new NaiveColorMap();
            M.Transpose(); //Need to transpose
            nc.Data = M.ToArray();
            M.Transpose(); //Transpose back as this is a reference to data held in the buffer
            nc.Palette = Microsoft.Research.DynamicDataDisplay.Common.Palettes.UniformLinearPalettes.RedGreenBluePalette;
            var bmp = nc.BuildImage();
            image.Source = bmp;

            //Set the color scale
            paletteControl.Palette = nc.Palette;
            paletteControl.Range = nc.Data.GetMinMax();
            
            //Set the size
            var visible = new Microsoft.Research.DynamicDataDisplay.DataRect(dfs.XOrigin, dfs.YOrigin, dfs.GridSize*dfs.NumberOfColumns, dfs.GridSize * dfs.NumberOfRows);            
            ViewportPanel.SetViewportBounds(image, visible);
            plotter.Visible = visible;

            //Write the bitmap
            this.UpdateLayout();
            string fname = BaseFileName + "TimeStep_" +T + "_Layer_" + L;
            Dfs3plotdfs0.MainWindow.SaveScreen(this, fname + ".jpg", (int)ActualWidth, (int)ActualHeight);

            //Now write the ascii grid
            using (StreamWriter sw = new StreamWriter(fname + ".asc"))
            {
              sw.Write(dfs.GetASCIIGrid(T, ItemNumber, L)); 
            }
          }
        }
      }
    }
  }
}
