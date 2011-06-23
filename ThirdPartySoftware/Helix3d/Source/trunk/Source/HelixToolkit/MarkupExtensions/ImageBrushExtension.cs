using System;
using System.IO;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HelixToolkit
{
    /// <summary>
    /// Markupextension for Image brushes
    /// Usage: Fill={helix:ImageBrush images\\myimage.png}
    /// </summary>
    public class ImageBrushExtension : MarkupExtension
    {
        private string path;
        public ImageBrushExtension(string path)
        {
            this.path = path;
        }

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath)) return null;
            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fullPath);
            image.EndInit();
            return new ImageBrush(image);
        }
    }
}