using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Implements a set of predefined materials.
    /// </summary>
    public static class Materials
    {
        public static Material Red = MaterialHelper.CreateMaterial(Brushes.Red);
        public static Material Green = MaterialHelper.CreateMaterial(Brushes.Green);
        public static Material Blue = MaterialHelper.CreateMaterial(Brushes.Blue);
        public static Material Yellow = MaterialHelper.CreateMaterial(Brushes.Yellow);
        public static Material Gold = MaterialHelper.CreateMaterial(Brushes.Gold);
        public static Material Black = MaterialHelper.CreateMaterial(Brushes.Black);
        public static Material White = MaterialHelper.CreateMaterial(Brushes.White);
        public static Material Gray = MaterialHelper.CreateMaterial(Brushes.Gray);
        public static Material LightGray = MaterialHelper.CreateMaterial(Brushes.LightGray);
        public static Material DarkGray = MaterialHelper.CreateMaterial(Brushes.DarkGray);
        public static Material Hue = MaterialHelper.CreateMaterial(BrushHelper.CreateHsvBrush(1.0));
        public static Material Rainbow = MaterialHelper.CreateMaterial(BrushHelper.CreateRainbowBrush());

    }
}