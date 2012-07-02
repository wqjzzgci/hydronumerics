using System;
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
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;

namespace DateTimeAxisTutorial
{
    public partial class Page : UserControl
    {
        private HorizontalDateTimeAxis dateAxis;

        public Page()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            #region Prepering arrays with data
            
            const int N = 10;
            double[] x = new double[N];
            double[] y = new double[N];

            TimeSpan step = new TimeSpan(11, 21, 31);

            x[0] =1;// new DateTime(2000,1,1);
            y[0] = 1;

            for (int i = 1; i < N; i++)
            {
              x[i] = i+1;// x[i - 1].AddYears(1);
                y[i] = (Int32)(y[i - 1] + Math.E) % 37;
                y[i] = i + 1;
            }
            #endregion

            //Here we replace default numeric axis with DateTime Axis
            //dateAxis = new HorizontalDateTimeAxis();
            //plotter.HorizontalAxis =dateAxis;

//            dateAxis = plotter.DateTimeHorizontalAxis;
            //Now we should set xMapping using ConvertToDouble method
            var xDataSource = x.AsXDataSource();
           // xDataSource.SetXMapping(d => dateAxis.ConvertToDouble(d));
            var yDataSource = y.AsYDataSource();

            CompositeDataSource compositeDataSource = xDataSource.Join(yDataSource);
            LineGraph line = new LineGraph(compositeDataSource,"Graph depends on DateTime");
            
            plotter.Children.Add(line);
            plotter.FitToView();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
