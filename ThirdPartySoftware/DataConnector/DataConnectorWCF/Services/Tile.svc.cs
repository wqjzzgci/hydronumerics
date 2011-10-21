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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.SqlServer.Types;

namespace DataConnectorWCF
{

    public class Tile : ITile
    {

        /* setup log4net logger */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /* global parameters */
        private string geomField = "the_geo"; // default name of Geography DataType column in tables
        private string srid = "4326"; // default name of SRID Constraint, EPSG:4326, of tables
        private int totalPoints = 0;
        private bool thematic = false;
        private int lvl;
        private int tileX;
        private int tileY;
        private int nwX;
        private int nwY;
        private double nwLon;
        private double nwLat;
        private double seLon;
        private double seLat;
        private int pixelX;
        private int pixelY;
        private Dictionary<string, LoadStyle> LayerStyle = new Dictionary<string, LoadStyle>();

        //8 color values in thematic range
        private string[] colorRange = new string[] { "#FF400000", "#FF804040", "#FFC04000", "#FFC08080", "#FFFF0000", "#FFFF8040", "#FFFFC080", "#FFFFFF80" };

        /// <summary>
        /// Get png tile 
        /// </summary>
        /// <param name="table">layer table to use</param>
        /// <param name="quadkey">quadkey for current tile</param>
        /// <param name="thematicstr">string true or false - thematic styling</param>
        /// <returns>png stream</returns>
        /// <remarks>
        ///   webHttp uses RESTful urel format
        ///   thematic map selection does not use caching
        /// </remarks>
        public Stream GetTile(string table, string quadkey, string thematicstr)
        {
            DateTime queryStart = DateTime.Now;
            DateTime queryStop;

            //LoadStyle - stroke, fill, opacity, pointRadius, valueCol, maxValue, skew factor
            LayerStyle.Add("countries", new LoadStyle("#FFFF0000", "#FF00FF00", 0.25, 1, "AREA", 6043.6, 128.0));
            LayerStyle.Add("statesprovinces", new LoadStyle("#FFFF0000", "#FF0000FF", 0.25, 1, "AREA", 321.6, 128.0));
            LayerStyle.Add("uscounties", new LoadStyle("#FFFF0000", "#FF00FFFF", 0.25, 2, "AREA", 74.22, 512.0));
            LayerStyle.Add("faults", new LoadStyle("#FFFF0000", "#FF000000", 1.0, 2, "ACODE", 6, 8.0));
            LayerStyle.Add("earthquakes", new LoadStyle("#FFFF0000", "#FFFFFF00", 1.0, 12, "OTHER_MAG1", 9.24, 10.0));

            thematic = thematicstr.ToLower().Equals("true");
            Encoding encoding = new UTF8Encoding();
            Bitmap tileBitmap = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(tileBitmap);
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            SqlDataReader rdr = null;
            try
            {
                conn.Open();
                MemoryStream imageAsMemoryStream = new MemoryStream();
                if (!thematic && isCached(table, quadkey, conn))
                {
                    // get cached tile from table blob
                    imageAsMemoryStream = GetCachedTile(table, quadkey, conn);
                }
                else
                {
                    //build tile since it is not in tile table or this is thematic map
                    StringBuilder query = new StringBuilder("SELECT ID," + LayerStyle[table].valueCol + "," + geomField + ".Reduce(@reduce) as " + geomField + " FROM [dbo].[" + table + "] WITH(INDEX(the_geo_sidx)) WHERE ");
                    query.Append(geomField + ".STIntersects(geography::STGeomFromText('POLYGON(('+@nwLon+' '+@nwLat+', '+@nwLon+' '+@seLat+', '+@seLon+' '+@seLat+', '+@seLon+' '+@nwLat+', '+@nwLon+' '+@nwLat+'))', @srid))=1");

                    lvl = quadkey.Length;
                    double reduce = int.Parse(ConfigurationSettings.AppSettings["reduceMax"]) - (lvl * 2000);
                    if (reduce < 0) reduce = 0;

                    QuadKeyToTileXY(quadkey, out tileX, out tileY, out lvl);
                    TileXYToPixelXY(tileX, tileY, out nwX, out nwY);

                    PixelXYToLatLong(nwX, nwY, lvl, out nwLat, out nwLon);
                    PixelXYToLatLong(nwX + 256, nwY + 256, lvl, out seLat, out seLon);
                    if (nwLon == -180.0) nwLon = -179.9;
                    queryStart = DateTime.Now;
                    SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                    cmd.Parameters.Add(new SqlParameter("reduce", reduce));
                    cmd.Parameters.Add(new SqlParameter("srid", srid));
                    cmd.Parameters.Add(new SqlParameter("nwLon", nwLon.ToString()));
                    cmd.Parameters.Add(new SqlParameter("nwLat", nwLat.ToString()));
                    cmd.Parameters.Add(new SqlParameter("seLon", seLon.ToString()));
                    cmd.Parameters.Add(new SqlParameter("seLat", seLat.ToString()));
                    log.Info(query.ToString());
                    log.Info(quadkey + " reduce=" + reduce + " srid=" + srid + " nwLon=" + nwLon + " nwLat=" + nwLat + " seLon=" + seLon + " seLat=" + seLat);

                    rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        double value = 1.0;
                        Color color = Color.White;
                        if (thematic) // thematic color by colValue range
                        {
                            try
                            {
                                value = double.Parse(rdr[LayerStyle[table].valueCol].ToString()) / LayerStyle[table].maxValue;
                            }
                            catch (Exception e)
                            {
                                value = 1.0;
                            }
                            for (int i = 1; i < colorRange.Length; i++)
                            {
                                if (value < (i / (LayerStyle[table].skewFactor)))
                                {
                                    color = ColorFromInt(colorRange[i - 1]);
                                    break;
                                }
                                else color = ColorFromInt(colorRange[colorRange.Length - 1]);
                            }
                        }

                        SqlGeography geo = (SqlGeography)rdr[geomField];
                        log.Debug(geo.STGeometryType().ToString().ToUpper());
                        switch (geo.STGeometryType().ToString().ToUpper())
                        {
                            case "POINT":
                                {
                                    RenderPoint(geo, table, g, color);
                                    break;
                                }
                            case "LINESTRING":
                                {
                                    RenderLinestring(geo, table, g, (int)rdr["ID"], color);
                                    break;
                                }
                            case "POLYGON":
                                {
                                    RenderPolygon(geo, table, g, (int)rdr["ID"], color);
                                    break;
                                }
                            case "MULTILINESTRING":
                                {
                                    RenderMultiLinestring(geo, table, g, (int)rdr["ID"], color);
                                    break;
                                }
                            case "MULTIPOLYGON":
                                {
                                    RenderMultiPolygon(geo, table, g, (int)rdr["ID"], color);
                                    break;
                                }
                            case "GEOMETRYCOLLECTION":
                                {
                                    RenderGeometryCollection(geo, table, g, (int)rdr["ID"], color);
                                    break;
                                }
                        }

                    }
                    queryStop = DateTime.Now;
                    log.Debug(String.Format("Query Time: {0,0:0}ms", (queryStop - queryStart).TotalMilliseconds));

                    tileBitmap.Save(imageAsMemoryStream, ImageFormat.Png);
                    if (rdr != null) rdr.Close();

                    if (!thematic && !isCached(table, quadkey, conn))
                    {
                        // cache tile to table blob
                        int iresult = SetCachedTile(table, quadkey, imageAsMemoryStream, conn);
                    }
                }

                imageAsMemoryStream.Position = 0;
                tileBitmap.Dispose();
                return imageAsMemoryStream;
            }
            catch (Exception e)
            {
                log.Error(e.Message);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (conn != null) conn.Close();
            }
            Assembly thisExe = Assembly.GetExecutingAssembly();
            return thisExe.GetManifestResourceStream("DataConnectorWCF.empty.png");
        }

