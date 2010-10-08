using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.DFS;

namespace HydroNumerics.MikeSheTools.ViewModel
{

  public enum SourceType
  {
    Oracle,
    KMSWeb,
    DFS2
  }

  public class DEMSourceConfiguration:BaseViewModel
  {

    private SourceType _st;
    private string _oracleServerName = "geusjup3.jupiter";
    private string _tableName = "FPH.DKDHM10";
    private string _userName = "mike11cs";
    private string _password = "mike11cs22";
    private string _dfs2File = "";

    public DFS2 DFSdem { get; set; }

    public string Dfs2File
    {
      get { return _dfs2File; }
      set
      {
        if (value != _dfs2File)
        {
          _st = SourceType.DFS2;
          _dfs2File = value;
          DFSdem = new DFS2(_dfs2File);
          NotifyPropertyChanged("Dfs2File");
        }
      }
    }


    public string Password
    {
      get { return _password; }
      set { _password = value; }
    }

    public string UserName
    {
      get { return _userName; }
      set { _userName = value; }
    }
  

    public string TableName
    {
      get { return _tableName; }
      set { _tableName = value; }
    }


    
    public string OracleServerName
    {
      get { return _oracleServerName; }
      set
      {
        if (value != _oracleServerName)
        {

          _oracleServerName = value;
          NotifyPropertyChanged("OracleServerName");
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
