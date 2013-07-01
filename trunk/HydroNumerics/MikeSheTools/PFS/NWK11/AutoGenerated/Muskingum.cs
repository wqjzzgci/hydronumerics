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
  public partial class Muskingum
  {

    private PFSKeyword _keyword;

    internal Muskingum(PFSKeyword keyword)
    {
       _keyword = keyword;
    }

    public Muskingum()
    {
       _keyword = new PFSKeyword("Muskingum");
    }
    public int NumberOfParameters
    {
      get { return _keyword.GetParametersNo(); }
    }

    public int GetValue(int index)
    {
      return _keyword.GetParameter(index + 1).ToInt();;
    }

    public void SetValue(int index, int value)
    {
      _keyword.GetParameter(index + 1).Value = value;
    }

    public void AddValue(int value)
    {
      _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer, value));
    }

  }
}