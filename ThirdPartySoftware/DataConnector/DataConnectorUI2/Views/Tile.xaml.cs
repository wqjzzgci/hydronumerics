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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using DataConnectorUI2.controls.HelpContent;
using Microsoft.Maps.MapControl;

namespace DataConnectorUI2
{
    /// <summary>
    /// Tile Service Navigation Page
    /// </summary>
    public partial class Tile : Page
    {
        private bool thematic = false;

        public Tile()
        {
            InitializeComponent();
            MainMap.CredentialsProvider = new ClientTokenCredentialsProvider(App.Current.Host.InitParams["key"]);
            this.SizeChanged += new SizeChangedEventHandler(SizeChangedUpdate);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainMap.ViewChangeStart += new EventHandler<MapEventArgs>(MainMap_ViewChangeStart);
            MainMap.ViewChangeEnd += new EventHandler<MapEventArgs>(MainMap_ViewChangeEnd);

            MainMap.MouseMove += new MouseEventHandler(MainMap_MouseMove);
            UncheckLayers();
            SetMapTileLayers("false");
            LayerTables.MapInstance = MainMap;
        }

        /// <summary>
        /// Initialize Tile source URIs
        ///     MapTileLayer Name must include table name i.e. 'tileCountries' for table 'countries'
        /// </summary>
        /// <param name="thematicstr">string "true" or "false"</param>
        /// <remarks>
        ///     Tile Service referense url is set in web.config AppSettings with key=TileService
        /// </remarks>
        private void SetMapTileLayers(string thematicstr)
        {
            foreach (UIElement item in MainMap.Children)
            {
                if (item is MapTileLayer)
                {
                    MapTileLayer mtile = item as MapTileLayer;
                    string layer = mtile.Name;
                    foreach (LocationRectTileSource tilesource in mtile.TileSources)
                    {
                        tilesource.UriFormat = App.Current.Host.InitParams["TileService"] + layer.Replace("layer", "").ToLower() + "/{quadkey}/"+thematicstr;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the map content of each layer and unchecks the layer checkbox
        /// </summary>
        private void UncheckLayers()
        {
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    cb.IsChecked = false;
                }
            }
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
            //not used
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
        /// Turns on tile layer
        /// Sets Color header by table selection
        /// </summary>
        public void ShowLayers()
        {
            foreach (UIElement item in LayerTables.LayerList.Children)
            {
                if (item is CheckBox)
                {
                    CheckBox cb = item as CheckBox;
                    string layer = cb.Name.Replace("checkbox", "layer");
                    MapTileLayer currentLayer = (MapTileLayer)MainMap.FindName(layer);
                    if ((bool)cb.IsChecked)
                    {
                        currentLayer.Visibility = Visibility.Visible;
                        if (thematic)
                        {
                            
                            layer = layer.Replace("layer", "");
                            switch (layer)
                            {
                                case "Countries":
                                    {
                                        headRange.Text = "Countries by";
                                        colorRange.Text = "increasing Area";
                                        break;
                                    }
                                case "StatesProvinces":
                                    {
                                        headRange.Text = "States and Provinces";
                                        colorRange.Text = "by increasing Area";
                                        break;
                                    }
                                case "USCounties":
                                    {
                                        headRange.Text = "US Counties by";
                                        colorRange.Text = "increasing Area";
                                        break;
                                    }
                                case "Faults":
                                    {
                                        headRange.Text = "Faults by";
                                        colorRange.Text = "increasing ACODE";
                                        break;
                                    }
                                case "Earthquakes":
                                    {
                                        headRange.Text = "Earthquakes by";
                                        colorRange.Text = "increasing Magnitude";
                                        break;
                                    }
                            }
                        }

                        
                    }
                    else
                    {
                        currentLayer.Visibility = Visibility.Collapsed;
                    }
                }
            }
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
                        u = new SQLTablesTile();
                        break;
                    }
                case "Metrics":
                    {
                        u = new MetricsTile();
                        break;
                    }
            }
            help.HelpContent.Content = u;
            help.Show();
        }

        /// <summary>
        /// Turn on thematic tiles
        /// </summary>
        /// <remarks>thematic tiles ar not cached</remarks>
        private void CheckBoxThematic_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if ((bool)cb.IsChecked)
            {
                thematic = true;
                colorChart.Visibility = Visibility.Visible;
                colorLayers.Visibility = Visibility.Collapsed;
                SetMapTileLayers("true");
            }
            else
            {
                thematic = false;
                colorLayers.Visibility = Visibility.Visible;
                colorChart.Visibility = Visibility.Collapsed;
                SetMapTileLayers("false");
            }
        }


    }
}
