using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

// http://linas.org/gle/
// http://sharpmap.codeplex.com/Thread/View.aspx?ThreadId=18864

namespace HelixToolkit
{
    /// <summary>
    /// Extrude a section along a path
    /// </summary>
    public class ExtrudedVisual3D : MeshElement3D
    {
        public static readonly DependencyProperty IsSectionClosedProperty =
            DependencyProperty.Register("IsSectionClosed", typeof(bool), typeof(PipeVisual3D),
                                        new UIPropertyMetadata(true, GeometryChanged));

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(Point3DCollection), typeof(ExtrudedVisual3D),
                                        new UIPropertyMetadata(new Point3DCollection(), GeometryChanged));

        public static readonly DependencyProperty SectionProperty =
            DependencyProperty.Register("Section", typeof(PointCollection), typeof(ExtrudedVisual3D),
                                        new UIPropertyMetadata(new PointCollection(), GeometryChanged));

        public static readonly DependencyProperty UpVectorProperty =
            DependencyProperty.Register("UpVector", typeof(Vector3D), typeof(PipeVisual3D),
                                        new UIPropertyMetadata(new Vector3D(0, 0, 1), GeometryChanged));

        public static readonly DependencyProperty IsPathClosedProperty =
            DependencyProperty.Register("IsPathClosed", typeof(bool), typeof(ExtrudedVisual3D),
                                        new UIPropertyMetadata(false, GeometryChanged));

        public Point3DCollection Path
        {
            get { return (Point3DCollection)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
        
        public PointCollection Section
        {
            get { return (PointCollection)GetValue(SectionProperty); }
            set { SetValue(SectionProperty, value); }
        }
        
        public Vector3D UpVector
        {
            get { return (Vector3D)GetValue(UpVectorProperty); }
            set { SetValue(UpVectorProperty, value); }
        }

        public bool IsSectionClosed
        {
            get { return (bool)GetValue(IsSectionClosedProperty); }
            set { SetValue(IsSectionClosedProperty, value); }
        }

        public bool IsPathClosed
        {
            get { return (bool)GetValue(IsPathClosedProperty); }
            set { SetValue(IsPathClosedProperty, value); }
        }
        
        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder();
            builder.AddTube(Path, null, null, Section, IsPathClosed, IsSectionClosed);
            return builder.ToMesh();
        }
    }
}