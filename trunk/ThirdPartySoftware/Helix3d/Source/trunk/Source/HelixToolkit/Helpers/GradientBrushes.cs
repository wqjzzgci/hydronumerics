using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Implements a set of predefined gradient brushes.
    /// Usage in XAML: Fill="{x:Static helix:GradientBrushes.Hue}"
    /// </summary>
    public static class GradientBrushes
    {
        public static LinearGradientBrush Hue = BrushHelper.CreateHsvBrush(1.0);
        public static LinearGradientBrush Rainbow = BrushHelper.CreateRainbowBrush();
        public static LinearGradientBrush BlueWhiteRed = BrushHelper.CreateGradientBrush(Colors.Blue,Colors.White,Colors.Red);

        public static LinearGradientBrush HueStripes = BrushHelper.CreateSteppedGradientBrush(Hue);
        public static LinearGradientBrush RainbowStripes = BrushHelper.CreateSteppedGradientBrush(Rainbow,24);
    }
}