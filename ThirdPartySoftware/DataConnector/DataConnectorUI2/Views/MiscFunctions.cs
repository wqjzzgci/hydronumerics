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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Globalization;

namespace DataConnectorUI2
{
    public class MiscFunctions
    {
        /// <summary>
        /// ColorFromInt
        /// Returns a Color from hex string i.e. #FF00FF00
        /// </summary>
        /// <param name="hex">string hex color with alpha, red, green, blue</param>
        /// <returns>Color</returns>
        public static Color ColorFromInt(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            int c = int.Parse(hex, NumberStyles.AllowHexSpecifier);
            return Color.FromArgb((byte)((c >> 0x18) & 0xff),
                (byte)((c >> 0x10) & 0xff),
                (byte)((c >> 8) & 0xff),
                (byte)(c & 0xff));
        }

        /// <summary>
        /// Map resolution formula from Bing Maps site:
        ///     http://msdn.microsoft.com/en-us/library/aa940990.aspx
        /// </summary>
        /// <param name="latitude">decimal latitude</param>
        /// <param name="zoomlevel">double zoom level</param>
        /// <returns></returns>
        public static double GetMapResolution(double latitude, double zoomlevel)
        {
            return 156543.04 * Math.Cos(latitude * 0.0174532925) / Math.Pow(2.0, zoomlevel);
        }

        static double EarthRadius = 6378137.0;
        static double ToRadians = Math.PI / 180;
        static double ToDegrees = 180 / Math.PI;
        static double minLat = -85.0;
        static double maxLat = 85.0;
        static double minLon = -180.0;
        static double maxLon = 180.0;

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }


        /// <summary>
        ///  Spherical Mercator Projection conversion  
        ///  using EPSG:3457 parameters 
        ///  matches Bing Maps coordinate system
        /// </summary>
        /// <param name="lon">double longitude</param>
        /// <param name="lat">double latitude</param>
        /// <returns>Point in Mercator X,Y</returns>
        public static Point Mercator(double lon, double lat)
        {
            /* spherical mercator 
             * epsg:900913 R= 6378137 
             * x = longitude
             * y= R*ln(tan(pi/4 +latitude/2)
             */
            lat = Clip(lat, minLat, maxLat);
            lon = Clip(lon, minLon, maxLon);
            double x = EarthRadius * ToRadians * lon;
            double y = EarthRadius * Math.Log(Math.Tan(ToRadians * (45 + lat / 2.0)));

            return new Point(x, y);
        }

        /// <summary>
        /// Inverse Spherical Mercator Projection conversion  
        ///   using EPSG:3457 parameters 
        ///   matches Bing Maps coordinate system
        /// </summary>
        /// <param name="x">double Mercator X</param>
        /// <param name="y">double Mercator Y</param>
        /// <returns>Point longitude,latitude</returns>
        /// <remarks>
        /// Note order of (longitude,latitude) it is not the same as Location order (lat,lon)
        /// </remarks>
        public static Point InverseMercator(double x, double y)
        {
            /* spherical mercator for Google, VE, Yahoo etc
             * epsg:900913 R= 6378137 
             */
            double lon = x / EarthRadius * ToDegrees;
            double lat = Math.Atan(Math.Sinh(y / EarthRadius)) * ToDegrees;
            return new Point(lon, lat);
        }

    }
}
