using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

using Microsoft.Research.DynamicDataDisplay;


namespace HydroNumerics.Core.WPF
{
  public class GrapItemsControl : FrameworkElement, IPlotterElement
  {


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
    DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(GrapItemsControl),
    new PropertyMetadata((s, e) => ((GrapItemsControl)s).SourceChanged()));

    public static readonly DependencyProperty ItemTemplateProperty =
    DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(GrapItemsControl),
    new PropertyMetadata((s, e) => ((GrapItemsControl)s).UpdateItems()));


    List<IPlotterElement> graphs = new List<IPlotterElement>();

    protected virtual void SourceChanged()
    {

        if (ItemsSource != null)
        {
          INotifyCollectionChanged incc = ItemsSource as INotifyCollectionChanged;

          incc = ItemsSource as INotifyCollectionChanged;
          if (incc != null)
          {
            incc.CollectionChanged += new NotifyCollectionChangedEventHandler(incc_CollectionChanged);
          }
        } 
      UpdateItems();
    }

    private void UpdateItems()
    {
      if (Plotter != null)
      {
        foreach (var v in graphs)
          Plotter.Children.Remove(v);
        graphs.Clear();
        if (ItemsSource != null & ItemTemplate != null)
        {
          foreach (var p in ItemsSource)
          {
            var visualItem = this.ItemTemplate.LoadContent() as FrameworkElement;
            visualItem.DataContext = p;
            graphs.Add((IPlotterElement)visualItem);
            Plotter.Children.Add((IPlotterElement)visualItem);
          }
        }
      }
    }

    void incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      UpdateItems();
    }


    #region IPlotterElement Members

    public void OnPlotterAttached(Plotter plotter)
    {

      Binding binding = new Binding();
      binding.Source = plotter;
      binding.Path = new PropertyPath("DataContext");
      var v = SetBinding(GrapItemsControl.DataContextProperty, binding);


      Plotter = plotter;
      UpdateItems();
    }

    public void OnPlotterDetaching(Plotter plotter)
    {

    }

    public Plotter Plotter { get; private set; }

    #endregion
  }
}
