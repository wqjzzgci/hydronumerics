using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The Expander3D translates all child Model3Ds relative to the ExpandOrigin.
    /// </summary>
    public class Expander3D : ModelVisual3D
    {
        public static readonly DependencyProperty ExpansionProperty =
            DependencyProperty.Register("Expansion", typeof (double), typeof (Expander3D),
                                        new UIPropertyMetadata(2.0, ExpansionChanged));

        public static readonly DependencyProperty ExpandOriginProperty =
            DependencyProperty.Register("ExpandOrigin", typeof (Point3D?), typeof (Expander3D),
                                        new UIPropertyMetadata(null, ExpansionChanged));

        private readonly Dictionary<Model3D, Transform3D> originalTransforms = new Dictionary<Model3D, Transform3D>();
        private Point3D actualExpandOrigin;

        public double Expansion
        {
            get { return (double) GetValue(ExpansionProperty); }
            set { SetValue(ExpansionProperty, value); }
        }

        public Point3D? ExpandOrigin
        {
            get { return (Point3D?) GetValue(ExpandOriginProperty); }
            set { SetValue(ExpandOriginProperty, value); }
        }

        private static void ExpansionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Expander3D) d).Expand();
        }

        private void Expand()
        {
            if (!ExpandOrigin.HasValue)
            {
                if (Content != null)
                    actualExpandOrigin = Content.Bounds.Location;
            }
            else
            {
                actualExpandOrigin = ExpandOrigin.Value;
            }

            Visual3DHelper.TraverseModel<GeometryModel3D>(Content, Expand);
        }

        private void Expand(GeometryModel3D model, Transform3D transformation)
        {
            Transform3D ot;
            if (originalTransforms.ContainsKey(model))
                ot = originalTransforms[model];
            else
            {
                ot = model.Transform;
                originalTransforms.Add(model, ot);
            }

            Transform3D totalTransform = Transform3DHelper.CombineTransform(transformation, ot);

            var mesh = model.Geometry as MeshGeometry3D;
            if (mesh == null)
                return;
            var bounds = new Rect3D();
            foreach (int i in mesh.TriangleIndices)
                bounds.Union(totalTransform.Transform(mesh.Positions[i]));

            Point3D p = bounds.Location;
            Vector3D d = p - actualExpandOrigin;
            d *= Expansion;
            Point3D p2 = actualExpandOrigin + d;
            var t = new TranslateTransform3D(p2 - p);

            model.Transform = Transform3DHelper.CombineTransform(ot, t);
        }

        public void ExpandTo(double value, double animationTime)
        {
            var a = new DoubleAnimation(value,
                                        new Duration(TimeSpan.FromMilliseconds(animationTime)))
                        {AccelerationRatio = 0.3, DecelerationRatio = 0.5};
            BeginAnimation(ExpansionProperty, a);
        }
    }

    public class Exploder3D : ModelVisual3D
    {
        public static readonly DependencyProperty IsExplodingProperty =
            DependencyProperty.Register("IsExploding", typeof (bool), typeof (Exploder3D), new UIPropertyMetadata(false));

        public bool IsExploding
        {
            get { return (bool) GetValue(IsExplodingProperty); }
            set { SetValue(IsExplodingProperty, value); }
        }
    }
}