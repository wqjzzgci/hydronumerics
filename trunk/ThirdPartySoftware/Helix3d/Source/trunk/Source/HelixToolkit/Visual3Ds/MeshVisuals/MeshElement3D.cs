using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// The <see cref="MeshElement3D"/> is a base class that contains a <see cref="GeometryModel3D"/> and 
    /// front/back <see cref="Material"/>s.
    /// Derived classes should override the Tesselate() method to generate the geometry.
    /// </summary>
    public abstract class MeshElement3D : ModelVisual3D
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof (Brush), typeof (MeshElement3D),
                                        new UIPropertyMetadata(null, FillChanged));

        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof (Material), typeof (MeshElement3D),
                                        new UIPropertyMetadata(null, MaterialChanged));

        public static readonly DependencyProperty BackMaterialProperty =
            DependencyProperty.Register("BackMaterial", typeof (Material), typeof (MeshElement3D),
                                        new UIPropertyMetadata(null, MaterialChanged));

        private readonly object _invalidateLock = "";
        private bool _doUpdates = true;
        private bool _isInvalidated;

        public MeshElement3D()
        {
            Content = new GeometryModel3D();
            CompositionTarget.Rendering += CompositionTarget_Rendering;

            InvalidateModel();
        }

        public Brush Fill
        {
            get { return (Brush) GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Material Material
        {
            get { return (Material) GetValue(MaterialProperty); }
            set { SetValue(MaterialProperty, value); }
        }

        public Material BackMaterial
        {
            get { return (Material) GetValue(BackMaterialProperty); }
            set { SetValue(BackMaterialProperty, value); }
        }

        public GeometryModel3D Model
        {
            get { return Content as GeometryModel3D; }
        }

        private static void FillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var el = (MeshElement3D) d;
            el.Material = MaterialHelper.CreateMaterial(el.Fill);
            el.BackMaterial = el.Material;
        }


        protected static void GeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D) d).GeometryChanged();
        }

        protected static void MaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MeshElement3D) d).MaterialChanged();
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            lock (_invalidateLock)
            {
                if (_isInvalidated)
                {
                    _isInvalidated = false;
                    GeometryChanged();
                    MaterialChanged();
                }
            }
        }

        private void InvalidateModel()
        {
            lock (_invalidateLock)
            {
                _isInvalidated = true;
            }
        }

        /// <summary>
        /// Forces an update to the geometry model and materials
        /// </summary>
        public void UpdateModel()
        {
            GeometryChanged();
            MaterialChanged();
        }

        protected void MaterialChanged()
        {
            if (!_doUpdates)
                return;

            GeometryModel3D model = Model;
            if (model == null)
                return;

            if (Material == null)
            {
                // use a default blue material
                model.Material = MaterialHelper.CreateMaterial(Brushes.Blue);
            }
            else
                model.Material = Material;

            // the back material may be null (invisible)
            model.BackMaterial = BackMaterial;
        }

        protected void GeometryChanged()
        {
            if (!_doUpdates)
                return;

            Model.Geometry = Tessellate();
        }

        public void DisableUpdates()
        {
            _doUpdates = false;
        }

        public void EnableUpdates()
        {
            _doUpdates = true;
        }


        /// <summary>
        /// Do the tesselation and return the <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns></returns>
        protected abstract MeshGeometry3D Tessellate();

        // alternative:
        /*
        private MeshGeometry3D Tessellate()
        {
            var mesh = new MeshGeometry3D();
            var positions = mesh.Positions;
            var normals = mesh.Normals;
            var textureCoordinates = mesh.TextureCoordinates;
            var triangleIndices = mesh.TriangleIndices;
            mesh.Positions = null;
            mesh.Normals = null;
            mesh.TextureCoordinates = null;
            mesh.TriangleIndices = null;

            Tessellate(positions, normals, textureCoordinates, triangleIndices);

            mesh.Positions = positions;
            mesh.Normals = normals;
            mesh.TextureCoordinates = textureCoordinates;
            mesh.TriangleIndices = triangleIndices;
    
        }

        protected abstract void Tessellate(Point3DCollection points, Vector3DCollection normals, PointCollection textureCoordinates, Int32Collection triangleIndices);
        */
    }
}