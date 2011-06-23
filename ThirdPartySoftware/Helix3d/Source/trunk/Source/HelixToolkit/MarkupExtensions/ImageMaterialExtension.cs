using System.Windows.Markup;

namespace HelixToolkit
{
    /// <summary>
    /// Markupextension for Image Materials
    /// Usage: Material={helix:ImageMaterial images\\myimage.png, Opacity=0.8}
    /// </summary>
    public class ImageMaterialExtension : MarkupExtension
    {
        private string path;
        public double Opacity { get; set; }

        public ImageMaterialExtension(string path)
        {
            this.path = path;
            Opacity = 1;
        }

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            return MaterialHelper.CreateImageMaterial(path, Opacity);
        }
    }
}