using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace TubeDemo
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            int n = 180;
            double r = 5;
            var pc = new Point3DCollection();
            for (int i = 0; i < n; i++)
            {
                double a = (double) i/n*Math.PI*2;
                pc.Add(new Point3D(Math.Cos(a)*r, Math.Sin(a)*r, Math.Cos(a*10)*0.3));
            }
            tube.Path = pc;
        }
    }
}