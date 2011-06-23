using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit
{
    /// <summary>
    /// LWO (Lightwave object) file reader
    /// http://www.martinreddy.net/gfx/3d/LWOB.txt
    /// http://www.modwiki.net/wiki/LWO_(file_format)
    /// http://www.newtek.com/lightwave/developers.php
    /// </summary>
    public class LwoReader : IModelReader
    {

        private BinaryReader reader;
        private Point3DCollection Points { get; set; }
        public string TexturePath { get; set; }

        public Collection<string> Surfaces { get; private set; }
        public Collection<MeshBuilder> Meshes { get; private set; }
        public Collection<Material> Materials { get; private set; }

        public Model3DGroup Read(string path)
        {
            var texturePath = Path.GetDirectoryName(path);
            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            TexturePath = texturePath;
            var result = Read(s);
            s.Close();
            return result;
        }

        public Model3DGroup Read(Stream s)
        {
            reader = new BinaryReader(s);

            long length = reader.BaseStream.Length;

            string headerID = ReadChunkId();
            if (headerID != "FORM")
                throw new FileFormatException("Unknown file");
            int headerSize = ReadChunkSize();

            if (headerSize + 8 != length)
                throw new FileFormatException("Incomplete file (file length does not match header)");

            string header2 = ReadChunkId();
            if (header2 != "LWOB")
                throw new FileFormatException("Unknown file");

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string id = ReadChunkId();
                int size = ReadChunkSize();
                Debug.WriteLine("{0}: {1}", id, size);

                switch (id)
                {
                    case "PNTS":
                        ReadPoints(size);
                        break;
                    case "SRFS":
                        ReadSurface(size);
                        break;
                    case "POLS":
                        ReadPolygons(size);
                        break;
                    case "SURF":
                    default:
                        // download the whole chunk
                        var bytes = ReadData(size);
                        break;
                }

            }

            reader.Close();

            return BuildModel();
        }

        private Model3DGroup BuildModel()
        {
            var modelGroup = new Model3DGroup();
            int index = 0;
            foreach (var mesh in Meshes)
            {
                var gm = new GeometryModel3D();
                gm.Geometry = mesh.ToMesh();
                gm.Material = Materials[index];
                gm.BackMaterial = gm.Material;
                modelGroup.Children.Add(gm);
                index++;
            }
            return modelGroup;
        }

        private void ReadPolygons(int size)
        {
            while (size > 0)
            {
                short nverts = ReadShortInt();
                if (nverts <= 0)
                    throw new NotSupportedException("details are not supported");
                var pts = new Point3DCollection(nverts);
                for (int i = 0; i < nverts; i++)
                {
                    int vidx = ReadShortInt();
                    pts.Add(Points[vidx]);
                }
                short surfaceIndex = ReadShortInt();
                size -= (2 + nverts) * 2;

                Meshes[surfaceIndex - 1].AddTriangleFan(pts);
            }
        }

        private void ReadSurface(int size)
        {
            Surfaces = new Collection<string>();
            Meshes = new Collection<MeshBuilder>();
            Materials = new Collection<Material>();

            string name = ReadString(size);
            var names = name.Split('\0');
            for (int i = 0; i < names.Length; i++)
            {
                string n = names[i];
                Surfaces.Add(n);
                Meshes.Add(new MeshBuilder());
                Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

                // If the length of the string (including the null) is odd, an extra null byte is added.
                // Then skip the next empty string.
                if ((n.Length + 1) % 2 == 1)
                    i++;
            }
        }

        private void ReadPoints(int size)
        {
            int nPoints = size / 4 / 3;
            Points = new Point3DCollection(nPoints);
            for (int i = 0; i < nPoints; i++)
            {
                float x = ReadFloat();
                float y = ReadFloat();
                float z = ReadFloat();
                Points.Add(new Point3D(x, y, z));
            }
        }

        private int ReadChunkSize()
        {
            return ReadInt();
        }

        /// <summary>
        /// Read big-endian short.
        /// </summary>
        /// <returns></returns>
        private short ReadShortInt()
        {
            var bytes = reader.ReadBytes(2);
            return BitConverter.ToInt16(new byte[] { bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read big-endian int.
        /// </summary>
        /// <returns></returns>
        private int ReadInt()
        {
            var bytes = reader.ReadBytes(4);
            return BitConverter.ToInt32(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        /// <summary>
        /// Read big-endian float.
        /// </summary>
        /// <returns></returns>
        private float ReadFloat()
        {
            var bytes = reader.ReadBytes(4);
            return BitConverter.ToSingle(new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] }, 0);
        }

        private string ReadChunkId()
        {
            var chars = reader.ReadChars(4);
            return new string(chars);
        }

        private string ReadString(int size)
        {
            var bytes = reader.ReadBytes(size);
            var enc = new ASCIIEncoding();
            var s = enc.GetString(bytes);
            return s.Trim('\0');
        }

        /// <summary>
        /// Read the data block of a chunk.
        /// </summary>
        /// <param name="size">Excluding header size</param>
        /// <returns></returns>
        private byte[] ReadData(int size)
        {
            return reader.ReadBytes(size);
        }
    }
}
