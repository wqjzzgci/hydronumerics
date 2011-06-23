using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace HelixToolkit
{
    public static class Viewport3DHelper
    {
        #region Cmera/Viewport info methods based on Charles Petzold "3D programming for Windows"

        #region CameraInfo

        /// <summary>
        /// Obtains the view transform matrix for a camera. (see page 327)
        /// </summary>
        /// <param name="camera">Camera to obtain the ViewMatrix for</param>
        /// <returns>A Matrix3D object with the camera view transform matrix, or a Matrix3D with all zeros if the "camera" is null.</returns>
        /// <exception cref="ApplicationException">if the 'camera' is neither of type MatrixCamera nor ProjectionCamera. </exception>
        public static Matrix3D GetViewMatrix(Camera camera)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera is MatrixCamera)
            {
                return (camera as MatrixCamera).ViewMatrix;
            }

            if (camera is ProjectionCamera)
            {
                // Reflector on: ProjectionCamera.CreateViewMatrix

                var projcam = camera as ProjectionCamera;

                var zAxis = -projcam.LookDirection;
                zAxis.Normalize();

                var xAxis = Vector3D.CrossProduct(projcam.UpDirection, zAxis);
                xAxis.Normalize();

                var yAxis = Vector3D.CrossProduct(zAxis, xAxis);
                var pos = (Vector3D) projcam.Position;

                return new Matrix3D(
                    xAxis.X, yAxis.X, zAxis.X, 0,
                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                    -Vector3D.DotProduct(xAxis, pos),
                    -Vector3D.DotProduct(yAxis, pos),
                    -Vector3D.DotProduct(zAxis, pos), 1);
            }

            throw new ApplicationException("unknown camera type");
        }

        /// <summary>
        /// Projection matrix, page 327-331
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        public static Matrix3D GetProjectionMatrix(Camera camera, double aspectRatio)
        {
            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera is MatrixCamera)
            {
                return (camera as MatrixCamera).ProjectionMatrix;
            }

            if (camera is OrthographicCamera)
            {
                var orthocam = camera as OrthographicCamera;

                double xScale = 2/orthocam.Width;
                double yScale = xScale*aspectRatio;
                double zNear = orthocam.NearPlaneDistance;
                double zFar = orthocam.FarPlaneDistance;

                // Hey, check this out!
                if (Double.IsPositiveInfinity(zFar))
                    zFar = 1E10;

                return new Matrix3D(xScale, 0, 0, 0,
                                    0, yScale, 0, 0,
                                    0, 0, 1/(zNear - zFar), 0,
                                    0, 0, zNear/(zNear - zFar), 1);
            }

            if (camera is PerspectiveCamera)
            {
                var perscam = camera as PerspectiveCamera;

                // The angle-to-radian formula is a little off because only
                // half the angle enters the calculation.
                double xScale = 1/Math.Tan(Math.PI*perscam.FieldOfView/360);
                double yScale = xScale*aspectRatio;
                double zNear = perscam.NearPlaneDistance;
                double zFar = perscam.FarPlaneDistance;
                double zScale = (zFar == double.PositiveInfinity ? -1 : (zFar/(zNear - zFar)));
                double zOffset = zNear*zScale;

                return new Matrix3D(xScale, 0, 0, 0,
                                    0, yScale, 0, 0,
                                    0, 0, zScale, -1,
                                    0, 0, zOffset, 0);
            }

            throw new ApplicationException("unknown camera type");
        }

        /// <summary>
        /// Get the combined view and projection transform
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="aspectRatio"></param>
        /// <returns></returns>
        public static Matrix3D GetTotalTransform(Camera camera, double aspectRatio)
        {
            var m = Matrix3D.Identity;

            if (camera == null)
            {
                throw new ArgumentNullException("camera");
            }

            if (camera.Transform != null)
            {
                var cameraTransform = camera.Transform.Value;

                if (!cameraTransform.HasInverse)
                {
                    throw new ApplicationException("camera transform has no inverse");
                }
                cameraTransform.Invert();
                m.Append(cameraTransform);
            }

            m.Append(GetViewMatrix(camera));
            m.Append(GetProjectionMatrix(camera, aspectRatio));
            return m;
        }


        public static Matrix3D GetInverseTransform(Camera cam, double aspectRatio)
        {
            var m = GetTotalTransform(cam, aspectRatio);

            if (!m.HasInverse)
            {
                throw new ApplicationException("camera transform has no inverse");
            }

            m.Invert();
            return m;
        }

        #endregion

        #region ViewportInfo

        public static Matrix3D GetTotalTransform(Viewport3DVisual vis)
        {
            var m = GetCameraTransform(vis);
            m.Append(GetViewportTransform(vis));
            return m;
        }

        public static Matrix3D GetTotalTransform(Viewport3D viewport)
        {
            var matx = GetCameraTransform(viewport);
            matx.Append(GetViewportTransform(viewport));
            return matx;
        }

        public static Matrix3D GetCameraTransform(Viewport3DVisual vis)
        {
            return GetTotalTransform(vis.Camera,
                                     vis.Viewport.Size.Width/vis.Viewport.Size.Height);
        }

        public static Matrix3D GetCameraTransform(Viewport3D viewport)
        {
            return GetTotalTransform(viewport.Camera,
                                     viewport.ActualWidth/viewport.ActualHeight);
        }

        public static Matrix3D GetViewportTransform(Viewport3DVisual vis)
        {
            return new Matrix3D(vis.Viewport.Width/2, 0, 0, 0,
                                0, -vis.Viewport.Height/2, 0, 0,
                                0, 0, 1, 0,
                                vis.Viewport.X + vis.Viewport.Width/2,
                                vis.Viewport.Y + vis.Viewport.Height/2, 0, 1);
        }

        public static Matrix3D GetViewportTransform(Viewport3D viewport)
        {
            return new Matrix3D(viewport.ActualWidth/2, 0, 0, 0,
                                0, -viewport.ActualHeight/2, 0, 0,
                                0, 0, 1, 0,
                                viewport.ActualWidth/2,
                                viewport.ActualHeight/2, 0, 1);
        }


        public static Point Point3DtoPoint2D(Viewport3D viewport, Point3D point)
        {
            var matrix = GetTotalTransform(viewport);
            var pointTransformed = matrix.Transform(point);
            var pt = new Point(pointTransformed.X, pointTransformed.Y);
            return pt;
        }

        public static Ray3D Point2DtoRay3D(Viewport3D viewport, Point ptIn)
        {
            Point3D pointNear, pointFar;
            if (!Point2DtoPoint3D(viewport, ptIn, out pointNear, out pointFar))
                return null;
            return new Ray3D(pointNear, pointFar);
        }

        public static bool Point2DtoPoint3D(Viewport3D viewport, Point ptIn, out Point3D pointNear, out Point3D pointFar)
        {
            pointNear = new Point3D();
            pointFar = new Point3D();

            var pointIn = new Point3D(ptIn.X, ptIn.Y, 0);
            var matrixViewport = GetViewportTransform(viewport);
            var matrixCamera = GetCameraTransform(viewport);

            if (!matrixViewport.HasInverse)
                return false;

            if (!matrixCamera.HasInverse)
                return false;

            matrixViewport.Invert();
            matrixCamera.Invert();

            var pointNormalized = matrixViewport.Transform(pointIn);
            pointNormalized.Z = 0.01;
            pointNear = matrixCamera.Transform(pointNormalized);
            pointNormalized.Z = 0.99;
            pointFar = matrixCamera.Transform(pointNormalized);

            return true;
        }

        #endregion

        #endregion

        #region Helper methods based on Eric Sink's twelve days of WPF

        // The Twelve Days of WPF 3D
        // http://www.ericsink.com/wpf3d/index.html

        // http://www.ericsink.com/wpf3d/3_Bitmap.html
        public static RenderTargetBitmap RenderBitmap(Viewport3D view, Brush background)
        {
            var bmp = new RenderTargetBitmap(
                (int) view.ActualWidth, (int) view.ActualHeight, 96, 96,
                PixelFormats.Pbgra32);

            // erase background
            var vRect = new Rectangle
                            {
                                Width = view.ActualWidth,
                                Height = view.ActualHeight,
                                Fill = background
                            };
            vRect.Arrange(new Rect(0, 0, vRect.Width, vRect.Height));
            bmp.Render(vRect);

            bmp.Render(view);
            return bmp;
        }

        public static RenderTargetBitmap RenderBitmap(Viewport3D view, double width, double height, Brush background)
        {
            double w = view.Width;
            double h = view.Height;
            ResizeAndArrange(view, width, height);
            var rtb = RenderBitmap(view, background);
            ResizeAndArrange(view, w, h);
            return rtb;
        }

        public static void Copy(Viewport3D view)
        {
            Clipboard.SetImage(RenderBitmap(view, Brushes.White));
        }

        public static void Copy(Viewport3D view, double width, double height, Brush background)
        {
            Clipboard.SetImage(RenderBitmap(view, width, height, background));
        }

        public static void SaveBitmap(Viewport3D view, string fileName)
        {
            var bmp = RenderBitmap(view, Brushes.White);
            BitmapEncoder encoder;
            string ext = Path.GetExtension(fileName);
            switch (ext.ToLower())
            {
                case ".jpg":
                    var jpg = new JpegBitmapEncoder();
                    jpg.Frames.Add(BitmapFrame.Create(bmp));
                    encoder = jpg;
                    break;
                case ".png":
                    var png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(bmp));
                    encoder = png;
                    break;
                default:
                    throw new InvalidOperationException("Not supported file format.");
            }
            using (Stream stm = File.Create(fileName))
            {
                encoder.Save(stm);
            }
        }

        public static void ResizeAndArrange(Viewport3D view, double width, double height)
        {
            view.Width = width;
            view.Height = height;
            if (double.IsNaN(width) || double.IsNaN(height))
                return;
            view.Measure(new Size(width, height));
            view.Arrange(new Rect(0, 0, width, height));
        }

        // http://www.ericsink.com/wpf3d/7_XAML.html
        public static void CopyXaml(Viewport3D view)
        {
            Clipboard.SetText(XamlWriter.Save(view));
        }


        // http://www.ericsink.com/wpf3d/A_AutoZoom.html
        /*        public static Rect Get2DBoundingBox(Viewport3D vp)
                {
                    bool bOK;

                    Viewport3DVisual vpv =
                        VisualTreeHelper.GetParent(
                            vp.Children[0]) as Viewport3DVisual;

                    Matrix3D m = _3DTools.MathUtils.TryWorldToViewportTransform(vpv, out bOK);

                    bool bFirst = true;
                    Rect r = new Rect();

                    foreach (Visual3D v3d in vp.Children)
                    {
                        if (v3d is ModelVisual3D)
                        {
                            ModelVisual3D mv3d = (ModelVisual3D)v3d;
                            if (mv3d.Content is GeometryModel3D)
                            {
                                GeometryModel3D gm3d =
                                    (GeometryModel3D)mv3d.Content;

                                if (gm3d.Geometry is MeshGeometry3D)
                                {
                                    MeshGeometry3D mg3d =
                                        (MeshGeometry3D)gm3d.Geometry;

                                    foreach (Point3D p3d in mg3d.Positions)
                                    {
                                        Point3D pb = m.Transform(p3d);
                                        Point p2d = new Point(pb.X, pb.Y);
                                        if (bFirst)
                                        {
                                            r = new Rect(p2d, new Size(1, 1));
                                            bFirst = false;
                                        }
                                        else
                                        {
                                            r.Union(p2d);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return r;
                }

              
                */


        public static void Print(Viewport3D vp, string description)
        {
            var dlg = new PrintDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                dlg.PrintVisual(vp, description);
            }
        }

        #endregion

        public static void Export(Viewport3D view, string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".png":
                    SaveBitmap(view, fileName);
                    break;
                case ".xaml":
                    ExportXaml(view, fileName);
                    break;
                case ".xml":
                    ExportKerkythea(view, fileName);
                    break;
                case ".obj":
                    ExportObj(view, fileName);
                    break;
                case ".x3d":
                    ExportX3d(view, fileName);
                    break;
                default:
                    throw new InvalidOperationException("Not supported file format.");
            }
        }

        private static void ExportX3d(Viewport3D view, string fileName)
        {
            var e = new X3DExporter(fileName);
            e.Export(view);
            e.Close();
        }

        private static void ExportObj(Viewport3D view, string fileName)
        {
            var e = new ObjExporter(fileName);
            e.Export(view);
            e.Close();
        }

        private static void ExportKerkythea(Viewport3D view, string fileName)
        {
            ExportKerkythea(view, fileName, Colors.White, (int) view.ActualWidth, (int) view.ActualHeight);
        }

        private static void ExportKerkythea(Viewport3D view, string fileName, Color background, int width, int height)
        {
            var e = new KerkytheaExporter(fileName) {Width = width, Height = height, BackgroundColor = background};
            e.Export(view);
            e.Close();
        }

        private static void ExportXaml(Viewport3D view, string fileName)
        {
            var e = new XamlExporter(fileName);
            e.Export(view);
            e.Close();
        }

        /// <summary>
        /// Get all lights in the Viewport3D
        /// </summary>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public static Light[] GetLights(Viewport3D viewport)
        {
            var models = SearchFor<Light>(viewport.Children);
            return models.Select(m => m as Light).ToArray();
        }

        /// <summary>
        /// Recursive search in a collection for objects of given type T 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<Model3D> SearchFor<T>(IEnumerable<Visual3D> collection)
        {
            var output = new List<Model3D>();
            SearchFor(collection, typeof (T), output);
            return output;
        }

        /// <summary>
        /// Recursive search for an object of a given type
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="type"></param>
        /// <param name="output"></param>
        private static void SearchFor(IEnumerable<Visual3D> collection, Type type, ICollection<Model3D> output)
        {
            foreach (var visual in collection)
            {
                var modelVisual = visual as ModelVisual3D;
                if (modelVisual != null)
                {
                    var model = modelVisual.Content;
                    if (model != null)
                    {
                        if (type.IsAssignableFrom(model.GetType()))
                            output.Add(model);

                        // recursive
                        SearchFor(modelVisual.Children, type, output);
                    }
                    var modelGroup = model as Model3DGroup;
                    if (modelGroup != null)
                    {
                        SearchFor(modelGroup.Children, type, output);
                    }
                }
            }
        }

        private static void SearchFor(IEnumerable<Model3D> collection, Type type, ICollection<Model3D> output)
        {
            foreach (var model in collection)
            {
                if (type.IsAssignableFrom(model.GetType()))
                    output.Add(model);

                var group = model as Model3DGroup;
                if (group != null)
                {
                    SearchFor(group.Children, type, output);
                }
            }
        }

        /// <summary>
        /// Find the Visual3D that is nearest given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Visual3D FindNearestVisual(Viewport3D viewport, Point position)
        {
            Point3D p;
            Vector3D n;
            DependencyObject obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
                return obj as Visual3D;

            return null;
        }

        /// <summary>
        /// Find the coordinates of the nearest point given a 2D position in the viewport
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Point3D? FindNearestPoint(Viewport3D viewport, Point position)
        {
            Point3D p;
            Vector3D n;
            DependencyObject obj;
            if (FindNearest(viewport, position, out p, out n, out obj))
                return p;

            return null;
        }

        public static bool FindNearest(Viewport3D viewport, Point position, out Point3D point, out Vector3D normal,
                                       out DependencyObject visual)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
            {
                point = new Point3D();
                normal = new Vector3D();
                visual = null;
                return false;
            }

            var hitParams = new PointHitTestParameters(position);

            double minimumDistance = double.MaxValue;
            var nearestPoint = new Point3D();
            var nearestNormal = new Vector3D();
            DependencyObject nearestObject = null;

            VisualTreeHelper.HitTest(viewport, null, delegate(HitTestResult hit)
                                                         {
                                                             var rayHit = hit as RayMeshGeometry3DHitTestResult;
                                                             if (rayHit != null)
                                                             {
                                                                 var mesh = rayHit.MeshHit;
                                                                 if (mesh != null)
                                                                 {
                                                                     var p1 = mesh.Positions[rayHit.VertexIndex1];
                                                                     var p2 = mesh.Positions[rayHit.VertexIndex2];
                                                                     var p3 = mesh.Positions[rayHit.VertexIndex3];
                                                                     double x = p1.X*rayHit.VertexWeight1 +
                                                                                p2.X*rayHit.VertexWeight2 +
                                                                                p3.X*rayHit.VertexWeight3;
                                                                     double y = p1.Y*rayHit.VertexWeight1 +
                                                                                p2.Y*rayHit.VertexWeight2 +
                                                                                p3.Y*rayHit.VertexWeight3;
                                                                     double z = p1.Z*rayHit.VertexWeight1 +
                                                                                p2.Z*rayHit.VertexWeight2 +
                                                                                p3.Z*rayHit.VertexWeight3;

                                                                     // point in local coordinates
                                                                     var p = new Point3D(x, y, z);

                                                                     // transform to global coordinates

                                                                     // first transform the Model3D hierarchy
                                                                     var t2 = GetTransform(rayHit.VisualHit,
                                                                                           rayHit.ModelHit);
                                                                     if (t2 != null)
                                                                         p = t2.Transform(p);

                                                                     // then transform the Visual3D hierarchy up to the Viewport3D ancestor
                                                                     var t = GetTransform(viewport, rayHit.VisualHit);
                                                                     if (t != null)
                                                                         p = t.Transform(p);

                                                                     double distance =
                                                                         (camera.Position - p).LengthSquared;
                                                                     if (distance < minimumDistance)
                                                                     {
                                                                         minimumDistance = distance;
                                                                         nearestPoint = p;
                                                                         nearestNormal = Vector3D.CrossProduct(p2 - p1,
                                                                                                               p3 - p1);
                                                                         nearestObject = hit.VisualHit;
                                                                     }
                                                                 }
                                                             }
                                                             return HitTestResultBehavior.Continue;
                                                         }, hitParams);

            point = nearestPoint;
            visual = nearestObject;
            normal = nearestNormal;

            if (minimumDistance == double.MaxValue)
                return false;

            normal.Normalize();
            return true;
        }

        public class HitResult
        {
            public Vector3D Normal { get; set; }
            public Point3D Position { get; set; }
            public double Distance { get; set; }
            public RayMeshGeometry3DHitTestResult RayHit { get; set; }

            public MeshGeometry3D Mesh
            {
                get { return RayHit.MeshHit; }
            }

            public Model3D Model
            {
                get { return RayHit.ModelHit; }
            }

            public Visual3D Visual
            {
                get { return RayHit.VisualHit; }
            }
        }

        /// <summary>
        /// Finds the hits for a given 2D viewport position.
        /// </summary>
        /// <param name="viewport">The viewport.</param>
        /// <param name="position">The position.</param>
        /// <returns>List of hits, sorted with the nearest hit first.</returns>
        public static List<HitResult> FindHits(Viewport3D viewport, Point position)
        {
            var camera = viewport.Camera as ProjectionCamera;
            if (camera == null)
                return null;

            var result = new List<HitResult>();
            HitTestResultCallback callback = hit =>
                                                 {
                                                     var rayHit = hit as RayMeshGeometry3DHitTestResult;
                                                     if (rayHit != null)
                                                     {
                                                         if (rayHit.MeshHit != null)
                                                         {
                                                             var p = GetGlobalHitPosition(rayHit, viewport);
                                                             var nn = GetNormalHit(rayHit);
                                                             var n = nn.HasValue ? nn.Value : new Vector3D(0, 0, 1);

                                                             result.Add(new HitResult
                                                                            {
                                                                                Distance = (camera.Position - p).Length,
                                                                                RayHit = rayHit,
                                                                                Normal = n,
                                                                                Position = p
                                                                            });
                                                         }
                                                     }
                                                     return HitTestResultBehavior.Continue;
                                                 };

            var hitParams = new PointHitTestParameters(position);
            VisualTreeHelper.HitTest(viewport, null, callback, hitParams);

            result.OrderBy(k => k.Distance);
            return result;
        }

        /// <summary>
        /// Gets the hit position transformed to global (viewport) coordinates.
        /// </summary>
        /// <param name="rayHit">The hit structure.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        private static Point3D GetGlobalHitPosition(RayMeshGeometry3DHitTestResult rayHit, Viewport3D viewport)
        {
            var p = rayHit.PointHit;

            // first transform the Model3D hierarchy
            var t2 = GetTransform(rayHit.VisualHit,
                                  rayHit.ModelHit);
            if (t2 != null)
                p = t2.Transform(p);

            // then transform the Visual3D hierarchy up to the Viewport3D ancestor
            var t = GetTransform(viewport,
                                 rayHit.VisualHit);
            if (t != null)
                p = t.Transform(p);
            return p;
        }

        /// <summary>
        /// Gets the normal for a hit test result.
        /// </summary>
        /// <param name="rayHit">The ray hit.</param>
        /// <returns></returns>
        private static Vector3D? GetNormalHit(RayMeshGeometry3DHitTestResult rayHit)
        {
            if ( (rayHit.MeshHit.Normals == null) || (rayHit.MeshHit.Normals.Count < 1) )
                return null;

            return rayHit.MeshHit.Normals[rayHit.VertexIndex1]*rayHit.VertexWeight1 +
                   rayHit.MeshHit.Normals[rayHit.VertexIndex2]*rayHit.VertexWeight2 +
                   rayHit.MeshHit.Normals[rayHit.VertexIndex3]*rayHit.VertexWeight3;
        }

        /// <summary>
        /// Get the total transform of a Visual3D
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="visual"></param>
        /// <returns></returns>
        public static GeneralTransform3D GetTransform(Viewport3D viewport, Visual3D visual)
        {
            if (visual == null)
                return null;
            foreach (var ancestor in viewport.Children)
            {
                if (visual.IsDescendantOf(ancestor))
                {
                    var g = new GeneralTransform3DGroup();

                    // this includes the visual.Transform
                    var ta = visual.TransformToAncestor(ancestor);
                    if (ta != null)
                        g.Children.Add(ta);

                    // add the transform of the top-level ancestor
                    g.Children.Add(ancestor.Transform);

                    return g;
                }
            }
            return visual.Transform;
        }

        /// <summary>
        /// Gets the transform from the specified Visual3D to the Model3D.
        /// </summary>
        /// <param name="visual">The source visual.</param>
        /// <param name="model">The target model.</param>
        /// <returns></returns>
        public static GeneralTransform3D GetTransform(Visual3D visual, Model3D model)
        {
            var mv = visual as ModelVisual3D;
            if (mv != null)
                return GetTransform(mv.Content, model, Transform3D.Identity);
            return null;
        }

        private static GeneralTransform3D GetTransform(Model3D current, Model3D model, Transform3D parentTransform)
        {
            var currentTransform = Transform3DHelper.CombineTransform(current.Transform, parentTransform);
            if (current == model)
                return currentTransform;
            var mg = current as Model3DGroup;
            if (mg != null)
            {
                foreach (var m in mg.Children)
                {
                    var result = GetTransform(m, model, currentTransform);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}