using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace HelixToolkit
{
    public static class MeshGeometryHelper
    {

        // Optimizing 3D Collections in WPF
        // http://blogs.msdn.com/timothyc/archive/2006/08/31/734308.aspx
        // - Remember to disconnect collections from the MeshGeometry when changing it

        public static Vector3DCollection CalculateNormals(MeshGeometry3D mesh)
        {
            return CalculateNormals(mesh.Positions, mesh.TriangleIndices);
        }

        public static Vector3DCollection CalculateNormals(IList<Point3D> positions, IList<int> triangleIndices)
        {
            var normals = new Vector3DCollection(positions.Count);
            for (int i = 0; i < positions.Count; i++)
                normals.Add(new Vector3D());
            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int index0 = triangleIndices[i];
                int index1 = triangleIndices[i + 1];
                int index2 = triangleIndices[i + 2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                Vector3D u = p1 - p0;
                Vector3D v = p2 - p0;
                Vector3D w = Vector3D.CrossProduct(u, v);
                w.Normalize();
                normals[index0] += w;
                normals[index1] += w;
                normals[index2] += w;
            }
            for (int i = 0; i < normals.Count; i++)
            {
                var w = normals[i];
                w.Normalize();
                normals[i] = w;
            }
            return normals;
        }

        public static void ChamferVertex(MeshGeometry3D mesh, int index)
        {
            throw new NotImplementedException();
        }

        public static void ChamferEdge(MeshGeometry3D mesh, int index0, int index1)
        {
            throw new NotImplementedException();

        }

        public static void Split(MeshGeometry3D mesh, Plane3D plane, out MeshGeometry3D above, out MeshGeometry3D below)
        {
            throw new NotImplementedException();
        }

        public static MeshGeometry3D Simplify(MeshGeometry3D mesh, double eps)
        {
            // Find common positions
            var dict = new Dictionary<int, int>(); // map position index to first occurence of same position
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                for (int j = i + 1; j < mesh.Positions.Count; j++)
                {
                    if (dict.ContainsKey(j))
                        continue;

                    double l2 = (mesh.Positions[i] - mesh.Positions[j]).LengthSquared;
                    if (l2 < eps)
                    {
                        dict.Add(j, i);
                    }
                }
            }

            var p = new Point3DCollection();
            var ti = new Int32Collection();

            // create new positions array
            var newIndex = new Dictionary<int, int>(); // map old index to new index
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    newIndex.Add(i, p.Count);
                    p.Add(mesh.Positions[i]);
                }
            }

            // Update triangle indices
            for (int i = 0; i < mesh.TriangleIndices.Count; i++)
            {
                int index = mesh.TriangleIndices[i];
                int j;
                if (dict.TryGetValue(index, out j))
                {
                    ti.Add(newIndex[j]);
                }
                else
                {
                    ti.Add(newIndex[index]);
                }
            }

            var result = new MeshGeometry3D();
            result.Positions = p;
            result.TriangleIndices = ti;
            return result;
        }

        #region Edge methods
        /// <summary>
        /// Find all edges in the mesh (each edge is only inclued once)
        /// </summary>
        /// <param name="mesh">a mesh</param>
        /// <returns>edge indices (minium index first)</returns>
        public static Int32Collection FindEdges(MeshGeometry3D mesh)
        {
            var edges = new Int32Collection();
            var dict = new HashSet<ulong>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + (j + 1) % 3];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((UInt32)minIndex, (UInt32)maxIndex);
                    if (!dict.Contains(key))
                    {
                        edges.Add(minIndex);
                        edges.Add(maxIndex);
                        dict.Add(key);
                    }
                }
            }
            return edges;
        }

        /// <summary>
        /// Create a 64-bit key from two 32-bit indices
        /// </summary>
        private static UInt64 CreateKey(UInt32 i0, UInt32 i1)
        {
            return ((UInt64)i0 << 32) + i1;
        }

        /// <summary>
        /// Extract two 32-bit indices from the 64-bit key
        /// </summary>
        private static void ReverseKey(UInt64 key, out UInt32 i0, out UInt32 i1)
        {
            i0 = (UInt32)(key >> 32);
            i1 = (UInt32)((key << 32) >> 32);
        }

        /// <summary>
        /// Find edges that are only connected to one triangle
        /// </summary>
        /// <param name="mesh">a mesh</param>
        /// <returns>edge indices</returns>
        public static Int32Collection FindBorderEdges(MeshGeometry3D mesh)
        {
            var dict = new Dictionary<ulong, int>();

            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + (j + 1) % 3];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index1, index0);
                    ulong key = CreateKey((UInt32)minIndex, (UInt32)maxIndex);
                    if (dict.ContainsKey(key))
                        dict[key] = dict[key] + 1;
                    else
                        dict.Add(key, 0);
                }
            }

            var edges = new Int32Collection();
            foreach (var kvp in dict)
            {
                // find edges only used by 1 triangle
                if (kvp.Value == 1)
                {
                    uint i0, i1;
                    ReverseKey(kvp.Key, out i0, out i1);
                    edges.Add((int)i0);
                    edges.Add((int)i1);
                }
            }
            return edges;
        }

        /// <summary>
        /// Finds all edges where the angle between adjacent triangle normals
        /// is larger than minimumAngle
        /// </summary>
        /// <param name="mesh">a mesh</param>
        /// <param name="minimumAngle">the minimum angle between the normals of two adjacent triangles (degrees)</param>
        /// <returns>edge indices</returns>
        public static Int32Collection FindSharpEdges(MeshGeometry3D mesh, double minimumAngle)
        {
            var coll = new Int32Collection();
            var dict = new Dictionary<ulong, Vector3D>();
            for (int i = 0; i < mesh.TriangleIndices.Count / 3; i++)
            {
                int i0 = i * 3;
                Point3D p0 = mesh.Positions[mesh.TriangleIndices[i0]];
                Point3D p1 = mesh.Positions[mesh.TriangleIndices[i0 + 1]];
                Point3D p2 = mesh.Positions[mesh.TriangleIndices[i0 + 2]];
                Vector3D n = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                n.Normalize();
                for (int j = 0; j < 3; j++)
                {
                    int index0 = mesh.TriangleIndices[i0 + j];
                    int index1 = mesh.TriangleIndices[i0 + (j + 1) % 3];
                    int minIndex = Math.Min(index0, index1);
                    int maxIndex = Math.Max(index0, index1);
                    ulong key = CreateKey((UInt32)minIndex, (UInt32)maxIndex);
                    if (dict.ContainsKey(key))
                    {
                        Vector3D n2 = dict[key];
                        n2.Normalize();
                        double angle = 180 / Math.PI * Math.Acos(Vector3D.DotProduct(n, n2));
                        if (angle > minimumAngle)
                        {
                            coll.Add(minIndex);
                            coll.Add(maxIndex);
                        }
                    }
                    else
                    {
                        dict.Add(key, n);
                    }
                }
            }
            return coll;
        }
        #endregion

        #region Debug methods
        public static void Validate(MeshGeometry3D mesh)
        {
            if (mesh.Normals != null && mesh.Normals.Count != 0 && mesh.Normals.Count != mesh.Positions.Count)
                Debug.WriteLine("Wrong number of normals");
            if (mesh.TextureCoordinates != null && mesh.TextureCoordinates.Count != 0 && mesh.TextureCoordinates.Count != mesh.Positions.Count)
                Debug.WriteLine("Wrong number of TextureCoordinates");
            if (mesh.TriangleIndices.Count % 3 != 0)
                Debug.WriteLine("TriangleIndices not complete");
            for (int i = 0; i < mesh.TriangleIndices.Count; i++)
            {
                int index = mesh.TriangleIndices[i];
                Debug.Assert(index >= 0 || index < mesh.Positions.Count,
                             "Wrong index " + index + " in triangle " + i / 3 + " vertex " + i % 3);
            }
        }
        #endregion

        public static MeshGeometry3D NoSharedVertices(MeshGeometry3D input)
        {
            var p = new Point3DCollection();
            var ti = new Int32Collection();
            Vector3DCollection n = null;
            if (input.Normals != null)
                n = new Vector3DCollection();
            PointCollection tc = null;
            if (input.TextureCoordinates != null)
                tc = new PointCollection();

            for (int i = 0; i < input.TriangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                int index0 = input.TriangleIndices[i0];
                int index1 = input.TriangleIndices[i1];
                int index2 = input.TriangleIndices[i2];
                var p0 = input.Positions[index0];
                var p1 = input.Positions[index1];
                var p2 = input.Positions[index2];
                p.Add(p0);
                p.Add(p1);
                p.Add(p2);
                ti.Add(i0);
                ti.Add(i1);
                ti.Add(i2);
                if (n != null)
                {
                    n.Add(input.Normals[index0]);
                    n.Add(input.Normals[index1]);
                    n.Add(input.Normals[index2]);
                }
                if (tc != null)
                {
                    tc.Add(input.TextureCoordinates[index0]);
                    tc.Add(input.TextureCoordinates[index1]);
                    tc.Add(input.TextureCoordinates[index2]);
                }
            }
            return new MeshGeometry3D() { Positions = p, TriangleIndices = ti, Normals = n, TextureCoordinates = tc };
        }

    }
}
// http://en.wikipedia.org/wiki/Geodesic_dome
