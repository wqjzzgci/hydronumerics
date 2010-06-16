﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;

using Expression.Blend.SampleData.SampleDataSource;

using System.Collections.ObjectModel;


using System.Windows.Controls.DataVisualization.Charting;

namespace SilverlightApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            Mymap.MouseDoubleClick += new EventHandler<Microsoft.Maps.MapControl.MapMouseEventArgs>(Mymap_MouseDoubleClick);

            ServiceReference3.Service1Client sc = new SilverlightApplication1.ServiceReference3.Service1Client();
            sc.DoWorkCompleted += new EventHandler<SilverlightApplication1.ServiceReference3.DoWorkCompletedEventArgs>(sc_DoWorkCompleted);

            MapPolyline line = new MapPolyline();
            line.Locations = new LocationCollection() { 
        new Location(55.715094, 12.51892), 
        new Location(55.715094, 12.1892), 
        new Location(55.615094, 12.1892), };

            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            line.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
            line.StrokeThickness = 5;
            polygon.Opacity = 0.7;
            polygon.Locations = new LocationCollection() { 
        new Location(55.715094, 12.51892), 
        new Location(55.715094, 12.1892), 
        new Location(55.615094, 12.1892), 
        new Location(55.615094, 12.51892) };

         

            Mymap.Children.Add(line);

            polygon.MouseLeftButtonUp += new MouseButtonEventHandler(polygon_MouseLeftButtonUp);

            var data = new SampleDataSource();

            RechargeData.ItemsSource = data.Collection;



            //sc.DoWorkAsync();

            //ServiceReference2.WellWrapperClient we = new SilverlightApplication1.ServiceReference2.WellWrapperClient();
            //we.GetXCompleted += new EventHandler<SilverlightApplication1.ServiceReference2.GetXCompletedEventArgs>(we_GetXCompleted);
            //we.GetXAsync();
            //sc.CloseAsync();
 
        }

        void polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        void Mymap_MouseDoubleClick(object sender, Microsoft.Maps.MapControl.MapMouseEventArgs e)
        {
          MyChart.Title = e.ViewportPoint.X + " og y er:  " + e.ViewportPoint.Y;
        }

        void sc_DoWorkCompleted(object sender, SilverlightApplication1.ServiceReference3.DoWorkCompletedEventArgs e)
        {
          MyChart.Title = e.Result;
        }

        void sc_DoWorkCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
        }

        void we_GetXCompleted(object sender, SilverlightApplication1.ServiceReference2.GetXCompletedEventArgs e)
        {
          ServiceReference2.JupiterWell w = e.Result;
          MyChart.Title = w.ID + " intake= " + w.IntakesA[0].IDNumber;
         
          
        }

        void s_GetDataCompleted(object sender, SilverlightApplication1.ServiceReference1.GetDataCompletedEventArgs e)
        {
            ObservableCollection<string> data = e.Result;
            MyChart.Title = data[1];

          
        }

    }
}
