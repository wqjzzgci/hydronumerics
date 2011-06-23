using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// Helper methods for Visual3D objects
    /// </summary>
    public static class Visual3DHelper
    {
        /// <summary>
        /// Finds the bounding box for a collection of Visual3Ds.
        /// </summary>
        /// <param name="children">The children.</param>
        /// <returns></returns>
        public static Rect3D FindBounds(Visual3DCollection children)
        {
            var bounds = Rect3D.Empty;
            foreach (var visual in children)
            {
                var b = FindBounds(visual, Transform3D.Identity);
                bounds.Union(b);
            }
            return bounds;
        }

        /// <summary>
        /// Finds the bounding box for the specified visual.
        /// </summary>
        /// <param name="visual">The visual.</param>
        /// <param name="transform">The transform if the visual.</param>
        /// <returns></returns>
        public static Rect3D FindBounds(Visual3D visual, Transform3D transform)
        {
            var bounds = Rect3D.Empty;
            var childTransform = Transform3DHelper.CombineTransform(visual.Transform, transform);
            var model = GetModel(visual);
            if (model != null)
            {
                // apply transform
                var transformedBounds = childTransform.TransformBounds(model.Bounds);
                bounds.Union(transformedBounds);
            }
            foreach (var child in GetChildren(visual))
            {
                var b = FindBounds(child, childTransform);
                bounds.Union(b);
            }
            return bounds;
        }

        private static readonly PropertyInfo Visual3DModelPropertyInfo =
            typeof(Visual3D).GetProperty("Visual3DModel", BindingFlags.Instance | BindingFlags.NonPublic);

        private static Model3D GetModel(Visual3D visual)
        {
            Model3D model;
            var mv = visual as ModelVisual3D;
            if (mv != null)
            {
                model = mv.Content;
            }
            else
            {
                model = Visual3DModelPropertyInfo.GetValue(visual, null) as Model3D;
            }
            return model;
        }

        private static IEnumerable<Visual3D> GetChildren(Visual3D visual)
        {
            int n = VisualTreeHelper.GetChildrenCount(visual);
            for (int i = 0; i < n; i++)
            {
                var child = VisualTreeHelper.GetChild(visual, i) as Visual3D;
                if (child == null)
                    continue;
                yield return child;
            }
        }
        /// <summary>
        /// Traverses the Visual3D/Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visuals">The visuals.</param>
        /// <param name="action">The action.</param>
        public static void Traverse<T>(Visual3DCollection visuals, Action<T, Transform3D> action) where T : Model3D
        {
            foreach (var child in visuals)
                Traverse(child, action);
        }

        /// <summary>
        /// Traverses the Visual3D/Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visual">The visual.</param>
        /// <param name="action">The action.</param>
        public static void Traverse<T>(Visual3D visual, Action<T, Transform3D> action) where T : Model3D
        {
            Traverse(visual, Transform3D.Identity, action);
        }

        private static void Traverse<T>(Visual3D visual, Transform3D transform,
                                        Action<T, Transform3D> action) where T : Model3D
        {
            var childTransform = Transform3DHelper.CombineTransform(visual.Transform, transform);
            var model = GetModel(visual);
            if (model != null)
            {
                TraverseModel(model, childTransform, action);
            }
            foreach (var child in GetChildren(visual))
            {
                Traverse(child, childTransform, action);
            }
        }

        /// <summary>
        /// Traverses the Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="action">The action.</param>
        public static void TraverseModel<T>(Model3D model, Action<T, Transform3D> action) where T : Model3D
        {
            TraverseModel(model, Transform3D.Identity, action);
        }

        /// <summary>
        /// Traverses the Model3D tree. Run the specified action for each Model3D.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">The model.</param>
        /// <param name="transform">The transform.</param>
        /// <param name="action">The action.</param>
        public static void TraverseModel<T>(Model3D model, Transform3D transform, Action<T, Transform3D> action)
            where T : Model3D
        {
            var mg = model as Model3DGroup;
            if (mg != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                foreach (var m in mg.Children)
                    TraverseModel(m, childTransform, action);
            }

            var gm = model as T;
            if (gm != null)
            {
                var childTransform = Transform3DHelper.CombineTransform(model.Transform, transform);
                action(gm, childTransform);
            }
        }

        public static T Find<T>(DependencyObject parent) where T : DependencyObject
        {
            // todo: this should be improved
            foreach (DependencyObject d in LogicalTreeHelper.GetChildren(parent))
            {
                var a = Find<T>(d);
                if (a != null) return a;
            }

            var model = parent as ModelVisual3D;
            if (model != null)
            {
                var modelgroup = model.Content as Model3DGroup;
                if (modelgroup != null)
                {
                    return modelgroup.Children.OfType<T>().FirstOrDefault();
                }
            }
            return null;
        }
    }
}