        /// <summary>
        /// isCached
        ///     determines if a cached tile exists in the tileTable
        /// </summary>
        /// <param name="table">string layer table</param>
        /// <param name="quadkey">string current quadkey</param>
        /// <param name="conn"> SQL Server Connection</param>
        /// <returns>bool true if tile exists in table</returns>
        private bool isCached(string table, string quadkey, SqlConnection conn)
        {
            bool cached = false;
            SqlCommand cmd = new SqlCommand("Select count(*) FROM tile" + table + " WHERE quadkey=@quadkey", conn);
            cmd.Parameters.Add(new SqlParameter("quadkey", quadkey));

            int cnt = (int)cmd.ExecuteScalar();
            if (cnt > 0) cached = true;
            return cached;
        }

        /// <summary>
        /// GetCachedTile
        ///     returns a tile from caching tileTable
        /// </summary>
        /// <param name="table">string layer table</param>
        /// <param name="quadkey">string current quadkey</param>
        /// <param name="conn"> SQL Server Connection</param>
        /// <returns>png stream</returns>
        private MemoryStream GetCachedTile(string table, string quadkey, SqlConnection conn)
        {
            SqlCommand cmdSelect = new SqlCommand("SELECT tile FROM tile" + table + " WHERE quadkey=@quadkey", conn);
            cmdSelect.Parameters.Add(new SqlParameter("quadkey", quadkey));

            byte[] image = (byte[])cmdSelect.ExecuteScalar();
            MemoryStream imageStream = new MemoryStream();
            imageStream.Write(image, 0, image.Length);
            return imageStream;
        }

