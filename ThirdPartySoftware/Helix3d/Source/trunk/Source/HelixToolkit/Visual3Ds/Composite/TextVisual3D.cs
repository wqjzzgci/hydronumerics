using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public class TextVisual3D : ModelVisual3D
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextVisual3D),
                                        new UIPropertyMetadata(null, VisualChanged));

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(TextVisual3D),
                                        new UIPropertyMetadata(Brushes.Black, VisualChanged));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(TextVisual3D),
                                        new UIPropertyMetadata(11.0, VisualChanged));

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point3D), typeof(TextVisual3D),
                                        new UIPropertyMetadata(new Point3D(0, 0, 0), VisualChanged));

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3D), typeof(TextVisual3D),
                                        new UIPropertyMetadata(new Vector3D(1, 0, 0), VisualChanged));

        public static readonly DependencyProperty UpProperty =
            DependencyProperty.Register("Up", typeof(Vector3D), typeof(TextVisual3D),
                                        new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        public Point3D Center
        {
            get { return (Point3D)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public Vector3D Direction
        {
            get { return (Vector3D)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public Vector3D Up
        {
            get { return (Vector3D)GetValue(UpProperty); }
            set { SetValue(UpProperty, value); }
        }

        private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextVisual3D)d).OnVisualChanged();
        }

        private void OnVisualChanged()
        {
            if (String.IsNullOrEmpty(Text))
                Content = null;
            else
                Content = CreateTextLabel3D(Text, Fill, true, Height, Center, Direction, Up);
        }

        // http://www.ericsink.com/wpf3d/4_Text.html

        /// <summary>
        /// Creates a ModelVisual3D containing a text label.
        /// </summary>
        /// <param name="text">The string</param>
        /// <param name="textColor">The color of the text.</param>
        /// <param name="bDoubleSided">Visible from both sides?</param>
        /// <param name="height">Height of the characters</param>
        /// <param name="center">The center of the label</param>
        /// <param name="over">Horizontal direction of the label</param>
        /// <param name="up">Vertical direction of the label</param>
        /// <returns>Suitable for adding to your Viewport3D</returns>
        public static GeometryModel3D CreateTextLabel3D(
            string text,
            Brush textColor,
            bool bDoubleSided,
            double height,
            Point3D center,
            Vector3D over,
            Vector3D up)
        {
            // First we need a textblock containing the text of our label
            var tb = new TextBlock(new Run(text));
            tb.Foreground = textColor;
            tb.FontFamily = new FontFamily("Arial");

            // Now use that TextBlock as the brush for a material
            var mat = new DiffuseMaterial();
            mat.Brush = new VisualBrush(tb);

            // We just assume the characters are square
            double width = text.Length * height;

            // Since the parameter coming in was the center of the label,
            // we need to find the four corners
            // p0 is the lower left corner
            // p1 is the upper left
            // p2 is the lower right
            // p3 is the upper right
            Point3D p0 = center - width / 2 * over - height / 2 * up;
            Point3D p1 = p0 + up * 1 * height;
            Point3D p2 = p0 + over * width;
            Point3D p3 = p0 + up * 1 * height + over * width;

            // Now build the geometry for the sign.  It's just a
            // rectangle made of two triangles, on each side.

            var mg = new MeshGeometry3D();
            mg.Positions = new Point3DCollection { p0, p1, p2, p3 };

            if (bDoubleSided)
            {
                mg.Positions.Add(p0); // 4
                mg.Positions.Add(p1); // 5
                mg.Positions.Add(p2); // 6
                mg.Positions.Add(p3); // 7
            }

            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(3);
            mg.TriangleIndices.Add(1);
            mg.TriangleIndices.Add(0);
            mg.TriangleIndices.Add(2);
            mg.TriangleIndices.Add(3);

            if (bDoubleSided)
            {
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(5);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(4);
                mg.TriangleIndices.Add(7);
                mg.TriangleIndices.Add(6);
            }

            // These texture coordinates basically stretch the
            // TextBox brush to cover the full side of the label.

            mg.TextureCoordinates.Add(new Point(0, 1));
            mg.TextureCoordinates.Add(new Point(0, 0));
            mg.TextureCoordinates.Add(new Point(1, 1));
            mg.TextureCoordinates.Add(new Point(1, 0));

            if (bDoubleSided)
            {
                mg.TextureCoordinates.Add(new Point(1, 1));
                mg.TextureCoordinates.Add(new Point(1, 0));
                mg.TextureCoordinates.Add(new Point(0, 1));
                mg.TextureCoordinates.Add(new Point(0, 0));
            }

            return new GeometryModel3D(mg, mat);

        }
    }
}