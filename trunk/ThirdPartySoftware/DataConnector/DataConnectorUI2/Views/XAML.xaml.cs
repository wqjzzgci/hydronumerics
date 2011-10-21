/* ******************************************************************************
 * 
 * Copyright 2010 Microsoft Corporation
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy of 
 * the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
 * MERCHANTABLITY OR NON-INFRINGEMENT. 
 *  
 * See the Apache 2 License for the specific language governing permissions and
 * limitations under the License.
 * 
 ******************************************************************************* */


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataConnectorUI2.controls.HelpContent;
using DataConnectorUI2.XAMLServiceReference;
using Microsoft.Maps.MapControl;

namespace DataConnectorUI2
{
    public partial class XAML : Page
    {
        //global variables
        private int totalFeatures = 0;
        private int totalPoints = 0;
        private int totalByteSize = 0;
        private int layerCnt = 0;
        private int recordLimit = 1000;
        private DateTime loadStart;
        private DateTime renderStart;
        private DateTime renderStop;
        private double queryTime = 0;
        private Dictionary<string, LoadStyle> LayerStyle = new Dictionary<string, LoadStyle>();

        public XAML()
        {
            InitializeComponent();
            MainMap.CredentialsProvider = new ClientTokenCredentialsProvider(App.Current.Host.InitParams["key"]);
            this.SizeChanged += new SizeChangedEventHandler(SizeChangedUpdate);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainMap.ViewChangeStart += new EventHandler<MapEventArgs>(MainMap_ViewChangeStart);
            MainMap.ViewChangeOnFrame += new EventHandler<MapEventArgs>(MainMap_ViewChangeOnFrame);
            MainMap.ViewChangeEnd += new EventHandler<MapEventArgs>(MainMap_ViewChangeEnd);

            MainMap.MouseMove += new MouseEventHandler(MainMap_MouseMove);
            drawtools.MapInstance = MainMap;
            LayerTables.MapInstance = MainMap;

            LayerStyle.Add("layerCountries", new LoadStyle("#FFFF0000", "#FF00FF00",0.25));
            LayerStyle.Add("layerStatesProvinces", new LoadStyle("#FFFF0000", "#FF0000FF",0.25));
            LayerStyle.Add("layerUSCounties", new LoadStyle("#FFFF0000", "#FF00FFFF",0.25));
            LayerStyle.Add("layerFaults", new LoadStyle("#FFFF0000", "#FF000000",1.0));
            LayerStyle.Add("layerEarthquakes", new LoadStyle("#FFFF0000", "#FFFFFF00",1.0));
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // not used
        }


        /// <summary>
        /// Adjusts location of drag panels on Map Control
        /// </summary>
        private void SizeChangedUpdate(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(SidePanelBorder, this.ActualWidth - SidePanelBorder.ActualWidth - 10);
            ScrollSidebar.MaxHeight = this.ActualHeight - 50;
        }

        /// <summary>
        /// Displays lat,lon location of cursor
        /// </summary>
        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            System.Windows.Point p = e.GetPosition(MainMap);
            Location LL = MainMap.ViewportPointToLocation(p);
            LLText.Text = String.Format("{0,10:0.000000},{1,11:0.000000}", LL.Latitude, LL.Longitude);
        }

        private void MainMap_ViewChangeStart(object sender, MapEventArgs e)
        {
            // not used
        }

        /// <summary>
        /// Adjusts buffer thickness for polyline buffer by zoom level
        /// </summary>
        private void MainMap_ViewChangeOnFrame(object sender, MapEventArgs e)
        {
            if (drawtools.drawbuffer != null)
            {
                drawtools.drawbuffer.StrokeThickness = drawtools.BufferSlider.Value * 2000
                    / MiscFunctions.GetMapResolution(drawtools.drawbuffer.Locations[0].Latitude, MainMap.ZoomLevel);
            }
        }

