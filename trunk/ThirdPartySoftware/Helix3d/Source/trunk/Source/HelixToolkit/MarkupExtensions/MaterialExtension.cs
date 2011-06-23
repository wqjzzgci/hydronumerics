using System.Windows.Markup;
using System.Windows.Media;

namespace HelixToolkit
{
    /// <summary>
    /// Markupextension for Materials
    /// Usage: Material={helix:Material Blue, Opacity=0.5}
    /// </summary>
    public class MaterialExtension : MarkupExtension
    {
        private readonly Color color;
        public double SpecularPower { get; set; }
        public double SpecularIntensity { get; set; }
        public double Opacity { get; set; }

        public MaterialExtension(Color color)
        {
            this.color = color;
            SpecularPower = 100;
            SpecularIntensity = 1;
            Opacity = 1;
        }

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            var diffuse = new SolidColorBrush(color);
            var specular = BrushHelper.CreateGrayBrush(SpecularIntensity);
            return MaterialHelper.CreateMaterial(diffuse, null, specular, Opacity, SpecularPower);
        }
    }
}