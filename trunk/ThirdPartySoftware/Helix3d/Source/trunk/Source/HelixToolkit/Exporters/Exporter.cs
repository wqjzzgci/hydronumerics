using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public interface IExporter
    {
        void Export(Viewport3D viewport);
        void Export(Visual3D visual);
        void Export(Model3D model);
        void Close();
    }

    public static class Exporters
    {
        /// <summary>
        /// File filter for all the supported exporters
        /// </summary>
        public static string Filter = "Bitmap (*.png;*.jpg)|*.png;*.jpg|XAML (*.xaml)|*.xml|Kerkythea (*.xml)|*.xml|Wavefront (*.obj)|*.obj|Wavefront zipped (*.objz)|*.objz|X3D (*.x3d)|*.x3d";
        // +"|VRML 97 (*.wrl)|*.wlf|POV-Ray (*.pov)|*.pov";
    }

    public abstract class Exporter : IExporter, IDisposable
    {

        protected virtual void ExportHeader()
        {
        }

        protected virtual void ExportViewport(Viewport3D viewport)
        {
        }

        protected virtual void ExportCamera(Camera camera)
        {
        }

        public virtual void Close()
        {
        }

        /// <summary>
        /// Exports the specified viewport.
        /// Exports model, camera and lights.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        public void Export(Viewport3D viewport)
        {
            ExportHeader();
            ExportViewport(viewport);

            // Export objects
            Visual3DHelper.Traverse<GeometryModel3D>(viewport.Children, ExportModel);

            // Export camera
            ExportCamera(viewport.Camera);

            // Export lights
            Visual3DHelper.Traverse<Light>(viewport.Children, ExportLight);
        }

        public void Export(Visual3D visual)
        {
            ExportHeader();
            Visual3DHelper.Traverse<GeometryModel3D>(visual, ExportModel);
        }

        public void Export(Model3D model)
        {
            ExportHeader();
            Visual3DHelper.TraverseModel<GeometryModel3D>(model, ExportModel);
        }

        protected virtual void ExportLight(Light light, Transform3D inheritedTransform)
        {
        }

        protected virtual void ExportModel(GeometryModel3D model, Transform3D inheritedTransform)
        {
        }

        public static void RenderBrush(string path, Brush brush, int w, int h)
        {
            var ib = brush as ImageBrush;
            if (ib != null)
            {
                var bi = ib.ImageSource as BitmapImage;
                if (bi != null)
                {
                    w = (int)bi.PixelWidth;
                    h = (int)bi.PixelHeight;
                }
            }
            var bmp = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
            var rect = new Grid { Background = brush, Width=1, Height=1 };
            rect.LayoutTransform = new ScaleTransform(w, h);
            rect.Arrange(new Rect(0, 0, w, h));
            bmp.Render(rect);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stm = File.Create(path))
            {
                encoder.Save(stm);
            }
        }


        public void Dispose()
        {
            Close();
        }
    }
}