        /// <summary>
        /// MainMap_ViewChangeEnd(
        ///     this event is triggered by user selection changes in side menu
        /// </summary>
        private void MainMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            if (MainMap != null)
            {
                ZLText.Text = String.Format("{0,5:0.00} ", MainMap.ZoomLevel);
                ShowLayers();
            }
        }

        /// <summary>
        /// Checks for valid AOI
        /// </summary>
        /// <param name="loc1">Location lat,lon</param>
        /// <param name="loc2">Location lat,lon</param>
        /// <returns>bool - also Shows Message to user</returns>
        private bool validAOI(Location loc1, Location loc2)
        {
            bool valid = true;
            if (loc1.Latitude == loc2.Latitude ||
                loc1.Longitude == loc2.Longitude)
            {
                valid = false;
                ShowMessage("AOI has zero width or height.\nPlease draw a valid area of interest.");
            }
            return valid;
        }

        /// <summary>
        /// Checks for Hemisphere crossing
        /// </summary>
        /// <param name="loc1">Location lat,lon</param>
        /// <param name="loc2">Location lat,lon</param>
        /// <returns>bool - also Shows Message to user</returns>
        private bool WithinHemisphere(Location loc1, Location loc2)
        {
            bool within = true;
            if (Math.Abs(loc1.Longitude - loc2.Longitude) > 180 ||
                Math.Abs(loc1.Latitude - loc2.Latitude) > 90)
            {
                within = false;
                ShowMessage("Bounding box cannot exceed hemisphere for Geography SQL Data Type Queries.\n delta longitude < 180 and delta latitude < 90 \n" +
                     String.Format(" actual AOI: {0,10:0.00} x {1,11:0.00}", Math.Abs(loc1.Longitude - loc2.Longitude), Math.Abs(loc1.Latitude - loc2.Latitude)) +
                     "\nPlease try a smaller area of interest.");
            }
            return within;
        }

        /// <summary>
        /// Calls WKT Service 
        ///     Initializes input parameters
        ///     Calls Asynch service
        /// </summary>
        /// <remarks>
        /// Drawtools leaves the area of interest definition on MapLayer 'layerDraw'
        /// </remarks>
        public void ShowLayers()
        {
            // get list of checked layers
            List<string> layers = GetLayers();
            layerCnt = layers.Count;
            if (layers.Count > 0)
            {
                totalFeatures = 0;
                totalPoints = 0;
                totalByteSize = 0;

                string queryType = null;
                string area = null;
                double radius = 0.0;
                double reduce = 4000 - MainMap.ZoomLevel * 500;
                if (reduce < 0) reduce = 0;
                if ((bool)drawtools.Proximity.IsChecked)
                {
                    queryType = "buffer";
                    Ellipse pt = (Ellipse)layerDraw.FindName("Proximity");
                    if (pt != null && pt.Tag != null)
                    {
                        Location loc = MapLayer.GetPosition(pt);
                        //radius(meters) longitude,latitude
                        if (pt.Tag != null) radius = Double.Parse(pt.Tag.ToString());
                        else radius = 1.0;
                        area = loc.Longitude + " " + loc.Latitude;
                    }
                }
                else if ((bool)drawtools.PolyBuffer.IsChecked)
                {
                    queryType = "buffer";
                    //buffer width(meters)  lon,lat lon,lat ... lon,lat
                    MapPolyline poly = (MapPolyline)layerDraw.FindName("Buffer");
                    if (poly != null)
                    {
                        Slider s = (Slider)drawtools.FindName("BufferSlider");
                        radius = (s.Value * 1000);
                        StringBuilder sb = new StringBuilder();
                        bool first = true;
                        foreach (Location l in poly.Locations)
                        {
                            if (!first) sb.Append(",");
                            sb.Append(l.Longitude + " " + l.Latitude);
                            first = false;
                        }
                        area = sb.ToString();
                    }
                }
                else if ((bool)drawtools.AOI.IsChecked)
                {
                    queryType = "bbox";
                    StringBuilder sb = new StringBuilder();
                    MapPolygon drawrect = ((MapPolygon)layerDraw.FindName("AOI"));

                    if (drawrect != null)
                    {
                        // check for hemisphere overrun
                        if (validAOI(drawrect.Locations[0], drawrect.Locations[2]) && 
                            WithinHemisphere(drawrect.Locations[0], drawrect.Locations[2]))
                        {
                            foreach (Location l in drawrect.Locations)
                            {
                                sb.Append(l.Longitude + " " + l.Latitude + ",");
                            }
                            sb.Append(drawrect.Locations[0].Longitude + " " + drawrect.Locations[0].Latitude);
                            area = sb.ToString();
                        }
                        else
                        {
                            drawtools.draw = true;
                        }
                    }
                }
                else if ((bool)drawtools.Viewport.IsChecked)
                {
                    queryType = "bbox";
                    StringBuilder sb = new StringBuilder();
                    LocationRect bounds = MainMap.BoundingRectangle;
                    // check for hemisphere overrun
                    if (validAOI(bounds.Southeast, bounds.Northwest) && 
                        WithinHemisphere(bounds.Southeast, bounds.Northwest))
                    {
                        sb.Append(bounds.Southeast.Longitude + " ");
                        sb.Append(bounds.Southeast.Latitude + ",");
                        sb.Append(bounds.Northeast.Longitude + " ");
                        sb.Append(bounds.Northeast.Latitude + ",");
                        sb.Append(bounds.Northwest.Longitude + " ");
                        sb.Append(bounds.Northwest.Latitude + ",");
                        sb.Append(bounds.Southwest.Longitude + " ");
                        sb.Append(bounds.Southwest.Latitude + ",");
                        sb.Append(bounds.Southeast.Longitude + " ");
                        sb.Append(bounds.Southeast.Latitude);
                        area = sb.ToString();
                    }
                    else
                    {
                        drawtools.draw = true;
                    }
                }
                if (area != null)
                {
                    // disable side menu and start load spinner until call returns
                    SidePanelBorder.IsHitTestVisible = false;
                    loaderStart();

                    foreach (string layer in layers)
                    {
                        XAMLClient svc = new XAMLClient("CustomBinding_IXAML");
                        svc.GetSQLDataXAMLCompleted += new EventHandler<GetSQLDataXAMLCompletedEventArgs>(XAMLService_GetSQLDataXAMLCompleted);
                        XAMLParameters parameters = new XAMLParameters();
                        parameters.table = layer.Replace("layer", "").ToLower();
                        parameters.querytype = queryType;
                        parameters.reduce = reduce;
                        parameters.radius = radius;
                        parameters.points = area;
                        svc.GetSQLDataXAMLAsync(parameters,layer);
                    }
                }
            }
            else
            {
                drawtools.draw = true;
            }
        }

        /// <summary>
        /// Return result of XAML Service call
        /// Render geometry to layer
        /// </summary>
        /// <remarks>
        /// if result is greater than recordLimit show a message to reduce area of interest
        /// </remarks>
        private void XAMLService_GetSQLDataXAMLCompleted(object sender, GetSQLDataXAMLCompletedEventArgs e)
        {
            string layer = e.UserState.ToString();
            if (e.Error == null)
            {
                int resultCnt = e.Result.OutputFields.Count;
                if (resultCnt > 0)
                {
                    if (resultCnt < recordLimit)
                    {
                        totalFeatures += e.Result.OutputFields.Count;
                        totalByteSize += e.Result.XAML.Length;
                        queryTime = e.Result.QueryTime;
                        totalPoints += e.Result.totalPoints;
                        renderStart = DateTime.Now;
                        MapLayer currentLayer = (MapLayer)MainMap.FindName(layer);
                        MapLayer newLayer = (MapLayer)XamlReader.Load(e.Result.XAML);
                        currentLayer.Children.Add(newLayer);
                       
                        foreach (XAMLServiceReference.XAMLFields shp in e.Result.OutputFields)
                        {
                            UIElement el = (UIElement)newLayer.FindName(shp.ID);
                            if (el != null)
                            {

                                el.MouseEnter += polygon_MouseEnter;
                                el.MouseLeave += polygon_MouseLeave;

                                StringBuilder label = new StringBuilder("ID = " + shp.ID + "\n");
                                foreach (KeyValuePair<string, string> field in shp.Fields)
                                {
                                    label.Append(field.Key + ":" + field.Value + "\n");
                                }
                               
                                ToolTip tt =  AddToolTip(label.ToString());
                                ToolTipService.SetToolTip(el, tt);

                                if (el.GetType().Equals(typeof(Pushpin)))
                                {
                                    Pushpin p = (Pushpin)el;
                                    p.PositionOrigin = PositionOrigin.BottomCenter;
                                    p.Background = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].fill));
                                    p.Foreground = new SolidColorBrush(Colors.Black);
                                    p.FontSize = 8;
                                }

                                if (el.GetType().Equals(typeof(MapLayer)))
                                {
                                    MapLayer p = (MapLayer)el;
                                    foreach (MapPolygon mp in p.Children)
                                    {
                                        mp.Fill = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].fill));
                                        mp.Stroke = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].stroke));
                                        mp.Opacity = LayerStyle[layer].opacity;
                                    }
                                }

                                if (el.GetType().Equals(typeof(MapPolyline)))
                                {
                                    MapPolyline p = (MapPolyline)el;
                                    p.Stroke = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].stroke));
                                    p.StrokeThickness = 2;
                                    p.StrokeMiterLimit = 0;
                                }
                                if (el.GetType().Equals(typeof(MapPolygon)))
                                {
                                    MapPolygon p = (MapPolygon)el;
                                    p.Stroke = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].stroke));
                                    p.Fill = new SolidColorBrush(MiscFunctions.ColorFromInt(LayerStyle[layer].fill));
                                    p.Opacity = LayerStyle[layer].opacity;
                                }
                            }
                        }
                        renderStop = DateTime.Now;
                    }
                    else
                    {
                        ShowMessage("Too many records for Vector display, " + resultCnt + ". Try a smaller area of interest.");
                    }
                }
            }
            else
            {
                ShowMessage("Error occurred while loading layer from database:" + e.Error.Message);
            }
            // if this is the last layer call cleanup loading and allow menu interaction
            if (--layerCnt == 0)
            {
                SidePanelBorder.IsHitTestVisible = true;
                loaderStop();
            }
            //update metrics
            ((TextBlock)metrics.FindName("features")).Text = totalFeatures.ToString();
            ((TextBlock)metrics.FindName("points")).Text = totalPoints.ToString();
            ((TextBlock)metrics.FindName("bytesize")).Text = totalByteSize.ToString();
        }

        /// <summary>
        /// Add ToolTip with styling parameters
        /// </summary>
        private ToolTip AddToolTip(string fields)
        {
            ToolTip tt = new ToolTip();
            tt.Opacity = 0.75;
            tt.Background = new SolidColorBrush(MiscFunctions.ColorFromInt("#FF222222"));
            tt.Foreground = new SolidColorBrush(Colors.White);
            tt.BorderBrush = new SolidColorBrush(Colors.Black);
            tt.Padding = new Thickness(5);
            tt.Placement = PlacementMode.Mouse;
            tt.Content = fields;
            return tt;
        }

        /// <summary>
        /// returns true if any layers are checked
        /// </summary>
        /// <returns>bool</returns>
        private bool LayersChecked()
        {
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    if ((bool)cb.IsChecked) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Scans Layer Control for checked layers
        /// </summary>
        /// <returns>List of layer names</returns>
        private List<string> GetLayers()
        {
            List<string> layers = new List<string>();
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    string layer = cb.Name.Replace("checkbox", "layer");
                    if ((bool)cb.IsChecked) layers.Add(layer);
                }
            }
            return layers;
        }


        /// <summary>
        /// event handler for polygon mouse enter hint
        /// reduces opacity on enter
        /// </summary>
        private void polygon_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement)
            {
                UIElement p = sender as UIElement;
                p.Opacity = 0.5;
                //p.Projection.SetValue(PlaneProjection.GlobalOffsetZProperty, 200.0);
            }
        }

        /// <summary>
        /// event handler for polygon mouse leave
        /// returns to original opacity
        /// </summary>
        private void polygon_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is UIElement)
            {
                UIElement p = sender as UIElement;
                //p.Projection.SetValue(PlaneProjection.GlobalOffsetZProperty, 0.0);
                p.Opacity = 1;
            }
        }

        /// <summary>
        /// event handler for point mouse enter hint
        /// sets a ZProperty value (Brings pushpin out of page, closer to viewer)
        /// </summary>
        private void pushpin_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement)
            {
                UIElement pin = sender as UIElement;
                pin.Projection.SetValue(PlaneProjection.GlobalOffsetZProperty, 200.0);
            }
        }


        /// <summary>
        /// event handler for point mouse leave
        /// sets a ZProperty value back to 0.0
        /// </summary>
        private void pushpin_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is UIElement)
            {
                UIElement pin = sender as UIElement;
                pin.Projection.SetValue(PlaneProjection.GlobalOffsetZProperty, 0.0);

            }
        }


        /// <summary>
        /// Start Load Spinner and initialize timing 
        /// </summary>
        private void loaderStart()
        {
            ClearLayersContent();
            drawtools.draw = false;
            loadStart = DateTime.Now;
            Loading.Visibility = Visibility.Visible;
            LoadSpinner.spinnerAnimate.Begin();
        }

        /// <summary>
        /// Stop Load Spinner
        /// Update metrics
        /// </summary>
        /// <remarks>
        /// To avoid unnecessarily consuming cpu cycles always stop spinner animation before collapsing visibility
        /// </remarks>
        private void loaderStop()
        {
            double total = (DateTime.Now - loadStart).TotalMilliseconds;
            ((TextBlock)metrics.FindName("loadtime")).Text = String.Format("{0,0:0}ms", (DateTime.Now - loadStart).TotalMilliseconds);
            ((TextBlock)metrics.FindName("rendertime")).Text = String.Format("{0,0:0}ms", (renderStop - renderStart).TotalMilliseconds);
            ((TextBlock)metrics.FindName("querytime")).Text = String.Format("{0,0:0}ms", queryTime);
            ((TextBlock)metrics.FindName("transmittime")).Text = String.Format("{0,0:0}ms", total - queryTime - (renderStop - renderStart).TotalMilliseconds);
            LoadSpinner.spinnerAnimate.Stop();
            Loading.Visibility = Visibility.Collapsed;
            drawtools.draw = true;
        }


        /// <summary>
        /// Manually initiate query
        /// </summary>
        private void Query_ButtonClick(object sender, RoutedEventArgs e)
        {
            ShowLayers();
        }

        /// <summary>
        /// Reset to original settings
        /// </summary>
        private void Reset_ButtonClick(object sender, RoutedEventArgs e)
        {
            ClearLayers();
            ClearQueryFilters();
            layerDraw.Children.Clear();
            ((CheckBox)LayerTables.FindName("checkboxCountries")).IsChecked = true;
            MainMap.SetView(new Location(0, 0), 2);
        }

        /// <summary>
        /// Clear contents of each layer 
        /// </summary>
        private void ClearLayersContent()
        {
            // Clears the map content of each layer
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    string layer = cb.Name.Replace("checkbox", "layer");
                    ((MapLayer)MainMap.FindName(layer)).Children.Clear();
                }
            }
        }

        /// <summary>
        /// Clear contents of each layer and uncheck layer
        /// </summary>
        private void ClearLayers()
        {
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    cb.IsChecked = false;
                    string layer = cb.Name.Replace("checkbox", "layer");
                    ((MapLayer)MainMap.FindName(layer)).Children.Clear();
                }
            }
        }


        /// <summary>
        /// Uncheck drawtool area of interest checkboxes
        /// </summary>
        private void ClearQueryFilters()
        {
            foreach (UIElement item in drawtools.FilterPanel.Children)
            {
                if (item is Grid)
                {
                    Grid g = item as Grid;
                    foreach (UIElement griditem in g.Children)
                    {
                        if (griditem is CheckBox)
                        {
                            CheckBox cb = griditem as CheckBox;
                            cb.IsChecked = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ShowMessage in a Silverlight popup ChildWindow
        /// </summary>
        public void ShowMessage(string message)
        {
            MessageWindow mw = new MessageWindow();
            mw.MessageText.Text = message;
            mw.Show();
        }

        /// <summary>
        /// Create HelpWindow
        /// add contents and open help window
        /// </summary>
        private void Help_ButtonClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            HelpWindow help = new HelpWindow();
            help.Title = base.Tag;
            UserControl u = null;
            switch (b.Tag.ToString())
            {
                case "SQL Tables":
                    {
                        u = new SQLTablesXAML();
                        break;
                    }
                case "Query Filter":
                    {
                        u = new QueryFilterXAML();
                        break;
                    }
                case "Metrics":
                    {
                        u = new MetricsXAML();
                        break;
                    }
            }

            help.HelpContent.Content = u;
            help.Show();
        }


    }
}
