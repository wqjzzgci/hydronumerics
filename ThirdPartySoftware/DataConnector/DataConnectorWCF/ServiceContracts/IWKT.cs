using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DataConnectorWCF
{
    [ServiceContract]
    public interface IWKT
    {

        /// <summary>
        ///  IWKT - WKT Interface
        /// </summary>
        /// <param name="Parameters">WKTParameters Parameters</param>
        /// <returns>WKTResponse</returns>
        /// <remarks>
        ///  input WKTParameters
        ///     string table - DB table to query
        ///     string querytype - buffer || bbox
        ///     string radius - distance in meters from center of buffer
        ///     string points - buffer = Point LL || Polyline LL
        ///                   - bbox = SE.LL, NE.LL, NW.LL, SW.LL, SE.LL
        ///     string reduce - a node generalizing factor for Douglas-Peucker algorithm
        ///                     http://msdn.microsoft.com/en-us/library/cc627410.aspx
        ///                     
        ///  output WKTResponse
        ///     int ErrorCode - success = 0, error >= 1
        ///     string OutputMessage - default "Success"*      
        ///     List<WKTShape> OutputShapes
        ///             string Shape.ID - "ID" column from table
        ///             Dictionary<string, string> Fields - Field Name:Field Value
        ///             string Shape.WKT = Geography Column as WKT Text
        ///     double QueryTime in miliseconds
        /// </remarks>
        [OperationContract]
        WKTResponse GetSQLDataWKT(WKTParameters Parameters);

    }

    [DataContract]
    public class WKTResponse
    {

        [DataMember]
        public int ErrorCode;

        [DataMember]
        public string OutputMessage;

        [DataMember]
        public List<WKTShape> OutputShapes;

        [DataMember]
        public double QueryTime;
    }

    [DataContract]
    public struct WKTParameters
    {
        [DataMember]
        public string table;

        [DataMember]
        public string querytype;

        [DataMember]
        public double radius;

        [DataMember]
        public string points;

        [DataMember]
        public double reduce;
    }

    [DataContract]
    public struct WKTShape
    {
        [DataMember]
        public string ID;

        [DataMember]
        public Dictionary<string, string> Fields;

        [DataMember]
        public string WKT;

    }
}