        /// <summary>
        /// SetCachedTile
        ///     Adds tile png to caching tileTable
        /// </summary>
        /// <param name="table">string layer table</param>
        /// <param name="quadkey">string current quadkey</param>
        /// <param name="imageStream">MemorySTream of png tile</param>
        /// <param name="conn"> SQL Server Connection</param>
        /// <returns>int ExecuteNonQuery() return</returns>
        private int SetCachedTile(string table, string quadkey, MemoryStream imageStream, SqlConnection conn)
        {

            imageStream.Position = 0;
            byte[] imageData = new byte[imageStream.Length];
            imageStream.Read(imageData, 0, (int)imageStream.Length);

            SqlCommand cmd = new SqlCommand("INSERT INTO tile" + table + "(quadkey,tile) values(@quadkey,@tile)", conn);
            cmd.Parameters.Add(new SqlParameter("quadkey", quadkey));
            cmd.Parameters.Add("@tile", SqlDbType.Image);
            cmd.Parameters["@tile"].Value = imageData;
            return cmd.ExecuteNonQuery();
        }


        #region render geography
        /// <summary>
        /// RenderPoint
        ///     Render a point to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="valueFill">Color the fill color style</param>
        private void RenderPoint(SqlGeography geo, string table, Graphics g, Color valueFill)
        {
            totalPoints++;
            double lat = (double)geo.Lat;
            double lon = (double)geo.Long;
            LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
            Point cp = new Point(pixelX - nwX, pixelY - nwY);
            if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
            SolidBrush myBrush = new SolidBrush(valueFill);
            int r = LayerStyle[table].pointRadius;
            g.FillEllipse(myBrush, new Rectangle(cp.X - r / 2, cp.Y - r / 2, r, r));
        }

        /// <summary>
        /// RenderLinestring
        ///     Render a linestring to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="valueFill">Color the stroke color style</param>
        private void RenderLinestring(SqlGeography geo, string table, Graphics g, int id, Color valueFill)
        {
            if (geo.STNumPoints() > 1)
            {
                totalPoints += (int)geo.STNumPoints();
                Point[] ptArray = new Point[(int)geo.STNumPoints()];
                double lon1 = 0.0;
                for (int j = 1; j <= geo.STNumPoints(); j++)
                {
                    double lat = (double)geo.STPointN(j).Lat;
                    double lon = (double)geo.STPointN(j).Long;
                    if (j > 1)
                    {
                        lon = HemisphereCorrection(lon, lon1, id);
                    }
                    LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
                    ptArray[j - 1] = new Point(pixelX - nwX, pixelY - nwY);
                    lon1 = lon;
                }
                if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
                GraphicsPath linePath = new GraphicsPath();
                linePath.AddLines(ptArray);
                Pen myPen = new Pen(valueFill);
                myPen.Width = 2;
                g.DrawPath(myPen, linePath);
            }

        }

