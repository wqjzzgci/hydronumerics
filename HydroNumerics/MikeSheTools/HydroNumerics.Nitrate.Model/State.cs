using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Core;

namespace HydroNumerics.Nitrate.Model
{
  public class State:BaseViewModel
  {

    public static string seperatorstring = ";";

    #region Sources

    private double? _Upstream;
    public double? Upstream
    {
      get { return _Upstream; }
      set
      {
        if (_Upstream != value)
        {
          _Upstream = value;
          NotifyPropertyChanged("Upstream");
        }
      }
    }
    

    private double? _GroundwaterInflow;
    public double? GroundwaterInflow
    {
      get { return _GroundwaterInflow; }
      set
      {
        if (_GroundwaterInflow != value)
        {
          _GroundwaterInflow = value;
          NotifyPropertyChanged("GroundwaterInflow");
        }
      }
    }

    private double? _AtmosphericDeposition;
    public double? AtmosphericDeposition
    {
      get { return _AtmosphericDeposition; }
      set
      {
        if (_AtmosphericDeposition != value)
        {
          _AtmosphericDeposition = value;
          NotifyPropertyChanged("AtmosphericDeposition");
        }
      }
    }

    private double? _PointSources;
    public double? PointSources
    {
      get { return _PointSources; }
      set
      {
        if (_PointSources != value)
        {
          _PointSources = value;
          NotifyPropertyChanged("PointSources");
        }
      }
    }
    


    private double? _OrganicN;
    public double? OrganicN
    {
      get { return _OrganicN; }
      set
      {
        if (_OrganicN != value)
        {
          _OrganicN = value;
          NotifyPropertyChanged("OrganicN");
        }
      }
    }

    #endregion


    #region Reductions
    private double? _ConstructedWetlands;
    public double? ConstructedWetlands
    {
      get { return _ConstructedWetlands; }
      set
      {
        if (_ConstructedWetlands != value)
        {
          _ConstructedWetlands = value;
          NotifyPropertyChanged("ConstructedWetlands");
        }
      }
    }

    private double? _InternalLakes;
    public double? InternalLakes
    {
      get { return _InternalLakes; }
      set
      {
        if (_InternalLakes != value)
        {
          _InternalLakes = value;
          NotifyPropertyChanged("InternalLakes");
        }
      }
    }

    private double? _InternalRivers;
    public double? InternalRivers
    {
      get { return _InternalRivers; }
      set
      {
        if (_InternalRivers != value)
        {
          _InternalRivers = value;
          NotifyPropertyChanged("InternalRivers");
        }
      }
    }


    private double? _Rivers;
    public double? Rivers
    {
      get { return _Rivers; }
      set
      {
        if (_Rivers != value)
        {
          _Rivers = value;
          NotifyPropertyChanged("Rivers");
        }
      }
    }

    private double? _BigLakes;
    public double? BigLakes
    {
      get { return _BigLakes; }
      set
      {
        if (_BigLakes != value)
        {
          _BigLakes = value;
          NotifyPropertyChanged("BigLakes");
        }
      }
    }
    

    #endregion

    private double? _DownStreamOutput;
    public double? DownStreamOutput
    {
      get { return _DownStreamOutput; }
      set
      {
        if (_DownStreamOutput != value)
        {
          _DownStreamOutput = value;
          NotifyPropertyChanged("DownStreamOutput");
        }
      }
    }
    

    private double? _WaterFlow;
    public double? WaterFlow
    {
      get { return _WaterFlow; }
      set
      {
        if (_WaterFlow != value)
        {
          _WaterFlow = value;
          NotifyPropertyChanged("WaterFlow");
        }
      }
    }

    private double? _BiasCorrectedWaterFlow;
    public double? BiasCorrectedWaterFlow
    {
      get { return _BiasCorrectedWaterFlow; }
      set
      {
        if (_BiasCorrectedWaterFlow != value)
        {
          _BiasCorrectedWaterFlow = value;
          NotifyPropertyChanged("BiasCorrectedWaterFlow");
        }
      }
    }
    

    private double? _Temperature;
    public double? Temperature
    {
      get { return _Temperature; }
      set
      {
        if (_Temperature != value)
        {
          _Temperature = value;
          NotifyPropertyChanged("Temperature");
        }
      }
    }


    private double? _Precipitation;
    public double? Precipitation
    {
      get { return _Precipitation; }
      set
      {
        if (_Precipitation != value)
        {
          _Precipitation = value;
          NotifyPropertyChanged("Precipitation");
        }
      }
    }


    public static string HeadLine()
    {
      StringBuilder s = new StringBuilder();

      s.Append("Temperature" + seperatorstring);
      s.Append("Precipitation" + seperatorstring);
      s.Append("WaterFlow" + seperatorstring);
      s.Append("BiasCorrectedWaterFlow" + seperatorstring);

      s.Append("GroundwaterInflow" + seperatorstring); 
      s.Append("AtmosphericDeposition" + seperatorstring);
      s.Append("OrganicN" + seperatorstring);
      s.Append("PointSources" + seperatorstring);
      
      s.Append("InternalLakes" + seperatorstring);
      s.Append("InternalRivers" + seperatorstring);
      s.Append("ConstructedWetlands" + seperatorstring);
      s.Append("BigLakes" + seperatorstring);
      s.Append("Rivers" + seperatorstring);

      s.Append("DownStreamOutput" + seperatorstring);

      return s.ToString();
    }


    public override string ToString()
    {
      StringBuilder s = new StringBuilder();

      s.Append(Temperature.ToString() + seperatorstring);
      s.Append(Precipitation.ToString() + seperatorstring);
      s.Append(WaterFlow.ToString() + seperatorstring);
      s.Append(BiasCorrectedWaterFlow.ToString() + seperatorstring);

      s.Append(GroundwaterInflow.ToString() + seperatorstring);
      s.Append(AtmosphericDeposition.ToString() + seperatorstring);
      s.Append(OrganicN.ToString() + seperatorstring);
      s.Append(PointSources.ToString() + seperatorstring);

      s.Append(InternalLakes.ToString() + seperatorstring);
      s.Append(InternalRivers.ToString() + seperatorstring);
      s.Append(ConstructedWetlands.ToString() + seperatorstring);
      s.Append(BigLakes.ToString() + seperatorstring);
      s.Append(Rivers.ToString() + seperatorstring);

      s.Append(DownStreamOutput.ToString() + seperatorstring);

      return s.ToString();
    }
    
  }
}
