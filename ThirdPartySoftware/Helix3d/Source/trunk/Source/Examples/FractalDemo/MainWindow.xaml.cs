using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit;
using Microsoft.Win32;

namespace FractalDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit();
        }

        private void ZoomToFit_Click(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit(400);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var d = new SaveFileDialog();
            d.Filter = Exporters.Filter;
            if (d.ShowDialog().Value)
            {
                Export(d.FileName);
            }
        }

        private void Export(string fileName)
        {
            if (fileName != null)
                view1.Export(fileName);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class MenuItemList : MenuItem
    {
           
    }
}
