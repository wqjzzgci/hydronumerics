using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// 3DStudio file reader
    /// </summary>
    public class StudioReader : IModelReader
    {
        #region 3DS Chunk IDs

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Local
        private enum ChunkID
        {
            // Primary chunk

            MAIN3DS = 0x4D4D,

            // Main Chunks

            EDIT3DS = 0x3D3D, // this is the start of the editor config
            KEYF3DS = 0xB000, // this is the start of the keyframer config
            VERSION = 0x0002,
            MESHVERSION = 0x3D3E,

            // sub defines of EDIT3DS

            EDIT_MATERIAL = 0xAFFF,
            EDIT_CONFIG1 = 0x0100,
            EDIT_CONFIG2 = 0x3E3D,
            EDIT_VIEW_P1 = 0x7012,
            EDIT_VIEW_P2 = 0x7011,
            EDIT_VIEW_P3 = 0x7020,
            EDIT_VIEW1 = 0x7001,
            EDIT_BACKGR = 0x1200,
            EDIT_AMBIENT = 0x2100,
            EDIT_OBJECT = 0x4000,

            EDIT_UNKNW01 = 0x1100,
            EDIT_UNKNW02 = 0x1201,
            EDIT_UNKNW03 = 0x1300,
            EDIT_UNKNW04 = 0x1400,
            EDIT_UNKNW05 = 0x1420,
            EDIT_UNKNW06 = 0x1450,
            EDIT_UNKNW07 = 0x1500,
            EDIT_UNKNW08 = 0x2200,
            EDIT_UNKNW09 = 0x2201,
            EDIT_UNKNW10 = 0x2210,
            EDIT_UNKNW11 = 0x2300,
            EDIT_UNKNW12 = 0x2302,
            EDIT_UNKNW13 = 0x3000,
            EDIT_UNKNW14 = 0xAFFF,

            // sub defines of EDIT_MATERIAL
            MAT_NAME01 = 0xA000,
            MAT_LUMINANCE = 0xA010,
            MAT_DIFFUSE = 0xA020,
            MAT_SPECULAR = 0xA030,
            MAT_SHININESS = 0xA040,
            MAT_MAP = 0xA200,
            MAT_MAPFILE = 0xA300,

            // sub defines of EDIT_OBJECT
            OBJ_TRIMESH = 0x4100,
            OBJ_LIGHT = 0x4600,
            OBJ_CAMERA = 0x4700,

            OBJ_UNKNWN01 = 0x4010,
            OBJ_UNKNWN02 = 0x4012, // Could be shadow

            // sub defines of OBJ_CAMERA
            CAM_UNKNWN01 = 0x4710,
            CAM_UNKNWN02 = 0x4720,

            // sub defines of OBJ_LIGHT
            LIT_OFF = 0x4620,
            LIT_SPOT = 0x4610,
            LIT_UNKNWN01 = 0x465A,

            // sub defines of OBJ_TRIMESH
            TRI_VERTEXL = 0x4110,
            TRI_FACEL2 = 0x4111,
            TRI_FACEL1 = 0x4120,
            TRI_FACEMAT = 0x4130,
            TRI_TEXCOORD = 0x4140,
            TRI_SMOOTH = 0x4150,
            TRI_LOCAL = 0x4160,
            TRI_VISIBLE = 0x4165,

            // sub defs of KEYF3DS

            KEYF_UNKNWN01 = 0xB009,
            KEYF_UNKNWN02 = 0xB00A,
            KEYF_FRAMES = 0xB008,
            KEYF_OBJDES = 0xB002,
            KEYF_HIERARCHY = 0xB030,
            KFNAME = 0xB010,

            //  these define the different color chunk types
            COL_RGB = 0x0010,
            COL_TRU = 0x0011, // RGB24
            COL_UNK = 0x0013,

            // defines for viewport chunks

            TOP = 0x0001,
            BOTTOM = 0x0002,
            LEFT = 0x0003,
            RIGHT = 0x0004,
            FRONT = 0x0005,
            BACK = 0x0006,
            USER = 0x0007,
            CAMERA = 0x0008, // = 0xFFFF is the actual code read from file
            LIGHT = 0x0009,
            DISABLED = 0x0010,
            BOGUS = 0x0011,
        }

        // ReSharper restore UnusedMember.Local
        // ReSharper restore InconsistentNaming

        #endregion

        // http://faydoc.tripod.com/formats/3ds.htm
        // http://www.gametutorials.com
        // http://code.google.com/p/lib3ds/
        // Visual3D vs Model3D: http://blogs.msdn.com/b/danlehen/archive/2005/10/09/478923.aspx

        private readonly Dictionary<string, Material> Materials = new Dictionary<string, Material>();

        private BinaryReader reader { get; set; }
        public string TexturePath { get; set; }
        private Model3DGroup Model { get; set; }

        public Model3DGroup Read(string path)
        {
            TexturePath = Path.GetDirectoryName(path);
            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            Model3DGroup model = Read(s);
            s.Close();
            return model;
        }

        public Model3DGroup Read(Stream s)
        {
            reader = new BinaryReader(s);

            long length = reader.BaseStream.Length;

            // http://gpwiki.org/index.php/Loading_3ds_files
            // http://www.flipcode.com/archives/3DS_File_Loader.shtml
            // http://sandy.googlecode.com/svn/trunk/sandy/as3/branches/3.0.2/src/sandy/parser/Parser3DS.as

            ChunkID headerID = ReadChunkId();
            if (headerID != ChunkID.MAIN3DS)
                throw new FileFormatException("Unknown file");
            int headerSize = ReadChunkSize();
            if (headerSize != length)
                throw new FileFormatException("Incomplete file (file length does not match header)");

            Model = new Model3DGroup();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                ChunkID id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine(id);

                switch (id)
                {
                    case ChunkID.EDIT_MATERIAL:
                        ReadMaterial(size);
                        break;
                    case ChunkID.EDIT_OBJECT:
                        ReadObject(size);
                        break;
                    case ChunkID.EDIT3DS:
                    case ChunkID.OBJ_CAMERA:
                    case ChunkID.OBJ_LIGHT:
                    case ChunkID.OBJ_TRIMESH:
                        // don't read the whole chunk, read the sub-defines...
                        break;

                    default:
                        // download the whole chunk
                        byte[] bytes = ReadData(size - 6);
                        break;
                }
            }

            reader.Close();
            return Model;
        }

        private int ReadChunkSize()
        {
            return (int) reader.ReadUInt32();
        }

        private ChunkID ReadChunkId()
        {
            return (ChunkID) reader.ReadUInt16();
        }

        private string ReadString()
        {
            var sb = new StringBuilder();
            while (true)
            {
                var ch = (char) reader.ReadByte();
                if (ch == 0)
                    break;
                sb.Append(ch);
            }
            return sb.ToString();
        }

        /* private string ReadString(int size)
         {
             var bytes = reader.ReadBytes(size);
             var enc = new ASCIIEncoding();
             var s = enc.GetString(bytes);
             return s.Trim('\0');
         }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">Excluding header size</param>
        /// <returns></returns>
        private byte[] ReadData(int size)
        {
            return reader.ReadBytes(size);
        }

        private void ReadObject(int msize)
        {
            int total = 6;

            string objectName = ReadString();
            total += objectName.Length + 1;

            while (total < msize)
            {
                ChunkID id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine(id);
                total += size;
                switch (id)
                {
                    case ChunkID.OBJ_TRIMESH:
                        ReadTriangularMesh(size);
                        break;
                        // case ChunkID.OBJ_CAMERA:
                    default:
                        {
                            byte[] bytes = ReadData(size - 6);
                            break;
                        }
                }
            }
        }

        private void ReadTriangularMesh(int chunkSize)
        {
            int bytesRead = 6;
            Point3DCollection vertices = null;
            Int32Collection faces = null;
            PointCollection texcoords = null;
            Matrix3D matrix = Matrix3D.Identity;
            List<FaceMaterial> facemat = null;

            while (bytesRead < chunkSize)
            {
                ChunkID id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine(" " + id);
                bytesRead += size;
                switch (id)
                {
                    case ChunkID.TRI_VERTEXL:
                        vertices = ReadVertexList();
                        break;
                    case ChunkID.TRI_FACEL1:
                        faces = ReadFaceList();
                        size -= (faces.Count/3*8 + 2);
                        facemat = ReadFaceMaterials(size - 6);
                        break;
                    case ChunkID.TRI_TEXCOORD:
                        texcoords = ReadTexCoords();
                        break;
                    case ChunkID.TRI_LOCAL:
                        matrix = ReadTransformation();
                        break;

                    default:
                        ReadData(size - 6);
                        break;
                }
            }

            if (facemat == null)
            {
                return;
            }
            // if (!matrix.IsIdentity)
            /*                for (int i = 0; i < vertices.Count; i++)
                            {
                                vertices[i] = DoTransform(matrix, vertices[i]);
                            }*/

            foreach (FaceMaterial fm in facemat)
            {
                var m = new MeshGeometry3D {Positions = vertices};
                var faces2 = new Int32Collection(fm.Faces.Count*3);
                foreach (int f in fm.Faces)
                {
                    faces2.Add(faces[f*3]);
                    faces2.Add(faces[f*3 + 1]);
                    faces2.Add(faces[f*3 + 2]);
                }
                m.TriangleIndices = faces2;
                m.TextureCoordinates = texcoords;
                var model = new GeometryModel3D {Geometry = m};
                if (Materials.ContainsKey(fm.Name))
                {
                    model.Material = Materials[fm.Name];
                    model.BackMaterial = model.Material;
                }
                else
                {
                    // use default material
                    //  MaterialHelper.CreateMaterial(Brushes.Brown);
                }

                Model.Children.Add(model);
            }
        }


        private List<FaceMaterial> ReadFaceMaterials(int msize)
        {
            int total = 6;
            var list = new List<FaceMaterial>();
            while (total < msize)
            {
                ChunkID id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine(id);
                total += size;
                switch (id)
                {
                    case ChunkID.TRI_FACEMAT:
                        {
                            string name = ReadString();
                            int n = reader.ReadUInt16();
                            var c = new Int32Collection();
                            for (int i = 0; i < n; i++)
                            {
                                c.Add(reader.ReadUInt16());
                            }
                            var fm = new FaceMaterial {Name = name, Faces = c};
                            list.Add(fm);
                            break;
                        }
                    case ChunkID.TRI_SMOOTH:
                        {
                            byte[] bytes = ReadData(size - 6);
                            break;
                        }
                    default:
                        {
                            byte[] bytes = ReadData(size - 6);
                            break;
                        }
                }
            }
            return list;
        }

        private void ReadMaterial(int msize)
        {
            int total = 6;
            string name = null;

            Color luminance = Colors.Transparent;
            Color diffuse = Colors.Transparent;
            Color specular = Colors.Transparent;
            Color shininess = Colors.Transparent;
            string texture = null;

            while (total < msize)
            {
                ChunkID id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine(id);
                total += size;

                switch (id)
                {
                    case ChunkID.MAT_NAME01:
                        name = ReadString();
                        // name = ReadString(size - 6);
                        break;

                    case ChunkID.MAT_LUMINANCE:
                        luminance = ReadColor(size);
                        break;

                    case ChunkID.MAT_DIFFUSE:
                        diffuse = ReadColor(size);
                        break;

                    case ChunkID.MAT_SPECULAR:
                        specular = ReadColor(size);
                        break;

                    case ChunkID.MAT_SHININESS:
                        byte[] bytes = ReadData(size - 6);
                        // shininess = ReadColor(r, size);
                        break;

                    case ChunkID.MAT_MAP:
                        texture = ReadMatMap(size - 6);
                        break;

                    case ChunkID.MAT_MAPFILE:
                        ReadData(size - 6);
                        break;

                    default:
                        ReadData(size - 6);
                        break;
                }
            }
            int specularPower = 100;
            var mg = new MaterialGroup();
            //mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(luminance)));
            if (texture != null)
            {
                Debug.WriteLine("Loading " + texture);
                string ext = Path.GetExtension(texture).ToLower();

                // TGA not supported - convert textures to .png!
                if (ext == ".tga")
                    texture = Path.ChangeExtension(texture, ".png");
                if (ext == ".bmp")
                    texture = Path.ChangeExtension(texture, ".jpg");

                string path = Path.Combine(TexturePath, texture);
                if (File.Exists(path))
                {
                    var img = new BitmapImage(new Uri(path, UriKind.Relative));
                    var textureBrush = new ImageBrush(img);
                    mg.Children.Add(new DiffuseMaterial(textureBrush));
                }
                else
                {
                    Debug.WriteLine(String.Format("Texture {0} not found in {1}", texture, TexturePath));
                    mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(diffuse)));
                }
            }
            else
            {
                mg.Children.Add(new DiffuseMaterial(new SolidColorBrush(diffuse)));
            }
            mg.Children.Add(new SpecularMaterial(new SolidColorBrush(specular), specularPower));

            if (name != null)
                Materials.Add(name, mg);
        }

        private string ReadMatMap(int size)
        {
            ChunkID id = ReadChunkId();
            int siz = ReadChunkSize();
            ushort f1 = reader.ReadUInt16();
            ushort f2 = reader.ReadUInt16();
            ushort f3 = reader.ReadUInt16();
            ushort f4 = reader.ReadUInt16();
            size -= 14;
            string cname = ReadString();
            size -= cname.Length + 1;
            byte[] morebytes = ReadData(size);
            return cname;
        }

        private Color ReadColor(int size)
        {
            // var bb = ReadData(reader, size - 6);
            // return Colors.White;

            ChunkID type = ReadChunkId();
            int csize = ReadChunkSize();
            size -= 6;
            switch (type)
            {
                case ChunkID.COL_RGB:
                    {
                        // not checked...
                        Debug.Assert(false);
                        float r = reader.ReadSingle();
                        float g = reader.ReadSingle();
                        float b = reader.ReadSingle();
                        return Color.FromScRgb(1, r, g, b);
                    }
                case ChunkID.COL_TRU:
                    {
                        byte r = reader.ReadByte();
                        byte g = reader.ReadByte();
                        byte b = reader.ReadByte();
                        return Color.FromArgb(0xFF, r, g, b);
                    }
                default:
                    ReadData(csize);
                    break;
            }
            return Colors.White;
        }

        private Point3DCollection ReadVertexList()
        {
            int size = reader.ReadUInt16();
            var pts = new Point3DCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                pts.Add(new Point3D(x, y, z));
            }
            return pts;
        }

        private Int32Collection ReadFaceList()
        {
            int size = reader.ReadUInt16();
            var faces = new Int32Collection(size*3);
            for (int i = 0; i < size; i++)
            {
                faces.Add(reader.ReadUInt16());
                faces.Add(reader.ReadUInt16());
                faces.Add(reader.ReadUInt16());
                float flags = reader.ReadUInt16();
            }
            return faces;
        }

        private PointCollection ReadTexCoords()
        {
            int size = reader.ReadUInt16();
            var pts = new PointCollection(size);
            for (int i = 0; i < size; i++)
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                pts.Add(new Point(x, 1 - y));
            }
            return pts;
        }

        private Vector3D ReadVector()
        {
            return new Vector3D(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        private Matrix3D ReadTransformation()
        {
            Vector3D localx = ReadVector();
            Vector3D localy = ReadVector();
            Vector3D localz = ReadVector();
            Vector3D origin = ReadVector();

            var matrix = new Matrix3D();

            matrix.M11 = localx.X;
            matrix.M21 = localx.Y;
            matrix.M31 = localx.Z;

            matrix.M12 = localy.X;
            matrix.M22 = localy.Y;
            matrix.M32 = localy.Z;

            matrix.M13 = localz.X;
            matrix.M23 = localz.Y;
            matrix.M33 = localz.Z;

            matrix.OffsetX = origin.X;
            matrix.OffsetY = origin.Y;
            matrix.OffsetZ = origin.Z;

            matrix.M14 = 0;
            matrix.M24 = 0;
            matrix.M34 = 0;
            matrix.M44 = 1;

            return matrix;
        }

        #region Nested type: FaceMaterial

        private class FaceMaterial
        {
            public string Name { get; set; }
            public Int32Collection Faces { get; set; }
        }

        #endregion
    }
}