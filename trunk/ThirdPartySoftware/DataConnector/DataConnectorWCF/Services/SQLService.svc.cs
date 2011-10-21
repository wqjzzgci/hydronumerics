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
using System.IO;

namespace DataConnectorWCF
{

    public class SQLService : ISQLService
    {

        /* setup log4net logger */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /* default global parameters */
        private string geomField = "the_geo"; // name of Geography DataType column in tables
        private string srid = "4326"; // name of SRID Constraint, EPSG:4326, of tables

        /// <summary>
        /// Experimental SQL service
        /// </summary>
        /// <param name="Parameters">SQLParameter Parameters</param>
        /// <returns>Stream</returns>
        public Stream GetSQLBBox(SQLParameter Parameters)
        {
            Encoding encoding = new UTF8Encoding();
            Stream WKTStream = new MemoryStream();
            if (Parameters.columnnames == null || Parameters.columnnames.Length == 0) Parameters.columnnames = "*";

            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            SqlDataReader rdr = null;
            try
            {
                StringBuilder query = new StringBuilder("SELECT " + Parameters.columnnames + "," + geomField + ".Reduce(" + Parameters.reduce + ").STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.table + "] WITH(INDEX(the_geo_sidx))");
                if (Parameters.points != null || Parameters.filter != null) query.Append(" WHERE ");
                query.Append(geomField + ".STIntersects(geography::STGeomFromText('POLYGON((" + Parameters.points + "))', " + srid + "))=1");

                if (Parameters.filter != null && Parameters.filter.Length>0)
                {
                    if (Parameters.points != null) query.Append(" AND ");
                    query.Append(Parameters.filter);
                }
                log.Info(query);

                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                conn.Open();
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Shape shp = new Shape();
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
                    WKTStream.Write(encoding.GetBytes(shp.WKT), 0, shp.WKT.Length);
                    WKTStream.Flush();
                }
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
            return WKTStream;
        }

    }
}
