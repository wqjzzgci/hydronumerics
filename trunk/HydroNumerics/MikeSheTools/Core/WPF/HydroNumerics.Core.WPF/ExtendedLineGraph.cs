using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay;


namespace HydroNumerics.Core.WPF
{
  public abstract class ExtendedLineGraph : LineGraph
  {
    protected ObservableCollection<Point> points = new ObservableCollection<Point>();


    public ExtendedLineGraph()
      : base()
    {
      //Set the datasource since that is called by other methods
    }


    public IEnumerable ItemsSource
    {
      get { return (IEnumerable)GetValue(ItemsSourceProperty); }
      set
      {
        SetValue(ItemsSourceProperty, value);
      }
    }

    public DataTemplate ItemTemplate
    {
      get { return (DataTemplate)GetValue(ItemTemplateProperty); }
      set { SetValue(ItemTemplateProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ExtendedLineGraph),
    new PropertyMetadata((s, e) => ((ExtendedLineGraph)s).SourceChanged()));

    public static readonly DependencyProperty ItemTemplateProperty =
    DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ExtendedLineGraph),
    new PropertyMetadata((s, e) => ((ExtendedLineGraph)s).UpdateItems()));

    protected virtual void UpdateItems()
    {


    }



   

    protected virtual void SourceChanged()
    {
      INotifyCollectionChanged incc = ItemsSource as INotifyCollectionChanged;

      incc = ItemsSource as INotifyCollectionChanged;
      if (incc != null)
      {
        incc.CollectionChanged += new NotifyCollectionChangedEventHandler(incc_CollectionChanged);
      }

      foreach (var item in ItemsSource)
      {
        if (item is BaseViewModel)
        {
          ((BaseViewModel)item).PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ExtendedLineGraph_PropertyChanged);
        }

      }

      UpdateItems();
    }

    void ExtendedLineGraph_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      UpdateCore();
    }

    void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateItems();
    }


    protected override void OnRenderCore(DrawingContext dc, RenderState state)
    {
     if (ItemsSource!=null && ItemsSource.GetEnumerator().MoveNext())
       try
       {
         base.OnRenderCore(dc, state);
       }
       catch (Exception e)
       { }
    }

    public override void OnPlotterAttached(Plotter plotter)
    {
      base.OnPlotterAttached(plotter);
      if (DataContext == null)
      {
        Binding binding = new Binding();
        binding.Source = plotter;
        binding.Path = new PropertyPath("DataContext");
        SetBinding(ExtendedLineGraph.DataContextProperty, binding);
      }
      UpdateItems();
    }

    public override void OnPlotterDetaching(Plotter plotter)
    {
      base.OnPlotterDetaching(plotter);
    }
  }
}



