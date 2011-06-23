using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    // TODO: under construction...

    /// <summary>
    /// This is a ModelVisual3D where the child Model3Ds are sorted by opacity.
    /// Opaque models are added first, then transparent models at decreasing distance from camera.
    /// </summary>
    [ContentProperty("Content")]
    public class OpacitySortingVisual3D : ModelVisual3D
    {
        private Model3DGroup opaqueChildren;
        private Model3DGroup transparentChildren;

        public Model3DCollection Models
        {
            get { return (Model3DCollection)GetValue(ModelsProperty); }
            set { SetValue(ModelsProperty, value); }
        }

        public static readonly DependencyProperty ModelsProperty =
            DependencyProperty.Register("Models", typeof(Model3DCollection), typeof(OpacitySortingVisual3D),
                                        new FrameworkPropertyMetadata(new Model3DCollection(), FrameworkPropertyMetadataOptions.AffectsRender, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((OpacitySortingVisual3D)d).OnContentChanged(args);
        }

        private void OnContentChanged(DependencyPropertyChangedEventArgs args)
        {

        }


        public OpacitySortingVisual3D()
        {
            opaqueChildren = new Model3DGroup();
            transparentChildren = new Model3DGroup();
            Children.Add(new ModelVisual3D { Content = opaqueChildren });
            Children.Add(new ModelVisual3D { Content = transparentChildren });
        }
    }
}