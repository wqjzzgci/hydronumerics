using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DHI.Generic.MikeZero;

using HydroNumerics.MikeSheTools.PFS.SheFile;

namespace HydroNumerics.MikeSheTools.PFS.WellFile
{
  public class Well:PFSMapper
  {
    private FILTERDATA _fILTERDATA;



    internal Well(PFSSection Section)
    {
      _pfsHandle = Section;

      _fILTERDATA = new FILTERDATA(_pfsHandle.GetSection("FILTERDATA", 1));
    }

    public string ID
    {
      get
      {
        return _pfsHandle.GetKeyword("ID", 1).GetParameter(1).ToString();
      }
    }

    public double XCOR
    {
      get
      {
        return _pfsHandle.GetKeyword("XCOR",1).GetParameter(1).ToDouble();
      }
    }

    public double YCOR
    {
      get
      {
        return _pfsHandle.GetKeyword("YCOR", 1).GetParameter(1).ToDouble();
      }
    }

    public double LEVEL
    {
      get
      {
        return _pfsHandle.GetKeyword("LEVEL", 1).GetParameter(1).ToDouble();
      }
    }

    public double WELLDEPHT1
    {
      get
      {
        return _pfsHandle.GetKeyword("WELLDEPHT1", 1).GetParameter(1).ToDouble();
      }
    }

    public string WELL_FIELD_ID
    {
      get
      {
        return _pfsHandle.GetKeyword("WELL_FIELD_ID", 1).GetParameter(1).ToString();
      }
    }

    public FILTERDATA FILTERDATA
    {
      get { return _fILTERDATA; }
    }


  }
}
