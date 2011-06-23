using System;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    public static class ModelImporter
    {
        public static Model3DGroup Load(string path)
        {
            if (path == null)
                return null;
            Model3DGroup model = null;
            string ext = System.IO.Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".3ds":
                    {
                        var r = new StudioReader();
                        model = r.Read(path);
                        break;
                    }
                case ".lwo":
                    {
                        var r = new LwoReader();
                        model = r.Read(path);
                        break;
                    }
                case ".obj":
                    {
                        var r = new ObjReader();
                        model = r.Read(path);
                        break;
                    }
                case ".objz":
                    {
                        var r = new ObjReader();
                        model = r.ReadZ(path);
                        break;
                    }
                case ".stl":
                    {
                        var r = new StLReader();
                        model = r.Read(path);
                        break;
                    }
                default:
                    throw new InvalidOperationException("File format not supported.");
            }
            return model;
        }
    }
}