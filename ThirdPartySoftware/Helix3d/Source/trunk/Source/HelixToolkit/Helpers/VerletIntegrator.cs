using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace HelixToolkit
{
    // TODO: this is experimental code, no optimizations done...

    // http://en.wikipedia.org/wiki/Verlet_integration
    // http://www.gamasutra.com/resource_guide/20030121/jacobson_01.shtml
    // http://code.google.com/p/verlet/
    // http://www.gamedev.net/reference/articles/article2200.asp

    public class VerletIntegrator
    {
        public VerletIntegrator()
        {
            Damping = 0.995f;
            Iterations = 4;

            Constraints = new List<Constraint>();
        }

        public double Damping { get; set; }
        public int Iterations { get; set; }

        public Point3D[] Positions { get; private set; }
        public Point3D[] Positions0 { get; private set; }
        public Vector3D[] Accelerations { get; private set; }
        public double[] InverseMass { get; private set; }

        public List<Constraint> Constraints { get; private set; }

        public void Resize(int n)
        {
            Positions = new Point3D[n];
            Positions0 = new Point3D[n];
            Accelerations = new Vector3D[n];
            InverseMass = new double[n];
        }

        public void FixPosition(int i)
        {
            InverseMass[i] = 0;
        }

        public void Init(MeshGeometry3D mesh)
        {
            Resize(mesh.Positions.Count);
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                Positions[i] = mesh.Positions[i];
                Positions0[i] = Positions[i];
                Accelerations[i] = new Vector3D();
            }
        }

        public void CreateConstraintsByMesh(MeshGeometry3D mesh, double relax)
        {
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int i0 = mesh.TriangleIndices[i];
                int i1 = mesh.TriangleIndices[i + 1];
                int i2 = mesh.TriangleIndices[i + 2];
                AddConstraint(i0, i1, relax);
                AddConstraint(i1, i2, relax);
                AddConstraint(i2, i0, relax);
            }
        }

        public void TransferPositions(MeshGeometry3D mesh)
        {
            lock (Positions)
            {
                var pc = new Point3DCollection();
                for (int i = 0; i < Positions.Length; i++)
                {
                    pc.Add(Positions[i]);
                }
                mesh.Positions = pc;
            }
        }

        public void AddConstraint(int A, int B, double relax)
        {
            var c = new DistanceConstraint(A, B);
            c.Restlength = (Positions[A] - Positions[B]).Length;
            c.RelaxationFactor = relax;
            c.Iterations = Iterations;
            Constraints.Add(c);
        }

        public void AddFloor(double friction)
        {
            for (int i = 0; i < Positions.Length; i++)
            {
                var c = new FloorConstraint(i, friction);
                Constraints.Add(c);
            }
        }

        public void AddSphere(Point3D center, double radius)
        {
            for (int i = 0; i < Positions.Length; i++)
            {
                var c = new SphereConstraint(i, center, radius);
                Constraints.Add(c);
            }
        }

        /*   public void InitPoint(Point3D v)
        {
            for (int i = 0; i < Positions.Length; i++)
            {
                Positions[i] = v;
                Positions0[i] = v;
                Accelerations[i] = new Vector3D();
            }
        }*/

        public void SetInverseMass(double invmass)
        {
            for (int i = 0; i < Positions.Length; i++)
                InverseMass[i] = invmass;
        }

        private double dtprev = 0;

        // Time corrected verlet integration
        // http://www.gamedev.net/reference/articles/article2200.asp

        private void Integrate(double dt)
        {
            if (dtprev == 0)
                dtprev = dt;

            lock (Positions)
            {
                for (int i = 0; i < Positions.Length; i++)
                {
                    if (InverseMass[i] == 0) continue;
                    Point3D temp = Positions[i];
                    Positions[i] += (Positions[i] - Positions0[i]) * dt / dtprev * Damping + Accelerations[i] * dt * dt;
                    Positions0[i] = temp;
                }
            }

            dtprev = dt;
        }

        public void SetForce(int index, Vector3D force)
        {
            Accelerations[index] = force * InverseMass[index];
        }

        public void ApplyGravity(Vector3D gravity)
        {
            for (int i = 0; i < Positions.Length; i++)
                Accelerations[i] = gravity * InverseMass[i];
        }

        private void SatisfyConstraints(int iteration)
        {
            foreach (Constraint c in Constraints)
            {
                c.Satisfy(this, iteration);
            }
        }

        public void TimeStep(double dt)
        {
            Integrate(dt);

            for (int j = 0; j < Iterations; j++)
            {
                SatisfyConstraints(j);
            }
        }
    }

    public abstract class Constraint
    {
        public abstract void Satisfy(VerletIntegrator vs, int iteration);
    }

    public class SphereConstraint : Constraint
    {
        public SphereConstraint(int index, Point3D center, double radius)
        {
            Index = index;
            Center = center;
            Radius = radius;
            RadiusSquared = radius * radius;
        }

        public int Index { get; set; }
        public Point3D Center { get; set; }
        public double Radius { get; set; }
        public double RadiusSquared { get; set; }

        public override void Satisfy(VerletIntegrator vs, int iteration)
        {
            Vector3D vec = Point3D.Subtract(vs.Positions[Index], Center);
            if (vec.LengthSquared < RadiusSquared)
            {
                vec.Normalize();
                vs.Positions[Index] = Center + vec * Radius * 1.1;
                vs.Positions0[Index] = vs.Positions[Index];
            }
        }
    }

    public class FloorConstraint : Constraint
    {
        public FloorConstraint(int index, double friction = 1.0)
        {
            Index = index;
            Friction = friction;
        }

        public int Index { get; set; }
        public double Friction { get; set; }

        public override void Satisfy(VerletIntegrator vs, int iteration)
        {
            int i = Index;
            Point3D x = vs.Positions[i];
            if (x.Z <= 0)
            {
                if (Friction != 0)
                {
                    double f = -x.Z * Friction;
                    Vector3D v = vs.Positions[i] - vs.Positions0[i];
                    v.Z = 0;

                    if (v.X > 0)
                    {
                        v.X -= f * v.X;
                        if (v.X < 0) v.X = 0;
                    }
                    else
                    {
                        v.X += f;
                        if (v.X > 0) v.X = 0;
                    }

                    if (v.Y > 0)
                    {
                        v.Y -= f * v.Y;
                        if (v.Y < 0) v.Y = 0;
                    }
                    else
                    {
                        v.Y += f;
                        if (v.Y > 0) v.Y = 0;
                    }

                    vs.Positions0[i] = vs.Positions[i] - v;
                }
                vs.Positions[i].Z = 0;
            }
        }
    }

    public class DistanceConstraint : Constraint
    {
        public DistanceConstraint(int A, int B)
        {
            Index1 = A;
            Index2 = B;
        }

        public int Index1 { get; set; }
        public int Index2 { get; set; }
        public double Restlength { get; set; }
        public double RelaxationFactor { get; set; }
        public int Iterations { get; set; }

        public override void Satisfy(VerletIntegrator vs, int iteration)
        {
            if (Iterations > iteration)
            {
                Point3D x1 = vs.Positions[Index1];
                Point3D x2 = vs.Positions[Index2];
                Vector3D delta = x2 - x1;

                double deltalength = delta.Length;
                double diff = (deltalength - Restlength);

                double div = deltalength * (vs.InverseMass[Index1] + vs.InverseMass[Index2]);

                if (Math.Abs(div) > 1e-8)
                {
                    diff /= div;
                    if (vs.InverseMass[Index1] != 0)
                        vs.Positions[Index1] += delta * diff * vs.InverseMass[Index1] * RelaxationFactor;
                    if (vs.InverseMass[Index2] != 0)
                        vs.Positions[Index2] -= delta * diff * vs.InverseMass[Index2] * RelaxationFactor;
                }
            }
        }
    } ;
}