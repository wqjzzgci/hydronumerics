using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HydroNumerics.HydroNet.Core;

namespace HydroNumerics.HydroNet.ViewModel
{
  public class ModelViewModel
  {

    private Model _model;
    private ObservableCollection<WaterBodyViewModel> _waterBodies;
    private ObservableCollection<StreamViewModel> _streams;
    private ObservableCollection<LakeViewModel> _lakes;


    public ModelViewModel(string FileName)
    {
      _model = ModelFactory.GetModel(FileName);
    }

    public ModelViewModel(Model M)
    {
      _model = M;
      Initialize();
    }

    private void Initialize()
    {
      _waterBodies = new ObservableCollection<WaterBodyViewModel>(_model._waterBodies.Select(var => new WaterBodyViewModel(var)));

      var streams = _model._waterBodies.Where(var => var is Stream);
      _streams = new ObservableCollection<StreamViewModel>(streams.Select(var => new StreamViewModel(var as Stream)));

      var lakes = _model._waterBodies.Where(var => var is Lake);
      _lakes = new ObservableCollection<LakeViewModel>(lakes.Select(var => new LakeViewModel(var as Lake)));

      _waterBodies.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_waterBodies_CollectionChanged);

    }


    /// <summary>
    /// This will not work because we are using wrappers. Needs to implement access to the wrapped object.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void _waterBodies_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      foreach (var I in e.OldItems)
        if (I is StreamViewModel)
          _streams.Remove(I as StreamViewModel);
    }


    public string Name
    {
      get
      {
        return "Model Name";
      }
    }


    public ObservableCollection<StreamViewModel> Streams
    {
      get
      {
        return _streams;
      }
    }

    public ObservableCollection<LakeViewModel> Lakes
    {
      get { return _lakes; }
    }

    public ObservableCollection<WaterBodyViewModel> WaterBodies
    {
      get { return _waterBodies; }
    }


  }
}
