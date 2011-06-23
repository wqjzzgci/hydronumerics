using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// AnaglyphViewer control
    /// 
    /// Inspirations
    /// - Petzold's anaglyph space station (using opacity)
    ///     http://www.charlespetzold.com/3D/ 
    /// - Greg Schechter multi input shader effects (for the AnaglyphEffect)
    ///     http://blogs.msdn.com/greg_schechter/archive/2008/09/27/a-more-useful-multi-input-effect.aspx
    /// - Barcinski & Jean-Jean: Making of Part III - Anaglyph
    ///     http://blog.barcinski-jeanjean.com/2008/10/17/making-of-part-iii-anaglyph/
    /// </summary>

    [ContentProperty("Children"), Localizability(LocalizationCategory.NeverLocalize)]
    public partial class AnaglyphView3D : StereoControl
    {
        public AnaglyphMethod Method
        {
            get { return (AnaglyphMethod)GetValue(MethodProperty); }
            set { SetValue(MethodProperty, value); }
        }

        public static readonly DependencyProperty MethodProperty =
            DependencyProperty.Register("Method", typeof(AnaglyphMethod), typeof(AnaglyphView3D), new UIPropertyMetadata(AnaglyphMethod.Gray));

        public double HorizontalOffset
        {
            get { return (double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register("HorizontalOffset", typeof(double), typeof(AnaglyphView3D), new UIPropertyMetadata(0.0, HorizontalOffsetChanged));

        private static void HorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnaglyphView3D) d).OnHorizontalOffsetChanged();
        }

        private void OnHorizontalOffsetChanged()
        {
//            RightView.Margin=new Thickness(HorizontalOffset,0,-HorizontalOffset,0);
        }


        public AnaglyphView3D()
        {
            InitializeComponent();
            BindViewports(LeftView,RightView);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Left:
                    HorizontalOffset-=0.001f;
                    break;
                case Key.Right:
                    HorizontalOffset+=0.001f;
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }
    }
}
