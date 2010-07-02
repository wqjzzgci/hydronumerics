using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Geometry.Net;


namespace HydroNumerics.Geometry.Net
{
  public class KMSData
  {
    /// <summary>
    /// Returns the height above mean sea level from "den digitale højdemodel" with two decimals. 
    /// A request takes almost 1 second
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static double GetHeight(XYPoint point, int UTMZone)
    {
      string url = String.Format("http://kmswww3.kms.dk/FindMinHoejde/Default.aspx?display=show&csIn=utm{2}_euref89&csOut=utm32_euref89&x={0}&y={1}&c=dk", point.X, point.Y,UTMZone);

      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      HttpWebResponse response = (HttpWebResponse)request.GetResponse();

      string resultString;

      using (StreamReader streamReader1 = new StreamReader(response.GetResponseStream()))
      {
        resultString = streamReader1.ReadToEnd();
      }

      int start = resultString.LastIndexOf("</strong>") + 9;
      int end = resultString.LastIndexOf("m</span>");

      string parsestring = resultString.Substring(start, end - start);

      return double.Parse(parsestring);
    }
  }
}
