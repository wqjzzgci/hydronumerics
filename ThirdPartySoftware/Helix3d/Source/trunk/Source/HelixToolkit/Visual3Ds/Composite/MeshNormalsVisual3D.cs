using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    // todo: this is under construction...
    // this is not very efficient - used just for debugging
    public class MeshNormals3D : ModelVisual3D
    {
        public static readonly DependencyProperty MeshProperty =
            DependencyProperty.Register("Mesh", typeof (MeshGeometry3D), typeof (MeshNormals3D),
                                        new UIPropertyMetadata(null, MeshChanged));

        public MeshGeometry3D Mesh
        {
            get { return (MeshGeometry3D) GetValue(MeshProperty); }
            set { SetValue(MeshProperty, value); }
        }

        protected static void MeshChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((MeshNormals3D) obj).UpdateVisuals();
        }

        protected void UpdateVisuals()
        {
            Children.Clear();

            for (int i = 0; i < Mesh.Positions.Count; i++)
            {
                var arrow = new ArrowVisual3D
                                {
                                    Point1 = Mesh.Positions[i],
                                    Point2 = Mesh.Positions[i] + Mesh.Normals[i],
                                    Diameter = 0.1,
                                    Fill = Brushes.Blue,
                                    ThetaDiv = 10
                                };
                Children.Add(arrow);
            }
        }
    }
}