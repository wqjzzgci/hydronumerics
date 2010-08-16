using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.Time.Core;
using HydroNumerics.HydroNet.Core;
using HydroNumerics.Geometry;
using HydroNumerics.Geometry.Shapes;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class DemoViewModel:INotifyPropertyChanged
  {

    private Lake _lake;

    public DemoViewModel(string Name, XYPolygon SurfaceArea)
    { }


    public double Depth
    {
      get { return _lake.Depth; }
      set
      {
        if (_lake.Depth != value)
        {
          _lake.Depth = value;
          NotifyPropertyChanged("Depth");
        }
      }
    }

    public TimespanSeries Evaporation
    {
      get
      {
        var evap = _lake.EvaporationBoundaries.FirstOrDefault();
        if (evap != null)
        {
          return ((EvaporationRateBoundary)evap).TimeValues;
        }
        return null;
      }
    }

    public TimespanSeries Precipitation
    {
      get
      {
        var precip = _lake.SinkSources[0];
          return ((FlowBoundary)precip).TimeValues;
      }
    }

    public TimespanSeries Inflow
    {
      get
      {
        var precip = _lake.SinkSources[1];
          return ((FlowBoundary)precip).TimeValues;
      }
    }

    public TimespanSeries IsotopeConc
    {
      get 
      {return _lake.Output.ChemicalsToLog[ChemicalFactory.Instance.GetChemical(ChemicalNames.IsotopeFraction)];}
    }

    public TimespanSeries ChlorideConc
    {
      get
      { return _lake.Output.ChemicalsToLog[ChemicalFactory.Instance.GetChemical(ChemicalNames.Cl)]; }
    }

    public double GWIsotopeConc
    {
      get
      {
       return ((WaterWithChemicals)((FlowBoundary)_lake.SinkSources[1]).WaterSample).GetConcentration(ChemicalNames.IsotopeFraction);
      }
      set
      {
        if (value != GWIsotopeConc)
        {
          ((WaterWithChemicals)((FlowBoundary)_lake.SinkSources[1]).WaterSample).SetConcentration(ChemicalNames.IsotopeFraction,value);
          NotifyPropertyChanged("GWIsotopeConc");
        }
      }
    }

    public double GWChloridConc
    {
      get
      {
        return ((WaterWithChemicals)((FlowBoundary)_lake.SinkSources[1]).WaterSample).GetConcentration(ChemicalNames.Cl);
      }
      set
      {
        if (value != GWChloridConc)
        {
          ((WaterWithChemicals)((FlowBoundary)_lake.SinkSources[1]).WaterSample).SetConcentration(ChemicalNames.Cl, value);
          NotifyPropertyChanged("GWChloridConc");
        }
      }
    }

    public double HydraulicConductivity
    {
      get { return ((GroundWaterBoundary)_lake.SinkSources[2]).HydraulicConductivity; }
      set
      {
        if (HydraulicConductivity != value)
        {
          ((GroundWaterBoundary)_lake.SinkSources[2]).HydraulicConductivity = value;
          NotifyPropertyChanged("HydraulicConductivity");
        }
      }
    }

    public double GroundwaterHead
    {
      get { return ((GroundWaterBoundary)_lake.SinkSources[2]).GroundwaterHead; }
      set
      {
        if (GroundwaterHead != value)
        {
          ((GroundWaterBoundary)_lake.SinkSources[2]).GroundwaterHead = value;
          NotifyPropertyChanged("GroundwaterHead");
        }
      }
    }


    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
      }
    }


    #endregion
  }
}
