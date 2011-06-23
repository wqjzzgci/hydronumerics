using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    /// <summary>
    /// This class helps building MeshGeometry3D instances.
    /// </summary>
    public class MeshBuilder
    {
        private static readonly Dictionary<int, PointCollection> CircleDictionary = new Dictionary<int, PointCollection>();

        private static readonly double a = Math.Sqrt(2.0 / (5.0 + Math.Sqrt(5.0)));
        private static readonly double b = Math.Sqrt(2.0 / (5.0 - Math.Sqrt(5.0)));

        private readonly Vector3D[] IcosahedronVertices = new[]
                                                              {
                                                                  new Vector3D(-a, 0, b), new Vector3D(a, 0, b),
                                                                  new Vector3D(-a, 0, -b), new Vector3D(a, 0, -b),
                                                                  new Vector3D(0, b, a), new Vector3D(0, b, -a),
                                                                  new Vector3D(0, -b, a), new Vector3D(0, -b, -a),
                                                                  new Vector3D(b, a, 0), new Vector3D(-b, a, 0),
                                                                  new Vector3D(b, -a, 0), new Vector3D(-b, -a, 0)
                                                              };

        private readonly int[] IcosehedronIndices = new[]
                                                        {
                                                            1, 4, 0, 4, 9, 0, 4, 5, 9, 8, 5, 4, 1, 8, 4,
                                                            1, 10, 8, 10, 3, 8, 8, 3, 5, 3, 2, 5, 3, 7, 2,
                                                            3, 10, 7, 10, 6, 7, 6, 11, 7, 6, 0, 11, 6, 1, 0,
                                                            10, 1, 6, 11, 0, 9, 2, 11, 9, 5, 2, 9, 11, 2, 7
                                                        };

        internal Vector3DCollection normals;
        internal Point3DCollection positions;
        internal PointCollection textureCoordinates;
        internal Int32Collection triangleIndices;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshBuilder"/> class.
        /// Normal and texture coordinate generation is turned on.
        /// </summary>
        public MeshBuilder()
            : this(true, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeshBuilder"/> class.
        /// </summary>
        /// <param name="generateNormals">generate normals</param>
        /// <param name="generateTextureCoordinates">generate texture coordinates</param>
        public MeshBuilder(bool generateNormals, bool generateTextureCoordinates)
        {
            positions = new Point3DCollection();
            triangleIndices = new Int32Collection();

            if (generateNormals)
                normals = new Vector3DCollection();
            if (generateTextureCoordinates)
                textureCoordinates = new PointCollection();
        }

        /// <summary>
        /// Gets the positions collection.
        /// </summary>
        /// <value>The positions.</value>
        public Point3DCollection Positions
        {
            get
            {
                if (positions == null)
                    positions = new Point3DCollection();
                return positions;
            }
        }

        /// <summary>
        /// Gets the normals.
        /// </summary>
        /// <value>The normals.</value>
        public Vector3DCollection Normals
        {
            get
            {
                if (normals == null)
                    normals = new Vector3DCollection();
                return normals;
            }
        }

        /// <summary>
        /// Gets the texture coordinates.
        /// </summary>
        /// <value>The texture coordinates.</value>
        public PointCollection TextureCoordinates
        {
            get
            {
                if (textureCoordinates == null)
                    textureCoordinates = new PointCollection();
                return textureCoordinates;
            }
        }

        /// <summary>
        /// Gets the triangle indices.
        /// </summary>
        /// <value>The triangle indices.</value>
        public Int32Collection TriangleIndices
        {
            get
            {
                if (triangleIndices == null)
                    triangleIndices = new Int32Collection();
                return triangleIndices;
            }
        }

        /// <summary>
        /// Converts the geometry to a <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <param name="freeze">freeze the mesh if set to <c>true</c>.</param>
        /// <returns></returns>
        public MeshGeometry3D ToMesh(bool freeze)
        {
            MeshGeometry3D mesh = ToMesh();
            if (freeze)
                mesh.Freeze();
            return mesh;
        }

        /// <summary>
        ///  Converts the geometry to a <see cref="MeshGeometry3D"/>.
        /// </summary>
        /// <returns></returns>
        public MeshGeometry3D ToMesh()
        {
            var mg = new MeshGeometry3D
                         {
                             Positions = positions,
                             TriangleIndices = triangleIndices,
                             Normals = normals,
                             TextureCoordinates = textureCoordinates
                         };
            return mg;
        }

        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2)
        {
            var uv0 = new Point(0, 0);
            var uv1 = new Point(1, 0);
            var uv2 = new Point(0, 1);
            AddTriangle(p0, p1, p2, uv0, uv1, uv2);
        }

        /// <summary>
        /// Adds a triangle.
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="uv0">The uv0.</param>
        /// <param name="uv1">The uv1.</param>
        /// <param name="uv2">The uv2.</param>
        public void AddTriangle(Point3D p0, Point3D p1, Point3D p2, Point uv0, Point uv1, Point uv2)
        {
            int i0 = positions.Count;

            positions.Add(p0);
            positions.Add(p1);
            positions.Add(p2);

            if (textureCoordinates != null)
            {
                textureCoordinates.Add(uv0);
                textureCoordinates.Add(uv1);
                textureCoordinates.Add(uv2);
            }

            if (normals != null)
            {
                var w = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                w.Normalize();
                normals.Add(w);
                normals.Add(w);
                normals.Add(w);
            }

            triangleIndices.Add(i0 + 0);
            triangleIndices.Add(i0 + 1);
            triangleIndices.Add(i0 + 2);
        }

        /// <summary>
        /// Adds a quad.
        /// 
        /// The nodes are arranged in counter-clockwise order
        ///  p3               p2
        ///   +---------------+
        ///   |               |
        ///   |               |
        ///   +---------------+
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        public void AddQuad(Point3D p0, Point3D p1, Point3D p2, Point3D p3)
        {
            var uv0 = new Point(0, 0);
            var uv1 = new Point(1, 0);
            var uv2 = new Point(0, 1);
            var uv3 = new Point(1, 1);
            AddQuad(p0, p1, p2, p3, uv0, uv1, uv2, uv3);
        }

        /// <summary>
        /// Adds a quad.
        /// 
        /// The nodes are arranged in counter-clockwise order
        ///  p3               p2
        ///   +---------------+
        ///   |               |
        ///   |               |
        ///   +---------------+
        ///  p0               p1
        /// </summary>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="p3">The p3.</param>
        /// <param name="uv0">The uv0.</param>
        /// <param name="uv1">The uv1.</param>
        /// <param name="uv2">The uv2.</param>
        /// <param name="uv3">The uv3.</param>
        public void AddQuad(Point3D p0, Point3D p1, Point3D p2, Point3D p3, Point uv0, Point uv1, Point uv2, Point uv3)
        {
            int i0 = positions.Count;

            positions.Add(p0);
            positions.Add(p1);
            positions.Add(p2);
            positions.Add(p3);

            if (textureCoordinates != null)
            {
                textureCoordinates.Add(uv0);
                textureCoordinates.Add(uv1);
                textureCoordinates.Add(uv2);
                textureCoordinates.Add(uv3);
            }

            if (normals != null)
            {
                var w = Vector3D.CrossProduct(p3 - p0, p1 - p0);
                w.Normalize();
                normals.Add(w);
                normals.Add(w);
                normals.Add(w);
                normals.Add(w);
            }

            triangleIndices.Add(i0 + 0);
            triangleIndices.Add(i0 + 1);
            triangleIndices.Add(i0 + 2);

            triangleIndices.Add(i0 + 2);
            triangleIndices.Add(i0 + 3);
            triangleIndices.Add(i0 + 0);
        }

        /// <summary>
        /// Adds a triangle strip to the mesh.
        /// http://en.wikipedia.org/wiki/Triangle_strip
        /// </summary>
        /// <param name="pos">The points.</param>
        /// <param name="norms">The normals.</param>
        /// <param name="texCoords">The texture coordinates.</param>
        public void AddTriangleStrip(
            Point3DCollection pos,
            Vector3DCollection norms,
            PointCollection texCoords)
        {
            // http://en.wikipedia.org/wiki/Triangle_strip

            int index0 = positions.Count;
            for (int i = 0; i < pos.Count; i++)
            {
                positions.Add(pos[i]);
                if (normals != null && norms != null)
                    normals.Add(norms[i]);
                if (textureCoordinates != null && texCoords != null)
                    textureCoordinates.Add(texCoords[i]);
            }
            int indexEnd = positions.Count;
            for (int i = index0; i + 2 < indexEnd; i += 2)
            {
                triangleIndices.Add(i);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);

                if (i + 3 < indexEnd)
                {
                    triangleIndices.Add(i + 1);
                    triangleIndices.Add(i + 3);
                    triangleIndices.Add(i + 2);
                }
            }
        }

        /// <summary>
        /// Adds a triangle fan to the mesh
        /// </summary>
        /// <param name="points">the fan points</param>
        public void AddTriangleFan(Point3DCollection points)
        {
            AddTriangleFan(points, null, null);
        }

        /// <summary>
        /// Adds a triangle fan to the mesh
        /// </summary>
        /// <param name="points">the fan points</param>
        /// <param name="norms">The normals.</param>
        /// <param name="texCoords">The texture coordinates.</param>
        public void AddTriangleFan(Point3DCollection points, Vector3DCollection norms,
                                   PointCollection texCoords)
        {
            int index0 = positions.Count;
            foreach (var p in points)
                positions.Add(p);
            if (textureCoordinates != null && texCoords != null)
                foreach (var tc in texCoords)
                    textureCoordinates.Add(tc);
            if (normals != null && norms != null)
                foreach (Vector3D n in norms)
                    normals.Add(n);

            int indexEnd = positions.Count;
            for (int i = index0; i + 2 < indexEnd; i++)
            {
                triangleIndices.Add(index0);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);
            }
        }

        public void AddTriangles(Point3DCollection points, Vector3DCollection norms,
                                 PointCollection texCoords)
        {
            Debug.Assert(points.Count > 0 && points.Count % 3 == 0);
            int index0 = positions.Count;
            foreach (var p in points)
                positions.Add(p);
            if (textureCoordinates != null && texCoords != null)
                foreach (var tc in texCoords)
                    textureCoordinates.Add(tc);
            if (normals != null && norms != null)
                foreach (var n in norms)
                    normals.Add(n);

            int indexEnd = positions.Count;
            for (int i = index0; i + 2 < indexEnd; i++)
            {
                triangleIndices.Add(i);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);
            }
        }

        /// <summary>
        /// Adds the quads.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="norms">The norms.</param>
        /// <param name="texCoords">The tex coords.</param>
        public void AddQuads(Point3DCollection points, Vector3DCollection norms, PointCollection texCoords)
        {
            Debug.Assert(points.Count > 0 && points.Count % 4 == 0);
            int index0 = positions.Count;
            foreach (Point3D p in points)
                positions.Add(p);
            if (textureCoordinates != null && texCoords != null)
                foreach (Point tc in texCoords)
                    textureCoordinates.Add(tc);
            if (normals != null && norms != null)
                foreach (Vector3D n in norms)
                    normals.Add(n);

            int indexEnd = positions.Count;
            for (int i = index0; i + 3 < indexEnd; i++)
            {
                triangleIndices.Add(i);
                triangleIndices.Add(i + 1);
                triangleIndices.Add(i + 2);

                triangleIndices.Add(i + 2);
                triangleIndices.Add(i + 3);
                triangleIndices.Add(i);
            }
        }

        /// <summary>
        /// Adds the lofted geometry.
        /// http://en.wikipedia.org/wiki/Loft_(3D)
        /// </summary>
        /// <param name="positionsList">List of lofting sections.</param>
        /// <param name="normalList">The normal list.</param>
        /// <param name="textureCoordinateList">The texture coordinate list.</param>
        public void AddLoftedGeometry(
            IList<Point3DCollection> positionsList,
            IList<Vector3DCollection> normalList,
            IList<PointCollection> textureCoordinateList
            )
        {
            int index0 = positions.Count;
            int n = -1;
            for (int i = 0; i < positionsList.Count; i++)
            {
                var pc = positionsList[i];

                // check that all curves have same number of points
                if (n == -1)
                    n = pc.Count;
                if (pc.Count != n)
                    throw new InvalidOperationException("All curves should have the same number of points");

                // add the points
                foreach (var p in pc)
                    positions.Add(p);

                // add normals 
                if (normals != null && normalList != null)
                {
                    var nc = normalList[i];
                    foreach (var normal in nc)
                        normals.Add(normal);
                }

                // add texcoords
                if (textureCoordinates != null && textureCoordinateList != null)
                {
                    PointCollection tc = textureCoordinateList[i];
                    foreach (Point t in tc)
                        textureCoordinates.Add(t);
                }
            }

            for (int i = 0; i + 1 < positionsList.Count; i++)
            {
                for (int j = 0; j + 1 < n; j++)
                {
                    int i0 = index0 + i * n + j;
                    int i1 = i0 + n;
                    int i2 = i1 + 1;
                    int i3 = i0 + 1;
                    triangleIndices.Add(i0);
                    triangleIndices.Add(i1);
                    triangleIndices.Add(i2);

                    triangleIndices.Add(i2);
                    triangleIndices.Add(i3);
                    triangleIndices.Add(i0);
                }
            }
        }

        /// <summary>
        /// Add the extruded geometry of the specified curve and direction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="xDirection">The x axis direction.</param>
        /// <param name="p0">The p0.</param>
        /// <param name="p1">The p1.</param>
        public void AddExtrudedGeometry(PointCollection points, Vector3D xDirection, Point3D p0, Point3D p1)
        {
            var yDirection = Vector3D.CrossProduct(xDirection, p1 - p0);
            yDirection.Normalize();
            xDirection.Normalize();

            int index0 = positions.Count;
            int np = 2 * points.Count;
            foreach (var p in points)
            {
                var v = xDirection * p.X + yDirection * p.Y;
                positions.Add(p0 + v);
                positions.Add(p1 + v);
                v.Normalize();
                if (normals != null)
                {
                    normals.Add(v);
                    normals.Add(v);
                }
                if (textureCoordinates != null)
                {
                    textureCoordinates.Add(new Point(0, 0));
                    textureCoordinates.Add(new Point(1, 0));
                }

                int i1 = index0 + 1;
                int i2 = (index0 + 2) % np;
                int i3 = (index0 + 2) % np + 1;

                triangleIndices.Add(i1);
                triangleIndices.Add(i2);
                triangleIndices.Add(index0);

                triangleIndices.Add(i1);
                triangleIndices.Add(i3);
                triangleIndices.Add(i2);
            }
        }

        /// <summary>
        /// Gets a circle section.
        /// The sections are cached in a dictionary.
        /// </summary>
        /// <param name="thetaDiv">The number of division.</param>
        /// <returns></returns>
        public static PointCollection GetCircle(int thetaDiv)
        {
            PointCollection circle;
            if (!CircleDictionary.TryGetValue(thetaDiv, out circle))
            {
                circle = new PointCollection();
                CircleDictionary.Add(thetaDiv, circle);
                for (int i = 0; i < thetaDiv; i++)
                {
                    double theta = Math.PI * 2 * ((double)i / (thetaDiv - 1));
                    circle.Add(new Point(Math.Cos(theta), Math.Sin(theta)));
                }
            }
            return circle;
        }

        /// <summary>
        /// Add a surface of revolution
        /// http://en.wikipedia.org/wiki/Surface_of_revolution
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="origin">The origin.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="thetaDiv">The theta div.</param>
        public void AddRevolvedGeometry(PointCollection points, Point3D origin, Vector3D direction, int thetaDiv)
        {
            direction.Normalize();

            // Find two unit vectors orthogonal to the specified direction
            var u = direction.FindAnyPerpendicular();
            var v = Vector3D.CrossProduct(direction, u);

            u.Normalize();
            v.Normalize();


            var circle = GetCircle(thetaDiv);

            int index0 = positions.Count;
            int nPoints = points.Count;

            int totalNodes = (points.Count - 1) * 2 * thetaDiv;
            int rowNodes = (points.Count - 1) * 2;

            for (int i = 0; i < thetaDiv; i++)
            {
                Vector3D w = v * circle[i].X + u * circle[i].Y;

                for (int j = 0; j + 1 < nPoints; j++)
                {
                    // Add segment
                    var q1 = origin + direction * points[j].X + w * points[j].Y;
                    var q2 = origin + direction * points[j + 1].X + w * points[j + 1].Y;


                    // todo:should not add segment if q1==q2 (corner point)
                    // const double eps = 1e-6;
                    // if (Point3D.Subtract(q1, q2).LengthSquared < eps)
                    //    continue;

                    double tx = points[j + 1].X - points[j].X;
                    double ty = points[j + 1].Y - points[j].Y;

                    var normal = -direction * ty + w * tx;
                    normal.Normalize();

                    positions.Add(q1);
                    positions.Add(q2);

                    if (normals != null)
                    {
                        normals.Add(normal);
                        normals.Add(normal);
                    }

                    if (textureCoordinates != null)
                    {
                        textureCoordinates.Add(new Point((double)i / (thetaDiv - 1), (double)j / (nPoints - 1)));
                        textureCoordinates.Add(new Point((double)i / (thetaDiv - 1), (double)(j + 1) / (nPoints - 1)));
                    }

                    int i0 = index0 + i * rowNodes + j * 2;
                    int i1 = i0 + 1;
                    int i2 = index0 + ((i + 1) * rowNodes + j * 2) % totalNodes;
                    int i3 = i2 + 1;

                    triangleIndices.Add(i1);
                    triangleIndices.Add(i2);
                    triangleIndices.Add(i0);

                    triangleIndices.Add(i1);
                    triangleIndices.Add(i3);
                    triangleIndices.Add(i2);
                }
            }
        }


        /// <summary>
        /// Add a sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="thetaDiv">The number of divisions around the sphere.</param>
        /// <param name="phiDiv">The number of divisions from top to bottom of the sphere.</param>
        public void AddSphere(Point3D center, double radius, int thetaDiv=20, int phiDiv=10)
        {
            int index0 = Positions.Count;
            double dt = 2 * Math.PI / thetaDiv;
            double dp = Math.PI / phiDiv;

            // todo: there are better ways to tesselate the sphere

            for (int pi = 0; pi <= phiDiv; pi++)
            {
                double phi = pi * dp;

                for (int ti = 0; ti <= thetaDiv; ti++)
                {
                    // we want to start the mesh on the x axis
                    double theta = ti * dt;

                    // Spherical coordinates
                    // http://mathworld.wolfram.com/SphericalCoordinates.html
                    double x = Math.Cos(theta) * Math.Sin(phi);
                    double y = Math.Sin(theta) * Math.Sin(phi);
                    double z = Math.Cos(phi);

                    var p = new Point3D(center.X + radius * x, center.Y + radius * y, center.Z + radius * z);
                    positions.Add(p);

                    if (normals != null)
                    {
                        var n = new Vector3D(x, y, z);
                        normals.Add(n);
                    }

                    if (textureCoordinates != null)
                    {
                        var uv = new Point(theta / (2 * Math.PI), phi / (Math.PI));
                        textureCoordinates.Add(uv);
                    }
                }
            }

            AddRectangularMeshTriangleIndices(index0, phiDiv + 1, thetaDiv + 1);
        }

        /// <summary>
        /// Add a regular icosahedron.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="shareVertices">if set to <c>true</c> [share vertices].</param>
        public void AddRegularIcosahedron(Point3D center, double radius, bool shareVertices)
        {
            // http://en.wikipedia.org/wiki/Icosahedron
            // http://www.gamedev.net/community/forums/topic.asp?topic_id=283350

            if (shareVertices)
            {
                int index0 = positions.Count;
                foreach (Vector3D v in IcosahedronVertices)
                    positions.Add(center + v * radius);
                foreach (int i in IcosehedronIndices)
                    triangleIndices.Add(index0 + i);
            }
            else
            {
                for (int i = 0; i + 2 < IcosehedronIndices.Length; i += 3)
                    AddTriangle(center + IcosahedronVertices[IcosehedronIndices[i]] * radius,
                                center + IcosahedronVertices[IcosehedronIndices[i + 1]] * radius,
                                center + IcosahedronVertices[IcosehedronIndices[i + 2]] * radius);
            }
        }

        public enum BoxFaces
        {
            Top = 0x1,
            Bottom = 0x2,
            Left = 0x4,
            Right = 0x8,
            Front = 0x10,
            Back = 0x20,
            All = Top | Bottom | Left | Right | Front | Back
        } ;

        /// <summary>
        /// Add a box align with the X, Y and Z axes.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="xLength">The width.</param>
        /// <param name="yLength">The length.</param>
        /// <param name="zLength">The height.</param>
        public void AddBox(Point3D center, double xLength, double yLength, double zLength)
        {
            AddBox(center, xLength, yLength, zLength, BoxFaces.All);
        }

        /// <summary>
        /// Add a box with specifed faces, align with the X, Y and Z axes.
        /// </summary>
        /// <param name="center">The center of the box.</param>
        /// <param name="xLength">The length of the box along the X axis.</param>
        /// <param name="yLength">The length of the box along the Y axis.</param>
        /// <param name="zLength">The length of the box along the Z axis.</param>
        /// <param name="faces">The faces to include.</param>
        public void AddBox(Point3D center, double xLength, double yLength, double zLength, BoxFaces faces)
        {
            if ((faces & BoxFaces.Front) == BoxFaces.Front)
                AddCubeFace(center, new Vector3D(0, 1, 0), new Vector3D(0, 0, 1), xLength, yLength, zLength);
            if ((faces & BoxFaces.Left) == BoxFaces.Left)
                AddCubeFace(center, new Vector3D(-1, 0, 0), new Vector3D(0, 0, 1), yLength, xLength, zLength);
            if ((faces & BoxFaces.Right) == BoxFaces.Right)
                AddCubeFace(center, new Vector3D(1, 0, 0), new Vector3D(0, 0, 1), yLength, xLength, zLength);
            if ((faces & BoxFaces.Back) == BoxFaces.Back)
                AddCubeFace(center, new Vector3D(0, -1, 0), new Vector3D(0, 0, 1), xLength, yLength, zLength);
            if ((faces & BoxFaces.Top) == BoxFaces.Top)
                AddCubeFace(center, new Vector3D(0, 0, 1), new Vector3D(0, -1, 0), zLength, yLength, xLength);
            if ((faces & BoxFaces.Bottom) == BoxFaces.Bottom)
                AddCubeFace(center, new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), zLength, yLength, xLength);
        }

        /// <summary>
        /// Adds a cube face.
        /// </summary>
        /// <param name="center">The center of the cube.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="up">The up vector for the face.</param>
        /// <param name="dist">The dist from the center of the cube to the face.</param>
        /// <param name="width">The width of the face.</param>
        /// <param name="height">The height of the face.</param>
        public void AddCubeFace(Point3D center, Vector3D normal, Vector3D up, double dist, double width, double height)
        {
            var right = Vector3D.CrossProduct(normal, up);
            var n = normal * dist / 2;
            up *= height / 2;
            right *= width / 2;
            var p1 = center + n - up - right;
            var p2 = center + n - up + right;
            var p3 = center + n + up + right;
            var p4 = center + n + up - right;

            int i0 = positions.Count;
            positions.Add(p1);
            positions.Add(p2);
            positions.Add(p3);
            positions.Add(p4);
            if (normals != null)
            {
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
                normals.Add(normal);
            }
            if (textureCoordinates != null)
            {
                textureCoordinates.Add(new Point(1, 1));
                textureCoordinates.Add(new Point(0, 1));
                textureCoordinates.Add(new Point(0, 0));
                textureCoordinates.Add(new Point(1, 0));
            }
            triangleIndices.Add(i0 + 2);
            triangleIndices.Add(i0 + 1);
            triangleIndices.Add(i0 + 0);
            triangleIndices.Add(i0 + 0);
            triangleIndices.Add(i0 + 3);
            triangleIndices.Add(i0 + 2);
        }

        /// <summary>
        /// Adds a pyramid.
        /// http://en.wikipedia.org/wiki/Pyramid_(geometry)
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="sideLength">Length of the side.</param>
        /// <param name="height">The height.</param>
        public void AddPyramid(Point3D center, double sideLength, double height)
        {
            var p1 = new Point3D(center.X - sideLength * 0.5, center.Y - sideLength * 0.5, center.Z);
            var p2 = new Point3D(center.X + sideLength * 0.5, center.Y - sideLength * 0.5, center.Z);
            var p3 = new Point3D(center.X + sideLength * 0.5, center.Y + sideLength * 0.5, center.Z);
            var p4 = new Point3D(center.X - sideLength * 0.5, center.Y + sideLength * 0.5, center.Z);
            var p5 = new Point3D(center.X, center.Y, center.Z + height);
            AddTriangle(p1, p2, p5);
            AddTriangle(p2, p3, p5);
            AddTriangle(p3, p4, p5);
            AddTriangle(p4, p1, p5);
        }

        /// <summary>
        /// Adds a cone.
        /// http://en.wikipedia.org/wiki/Cone_(geometry)
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="baseRadius">The base radius.</param>
        /// <param name="topRadius">The top radius.</param>
        /// <param name="height">The height.</param>
        /// <param name="baseCap">Include a base cap if set to <c>true</c>.</param>
        /// <param name="topCap">Include the top cap if set to <c>true</c>.</param>
        /// <param name="thetaDiv">The number of divisions around the cone.</param>
        public void AddCone(Point3D origin, Vector3D direction, double baseRadius, double topRadius, double height,
                            bool baseCap, bool topCap, int thetaDiv)
        {
            var pc = new PointCollection();
            if (baseCap)
                pc.Add(new Point(0, 0));
            pc.Add(new Point(0, baseRadius));
            pc.Add(new Point(height, topRadius));
            if (topCap)
                pc.Add(new Point(height, 0));
            AddRevolvedGeometry(pc, origin, direction, thetaDiv);
        }

        /// <summary>
        /// Adds a cylinder to the mesh.
        /// http://en.wikipedia.org/wiki/Cylinder_(geometry)
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="diameter">The diameter.</param>
        /// <param name="thetaDiv">The theta div.</param>
        public void AddCylinder(Point3D p1, Point3D p2, double diameter, int thetaDiv)
        {
            Vector3D n = p2 - p1;
            double l = n.Length;
            n.Normalize();
            AddCone(p1, n, diameter / 2, diameter / 2, l, false, false, thetaDiv);
        }

        /// <summary>
        /// Adds a collection of edges as cylinders.
        /// </summary>
        /// <param name="pts">The PTS.</param>
        /// <param name="edges">The edges.</param>
        /// <param name="diameter">The diameter.</param>
        /// <param name="thetaDiv">The theta div.</param>
        public void AddEdges(IList<Point3D> pts, IList<int> edges, double diameter, int thetaDiv)
        {
            for (int i = 0; i < edges.Count - 1; i += 2)
            {
                AddCylinder(pts[edges[i]], pts[edges[i + 1]], diameter, thetaDiv);
            }
        }

        /// <summary>
        /// Add a single node.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="normal">The normal.</param>
        /// <param name="textureCoordinate">The texture coordinate.</param>
        public void AddNode(Point3D pos, Vector3D normal, Point textureCoordinate)
        {
            Positions.Add(pos);
            if (normals != null)
                normals.Add(normal);
            if (textureCoordinates != null)
                textureCoordinates.Add(textureCoordinate);
        }

        /// <summary>
        /// Adds a rectangular mesh (m x n points).
        /// </summary>
        /// <param name="points">The one-dimensional array of points. The points are stored row-by-row.</param>
        /// <param name="columns">The number of columns in the rectangular mesh.</param>
        public void AddRectangularMesh(IList<Point3D> points, int columns)
        {
            int index0 = Positions.Count;

            foreach (var pt in points)
                positions.Add(pt);
            int rows = points.Count / columns;

            AddRectangularMeshTriangleIndices(index0, rows, columns);
            if (normals != null)
            {
                AddRectangularMeshNormals(index0, rows, columns);
            }
            if (textureCoordinates != null)
            {
                AddRectangularMeshTextureCoordinates(rows, columns);
            }
        }

        private void AddRectangularMeshTriangleIndices(int index0, int rows, int columns)
        {
            for (int i = 0; i < rows - 1; i++)
            {
                for (int j = 0; j < columns - 1; j++)
                {
                    int ij = i * columns + j;
                    triangleIndices.Add(index0 + ij);
                    triangleIndices.Add(index0 + ij + 1 + columns);
                    triangleIndices.Add(index0 + ij + 1);

                    triangleIndices.Add(index0 + ij + 1 + columns);
                    triangleIndices.Add(index0 + ij);
                    triangleIndices.Add(index0 + ij + columns);
                }
            }
        }

        private void AddRectangularMeshTriangleIndices(int index0, int rows, int columns, bool rowsClosed, bool columnsClosed)
        {
            int m2 = rows - 1;
            int n2 = columns - 1;
            if (columnsClosed) m2++;
            if (rowsClosed) n2++;

            for (int i = 0; i < m2; i++)
            {
                for (int j = 0; j < n2; j++)
                {
                    int ij00 = index0 + i * columns + j;
                    int ij01 = index0 + i * columns + (j + 1) % columns;
                    int ij10 = index0 + ((i + 1) % rows) * columns + j;
                    int ij11 = index0 + ((i + 1) % rows) * columns + (j + 1) % columns;
                    triangleIndices.Add(ij00);
                    triangleIndices.Add(ij01);
                    triangleIndices.Add(ij11);

                    triangleIndices.Add(ij11);
                    triangleIndices.Add(ij10);
                    triangleIndices.Add(ij00);
                }
            }
        }

        private void AddRectangularMeshTextureCoordinates(int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                double v = (double)i / (rows - 1);
                for (int j = 0; j < columns; j++)
                {
                    double u = (double)j / (columns - 1);
                    textureCoordinates.Add(new Point(u, v));
                }
            }
        }

        private void AddRectangularMeshNormals(int index0, int rows, int columns)
        {
            for (int i = 0; i < rows; i++)
            {
                int i1 = i + 1;
                if (i1 == rows) i1--;
                int i0 = i1 - 1;
                for (int j = 0; j < columns; j++)
                {
                    int j1 = j + 1;
                    if (j1 == columns) j1--;
                    int j0 = j1 - 1;
                    var normal = Vector3D.CrossProduct(Point3D.Subtract(positions[index0 + i1 * columns + j0], positions[index0 + i0 * columns + j0]),
                                                       Point3D.Subtract(positions[index0 + i0 * columns + j1], positions[index0 + i0 * columns + j0]));
                    normal.Normalize();
                    normals.Add(normal);
                }
            }
        }

        /// <summary>
        /// Adds a rectangular mesh defined by a two-dimensional arrary of points.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <param name="closed0">set to <c>true</c> if the mesh is closed in the 1st dimension.</param>
        /// <param name="closed1">set to <c>true</c> if the mesh is closed in the 2nd dimension.</param>
        public void AddRectangularMesh(Point3D[,] pts, bool closed0, bool closed1)
        {
            int rows = pts.GetUpperBound(0) + 1;
            int columns = pts.GetUpperBound(1) + 1;
            int index0 = positions.Count;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                    positions.Add(pts[i, j]);

            AddRectangularMeshTriangleIndices(index0, rows, columns, closed0, closed1);

            if (normals != null)
            {
                AddRectangularMeshNormals(index0, rows, columns);
            }
            if (textureCoordinates != null)
            {
                AddRectangularMeshTextureCoordinates(rows, columns);
            }
        }

        /// <summary>
        /// Appends the specified mesh.
        /// </summary>
        /// <param name="mesh">The mesh.</param>
        public void Append(MeshBuilder mesh)
        {
            int i0 = Positions.Count;
            foreach (Point3D p in mesh.Positions)
                Positions.Add(p);
            foreach (int idx in mesh.TriangleIndices)
                TriangleIndices.Add(idx + i0);
            if (normals != null)
            {
                foreach (Vector3D p in mesh.normals)
                    normals.Add(p);
            }
            if (textureCoordinates != null)
            {
                foreach (Point p in mesh.textureCoordinates)
                    textureCoordinates.Add(p);
            }
        }

        /// <summary>
        /// Adds a tube.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="values">The texture coordinate X values.</param>
        /// <param name="diameter">The diameter per point.</param>
        /// <param name="thetaDiv">The number of divisions around the tube.</param>
        /// <param name="isTubeClosed">If the tube is closed set to <c>true</c>.</param>
        public void AddTube(IList<Point3D> path, double[] values, double[] diameter, int thetaDiv, bool isTubeClosed)
        {
            PointCollection pc = GetCircle(thetaDiv);
            AddTube(path, values, diameter, pc, isTubeClosed, true);
        }

        /// <summary>
        /// Adds a tube.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="values">The texture coordinate X values.</param>
        /// <param name="diameter">The diameter per point.</param>
        /// <param name="Section">The section.</param>
        /// <param name="isTubeClosed">If the tube is closed set to <c>true</c>.</param>
        /// <param name="isSectionClosed">if set to <c>true</c> [is section closed].</param>
        public void AddTube(IList<Point3D> path, double[] values, double[] diameter, PointCollection section, bool isTubeClosed, bool isSectionClosed)
        {
            int index0 = positions.Count;
            int pathLength = path.Count;
            int sectionLength = section.Count;
            if (pathLength < 2 || sectionLength < 2)
                return;

            var up = (path[1] - path[0]).FindAnyPerpendicular();
            // todo: could also specify a default up direction

            for (int i = 0; i < pathLength; i++)
            {
                double r = diameter != null ? diameter[i] / 2 : 1;
                int i0 = i > 0 ? i - 1 : i;
                int i1 = i + 1 < pathLength ? i + 1 : i;

                var forward = path[i1] - path[i0];
                var right = Vector3D.CrossProduct(up, forward);
                up = Vector3D.CrossProduct(forward, right);
                up.Normalize();
                right.Normalize();
                var u = right;
                var v = up;
                for (int j = 0; j < sectionLength; j++)
                {
                    var w = section[j].X * u * r + section[j].Y * v * r;
                    var q = path[i] + w;
                    positions.Add(q);
                    if (normals != null)
                    {
                        w.Normalize();
                        normals.Add(w);
                    }
                    if (textureCoordinates != null)
                    {
                        textureCoordinates.Add(values != null ? new Point(values[i], 0) : new Point());
                    }
                }
            }
            AddRectangularMeshTriangleIndices(index0, pathLength, sectionLength, isSectionClosed, isTubeClosed);
        }

        /// <summary>
        /// Appends the specified points and triangles.
        /// </summary>
        /// <param name="pts">The points.</param>
        /// <param name="tri">The triangle indices.</param>
        public void Append(IList<Point3D> pts, IList<int> tri)
        {
            int index0 = positions.Count;
            foreach (var p in pts)
                positions.Add(p);
            foreach (int i in tri)
                triangleIndices.Add(index0 + i);
        }

        /// <summary>
        /// Adds the arrow to the mesh.
        /// </summary>
        /// <param name="point1">The start point.</param>
        /// <param name="point2">The end point.</param>
        /// <param name="diameter">The diameter of the tube.</param>
        /// <param name="headLength">Length of the head (in diameter units).</param>
        /// <param name="thetaDiv">The number of divisions around the arrow.</param>
        public void AddArrow(Point3D point1, Point3D point2, double diameter, double headLength = 3, int thetaDiv= 18)
        {
            var dir = point2 - point1;
            double length = dir.Length;
            double r = diameter / 2;

            var pc = new PointCollection
                         {
                             new Point(0, 0),
                             new Point(0, r),
                             new Point(length - diameter*headLength, r),
                             new Point(length - diameter*headLength, r*2),
                             new Point(length, 0)
                         };

            AddRevolvedGeometry(pc, point1, dir, thetaDiv);
        }

        public void AddPipe(Point3D point1, Point3D point2, double innerDiameter, double diameter, int thetaDiv)
        {
            var dir = point2 - point1;

            double height = dir.Length;
            dir.Normalize();

            var pc = new PointCollection
                         {
                             new Point(0, innerDiameter/2),
                             new Point(0, diameter/2),
                             new Point(height, diameter/2),
                             new Point(height, innerDiameter/2)
                         };
            if (innerDiameter > 0)
                pc.Add(new Point(0, innerDiameter / 2));

            AddRevolvedGeometry(pc, point1, dir, thetaDiv);
        }

        /// <summary>
        /// Makes sure no triangles share the same vertex.
        /// </summary>
        private void NoSharedVertices()
        {
            var p = new Point3DCollection();
            var ti = new Int32Collection();
            Vector3DCollection n = null;
            if (normals != null)
                n = new Vector3DCollection();
            PointCollection tc = null;
            if (textureCoordinates != null)
                tc = new PointCollection();

            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                int index0 = triangleIndices[i0];
                int index1 = triangleIndices[i1];
                int index2 = triangleIndices[i2];
                var p0 = positions[index0];
                var p1 = positions[index1];
                var p2 = positions[index2];
                p.Add(p0);
                p.Add(p1);
                p.Add(p2);
                ti.Add(i0);
                ti.Add(i1);
                ti.Add(i2);
                if (n != null)
                {
                    n.Add(normals[index0]);
                    n.Add(normals[index1]);
                    n.Add(normals[index2]);
                }
                if (tc != null)
                {
                    tc.Add(textureCoordinates[index0]);
                    tc.Add(textureCoordinates[index1]);
                    tc.Add(textureCoordinates[index2]);
                }
            }
            positions = p;
            triangleIndices = ti;
            normals = n;
            textureCoordinates = tc;
        }

        /// <summary>
        /// Chamfers the specified corner (experimental code).
        /// </summary>
        /// <param name="p">The corner point.</param>
        /// <param name="d">The chamfer distance.</param>
        /// <param name="eps">The corner search limit distance.</param>
        /// <param name="chamferPoints">If this parameter is provided, the collection will be filled with the generated chamfer points.</param>
        public void ChamferCorner(Point3D p, double d, double eps = 1e-6, ICollection<Point3D> chamferPoints = null)
        {
            NoSharedVertices();

            normals = null;
            textureCoordinates = null;

            var cornerNormal = FindCornerNormal(p, eps);

            var newCornerPoint = p - cornerNormal * d;
            int index0 = positions.Count;
            positions.Add(newCornerPoint);

            var plane = new Plane3D(newCornerPoint, cornerNormal);

            int ntri = triangleIndices.Count;

            for (int i = 0; i < ntri; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                var p0 = positions[triangleIndices[i0]];
                var p1 = positions[triangleIndices[i1]];
                var p2 = positions[triangleIndices[i2]];
                double d0 = (p - p0).LengthSquared;
                double d1 = (p - p1).LengthSquared;
                double d2 = (p - p2).LengthSquared;
                double mind = Math.Min(d0, Math.Min(d1, d2));
                if (mind > eps)
                    continue;
                if (d1 < eps)
                {
                    i0 = i + 1;
                    i1 = i + 2;
                    i2 = i;
                }
                if (d2 < eps)
                {
                    i0 = i + 2;
                    i1 = i;
                    i2 = i + 1;
                }

                p0 = positions[triangleIndices[i0]];
                p1 = positions[triangleIndices[i1]];
                p2 = positions[triangleIndices[i2]];

                // p0 is the corner vertex (at index i0)
                // find the intersections between the chamfer plane and the two edges connected to the corner
                var p01 = plane.LineIntersection(p0, p1);
                var p02 = plane.LineIntersection(p0, p2);

                if (p01 == Plane3D.InPlane || p01 == Plane3D.NoIntersection)
                    continue;
                if (p02 == Plane3D.InPlane || p02 == Plane3D.NoIntersection)
                    continue;

                if (chamferPoints != null)
                {
                    // add the chamfered points
                    if (!chamferPoints.Contains(p01))
                        chamferPoints.Add(p01);
                    if (!chamferPoints.Contains(p02))
                        chamferPoints.Add(p02);
                }

                int i01 = i0;
                // change the original triangle to use the first chamfer point
                positions[triangleIndices[i01]] = p01;

                int i02 = positions.Count;
                positions.Add(p02);
                // add a new triangle for the other chamfer point
                triangleIndices.Add(i01);
                triangleIndices.Add(i2);
                triangleIndices.Add(i02);

                // add a triangle connecting the chamfer points and the new corner point
                triangleIndices.Add(index0);
                triangleIndices.Add(i01);
                triangleIndices.Add(i02);
            }

            NoSharedVertices();
        }

        /// <summary>
        /// Finds a normal to the specified corner.
        /// </summary>
        /// <param name="p">The corner point.</param>
        /// <param name="eps">The corner search limit distance.</param>
        /// <returns></returns>
        private Vector3D FindCornerNormal(Point3D p, double eps)
        {
            var sum = new Vector3D();
            int count = 0;
            var addedNormals = new HashSet<Vector3D>();
            for (int i = 0; i < triangleIndices.Count; i += 3)
            {
                int i0 = i;
                int i1 = i + 1;
                int i2 = i + 2;
                var p0 = positions[triangleIndices[i0]];
                var p1 = positions[triangleIndices[i1]];
                var p2 = positions[triangleIndices[i2]];

                // check if any of the vertices are on the corner
                double d0 = (p - p0).LengthSquared;
                double d1 = (p - p1).LengthSquared;
                double d2 = (p - p2).LengthSquared;
                double mind = Math.Min(d0, Math.Min(d1, d2));
                if (mind > eps)
                    continue;

                // calculate the triangle normal and check if this face is already added
                var normal = Vector3D.CrossProduct(p1 - p0, p2 - p0);
                normal.Normalize();

                // todo: need to use the epsilon value to compare the normals?
                if (addedNormals.Contains(normal))
                    continue;

                // todo: this does not work yet
                //double dp = 1;
                //foreach (var n in addedNormals)
                //{
                //    dp = Math.Abs(Vector3D.DotProduct(n, normal) - 1);
                //    if (dp < eps)
                //        continue;
                //}
                //if (dp < eps)
                //{
                //    continue;
                //}

                count++;
                sum += normal;
                addedNormals.Add(normal);
            }
            if (count == 0)
                return new Vector3D();
            return sum * (1.0 / count);
        }
    }
}