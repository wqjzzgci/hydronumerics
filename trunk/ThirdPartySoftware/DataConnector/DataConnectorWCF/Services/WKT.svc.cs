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
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Types;
using log4net;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.ServiceModel.Web;

namespace DataConnectorWCF
{
    public class WKT : IWKT
    {

        /* setup log4net logger */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /* default global parameters */
        private string geomField = "the_geo"; // name of Geography DataType column in tables
        private string srid = "4326"; // name of SRID Constraint, EPSG:4326, of tables
        private int recordLimit = 1500;

        /// <summary>
        /// GetSQLDataWKT
        ///     returns WKT with results of SQL Server query
        /// </summary>
        /// <param name="Parameters">WKTParameters required for query</param>
        public WKTResponse GetSQLDataWKT(WKTParameters Parameters)
        {

            

            DateTime queryStart = DateTime.Now;
            DateTime queryStop;
            int recordCnt = 0;

            WKTResponse _WKTResponse = new WKTResponse();
            _WKTResponse.ErrorCode = 0;
            _WKTResponse.OutputMessage = "Success";
            _WKTResponse.OutputShapes = new List<WKTShape>();
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            SqlDataReader rdr = null;
            try
            {
                if (Parameters.querytype == null || Parameters.querytype.Length == 0) throw new ArgumentException("Invalid Parameters: querytype=\""+Parameters.querytype+"\"");
                if (Parameters.querytype.ToLower().Equals("buffer") && Parameters.radius <= 0) throw new ArgumentException("Invalid Parameters: querytype requires radius >0 - \"" + Parameters.radius + "\"");
                if (Parameters.points.Length == 0 && Parameters.querytype.ToLower().Equals("buffer")) throw new ArgumentException("Invalid Parameters: points must contain at least one point for querytype buffer");
                if (Parameters.points.Split(',').Length != 5 && Parameters.querytype.ToLower().Equals("bbox")) throw new ArgumentException("Invalid Parameters: points must contain 5 points for a closed querytype bbox");

                StringBuilder query = new StringBuilder();
                
                if (Parameters.querytype.ToLower().Equals("bbox"))
                {   //BBox
                    query.Append("SELECT *," + geomField + ".Reduce(@reduce).STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.table + "] WITH(INDEX(the_geo_sidx)) WHERE ");
                    query.Append(geomField + ".STIntersects(geography::STGeomFromText('POLYGON(('+@points+'))', @srid))=1");
                }
                else if (Parameters.querytype.ToLower().Equals("buffer"))
                {
                    query.Append("SELECT *," + geomField + ".Reduce(@reduce).STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.table + "] WITH(INDEX(the_geo_sidx)) WHERE ");
                    if (Parameters.points.Split(',').Length > 1)
                    {   //Polyline Buffer
                        query.Append(geomField + ".STIntersects(geography::STGeomFromText('LINESTRING('+@points+')', @srid).STBuffer(@radius))=1");
                    }
                    else
                    {   //Point Buffer
                        query.Append(geomField + ".STIntersects(geography::STGeomFromText('POINT('+@points+')', @srid).STBuffer(@radius))=1");
                    }
                }
                log.Info(query);

                queryStart = DateTime.Now;
                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                cmd.Parameters.Add(new SqlParameter("reduce", Parameters.reduce));
                cmd.Parameters.Add(new SqlParameter("srid", srid));
                cmd.Parameters.Add(new SqlParameter("points", Parameters.points));
                cmd.Parameters.Add(new SqlParameter("radius", Parameters.radius));
                conn.Open();
                
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    WKTShape shp = new WKTShape();
                    shp.Fields = new Dictionary<string, string>();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i));
                        if (rdr[i].GetType().Equals(typeof(SqlGeography)) ||
                            rdr[i].GetType().Equals(typeof(SqlGeometry))
                            )
                        { //skip
                            log.Debug("skip " + rdr[i].GetType());
                        }
                        else
                        {
                            if (rdr.GetName(i).Equals("ID"))
                            {
                                shp.ID = rdr[i].ToString();
                                log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i) + "  " + rdr[i].ToString());
                            }
                            else if (rdr.GetName(i).Equals(geomField + "wkt"))
                            {
                                log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i) + "  " + rdr[i].ToString());
                                shp.WKT = rdr[i].ToString();
                            }
                            else
                            {
                                shp.Fields.Add(rdr.GetName(i), rdr[i].ToString());
                                log.Debug(rdr.GetName(i) + " : " + rdr[i].ToString());
                            }
                        }
                    }
                    log.Debug(shp.ID + "  " + shp.WKT);
                    if (recordCnt++ > recordLimit) throw new Exception("Query result exceeds limit "+recordLimit);
                    _WKTResponse.OutputShapes.Add(shp);
                }
            }
            catch (ArithmeticException e)
            {
                ServiceException(_WKTResponse,"ArithmeticException "+ e.Message, 3);
            }
            catch (ArgumentException e)
            {
                ServiceException(_WKTResponse, "ArgumentException "+e.Message,1);
            }
            catch (Exception e)
            {
                ServiceException(_WKTResponse, e.Message,2);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (conn != null) conn.Close();
            }
            queryStop = DateTime.Now;
            log.Debug(String.Format("Query Time: {0,0:0}ms", (queryStop - queryStart).TotalMilliseconds));
            _WKTResponse.QueryTime = (queryStop - queryStart).TotalMilliseconds;

            return _WKTResponse;
        }

  


        /// <summary>
        /// ServiceException
        ///     sets response for exception
        /// </summary>
        /// <param name="response">WKTResponse</param>
        /// <param name="message">string exception messsage</param>
        /// <param name="code">int error code</param>
        public void ServiceException(WKTResponse response, string message, int code)
        {
            response.ErrorCode = code;
            response.OutputMessage = message;
            response.OutputShapes.Clear();
            log.Error(message);
        }

    }
}
