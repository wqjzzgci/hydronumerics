﻿using System;
using System.Collections.ObjectModel;
using System.IO;
﻿using System.Globalization;
﻿using System.Text.RegularExpressions;
﻿using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace HelixToolkit
{
    /// <summary>
    /// StL (StereoLithography) file reader
    /// </summary>
    public class StLReader : IModelReader
    {
        private BinaryReader binaryReader { get; set; }
        private StreamReader asciiReader { get; set; }

        public Collection<MeshBuilder> Meshes { get; private set; }
        public Collection<Material> Materials { get; private set; }
        private int index = 0;
        private Color last;

        public Model3DGroup Read(string path)
        {
            Meshes = new Collection<MeshBuilder>();
            Materials = new Collection<Material>();

            var s = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

            var result = Read(s);
            s.Close();
            return result;
        }

        public Model3DGroup Read(Stream s)
        {
            binaryReader = new BinaryReader(s);

            long length = binaryReader.BaseStream.Length;

            if (length < 84)
                throw new FileFormatException("Incomplete file");

            String header = ReadHeaderB();
            UInt32 numberTriangles = ReadNumberTrianglesB();

            s.Position = 0;

            if (length - 84 != numberTriangles * 50)
                ReadA(s);
            else
                ReadB(s);

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

        public void ReadA(Stream s)
        {
            asciiReader = new StreamReader(s);

            Meshes.Add(new MeshBuilder(true, true));
            Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

            while (!asciiReader.EndOfStream)
            {
                var line = asciiReader.ReadLine().Trim();
                if (line.Length == 0 || line.StartsWith("\0") || line.StartsWith("#") || line.StartsWith("!") || line.StartsWith("$"))
                    continue;
                string id, values;
                SplitLine(line, out id, out values);
                switch (id)
                {
                    case "solid":
                        break;
                    case "facet":
                        ReadTriangleA(values);
                        break;
                    case "endsolid":
                        break;
                }
            }
            asciiReader.Close();

        }

        private static void SplitLine(string line, out string id, out string values)
        {
            int idx = line.IndexOf(' ');
            if (idx == -1)
            {
                id = line;
                values = "";
            }
            else
            {
                id = line.Substring(0, idx).ToLower();
                values = line.Substring(idx + 1);
            }
        }

        readonly Regex normalRegex = new Regex(@"normal\s*(\S*)\s*(\S*)\s*(\S*)");

        private Vector3D ParseNormalA(String normal)
        {
            var match=normalRegex.Match(normal);
            if (!match.Success)
                throw new FileFormatException("Unexpected line.");
            
            double x = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double y = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            double z = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);           
            
            return new Vector3D(x,y,z);
        }

        readonly Regex vertexRegex = new Regex(@"vertex\s*(\S*)\s*(\S*)\s*(\S*)");

        private Point3D ReadVertexA()
        {
            var line = asciiReader.ReadLine().Trim();

            var match = vertexRegex.Match(line);
            if (!match.Success)
                throw new FileFormatException("Unexpected line.");

            double x = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            double y = double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            double z = double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);

            return new Point3D(x, y, z);
        }

        private void ReadLineA(String token)
        {
            var line = asciiReader.ReadLine().Trim();
            int idx = line.IndexOf(' ');
            string id, values;
            SplitLine(line, out id, out values);

            if (!String.Equals(token, id, StringComparison.OrdinalIgnoreCase))
                throw new FileFormatException("Unexpected line.");
        }

        private void ReadTriangleA(String normal)
        {
            Vector3D n = ParseNormalA(normal);
            ReadLineA("outer");
            Point3D v1 = ReadVertexA();
            Point3D v2 = ReadVertexA();
            Point3D v3 = ReadVertexA();
            ReadLineA("endloop");
            ReadLineA("endfacet");

            if (Materials.Count < index + 1)
            {
                Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));
            }

            if (Meshes.Count < index + 1)
                Meshes.Add(new MeshBuilder(true, true));

            Meshes[index].AddTriangle(v1, v2, v3);
        }

        public void ReadB(Stream s)
        {
            long length = binaryReader.BaseStream.Length;

            if (length < 84)
                throw new FileFormatException("Incomplete file");

            String header = ReadHeaderB();
            UInt32 numberTriangles = ReadNumberTrianglesB();

            index = 0;
            Meshes.Add(new MeshBuilder(true, true));
            Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));

            for (int i = 0; i < numberTriangles; i++)
            {
                ReadTriangleB();
            }

            binaryReader.Close();
        }

        /// <summary>
        /// Read UInt32.
        /// </summary>
        /// <returns></returns>
        private UInt32 ReadUInt32B()
        {
            var bytes = binaryReader.ReadBytes(4);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Read UInt16.
        /// </summary>
        /// <returns></returns>
        private UInt16 ReadUInt16B()
        {
            var bytes = binaryReader.ReadBytes(2);
            return BitConverter.ToUInt16(bytes, 0);
        }

        /// <summary>
        /// Read float (4 byte)
        /// </summary>
        /// <returns></returns>
        private float ReadFloatB()
        {
            var bytes = binaryReader.ReadBytes(4);
            return BitConverter.ToSingle(bytes, 0);
        }

        private String ReadHeaderB()
        {
            var chars = binaryReader.ReadChars(80);
            return new String(chars);
        }

        private UInt32 ReadNumberTrianglesB()
        {
            return ReadUInt32B();
        }

        private void ReadTriangleB()
        {
            Color current;
            bool hasColor = false;
            bool sameColor = true;

            float ni = ReadFloatB();
            float nj = ReadFloatB();
            float nk = ReadFloatB();
            Vector3D n = new Vector3D(ni, nj, nk);

            float v1x = ReadFloatB();
            float v1y = ReadFloatB();
            float v1z = ReadFloatB();
            Point3D v1 = new Point3D(v1x, v1y, v1z);

            float v2x = ReadFloatB();
            float v2y = ReadFloatB();
            float v2z = ReadFloatB();
            Point3D v2 = new Point3D(v2x, v2y, v2z);

            float v3x = ReadFloatB();
            float v3y = ReadFloatB();
            float v3z = ReadFloatB();
            Point3D v3 = new Point3D(v3x, v3y, v3z);

            //UInt16 attrib = ReadUInt16();
            var attrib = Convert.ToString(ReadUInt16B(), 2).PadLeft(16, '0').ToCharArray();
            hasColor = attrib[0].Equals('1');

            if (hasColor)
            {
                int blue = attrib[15].Equals('1') ? 1 : 0;
                blue = attrib[14].Equals('1') ? blue + 2 : blue;
                blue = attrib[13].Equals('1') ? blue + 4 : blue;
                blue = attrib[12].Equals('1') ? blue + 8 : blue;
                blue = attrib[11].Equals('1') ? blue + 16 : blue;
                int b = blue * 8;

                int green = attrib[10].Equals('1') ? 1 : 0;
                green = attrib[9].Equals('1') ? green + 2 : green;
                green = attrib[8].Equals('1') ? green + 4 : green;
                green = attrib[7].Equals('1') ? green + 8 : green;
                green = attrib[6].Equals('1') ? green + 16 : green;
                int g = green * 8;

                int red = attrib[5].Equals('1') ? 1 : 0;
                red = attrib[4].Equals('1') ? red + 2 : red;
                red = attrib[3].Equals('1') ? red + 4 : red;
                red = attrib[2].Equals('1') ? red + 8 : red;
                red = attrib[1].Equals('1') ? red + 16 : red;
                int r = red * 8;

                current = Color.FromRgb(Convert.ToByte(r), Convert.ToByte(g), Convert.ToByte(b));
                sameColor = Color.Equals(last, current);

                if (!sameColor)
                {
                    last = current;
                    index++;
                }

                if (Materials.Count < index + 1)
                {
                    Materials.Add(MaterialHelper.CreateMaterial(current));
                }

            }
            else
            {
                if (Materials.Count < index + 1)
                {
                    Materials.Add(MaterialHelper.CreateMaterial(Brushes.Blue));
                }
            }

            if (Meshes.Count < index + 1)
                Meshes.Add(new MeshBuilder(true, true));

            Meshes[index].AddTriangle(v1, v2, v3);
        }


    }
}