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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;

namespace DataConnectorUI2.controls
{
    /// <summary>
    /// DrawTools Control. 
    ///     Draw Tools
    ///         1. drawline
    ///         2. drawrect
    ///         3. drawcir
    ///         4. drawpt
    /// </summary>
    public partial class DrawTools : UserControl
    {

        private bool startdraw = false;
        public bool draw = false;
        private MapPolyline drawline;
        public MapPolyline drawbuffer;
        private MapPolygon drawrect;
        private MapPolygon drawcir;
        private Location centerpoint;
        private Ellipse center;
        private double dist;

        private bool isLoaded = false;

        public DrawTools()
        {
            InitializeComponent();
            isLoaded = true;
        }

        /// <remarks>
        ///  map reference used for communicating to parent map control
        /// </remarks>
        public MapLayer _layerDraw;
        private Map _map;
        public Map MapInstance
        {
            get
            {
                return _map;
            }
            set
            {
                _map = value;
                _layerDraw = (MapLayer)_map.FindName("layerDraw");
            }
        }



        #region Draw Tools
        /// <summary>
        /// Selection of tool type.
        ///     Proximity
        ///     PolyBuffer
        ///     AOI
        ///     Viewport
        /// </summary>
        private void Draw_RadioButton_Click(object sender, RoutedEventArgs e)
        {
            ClearDraw();
            CheckBox scb = sender as CheckBox;
            if (scb.IsChecked == true)
            {
                ClearFilterChecks();

                switch (scb.Name)
                {
                    case "Proximity":
                        {
                            draw = true;
                            Proximity.IsChecked = true;
                            startdraw = true;
                            _map.MouseLeftButtonUp += new MouseButtonEventHandler(DrawPoint_MouseLeftButtonUp);
                            
                            break;
                        }
                    case "PolyBuffer":
                        {
                            draw = true;
                            PolyBuffer.IsChecked = true;
                            startdraw = true;
                            _map.MouseLeftButtonUp += new MouseButtonEventHandler(DrawPolyline_MouseClick);

                            break;
                        }
                    case "AOI":
                        {
                            draw = true;
                            AOI.IsChecked = true;
                            startdraw = true;
                            _map.MouseLeftButtonUp += new MouseButtonEventHandler(DrawRect_MouseClick);
                            break;
                        }
                    case "Viewport":
                        {
                            Viewport.IsChecked = true;
                            _map.SetView(_map.Center, _map.ZoomLevel);
                            break;
                        }
                }
            }

        }

        private void MaskPan(object sender, MapMouseEventArgs e)
        {
            //mask map pan events
            e.Handled = true;
        }

        /// <summary>
        /// Proximity setup and teardown
        /// </summary>
        private void DrawPoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(draw)
            {
                Location loc = _map.ViewportPointToLocation(e.GetPosition(_map));
                if (startdraw)
                {
                    //first click
                    _layerDraw.Children.Clear();
                    centerpoint = loc;
                    e.Handled = true;
                    _map.MouseMove += new MouseEventHandler(DrawPoint_MouseMove);
                    drawcir = new MapPolygon();
                    drawcir.Stroke = (SolidColorBrush)this.Resources["DrawToolStrokeBrush"];
                    drawcir.Fill = (SolidColorBrush)this.Resources["DrawToolFillBrush"];
                    drawcir.StrokeThickness = 2;
                    drawcir.Locations = DrawCircle(loc, 0.0, 10);
                    _layerDraw.Children.Add(drawcir);

                    center = new Ellipse();
                    center.Name = "Proximity";
                    center.Fill = (SolidColorBrush)this.Resources["DrawToolStrokeBrush"];
                    center.Width = 5;
                    center.Height = 5;
                    MapLayer.SetPositionOrigin(center, PositionOrigin.Center);
                    MapLayer.SetPosition(center, loc);
                    _layerDraw.Children.Add(center);
                    startdraw = false;

                }
                else
                {
                    //second click
                    draw = false;
                    double d = HaversineDistance(centerpoint, loc);
                    center.Tag = HaversineDistance(centerpoint, loc).ToString();
                    _map.MouseMove -= DrawPoint_MouseMove;
                    startdraw = true;
                    //initiate a map ViewChange event
                    _map.SetView(_map.Center, _map.ZoomLevel);
                }
            }
        }

        /// <summary>
        /// Dynamic stretch for proximity draw tool
        /// </summary>
        private void DrawPoint_MouseMove(object sender, MouseEventArgs e)
        {
                Location p = _map.ViewportPointToLocation(e.GetPosition(_map));
                double dx = (p.Longitude - centerpoint.Longitude);
                double dy = (p.Latitude - centerpoint.Latitude);
                dist = Math.Sqrt(dx * dx + dy * dy);
                drawcir.Locations = DrawCircle(centerpoint, dist, 10);
        }

