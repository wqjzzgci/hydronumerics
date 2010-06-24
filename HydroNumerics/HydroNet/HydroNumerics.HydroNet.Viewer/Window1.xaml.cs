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
    public static ObservableCollection<ModelViewModel> _models = new ObservableCollection<ModelViewModel>();

    public Window1()
    {
      InitializeComponent();

      ModelViewModel mp = new ModelViewModel(@"c:\temp\setup.xml");
      this.DataContext = mp;
      _models.Add(mp);

      String imageURI = GetImagery("55.715094, 12.51892");
      image1.Source = new BitmapImage(new Uri(imageURI));
    }

    private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {

    }

    private void diagram1_NodeCreated(object sender, MindFusion.Diagramming.Wpf.NodeEventArgs e)
    {

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
      mapUriOptions.ZoomLevel = 17;

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
