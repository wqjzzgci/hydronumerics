using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using HydroNumerics.MikeSheTools.Mike11;

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
          NotifyPropertyChanged("NumberOfBranches");
          NotifyPropertyChanged("NumberofAttachedCrossSections");
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
          NotifyPropertyChanged("NumberOfBranches");
        }
      }
    }

    public int NumberOfBranches
    {
      get
      {
        return _m11Model.network.Branches.Count();
      }
    }

    public int NumberofAttachedCrossSections
    {
      get
      {
        return _m11Model.network.Branches.Sum(var=>var.CrossSections.Count());
      }
    }
  }
}
