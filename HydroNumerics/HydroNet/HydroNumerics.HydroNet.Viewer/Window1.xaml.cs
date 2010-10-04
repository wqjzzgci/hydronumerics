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
using System.Collections.ObjectModel;

using System.Windows.Controls.DataVisualization.Charting;


using HydroNumerics.Geometry;
using HydroNumerics.Geometry.UTMConversion;

using HydroNumerics.HydroNet.Core;
using HydroNumerics.HydroNet.ViewModel;
using HydroNumerics.Time.Tools;
using HydroNumerics.Time.Core;

using HydroNumerics.HydroNet.View.GeocodeService;
using HydroNumerics.HydroNet.View.ImageryService;
using HydroNumerics.HydroNet.View.RouteService;
using HydroNumerics.HydroNet.View.SearchService;



namespace HydroNumerics.HydroNet.View
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    WFDLakesViewModel wfd;
    public Window1()
    {
      InitializeComponent();

      wfd = new WFDLakesViewModel();
      tabControl1.DataContext = wfd;


      tabControl1.SelectionChanged += new SelectionChangedEventHandler(tabControl1_SelectionChanged);

      LineSeries LS = new LineSeries();
      LS.ItemsSource = wfd.Precipitation.Items;
      LS.DependentValuePath = "Value";
      LS.IndependentValuePath = "EndTime";
      LS.Title = wfd.Precipitation.Name;
      this.EvapPrecip.Series.Add(LS);

      LineSeries LS2 = new LineSeries();
      LS2.ItemsSource = wfd.Evaporation.Items;
      LS2.DependentValuePath = "Value";
      LS2.IndependentValuePath = "EndTime";
      LS2.Title = wfd.Evaporation.Name;
      this.EvapPrecip.Series.Add(LS2);

      Datagrid1.SelectionMode = Microsoft.Windows.Controls.DataGridSelectionMode.Single;
      Datagrid1.SelectionChanged += new SelectionChangedEventHandler(Datagrid1_SelectionChanged);

    }

    void Datagrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      double x = (double)((System.Data.DataRowView)Datagrid1.SelectedItem)[11];
      double y = (double)((System.Data.DataRowView)Datagrid1.SelectedItem)[12];

      var utm = new GPS.UTM(x,y,32);

      try
      {
        String imageURI = GetImagery(utm.Latitude.GetAbsoluteDecimalCoordinate() + ", " + utm.Longitude.GetDecimalCoordinate());
        image1.Source = new BitmapImage(new Uri(imageURI));
      }
      catch (Exception ee)
      { }
    }

    void tabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
     
      WFDLake wf;
      if (Datagrid1.SelectedItem == null)
        wf = wfd.Lakes["Vedsted Sø"];
      else
        wf = wfd.Lakes[(string)((System.Data.DataRowView)Datagrid1.SelectedItem)[0]];
      
      
      WaterBal.DataContext = new WaterBodyViewModel((AbstractWaterBody)ModelFactory.GetModel(@"C:\Users\Jacob\Work\HydroNumerics\HydroNet\TestData\vedsted2.xml")._waterBodies.First());
    }


 
    private string GetImagery(string locationString)
    {
      string key = "AnMBF3YY-9Cu2a6Og_vid1aCqNpho_WUux8hnOKgtYAK7zFR-WUtXXz31fxPNaCP";
      MapUriRequest mapUriRequest = new MapUriRequest();

      // Set credentials using a valid Bing Maps key
      mapUriRequest.Credentials = new ImageryService.Credentials();
      mapUriRequest.Credentials.ApplicationId = key;

      // Set the location of the requested image
      mapUriRequest.Center = new ImageryService.Location();
      string[] digits = locationString.Split(',');
      mapUriRequest.Center.Latitude = double.Parse(digits[0].Trim());
      mapUriRequest.Center.Longitude = double.Parse(digits[1].Trim());

      // Set the map style and zoom level
      MapUriOptions mapUriOptions = new MapUriOptions();
      mapUriOptions.Style = MapStyle.AerialWithLabels;
      mapUriOptions.ZoomLevel = 13;

      // Set the size of the requested image in pixels
      mapUriOptions.ImageSize = new ImageryService.SizeOfint();
      mapUriOptions.ImageSize.Height = 400;
      mapUriOptions.ImageSize.Width = 600;

      mapUriRequest.Options = mapUriOptions;



      //Make the request and return the URI
      ImageryServiceClient imageryService = new ImageryServiceClient("BasicHttpBinding_IImageryService");
      MapUriResponse mapUriResponse = imageryService.GetMapUri(mapUriRequest);
      return mapUriResponse.Uri;
    }

    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
  }
}
