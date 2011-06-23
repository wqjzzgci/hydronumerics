using System.Windows.Markup;

namespace HelixToolkit
{
    /// <summary>
    /// Gradient brush type
    /// </summary>
    public enum GradientBrushType
    {
        /// <summary>
        /// Hue gradient
        /// </summary>
        Hue,
        /// <summary>
        /// Rainbow gradient
        /// </summary>
        Rainbow
    }

    /// <summary>
    /// Markupextension for Materials
    /// Usage: Material={helix:Gradient Rainbow}
    /// </summary>
    public class GradientExtension : MarkupExtension
    {
        private readonly GradientBrushType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="GradientExtension"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public GradientExtension(GradientBrushType type)
        {
            this.type = type;
        }

        /// <summary>
        /// Returns the gradient brush of the specified type.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        /// The brush to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            switch (type)
            {
                case GradientBrushType.Hue: return GradientBrushes.Hue;
                case GradientBrushType.Rainbow: return GradientBrushes.Rainbow;
                default: return null;
            }
        }
    }
}