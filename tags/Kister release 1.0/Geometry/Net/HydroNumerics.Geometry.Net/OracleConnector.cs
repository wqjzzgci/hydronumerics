using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.OracleClient;
using HydroNumerics.Geometry;

namespace HydroNumerics.Geometry.Net
{
  public class OracleConnector
  {

    public string ServerName { get; set; }
    public int PortNumber { get; set; }
    public string DatabaseName { get; set; }

    public string TableName { get; set; }
    public string UserName {get;set;}
    public string Password { get; set; }

    public string ConnectionString { get; set; }

    public bool Connected { get; set; }
    private OracleConnection oOracleConn;
    
    public OracleConnector(string ServerName, int PortNumber, string TableName, string UserName, string Password, string DatabaseName)
    {
      this.ServerName = ServerName;
      this.PortNumber = PortNumber;
      this.TableName = TableName;
      this.UserName = UserName;
      this.Password = Password;
      this.DatabaseName = DatabaseName;
      //      Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = MyHost)(PORT = MyPort))(CONNECT_DATA = (SERVICE_NAME = MyOracleSID))); User Id = myUsername; Password = myPassword;
      BuildConnectionString(); 
    }

    private void BuildConnectionString()
    {
//      ConnectionString = string.Format("Provider=msdaora;Data Source={0};User Id={1};Password={2};", ServerName, UserName, Password);


      ConnectionString = string.Format("Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = {0})(PORT = {1}))(CONNECT_DATA = (SERVER = DEDICATED) (SID = {4}))); User Id = {2}; Password = {3};", ServerName, PortNumber, UserName, Password, DatabaseName);
    }


    public object ReadFirst()
    {
      Connect();
      var com = oOracleConn.CreateCommand();

      com.CommandText = "select * from " + TableName;

      return com.ExecuteScalar();

    }

    public void Connect()
    {
      oOracleConn = new OracleConnection();
      oOracleConn.ConnectionString = ConnectionString;
      oOracleConn.Open();
    }

    public bool TryGetHeight(IXYPoint point, out double? Height)
    {

//      if (oOracleConn == null)
        Connect();

      Height = null;
      // select sætning fra Frants.
      //select sdo_geor.getCellValue(rast,0,sdo_geometry(2001,32632,sdo_point_type(719000,6178000,null),null,null),1) val from dhm_test

      string select = string.Format("select sdo_geor.getCellValue(rast,0,sdo_geometry(2001,32632,sdo_point_type({0},{1},null),null,null),1) val from {2}",point.X,point.Y, TableName);

      var com = oOracleConn.CreateCommand();

      com.CommandText=select;

      Height = (double)com.ExecuteScalar();

      return true;
    }

    public void Dispose()
    {
      if (oOracleConn != null)
        oOracleConn.Dispose();
    }

  }
}
