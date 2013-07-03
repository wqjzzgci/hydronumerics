using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.NWK11
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class Data
  {

    internal PFSKeyword _keyword;

    internal Data(PFSKeyword keyword)
    {
       _keyword = keyword;
    }

    public Data(string keywordname)
    {
       _keyword = new PFSKeyword(keywordname);
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Double, 0));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer, 0));
    }
    public double Par1
    {
      get { return _keyword.GetParameter(1).ToDouble();}
      set { _keyword.GetParameter(1).Value = value;}
    }

    public int Par2
    {
      get { return _keyword.GetParameter(2).ToInt();}
      set { _keyword.GetParameter(2).Value = value;}
    }

  }
}