        /// <summary>
        /// RenderPolygon
        ///     Render a polygon to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="valueFill">Color the fill color style</param>
        private void RenderPolygon(SqlGeography geo, string table, Graphics g, int id, Color valueFill)
        {

            if (geo.NumRings() > 0)
            {
                totalPoints += (int)geo.STNumPoints();
                for (int j = 1; j <= geo.NumRings(); j++)
                {
                    if (geo.RingN(j).STNumPoints() > 1)
                    {
                        Point[] ptArray = new Point[(int)geo.RingN(j).STNumPoints()];
                        double lon1 = 0.0;
                        for (int k = 1; k <= geo.RingN(j).STNumPoints(); k++)
                        {
                            double lat = (double)geo.RingN(j).STPointN(k).Lat;
                            double lon = (double)geo.RingN(j).STPointN(k).Long;
                            if (k > 1)
                            {
                                lon = HemisphereCorrection(lon, lon1, id);
                            }
                            LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
                            ptArray[k - 1] = new Point(pixelX - nwX, pixelY - nwY);
                            lon1 = lon;
                        }
                        if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
                        GraphicsPath polygonRegion = new GraphicsPath();
                        polygonRegion.AddPolygon(ptArray);
                        Region region = new Region(polygonRegion);

                        SolidBrush myBrush = new SolidBrush(valueFill);
                        g.FillRegion(myBrush, region);
                        Pen myPen = new Pen(ColorFromInt(LayerStyle[table].stroke));
                        myPen.Width = 1;
                        g.DrawPolygon(myPen, ptArray);
                    }
                }

            }

        }

        /// <summary>
        /// RenderMultiLinestring
        ///     Render a MultiLinestring to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="id">int record id</param>
        /// <param name="valueFill">Color the stroke color style</param>
        private void RenderMultiLinestring(SqlGeography geo, string table, Graphics g, int id, Color valueFill)
        {

            if (geo.STNumGeometries() > 0)
            {
                totalPoints += (int)geo.STNumPoints();

                for (int j = 1; j <= geo.STNumGeometries(); j++)
                {
                    if (geo.STGeometryN(j).NumRings() > 0)
                    {
                        for (int k = 1; k <= geo.STGeometryN(j).NumRings(); k++)
                        {
                            if (geo.STGeometryN(j).RingN(k).STNumPoints() > 1)
                            {
                                Point[] ptArray = new Point[(int)geo.STNumPoints()];
                                double lon1 = 0.0;
                                for (int m = 1; m <= geo.STGeometryN(j).RingN(k).STNumPoints(); m++)
                                {
                                    double lat = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Lat;
                                    double lon = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Long;
                                    if (m > 1)
                                    {
                                        lon = HemisphereCorrection(lon, lon1, id);
                                    }
                                    LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
                                    ptArray[m - 1] = new Point(pixelX - nwX, pixelY - nwY);
                                    lon1 = lon;
                                }
                                if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
                                GraphicsPath linePath = new GraphicsPath();
                                linePath.AddLines(ptArray);
                                Pen myPen = new Pen(valueFill);
                                myPen.Width = 2;
                                g.DrawPath(myPen, linePath);
                            }
                        }
                    }
                }

            }

        }

        /// <summary>
        /// RenderMultiPolygon
        ///     Render a Multipolygon to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="id">int record id</param>
        /// <param name="valueFill">Color the fill color style</param>
        private void RenderMultiPolygon(SqlGeography geo, string table, Graphics g, int id, Color valueFill)
        {

            if (geo.STNumGeometries() > 0)
            {
                totalPoints += (int)geo.STNumPoints();

                for (int j = 1; j <= geo.STNumGeometries(); j++)
                {
                    if (geo.STGeometryN(j).NumRings() > 0)
                    {
                        for (int k = 1; k <= geo.STGeometryN(j).NumRings(); k++)
                        {
                            if (geo.STGeometryN(j).RingN(k).STNumPoints() > 1)
                            {
                                Point[] ptArray = new Point[(int)geo.STGeometryN(j).RingN(k).STNumPoints()];
                                double lon1 = 0.0;
                                int count = (int)geo.STGeometryN(j).RingN(k).STNumPoints();
                                for (int m = 1; m <= geo.STGeometryN(j).RingN(k).STNumPoints(); m++)
                                {
                                    double lat = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Lat;
                                    double lon = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Long;
                                    if (m > 1)
                                    {
                                        lon = HemisphereCorrection(lon, lon1, id);
                                    }
                                    LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
                                    ptArray[m - 1] = new Point(pixelX - nwX, pixelY - nwY);
                                    lon1 = lon;
                                }
                                if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
                                GraphicsPath polygonRegion = new GraphicsPath();
                                polygonRegion.AddPolygon(ptArray);
                                Region region = new Region(polygonRegion);

                                g.FillRegion(new SolidBrush(valueFill), region);
                                Pen myPen = new Pen(ColorFromInt(LayerStyle[table].stroke));
                                myPen.Width = 1;
                                g.DrawPolygon(myPen, ptArray);
                            }
                        }
                    }
                }
            }

        }


