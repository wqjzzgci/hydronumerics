using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Core;
using HydroNumerics.Geometry.Net;



namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class M11ViewModel:BaseViewModel
  {
    private M11Setup _m11Model;
    private string _sim11FileName="";
    private string _nwk11FileName="";

    public ObservableCollection<M11Branch> Branches{get;private set;}
    public ObservableCollection<CrossSection> SelectedCrossSections {get;private set;}


    public M11ViewModel()
    {
      _m11Model = new M11Setup();
      SelectedCrossSections = new ObservableCollection<CrossSection>();
      SelectedCrossSections.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedCrossSections_CollectionChanged);
    }

    void SelectedCrossSections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null)
      {
        foreach (var v in e.NewItems)
        {
          CrossSection cv = v as CrossSection;
          if (!cv.DEMHeight.HasValue)
          {
            double? val;
            if (KMSData.TryGetHeight(cv.MidStreamLocation, 32, out val))
              cv.DEMHeight = val;
          }
        }
      }
    }

    public void WriteToShape(string FilePrefix)
    {
      _m11Model.WriteToShape(FilePrefix);
    }

    public string Sim11FileName
    {
      get
      {
        return _sim11FileName;
      }
      set
      {
        if (value != _sim11FileName)
        {
        _m11Model.ReadSetup(value);
          _sim11FileName = value;
          Branches = new ObservableCollection<M11Branch>(_m11Model.network.Branches);
        }
      }
    }

    public string Nwk11FileName
    {
      get { return _nwk11FileName;}
      set
      {
        if(value!=_nwk11FileName)
        {
          _m11Model.ReadNetwork(value);
          _nwk11FileName = value;
        }
      }
    }
  }
}