        /// <summary>
        /// DrawCircle
        ///     function to draw a circle 
        /// </summary>
        /// <param name="cp">Center point Location lat,lon</param>
        /// <param name="radius">double radius in decimal degrees</param>
        /// <param name="step">int step to indicate the increment to step around 360 degree circle</param>
        /// <returns></returns>
        private LocationCollection DrawCircle(Location cp, double radius, int step)
        {
            LocationCollection locs = new LocationCollection();
            drawcir.Locations = new LocationCollection();
            for (float angle = 0; angle <= 360; angle += step)
            {
                Location pt = new Location();
                pt.Longitude = (radius * Math.Cos(angle * 0.0174532925)) + cp.Longitude;
                pt.Latitude = (radius * Math.Sin(angle * 0.0174532925)) + cp.Latitude;
                locs.Add(pt);
            }
            return locs;
        }

        /// <summary>
        /// DrawPolyline_MouseClick
        ///     draw polyline as a series of click nodes
        ///     drawn as two polylines
        ///         base buffer thickness
        ///         top polyline
        /// </summary>
        private void DrawPolyline_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if(draw)
            {
                Location loc = _map.ViewportPointToLocation(e.GetPosition(_map));
                if (startdraw)
                {
                    //first click
                    _layerDraw.Children.Clear();
                    //add a profile drawline of zero length
                    _layerDraw.CaptureMouse();
                    _map.MouseMove += new MouseEventHandler(DrawPolyline_MouseMove);
                    _map.MouseDoubleClick += new EventHandler<MapMouseEventArgs>(DrawPolyline_MouseDoubleClick);

                    drawbuffer = new MapPolyline();
                    drawbuffer.Name = "Buffer";
                    drawbuffer.StrokeEndLineCap = PenLineCap.Round;
                    drawbuffer.StrokeStartLineCap = PenLineCap.Round;
                    drawbuffer.StrokeLineJoin = PenLineJoin.Round;
                    drawbuffer.StrokeMiterLimit = 0;
                    drawbuffer.Stroke = (SolidColorBrush)this.Resources["DrawToolFillBrush"];

                    drawbuffer.Locations = new LocationCollection();
                    drawbuffer.Locations.Add(loc);
                    drawbuffer.Locations.Add(loc);
                    _layerDraw.Children.Add(drawbuffer);

                    drawline = new MapPolyline();
                    drawline.Stroke = (SolidColorBrush)this.Resources["DrawToolStrokeBrush"];
                    drawline.StrokeThickness = 2;
                    drawline.StrokeEndLineCap = PenLineCap.Flat;
                    drawline.StrokeLineJoin = PenLineJoin.Bevel;
                    drawline.StrokeMiterLimit = 0;
                    drawline.Locations = new LocationCollection();
                    drawline.Locations.Add(loc);
                    drawline.Locations.Add(loc);
                    _layerDraw.Children.Add(drawline);
                    startdraw = false;
                }
                else
                {
                    //subsequent clicks
                    drawline.Locations.Add(loc);
                    drawbuffer.Locations.Add(loc);
                    if (drawbuffer.Locations.Count < 4)
                    {
                        drawbuffer.StrokeThickness = BufferSlider.Value * 2000 / GetMapResolution(loc.Latitude, _map.ZoomLevel);
                    }
                }
           }
 
        }

        /// <summary>
        /// Polyline rubberband line segment stretch
        /// </summary>
        private void DrawPolyline_MouseMove(object sender, MouseEventArgs e)
        {
                if (!startdraw)
                {
                    Location loc = _map.ViewportPointToLocation(e.GetPosition(_map));
                    drawbuffer.Locations[drawbuffer.Locations.Count - 1] = loc;
                    drawline.Locations[drawline.Locations.Count - 1] = loc;
                }
        }

        /// <summary>
        /// Double click finishes buffer polyline draw
        /// </summary>
        private void DrawPolyline_MouseDoubleClick(object sender, MapMouseEventArgs e)
        {
                if (!startdraw)
                {
                    draw = false;
                    e.Handled = true;
                    Location loc = _map.ViewportPointToLocation(e.ViewportPoint);
                    drawbuffer.Locations[drawbuffer.Locations.Count - 1] = loc;
                    drawline.Locations[drawline.Locations.Count - 1] = loc;
                    startdraw = true;
                    //initiate a map ViewChange event
                    _map.SetView(_map.Center, _map.ZoomLevel);
                }
        }

        /// <summary>
        /// Slider changes buffer radius StrokeThickness approximated using GetMapResolution
        /// </summary>
        private void BufferSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isLoaded)
            {
                BufferText.Text = string.Format("{0:F1}", e.NewValue);
                if (drawbuffer != null)
                {
                    draw = false;
                    drawbuffer.StrokeThickness = BufferSlider.Value * 2000 / GetMapResolution(drawbuffer.Locations[0].Latitude, _map.ZoomLevel);
                    //initiate a map ViewChange event
                    _map.SetView(_map.Center, _map.ZoomLevel);
                }
            }
        }

        /// <summary>
        /// DrawRect_MouseClick
        ///     Rubberband rectangle as MapPolygon drawn from two clicks
        /// </summary>
        private void DrawRect_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (draw)
            {
                Location loc = _map.ViewportPointToLocation(e.GetPosition(_map));
                if (startdraw)
                {
                    //first click
                    _layerDraw.Children.Clear();
                    e.Handled = true;
                    _map.MouseMove += DrawRect_MouseMove;
                    drawrect = new MapPolygon();
                    drawrect.Name = "AOI";
                    drawrect.Stroke = (SolidColorBrush)this.Resources["DrawToolStrokeBrush"];
                    drawrect.StrokeThickness = 2;
                    drawrect.Fill = (SolidColorBrush)this.Resources["DrawToolFillBrush"];
                    drawrect.Locations = new LocationCollection();
                    drawrect.Locations.Add(loc);
                    drawrect.Locations.Add(loc);
                    drawrect.Locations.Add(loc);
                    drawrect.Locations.Add(loc);
                    _layerDraw.Children.Add(drawrect);
                    startdraw = false;
                }
                else
                {
                    //second click
                    draw = false;
                    startdraw = true;
                    _map.MouseMove -= DrawRect_MouseMove;
                    //initiate a map ViewChange event
                    _map.SetView(_map.Center, _map.ZoomLevel);
                }
            }
        }

        /// <summary>
        /// Rectangle rubberband stretch
        /// </summary>
        private void DrawRect_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(_layerDraw);
            Location loc = _map.ViewportPointToLocation(p);
            //For Geography order must be counterclockwise
            double order = (loc.Longitude-drawrect.Locations[0].Longitude) * (loc.Latitude-drawrect.Locations[0].Latitude);
            if (order < 0)
            {
                drawrect.Locations[3] = new Location(drawrect.Locations[0].Latitude, loc.Longitude);
                drawrect.Locations[2] = loc;
                drawrect.Locations[1] = new Location(loc.Latitude, drawrect.Locations[0].Longitude);
            }
            else
            {
                drawrect.Locations[1] = new Location(drawrect.Locations[0].Latitude, loc.Longitude);
                drawrect.Locations[2] = loc;
                drawrect.Locations[3] = new Location(loc.Latitude, drawrect.Locations[0].Longitude);
            }
        }

        /// <summary>
        /// Cleanup Draw tools
        /// </summary>
        private void ClearDraw()
        {
            startdraw = false;
            _layerDraw.Children.Clear();
            _map.MouseLeftButtonUp -= DrawPoint_MouseLeftButtonUp;
            _map.MouseLeftButtonUp -= DrawPolyline_MouseClick;
            _map.MouseLeftButtonUp -= DrawRect_MouseClick;
            _map.MouseMove -= DrawPoint_MouseMove;
            _map.MouseMove -= DrawPolyline_MouseMove;
            _map.MouseMove -= DrawRect_MouseMove;

        }

        /// <summary>
        /// Clear checkboxes in draw tools selection
        /// </summary>
        public void ClearFilterChecks()
        {
            Proximity.IsChecked = false;
            PolyBuffer.IsChecked = false;
            AOI.IsChecked = false;
            Viewport.IsChecked = false;
        }

        #endregion

        /// <summary>
        /// Map resolution formula from Bing Maps site:
        ///     http://msdn.microsoft.com/en-us/library/aa940990.aspx
        /// </summary>
        /// <param name="latitude">decimal latitude</param>
        /// <param name="zoomlevel">double zoom level</param>
        /// <returns></returns>
        private double GetMapResolution(double latitude, double zoomlevel)
        {
            return 156543.04 * Math.Cos(latitude * 0.0174532925) / Math.Pow(2.0, zoomlevel);
        }


        /// <summary>
        /// HaversineDistance
        ///     Calculates shortest surface distance between to points
        ///     for a Spheroidal earth model
        ///     ref: http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="P1">Location lat,lon</param>
        /// <param name="P2">Location lat,lon</</param>
        /// <returns>double distance</returns>
        private double HaversineDistance(Location P1, Location P2)
        {
            double EarthRadius = 6378137.0;
            double ToRadians = Math.PI / 180;
           
            double dLat = (P2.Latitude - P1.Latitude) * ToRadians;
            double dLon = (P2.Longitude - P1.Longitude) * ToRadians;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                     Math.Cos(P1.Latitude * ToRadians) * Math.Cos(P2.Latitude * ToRadians) *
                     Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadius * c;
        }


        /// <summary>
        /// ColorFromInt
        /// Returns a Color from hex string i.e. #FF00FF00
        /// </summary>
        /// <param name="hex">string hex color with alpha, red, green, blue</param>
        /// <returns>Color</returns>
        private Color ColorFromInt(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            int c = int.Parse(hex, NumberStyles.AllowHexSpecifier);
            return Color.FromArgb((byte)((c >> 0x18) & 0xff),
                (byte)((c >> 0x10) & 0xff),
                (byte)((c >> 8) & 0xff),
                (byte)(c & 0xff));
        }
    }
}
