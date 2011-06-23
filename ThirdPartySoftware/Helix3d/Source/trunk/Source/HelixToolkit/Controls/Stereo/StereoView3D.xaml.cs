using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Interaction logic for StereoView3D.xaml
    /// </summary>
    public partial class StereoView3D : StereoControl
    {

        public StereoView3D()
        {
            InitializeComponent();
            BindViewports(LeftView,RightView);            
        }

    }
}
