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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl;

namespace DataConnectorUI2.controls
{
    /// <summary>
    /// Control for table layer selection
    /// </summary>
    public partial class Layers : UserControl
    {
        private bool isLoaded = false;

        //table to layerName mapping
        private Dictionary<string, string> layerNames = new Dictionary<string, string>()
        {
            {"countries","layerCountries"},
            {"states-provinces","layerStatesProvinces"},
            {"USCounties","layerUSCounties"},
            {"faults","layerFaults"},
            {"earthquakes","layerEarthquakes"}
        };

        public Layers()
        {
            InitializeComponent();
            isLoaded = true;
        }

        /// <remarks>
        ///  map reference used for communicating to parent map control
        /// </remarks>
        private Map _map;
        public Map MapInstance
        {
            get {return _map;}
            set { _map = value; }
        }

        /// <summary>
        /// Opacity adjustment slider
        /// </summary>
        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isLoaded)
            {
                Slider s = sender as Slider;
                string layer = s.Name.Replace("slider", "layer");
                if (_map.FindName(layer).GetType().Equals(typeof(MapTileLayer)))
                { //tiles
                    MapTileLayer ml = _map.FindName(layer) as MapTileLayer;
                    if (ml != null) ml.Opacity = e.NewValue;
                }
                else if (_map.FindName(layer.Replace("layer", "tile")) != null)
                {   //hybrid - Tiles and XAML
                    MapTileLayer mtl = _map.FindName(layer.Replace("layer", "tile")) as MapTileLayer;
                    if (mtl != null) mtl.Opacity = e.NewValue;
                    MapLayer ml = _map.FindName(layer) as MapLayer;
                    if (ml != null) ml.Opacity = e.NewValue;
                }
                else
                {  // WKT and XAML
                    MapLayer ml = _map.FindName(layer) as MapLayer;
                    if (ml != null) ml.Opacity = e.NewValue;
                }
            }
        }

        /// <summary>
        /// Checkbox selection of table layers
        /// </summary>
        private void  LayerCheckBox_Click(object sender, RoutedEventArgs e)
        {
            
            CheckBox cb = sender as CheckBox;
            string layerName = cb.Name.Replace("checkbox", "layer");
            if ((bool)cb.IsChecked)
            {   //Layer Checked
                //initiate a map ViewChange event
                _map.SetView(_map.Center, _map.ZoomLevel); 
            }
            else
            {   //Layer Unchecked
                if (_map.FindName(layerName).GetType().Equals(typeof(MapTileLayer)))
                {   //tiles 
                    ((MapTileLayer)_map.FindName(layerName)).Visibility = Visibility.Collapsed;
                }
                else if (_map.FindName(layerName.Replace("layer", "tile")) != null)
                {   //hybrid tiles 
                    ((MapTileLayer)_map.FindName(layerName.Replace("layer", "tile"))).Visibility = Visibility.Collapsed;
                    ((MapLayer)_map.FindName(layerName)).Children.Clear();
                }
                else
                {   //WKT and XAML
                    ((MapLayer)_map.FindName(layerName)).Children.Clear();
                }
            }
        }
    }
}