        /// <summary>
        /// RenderGeometryCollection
        ///     Render a GeometryCollection to Graphics
        /// </summary>
        /// <param name="geo">SqlGeography geo</param>
        /// <param name="table">string layer table</param>
        /// <param name="g">Graphics used to draw to</param>
        /// <param name="id">int record id</param>
        /// <param name="valueFill">Color the fill color style</param>
        private void RenderGeometryCollection(SqlGeography geo, string table, Graphics g, int id, Color valueFill)
        {
            int numGeom = (int)geo.STNumGeometries();
            if (geo.STNumGeometries() > 0)
            {
                for (int j = 1; j <= geo.STNumGeometries(); j++)
                {
                    if (geo.STGeometryN(j).NumRings() > 0)
                    {
                        for (int k = 1; k <= geo.STGeometryN(j).NumRings(); k++)
                        {
                            if (geo.STGeometryN(j).RingN(k).STNumPoints() > 1)
                            {
                                double lon1 = 0.0;
                                Point[] ptArray = new Point[(int)geo.STGeometryN(j).RingN(k).STNumPoints()];
                                for (int m = 1; m <= geo.STGeometryN(j).RingN(k).STNumPoints(); m++)
                                {
                                    double lat = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Lat;
                                    double lon = (double)geo.STGeometryN(j).RingN(k).STPointN(m).Long;

                                    if (m > 1)
                                    {
                                        lon = HemisphereCorrection(lon, lon1, id);
                                    }

                                    LatLongToPixelXY(lat, lon, lvl, out pixelX, out pixelY);
                                    ptArray[m - 1] = new Point(pixelX - nwX, pixelY - nwY);
                                    lon1 = lon;
                                }
                                if (valueFill.Equals(Color.White)) valueFill = ColorFromInt(LayerStyle[table].fill);
                                GraphicsPath extRingRegion = new GraphicsPath();
                                extRingRegion.AddPolygon(ptArray);
                                Region region = new Region(extRingRegion);
                                g.FillRegion(new SolidBrush(valueFill), region);
                                Pen myPen = new Pen(ColorFromInt(LayerStyle[table].stroke));
                                myPen.Width = 1;
                                g.DrawPolygon(myPen, ptArray);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// HemisphereCorrection
        ///     attempts to correct polygons crossing International Dataline
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lon1"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private double HemisphereCorrection(double lon, double lon1, int id)
        {
            //truncate polygon to nearest hemisphere boundary
            if ((lon < 0.0 && lon1 > 0.0) || (lon > 0.0 && lon1 < 0.0))
            { // crosses hemisphere - use shorter of distances to opposite hemisphere boundaries
                log.Debug("Crosses Hemisphere: " + lon + " " + lon1 + " id=" + id);
                double d1 = Math.Abs(lon1 - 180);
                double d2 = Math.Abs(lon1 - 0.0);
                if (lon1 > 0)
                {
                    if (d1 < d2) lon = 180.0;
                    else lon = 0.0;
                }
                else
                {
                    if (d1 < d2) lon = -180.0;
                    else lon = 0.0;
                }
            }
            return lon;
        }


        #endregion

        #region Helper Functions



        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        /// <remarks>
        ///     Most helper functions are from MSDN site:
        ///     http://msdn.microsoft.com/en-us/library/bb259689.aspx
        ///</remarks>
        private static double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }



        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public static uint MapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }



        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified
        /// latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The ground resolution, in meters per pixel.</returns>
        public static double GroundResolution(double latitude, int levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * EarthRadius / MapSize(levelOfDetail);
        }



        /// <summary>
        /// Determines the map scale at a specified latitude, level of detail,
        /// and screen resolution.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// map scale.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="screenDpi">Resolution of the screen, in dots per inch.</param>
        /// <returns>The map scale, expressed as the denominator N of the ratio 1 : N.</returns>
        public static double MapScale(double latitude, int levelOfDetail, int screenDpi)
        {
            return GroundResolution(latitude, levelOfDetail) * screenDpi / 0.0254;
        }



        /// <summary>
        /// Converts a point from latitude/longitude WGS-84 coordinates (in degrees)
        /// into pixel XY coordinates at a specified level of detail.
        /// </summary>
        /// <param name="latitude">Latitude of the point, in degrees.</param>
        /// <param name="longitude">Longitude of the point, in degrees.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="pixelX">Output parameter receiving the X coordinate in pixels.</param>
        /// <param name="pixelY">Output parameter receiving the Y coordinate in pixels.</param>
        public static void LatLongToPixelXY(double latitude, double longitude, int levelOfDetail, out int pixelX, out int pixelY)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            longitude = Clip(longitude, MinLongitude, MaxLongitude);

            double x = (longitude + 180) / 360;
            double sinLatitude = Math.Sin(latitude * Math.PI / 180);
            double y = 0.5 - Math.Log((1 + sinLatitude) / (1 - sinLatitude)) / (4 * Math.PI);

            uint mapSize = MapSize(levelOfDetail);
            pixelX = (int)Clip(x * mapSize + 0.5, 0, mapSize - 1);
            pixelY = (int)Clip(y * mapSize + 0.5, 0, mapSize - 1);
        }



        /// <summary>
        /// Converts a pixel from pixel XY coordinates at a specified level of detail
        /// into latitude/longitude WGS-84 coordinates (in degrees).
        /// </summary>
        /// <param name="pixelX">X coordinate of the point, in pixels.</param>
        /// <param name="pixelY">Y coordinates of the point, in pixels.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <param name="latitude">Output parameter receiving the latitude in degrees.</param>
        /// <param name="longitude">Output parameter receiving the longitude in degrees.</param>
        public static void PixelXYToLatLong(int pixelX, int pixelY, int levelOfDetail, out double latitude, out double longitude)
        {
            double mapSize = MapSize(levelOfDetail);
            double x = (Clip(pixelX, 0, mapSize - 1) / mapSize) - 0.5;
            double y = 0.5 - (Clip(pixelY, 0, mapSize - 1) / mapSize);

            latitude = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * Math.PI)) / Math.PI;
            longitude = 360 * x;
        }

        private const double EarthRadius = 6378137;
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;

        /// <summary>
        /// Converts pixel XY coordinates into tile XY coordinates of the tile containing
        /// the specified pixel.
        /// </summary>
        /// <param name="pixelX">Pixel X coordinate.</param>
        /// <param name="pixelY">Pixel Y coordinate.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        public static void PixelXYToTileXY(int pixelX, int pixelY, out int tileX, out int tileY)
        {
            tileX = pixelX / 256;
            tileY = pixelY / 256;
        }



        /// <summary>
        /// Converts tile XY coordinates into pixel XY coordinates of the upper-left pixel
        /// of the specified tile.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="pixelX">Output parameter receiving the pixel X coordinate.</param>
        /// <param name="pixelY">Output parameter receiving the pixel Y coordinate.</param>
        public static void TileXYToPixelXY(int tileX, int tileY, out int pixelX, out int pixelY)
        {
            pixelX = tileX * 256;
            pixelY = tileY * 256;
        }



        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        public static string TileXYToQuadKey(int tileX, int tileY, int levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }


        /// <summary>
        /// Converts a QuadKey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        public static void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            tileX = tileY = 0;
            levelOfDetail = quadKey.Length;
            for (int i = levelOfDetail; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                switch (quadKey[levelOfDetail - i])
                {
                    case '0':
                        break;

                    case '1':
                        tileX |= mask;
                        break;

                    case '2':
                        tileY |= mask;
                        break;

                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;

                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
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


        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        public static Color RandomColor()
        {
            Random randomSeed = new Random();
            return Color.FromArgb(
                randomSeed.Next(256),
                randomSeed.Next(256),
                randomSeed.Next(256)
            );
        }


        #endregion


    }
}
