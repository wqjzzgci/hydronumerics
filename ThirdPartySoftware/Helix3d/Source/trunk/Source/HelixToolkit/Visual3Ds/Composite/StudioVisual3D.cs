using System;
using System.Windows.Media.Media3D;
using System.Windows;

namespace HelixToolkit
{
    /// <summary>
    /// Visual model - reading model from file (.3ds, .obj. .lwo or .stl)
    /// </summary>
    public class StudioVisual3D : ModelVisual3D
    {
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string), typeof(StudioVisual3D), new UIPropertyMetadata(null, SourceChanged));

        protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((StudioVisual3D)obj).UpdateModel();
        }

        private readonly ModelVisual3D visualChild;

        public StudioVisual3D()
        {
            visualChild = new ModelVisual3D();
            Children.Add(visualChild);
        }

        void UpdateModel()
        {
            visualChild.Content = ModelImporter.Load(Source);
        }

    }
}
