using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;

using HydroNumerics.Geometry;
using HydroNumerics.Geometry.UTMConversion;

namespace HydroNumerics.Geometry.Net
{
  public class KMSData
  {
    IAsyncResult result;
    HttpWebRequest request;

    public void BeginGetHeight(XYPoint point, int UTMZone)
    {
      string url = String.Format("http://kmswww3.kms.dk/FindMinHoejde/Default.aspx?display=show&csIn=utm{2}_euref89&csOut=utm32_euref89&x={0}&y={1}&c=dk", point.X, point.Y, UTMZone);

      request = (HttpWebRequest)WebRequest.Create(url);
      result= (IAsyncResult) request.BeginGetResponse(new AsyncCallback(RespCallback),null);
    }

    private void RespCallback(IAsyncResult asynchronousResult)
    {
      HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

      string resultString;

      using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
      {
        resultString = streamReader1.ReadToEnd();
      }

      int start = resultString.LastIndexOf("</strong>") + 9;
      int end = resultString.LastIndexOf("m</span>");

      string parsestring = resultString.Substring(start, end - start);
    }



    public static bool TryGetHeight(double latitude, double longitude, out double? Height)
    {
      var lat = new GPS.Coordinate((decimal)latitude, GPS.CoordinateType.Latitude);
      var longi = new GPS.Coordinate((decimal)longitude, GPS.CoordinateType.Longitude);
      var utm = new GPS.UTM(lat, longi);
      return TryGetHeight(new XYPoint(utm.Easting, utm.Northing), utm.Zone, out Height);

    }

    /// <summary>
    /// Returns the height above mean sea level from "den digitale højdemodel" with two decimals. 
    /// Coordinates must be UTM
    /// A request takes almost 1 second
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static bool TryGetHeight(IXYPoint point, int UTMZone, out double? Height)
    {
      Height = null;
      string url = String.Format("http://kmswwwudv1.kms.dk/FindMinHoejde/Default.aspx?display=show&csIn=utm{2}_euref89&csOut=utm32_euref89&x={0}&y={1}&c=dk", point.X, point.Y, UTMZone);

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      string resultString;

      using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
      {
        resultString = streamReader1.ReadToEnd();
      }

      int start = resultString.LastIndexOf("</strong>") + 9;
      int end = resultString.LastIndexOf("m</span>");

      if(start!=-1 & end!= -1)
      {
        string parsestring = resultString.Substring(start, end - start);
        Height =double.Parse(parsestring, new CultureInfo("da-DK", false));
        return true;
      }
      return false;
    }
  }
}
