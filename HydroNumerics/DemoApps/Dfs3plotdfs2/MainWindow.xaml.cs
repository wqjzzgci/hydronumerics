using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

using HydroNumerics.MikeSheTools.Core;
using HydroNumerics.MikeSheTools.DFS;

using MathNet.Numerics.LinearAlgebra;
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

        int[] TimeSteps = Dfs3plotdfs0.MainWindow.ParseString(TimeStepsAsString, 0, dfs.NumberOfTimeSteps - 1);
        int[] Layers = Dfs3plotdfs0.MainWindow.ParseString(LayersAsString, 0, dfs.NumberOfLayers - 1);

//        d3:ViewportPanel.ViewportBounds="0,0,1,1"
        
        foreach (int T in TimeSteps)
        {
          foreach (int L in Layers)
          {
            Matrix M = dfs.GetData(T, ItemNumber)[L];
            NaiveColorMap nc = new NaiveColorMap();
            nc.Data = M.CopyToArray();
            nc.Palette = Microsoft.Research.DynamicDataDisplay.Common.Palettes.UniformLinearPalettes.RedGreenBluePalette;


            paletteControl.Palette = nc.Palette;
            paletteControl.Range = nc.Data.GetMinMax();
            

            var visible = new Microsoft.Research.DynamicDataDisplay.DataRect(dfs.XOrigin, dfs.YOrigin, dfs.GridSize*dfs.NumberOfColumns, dfs.GridSize * dfs.NumberOfRows);
            var bmp = nc.BuildImage();
            ViewportPanel.SetViewportBounds(image, visible);
            image.Source = bmp;
            plotter.Visible = visible;
            this.UpdateLayout();
            Dfs3plotdfs0.MainWindow.SaveScreen(this, @"c:\temp\test.jpg", (int)ActualWidth, (int)ActualHeight);
          }


        }
      }
    }
  }
}
