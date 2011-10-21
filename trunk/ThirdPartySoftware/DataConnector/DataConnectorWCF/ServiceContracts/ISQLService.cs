using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace DataConnectorWCF
{
    [ServiceContract]
    public interface ISQLService
    {

        /// <summary>
        /// ISQLService SQLService Interface - experimental
        /// </summary>
        /// <param name="Parameters">SQLParameter Parameters</param>
        /// <returns>Stream</returns>
        /// <remarks>
        ///  Experimental
        ///   input SQLParameter
        ///      string table - DB table to query
        ///      string columnnames - comma delimited list of column names; "" = ///
        ///      string radius - distance in meters from center of buffer
        ///      string points - buffer = radius in meters, Center Point LL || Polyline Points LL
        ///                    - bbox = SE.LL, NE.LL, NW.LL, SW.LL, SE.LL
        ///      string filter - any valid additional Filter in addition to spatial filter
        ///      string reduce - a node generalizing factor for Douglas-Peucker algorithm
        ///                      http://msdn.microsoft.com/en-us/library/cc627410.aspx
        ///                      
        ///   output WKTOutput
        ///      int ErrorCode - success = 0, error = 1
        ///      string OutputMessage - default "Success"    
        ///      List<Shape> OutputShapes
        ///              string Shape.ID - "ID" column from table
        ///              string Shape.WKT = Geography Column as WKT Text
        /// </remarks>
        [OperationContract]
        Stream GetSQLBBox(SQLParameter Parameters);
    }


    [DataContract]
    public struct SQLParameter
    {
        [DataMember]
        public string table;

        [DataMember]
        public string columnnames;

        [DataMember]
        public string querytype;

        [DataMember]
        public double radius;

        [DataMember]
        public string points;

        [DataMember]
        public string filter;

        [DataMember]
        public double reduce;
    }

    [DataContract]
    public struct Shape
    {
        [DataMember]
        public string ID;

        [DataMember]
        public Dictionary<string, string> Fields;

        [DataMember]
        public string WKT;

    }
}
