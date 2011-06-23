using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The ClonedElement3D renders a cloned visual - set the source to the ModelVisual3D you want to clone.
    /// This is used for Stereo views
    /// </summary>
    public class ClonedElement3D : ModelVisual3D
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof (ModelVisual3D), typeof (ClonedElement3D),
                                        new UIPropertyMetadata(null, SourceChanged));

        public ModelVisual3D Source
        {
            get { return (ModelVisual3D) GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }


        protected static void SourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ClonedElement3D) d).SourceChanged();
        }

        protected void SourceChanged()
        {
            if (Source == null)
            {
                Content = null;
                return;
            }

            var clonedModel = Source.Content.Clone();
            Content = clonedModel;
        }
    }
}