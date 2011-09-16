using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core.WPF;
using HydroNumerics.MikeSheTools.DFS;
using HydroNumerics.Geometry.Net;
using HydroNumerics.Geometry;


namespace HydroNumerics.MikeSheTools.ViewModel
{

  public enum SourceType
  {
    Oracle,
    KMSWeb,
    DFS2
  }

  public class DEMSourceConfiguration:NotifyPropertyChangedBase
  {
    public OracleConnector Oracle {get;private set;}
    private SourceType _st;
    private string _dfs2File = "";

    private DFS2 DFSdem;


    public DEMSourceConfiguration()
    {
      Oracle = new OracleConnector("geusjup3", 1685, "FPH.DKDHM10", "mike11cs", "mike11cspw", "jupiter");
      _st = SourceType.Oracle;
    }

   


    public string Dfs2File
    {
      get { return _dfs2File; }
      set
      {
        if (value != _dfs2File)
        {
          _dfs2File = value;
          NotifyPropertyChanged("Dfs2File");
        }
      }
    }


    public void LoadDfs2(string FileName)
    {
      _st = SourceType.DFS2;
      Dfs2File  = FileName;
      DFSdem = new DFS2(_dfs2File);

    }

    public bool TryFindDemHeight(double x, double y, out double? height)
    {
      return TryFindDemHeight(new XYPoint(x, y), out height);
    }


    
    /// <summary>
    /// Returns the height at the point using the method selected with the enums
    /// </summary>
    /// <param name="point"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public bool TryFindDemHeight(IXYPoint point, out double? height)
    {
      height = null;

      switch (DEMSource)
      {
        case SourceType.Oracle:
          return Oracle.TryGetHeight(point, out height);
        case SourceType.KMSWeb:
          return KMSData.TryGetHeight(point, 32, out height);
        case SourceType.DFS2:
          int col = DFSdem.GetColumnIndex(point.X);
          int row = DFSdem.GetRowIndex(point.Y);
          if (col >= 0 & row >= 0)
          {
            height = DFSdem.GetData(0, 1)[row, col];
            return true;
          }
          else
            return false;
        default:
          return false;
      }
      return false;
    }



    public string Password
    {
      get { return Oracle.Password; }
      set { 
        Oracle.Password = value;
        Oracle.BuildConnectionString();
        NotifyPropertyChanged("ConnectionString");
      }
    }

    public string UserName
    {
      get { return Oracle.UserName; }
      set { Oracle.UserName = value;
      Oracle.BuildConnectionString();
      NotifyPropertyChanged("ConnectionString");
      }
    }
  

    public string TableName
    {
      get { return Oracle.TableName; }
      set { Oracle.TableName = value; }
    }

    /// <summary>
    /// Get and set the Connectionstring
    /// </summary>
    public string ConnectionString
    {
      get { return Oracle.ConnectionString; }
      set
      {
        if (value != Oracle.ConnectionString)
        {
          Oracle.ConnectionString = value;
          NotifyPropertyChanged("ConnectionString");
        }
      }
    }

    
    public string OracleServerName
    {
      get { return Oracle.ServerName; }
      set
      {
        if (value != Oracle.ServerName)
        {

          Oracle.ServerName = value;
          NotifyPropertyChanged("OracleServerName");
          Oracle.BuildConnectionString();
          NotifyPropertyChanged("ConnectionString");

        }
      }
    }

    public int PortNumber
    {
      get { return Oracle.PortNumber; }
      set { Oracle.PortNumber = value;
      Oracle.BuildConnectionString();
      NotifyPropertyChanged("ConnectionString");
      }
    }

    public string DatabaseName
    {
      get { return Oracle.DatabaseName; }
      set
      {
        if (value != Oracle.DatabaseName)
        {

          Oracle.DatabaseName = value;
          NotifyPropertyChanged("DatabaseName");
          Oracle.BuildConnectionString();
          NotifyPropertyChanged("ConnectionString");
        }
      }
    }

    
    /// <summary>
    /// Gets and sets source type
    /// </summary>
    public SourceType DEMSource
    {
      get
      {
        return _st;
      }
      set
      {
        if (value!=_st)
        {
          _st =value;
          NotifyPropertyChanged("DEMSource");
        }
      }
    }

  }
}
