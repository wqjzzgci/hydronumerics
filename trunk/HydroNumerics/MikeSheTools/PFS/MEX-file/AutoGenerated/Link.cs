using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;
using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.MEX
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class Link
  {

    private PFSKeyword _keyword;

    internal Link(PFSKeyword keyword)
    {
       _keyword = keyword;
    }

    public Link()
    {
       _keyword = new PFSKeyword("Link");
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Double));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Double));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.String));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Integer));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
       _keyword.AddParameter(new PFSParameter(PFSParameterType.Missing));
    }
    public string Par1
    {
      get { return _keyword.GetParameter(1).ToString();}
      set { _keyword.GetParameter(1).Value = value;}
    }

    public string Par2
    {
      get { return _keyword.GetParameter(2).ToString();}
      set { _keyword.GetParameter(2).Value = value;}
    }

    public string Par3
    {
      get { return _keyword.GetParameter(3).ToString();}
      set { _keyword.GetParameter(3).Value = value;}
    }

    public int Par4
    {
      get { return _keyword.GetParameter(4).ToInt();}
      set { _keyword.GetParameter(4).Value = value;}
    }

    public string Par5
    {
      get { return _keyword.GetParameter(5).ToString();}
      set { _keyword.GetParameter(5).Value = value;}
    }

    public int Par6
    {
      get { return _keyword.GetParameter(6).ToInt();}
      set { _keyword.GetParameter(6).Value = value;}
    }

    public double Par7
    {
      get { return _keyword.GetParameter(7).ToDouble();}
      set { _keyword.GetParameter(7).Value = value;}
    }

    public string Par8
    {
      get { return _keyword.GetParameter(8).ToString();}
      set { _keyword.GetParameter(8).Value = value;}
    }

    public string Par9
    {
      get { return _keyword.GetParameter(9).ToString();}
      set { _keyword.GetParameter(9).Value = value;}
    }

    public string Par10
    {
      get { return _keyword.GetParameter(10).ToString();}
      set { _keyword.GetParameter(10).Value = value;}
    }

    public string Par11
    {
      get { return _keyword.GetParameter(11).ToString();}
      set { _keyword.GetParameter(11).Value = value;}
    }

    public string Par12
    {
      get { return _keyword.GetParameter(12).ToString();}
      set { _keyword.GetParameter(12).Value = value;}
    }

    public string Par13
    {
      get { return _keyword.GetParameter(13).ToString();}
      set { _keyword.GetParameter(13).Value = value;}
    }

    public double Par14
    {
      get { return _keyword.GetParameter(14).ToDouble();}
      set { _keyword.GetParameter(14).Value = value;}
    }

    public int Par15
    {
      get { return _keyword.GetParameter(15).ToInt();}
      set { _keyword.GetParameter(15).Value = value;}
    }

    public string Par16
    {
      get { return _keyword.GetParameter(16).ToString();}
      set { _keyword.GetParameter(16).Value = value;}
    }

    public string Par17
    {
      get { return _keyword.GetParameter(17).ToString();}
      set { _keyword.GetParameter(17).Value = value;}
    }

    public int Par18
    {
      get { return _keyword.GetParameter(18).ToInt();}
      set { _keyword.GetParameter(18).Value = value;}
    }

    public string Par19
    {
      get { return _keyword.GetParameter(19).ToString();}
      set { _keyword.GetParameter(19).Value = value;}
    }

    public int Par20
    {
      get { return _keyword.GetParameter(20).ToInt();}
      set { _keyword.GetParameter(20).Value = value;}
    }

    public int Par21
    {
      get { return _keyword.GetParameter(21).ToInt();}
      set { _keyword.GetParameter(21).Value = value;}
    }

    public string Par22
    {
      get { return _keyword.GetParameter(22).ToString();}
      set { _keyword.GetParameter(22).Value = value;}
    }

    public string Par23
    {
      get { return _keyword.GetParameter(23).ToString();}
      set { _keyword.GetParameter(23).Value = value;}
    }

    public string Par24
    {
      get { return _keyword.GetParameter(24).ToString();}
      set { _keyword.GetParameter(24).Value = value;}
    }

    public string Par25
    {
      get { return _keyword.GetParameter(25).ToString();}
      set { _keyword.GetParameter(25).Value = value;}
    }

    public string Par26
    {
      get { return _keyword.GetParameter(26).ToString();}
      set { _keyword.GetParameter(26).Value = value;}
    }

    public string Par27
    {
      get { return _keyword.GetParameter(27).ToString();}
      set { _keyword.GetParameter(27).Value = value;}
    }

    public string Par28
    {
      get { return _keyword.GetParameter(28).ToString();}
      set { _keyword.GetParameter(28).Value = value;}
    }

    public string Par29
    {
      get { return _keyword.GetParameter(29).ToString();}
      set { _keyword.GetParameter(29).Value = value;}
    }

    public string Par30
    {
      get { return _keyword.GetParameter(30).ToString();}
      set { _keyword.GetParameter(30).Value = value;}
    }

    public string Par31
    {
      get { return _keyword.GetParameter(31).ToString();}
      set { _keyword.GetParameter(31).Value = value;}
    }

    public string Par32
    {
      get { return _keyword.GetParameter(32).ToString();}
      set { _keyword.GetParameter(32).Value = value;}
    }

  }
}