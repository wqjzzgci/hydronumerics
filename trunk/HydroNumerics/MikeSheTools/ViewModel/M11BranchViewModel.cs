using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Research.DynamicDataDisplay.DataSources;

using HydroNumerics.MikeSheTools.Mike11;
using HydroNumerics.Core.WPF;
using HydroNumerics.Geometry;

namespace HydroNumerics.MikeSheTools.ViewModel
{
  public class M11BranchViewModel:BaseViewModel
  {


    private M11Branch _branch;

    /// <summary>
    /// Gets and sets Branch;
    /// </summary>
    public M11Branch Branch
    {
      get { return _branch; }
      set
      {
        if (value != _branch)
        {
          _branch = value;
          NotifyPropertyChanged("Branch");
        }
      }
    }


    private List<M11BranchViewModel> upstreamBranches;

    /// <summary>
    /// Gets and sets UpstreamBranches;
    /// </summary>
    public List<M11BranchViewModel> UpstreamBranches
    {
      get { return upstreamBranches; }
      set
      {
        if (value != upstreamBranches)
        {
          upstreamBranches = value;
          NotifyPropertyChanged("UpstreamBranches");
        }
      }
    }

    
    

    public M11BranchViewModel(M11Branch Branch)
    {
      this.Branch =Branch;
      UpstreamBranches = new List<M11BranchViewModel>(Branch.UpstreamBranches.Select(b => new M11BranchViewModel(b)));
    }

    CompositeDataSource _profile;
    public CompositeDataSource Profile
    {
      get
      {
        if (_profile == null)
        {
          var xData = new EnumerableDataSource<double>(Branch.CrossSections.Select(cr => cr.Chainage));
          xData.SetXMapping(x => x);
          var yData = new EnumerableDataSource<double>(Branch.CrossSections.Select(cr => cr.MaxHeightMrk1and3));
          yData.SetYMapping(y => y);
          _profile = xData.Join(yData);
        }
        return _profile;
      }
    }



    private double chainageOffset=0;

    /// <summary>
    /// Gets and sets ChainageOffset;
    /// </summary>
    public double ChainageOffset
    {
      get { return chainageOffset; }
      set
      {
        if (value != chainageOffset)
        {
          _profileOffset = null;//Reset
          chainageOffset = value;
          NotifyPropertyChanged("ChainageOffset");
        }
      }
    }

    


    CompositeDataSource _profileOffset;
    public CompositeDataSource ProfileOffset
    {
      get
      {
        if (_profileOffset == null)
        {
          var xData = new EnumerableDataSource<double>(Branch.CrossSections.Select(cr => cr.Chainage));

          xData.SetXMapping(x =>ChainageOffset- (Branch.ChainageEnd - x));
          var yData = new EnumerableDataSource<double>(Branch.CrossSections.Select(cr => cr.MaxHeightMrk1and3));
          yData.SetYMapping(y => y);
          _profileOffset = xData.Join(yData);
        }
        return _profileOffset;
      }
    }



    CompositeDataSource _network;
    public CompositeDataSource Network
    {
      get
      {
        if (_network == null)
        {
          var xData = new EnumerableDataSource<double>(Branch.Points.Select(p=>p.X));
          xData.SetXMapping(x => x);
          var yData = new EnumerableDataSource<double>(Branch.Points.Select(p=>p.Y));
          yData.SetYMapping(y => y);
          _network = xData.Join(yData);
        }
        return _network;
      }
    }


  }
}
