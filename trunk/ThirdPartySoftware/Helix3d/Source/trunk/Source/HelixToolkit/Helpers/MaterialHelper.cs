using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Creates diffuse/specular materials
    /// </summary>
    public static class MaterialHelper
    {
        public static double DefaultSpecularPower = 100;

        /// <summary>
        /// Creates a material for the specified color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Material CreateMaterial(Color color)
        {
            return CreateMaterial(new SolidColorBrush(color));
        }

        /// <summary>
        /// Creates a material for the specified color and opacity.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        public static Material CreateMaterial(Color color, double opacity)
        {
            return CreateMaterial(Color.FromArgb((byte)(opacity * 255), color.R, color.G, color.B));
        }

        /// <summary>
        /// Creates a material for the specified brush.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns></returns>
        public static Material CreateMaterial(Brush brush)
        {
            return CreateMaterial(brush, DefaultSpecularPower);
        }

        /// <summary>
        /// Creates a material with the specifed brush as diffuse material. 
        /// This method will also add a white specular material.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <returns></returns>
        public static Material CreateMaterial(Brush brush, double specularPower)
        {
            var mg = new MaterialGroup();
            mg.Children.Add(new DiffuseMaterial(brush));
            if (specularPower > 0)
                mg.Children.Add(new SpecularMaterial(Brushes.White, specularPower));
            return mg;
        }

        /// <summary>
        /// Creates a material with the specified diffuse, emissive and specular brushes.
        /// </summary>
        /// <param name="diffuse">The diffuse.</param>
        /// <param name="emissive">The emissive.</param>
        /// <param name="specular">The specular.</param>
        /// <param name="opacity">The opacity.</param>
        /// <param name="specularPower">The specular power.</param>
        /// <returns></returns>
        public static Material CreateMaterial(Brush diffuse, Brush emissive, Brush specular, double opacity, double specularPower)
        {
            var mg = new MaterialGroup();
            if (diffuse != null)
            {
                diffuse = diffuse.Clone();
                diffuse.Opacity = opacity;
                mg.Children.Add(new DiffuseMaterial(diffuse));
            }
            if (emissive != null)
            {
                emissive = emissive.Clone();
                emissive.Opacity = opacity;
                mg.Children.Add(new EmissiveMaterial(emissive));
            }
            if (specular != null)
            {
                specular = specular.Clone();
                specular.Opacity = opacity;
                mg.Children.Add(new SpecularMaterial(specular, specularPower));
            }
            return mg;
        }

        /// <summary>
        /// Creates a material from the specified bitmap file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Material CreateImageMaterial(string path)
        {
            return CreateImageMaterial(path, 1);
        }

        /// <summary>
        /// Creates a material from the specified bitmap file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        public static Material CreateImageMaterial(string path, double opacity)
        {
            var fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath)) return null;

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fullPath);
            image.EndInit();
            return CreateImageMaterial(image, opacity);
        }

        /// <summary>
        /// Creates a material from the specified BitmapImage.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="opacity">The opacity.</param>
        /// <returns></returns>
        public static Material CreateImageMaterial(BitmapImage image, double opacity)
        {
            var brush = new ImageBrush(image);
            brush.Opacity = opacity;
            return new DiffuseMaterial(brush);
        }

        public static void ChangeOpacity(Material material, double d)
        {
            var mg = material as MaterialGroup;
            if (mg!=null)
                foreach (var m in mg.Children)
                    ChangeOpacity(m, d);
            var dm = material as DiffuseMaterial;
            if (dm != null)
            {
                var scb = dm.Brush as SolidColorBrush;
                if (scb != null)
                    scb.Opacity = d;
            }
        }
    }
}