using System;
using System.Collections.Generic;
using DHI.Generic.MikeZero;

namespace HydroNumerics.MikeSheTools.PFS.SheFile
{
  /// <summary>
  /// This is an autogenerated class. Do not edit. 
  /// If you want to add methods create a new partial class in another file
  /// </summary>
  public partial class CompControlParaSZPCGTrans: PFSMapper
  {


    internal CompControlParaSZPCGTrans(PFSSection Section)
    {
      _pfsHandle = Section;

      for (int i = 1; i <= Section.GetSectionsNo(); i++)
      {
        PFSSection sub = Section.GetSection(i);
        switch (sub.Name)
        {
          default:
            _unMappedSections.Add(sub.Name);
          break;
        }
      }
    }

    public int Touched
    {
      get
      {
        return _pfsHandle.GetKeyword("Touched", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("Touched", 1).GetParameter(1).Value = value;
      }
    }

    public int IsDataUsedInSetup
    {
      get
      {
        return _pfsHandle.GetKeyword("IsDataUsedInSetup", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("IsDataUsedInSetup", 1).GetParameter(1).Value = value;
      }
    }

    public int MaxNoOfIter
    {
      get
      {
        return _pfsHandle.GetKeyword("MaxNoOfIter", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("MaxNoOfIter", 1).GetParameter(1).Value = value;
      }
    }

    public int GradualDrainActivation
    {
      get
      {
        return _pfsHandle.GetKeyword("GradualDrainActivation", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("GradualDrainActivation", 1).GetParameter(1).Value = value;
      }
    }

    public double MaxHeadChange
    {
      get
      {
        return _pfsHandle.GetKeyword("MaxHeadChange", 1).GetParameter(1).ToDouble();
      }
      set
      {
        _pfsHandle.GetKeyword("MaxHeadChange", 1).GetParameter(1).Value = value;
      }
    }

    public int HorizontalConductanceAveraging
    {
      get
      {
        return _pfsHandle.GetKeyword("HorizontalConductanceAveraging", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("HorizontalConductanceAveraging", 1).GetParameter(1).Value = value;
      }
    }

    public double MaxResidualError
    {
      get
      {
        return _pfsHandle.GetKeyword("MaxResidualError", 1).GetParameter(1).ToDouble();
      }
      set
      {
        _pfsHandle.GetKeyword("MaxResidualError", 1).GetParameter(1).Value = value;
      }
    }

    public double SatThicknessThreshold
    {
      get
      {
        return _pfsHandle.GetKeyword("SatThicknessThreshold", 1).GetParameter(1).ToDouble();
      }
      set
      {
        _pfsHandle.GetKeyword("SatThicknessThreshold", 1).GetParameter(1).Value = value;
      }
    }

    public double UnderRelaxFactor
    {
      get
      {
        return _pfsHandle.GetKeyword("UnderRelaxFactor", 1).GetParameter(1).ToDouble();
      }
      set
      {
        _pfsHandle.GetKeyword("UnderRelaxFactor", 1).GetParameter(1).Value = value;
      }
    }

    public int UnderRelaxation
    {
      get
      {
        return _pfsHandle.GetKeyword("UnderRelaxation", 1).GetParameter(1).ToInt();
      }
      set
      {
        _pfsHandle.GetKeyword("UnderRelaxation", 1).GetParameter(1).Value = value;
      }
    }

  }
}