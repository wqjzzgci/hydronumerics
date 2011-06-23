using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace LifeSimulationDemo
{
    public class Creature : ModelVisual3D
    {
        public Gender Gender { get; set; }
        public CreatureState State { get; set; }
        public double Age { get; set; }
        public double CurrentHeight { get; set; }
        public double GrownupHeight { get; set; }
        public double Mass { get; set; }
        public double BMI { get; set; }

        public Point3D Position { get; set; }
        public double Heading { get; set; }
        public double Speed { get; set; }
        public double AngularSpeed { get; set; }
        public double Acceleration { get; set; }

        public double MaxSpeed { get; set; }
        public double Fitness { get; set; }
        public double Energy { get; set; }
        public double Agility { get; set; }

        public double BirthTime { get; set; }
        public double ChildhoodDuration { get; set; }
        public double LifeTime { get; set; }
        public double LastBabyTime { get; set; }

        public double PrivacyDist { get; set; }
        public double IncubationDuration { get; set; }

        public Creature Mother { get; set; }
        public Creature Father { get; set; }

        public Action<Creature, Creature> ReproduceAction { get; set; }

        protected GeometryModel3D model;
        private ScaleTransform3D scale;
        private TranslateTransform3D translation;
        private AxisAngleRotation3D rotation;
        private static MeshGeometry3D eggGeometry;
        private static MeshGeometry3D childGeometry;
        private static MeshGeometry3D creatureGeometry;
        private static MeshGeometry3D deadGeometry;
        private static Material eggMaterial;
        private static Material maleMaterial;
        private static Material femaleMaterial;
        private static Material deadMaterial;

        static Creature()
        {
            var egg = new MeshBuilder();
            egg.AddSphere(new Point3D(0, 0, 0.2), 0.2, 24, 12);
            eggGeometry = egg.ToMesh();

            var child = new MeshBuilder();
            child.AddSphere(new Point3D(0, 0, 1), 1, 24, 12);
            child.AddSphere(new Point3D(1, 0, 1), 0.5, 24, 12);
            child.AddSphere(new Point3D(1.5, 0.2, 1), 0.1, 24, 12);
            child.AddSphere(new Point3D(1.5, -0.2, 1), 0.1, 24, 12);
            childGeometry = child.ToMesh();

            var creature = new MeshBuilder();
            creature.AddSphere(new Point3D(0, 0, 1), 1, 24, 12);
            creature.AddSphere(new Point3D(0, 0, 2), 0.5, 24, 12);
            creature.AddSphere(new Point3D(0.5, 0.2, 2), 0.1, 24, 12);
            creature.AddSphere(new Point3D(0.5, -0.2, 2), 0.1, 24, 12);
            creatureGeometry = creature.ToMesh();

            var coffin = new MeshBuilder();
            coffin.AddBox(new Point3D(0, 0, 0.25), 1, 1, 1);
            deadGeometry = coffin.ToMesh();

            maleMaterial = MaterialHelper.CreateMaterial(Brushes.Blue);
            femaleMaterial = MaterialHelper.CreateMaterial(Brushes.IndianRed);
            eggMaterial = MaterialHelper.CreateMaterial(Brushes.AntiqueWhite);
            deadMaterial = MaterialHelper.CreateMaterial(Brushes.Black);

        }

        public Creature(Point3D position, Creature mother, Creature father)
        {
            Mother = mother;
            Father = father;
            Gender = randomizer.Next(2) == 0 ? Gender.Male : Gender.Female;

            Position = position;

            Heading = 0;
            Speed = 0;
            AngularSpeed = 0;
            Acceleration = 0;
            BirthTime = GetRandom(5, 3);
            ChildhoodDuration = GetRandom(20, 10);
            LifeTime = GetRandom(90, 30);
            PrivacyDist = 2.5;
            double bmiMean = 25;
            double maxSpeedMean = 3;
            double heightMean = 1.75;
            if (mother != null)
            {
                heightMean = (Mother.GrownupHeight + Father.GrownupHeight) / 2;
                bmiMean = (Mother.BMI + Father.BMI) / 2;
                maxSpeedMean = (Mother.MaxSpeed + Father.MaxSpeed) / 2;
            }
            BMI = GetRandom(bmiMean, bmiMean / 2);
            GrownupHeight = GetRandom(heightMean, 0.5);
            MaxSpeed = Gender == Gender.Male ? GetRandom(maxSpeedMean + 2, 1) : GetRandom(maxSpeedMean, 1);
            Agility = GetRandom(15, 10);
            Fitness = GetRandom(1, 0.5);
            Energy = 1;

            model = new GeometryModel3D();

            translation = new TranslateTransform3D();
            scale = new ScaleTransform3D();
            var rotationT = new RotateTransform3D();
            rotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            rotationT.Rotation = rotation;

            scale.ScaleX = 1;
            scale.ScaleY = 1;
            scale.ScaleZ = 1;

            var tg = new Transform3DGroup();
            tg.Children.Add(scale);
            tg.Children.Add(rotationT);
            tg.Children.Add(translation);
            model.Transform = tg;

            Content = model;
        }

        public bool IsBorn()
        {
            return Age > BirthTime;
        }

        public bool IsBaby()
        {
            return Age < BirthTime + ChildhoodDuration;
        }

        private static Random randomizer = new Random();
        private double GetRandom(double mean, double stdev)
        {
            double dev = stdev > 0 ? randomizer.NextDouble() * stdev * 2 - stdev : 0;
            return mean + dev;
        }

        public void Update(double dt, IList<Creature> creatures)
        {
            Age += dt;
            UpdateState();
            UpdateGeometry();
            UpdateMass();

            if (IncubationDuration > 0)
            {
                Speed = 0;
                IncubationDuration -= dt;
                return;
            }

            Energy += Fitness * dt;
            Energy -= Speed / MaxSpeed * dt;

            // Integrate
            double phi = Heading / 180 * Math.PI;
            var heading = new Vector3D(Math.Cos(phi), Math.Sin(phi), 0);

            var force = GetForce(creatures);
            Acceleration = force.Length / Mass;

            double angle = Math.Atan2(force.Y, force.X) / Math.PI * 180;
            double anglediff = angle - Heading;
            if (anglediff > 180) anglediff -= 360;
            if (anglediff < -180) anglediff += 360;

            //AngularSpeed += 0.1*anglediff * dt;
            AngularSpeed = AngularSpeed * 0.9 + Agility * anglediff * dt;

            Position = Position + heading * (Speed * dt);

            Speed += Acceleration * dt;
            Speed *= Energy;

            if (Speed > MaxSpeed)
                Speed = MaxSpeed;
            Heading += AngularSpeed * dt;

            translation.OffsetX = Position.X;
            translation.OffsetY = Position.Y;
            translation.OffsetZ = Position.Z;

            double xyscale = CurrentHeight * BMI / 25;
            scale.ScaleX = xyscale;
            scale.ScaleY = xyscale;
            scale.ScaleZ = CurrentHeight;

            rotation.Angle = Heading;
        }

        public void Kill()
        {
            State = CreatureState.Dead;
        }

        public virtual bool Attack(Creature c)
        {
            return false;
        }

        public virtual bool Loves(Creature c)
        {
            return Gender == Gender.Male && c.Gender == Gender.Female && c.IsReproductive();
        }

        private void UpdateState()
        {
            if (Age > LifeTime || State == CreatureState.Dead)
            {
                State = CreatureState.Dead;
                CurrentHeight = 1;
                Speed = 0;
                AngularSpeed = 0;
            }
            else
                if (Age > BirthTime + ChildhoodDuration)
                {
                    State = CreatureState.Adult;
                    CurrentHeight = GrownupHeight;
                }
                else
                    if (Age > BirthTime)
                    {
                        State = CreatureState.Child;
                        CurrentHeight = 0.1 + (GrownupHeight - 0.1) * (Age - BirthTime) / ChildhoodDuration;
                    }
                    else
                    {
                        State = CreatureState.Egg;
                        CurrentHeight = 1;
                    }

        }
        protected virtual void UpdateGeometry()
        {
            if (State == CreatureState.Egg && model.Geometry != eggGeometry)
            {
                model.Geometry = eggGeometry;
                model.Material = eggMaterial;
                model.BackMaterial = eggMaterial;
            }
            if (State == CreatureState.Child && model.Geometry != childGeometry)
            {
                model.Geometry = childGeometry;
                model.Material = Gender == Gender.Male ? maleMaterial : femaleMaterial;
                model.BackMaterial = model.Material;
            }
            if (State == CreatureState.Adult && model.Geometry != creatureGeometry)
            {
                model.Geometry = creatureGeometry;
                model.Material = Gender == Gender.Male ? maleMaterial : femaleMaterial;
                model.BackMaterial = model.Material;
            }
            if (State == CreatureState.Dead && model.Geometry != deadGeometry)
            {
                model.Geometry = deadGeometry;
                model.Material = deadMaterial;
                model.BackMaterial = model.Material;
            }
        }

        private void UpdateMass()
        {
            Mass = BMI * CurrentHeight * CurrentHeight;
            /*            if (IsBaby())
                            Mass = Age;
                        if (Age > 300)
                            Mass *= 0.99;*/
        }

        private Vector3D GetForce(IList<Creature> creatures)
        {
            var total = new Vector3D(0, 0, 0);
            if (State == CreatureState.Egg || State == CreatureState.Dead)
                return total;


            Point3D totPos = new Point3D();
            foreach (var c in creatures)
            {
                Point3D.Add(totPos, new Vector3D(c.Position.X, c.Position.Y, c.Position.Z));
            }
            int n = creatures.Count();
            var center = new Point3D(totPos.X / n, totPos.Y / n, totPos.Z / n);
            var cdir = Point3D.Subtract(center, Position);
            double cf = 10 * cdir.LengthSquared;
            cdir.Normalize();
            //total += cdir;


            foreach (var c in creatures)
            {
                if (c == this)
                    continue;
                var dir = Point3D.Subtract(c.Position, Position);
                double distance = dir.Length;
                double distanceSquared = dir.LengthSquared;

                if (distance < 1e-6)
                    continue;

                dir.Normalize();
                double attraction = GetAttraction(c);

                if (distance < PrivacyDist && !Attack(c) && !Loves(c))
                {
                    attraction = -100;
                }
                ProximityCheck(c, distance);

                var f = 100 * attraction / Math.Sqrt(distanceSquared);
                var force = dir * f;
                total += force;
            }
            return total;
        }

        protected virtual void ProximityCheck(Creature c, double distance)
        {
            if (distance < PrivacyDist)
            {
                if (Gender == Gender.Female && IsReproductive()
                        && c.Gender == Gender.Male && c.IsReproductive()
                        && !IsInFamily(c)
                        && GetType() == c.GetType())
                {
                    ReproduceAction(this, c);
                    LastBabyTime = Age;
                    IncubationDuration = 2;
                }
            }
        }

        private bool IsInFamily(Creature creature)
        {
            if (Mother == null)
                return false;
            if (Mother == creature.Mother)
                return true;
            if (Mother == creature)
                return true;
            if (this == creature.Mother)
                return true;
            return false;
        }

        protected virtual double GetAttraction(Creature c)
        {
            // Children stay close to their mother
            if (c == Mother && IsChild() && !Mother.IsDead())
                return 6;

            // Brothers and sisters are close when they are children
            if (Mother != null && Mother == c.Mother && IsChild())
                return 4;

            // They are really scared of predators
            if (GetType() == typeof(Creature) && c.GetType() == typeof(Predator))
                return -10;

            // Children play together
            if (State == CreatureState.Child && c.IsChild())
                return 2;

            // They are not interested in the dead
            if (c.State == CreatureState.Dead)
                return -1;

            // Males are really attracted to reproductive females
            if (Gender == Gender.Male && IsReproductive() && c.Gender == Gender.Female && c.IsReproductive())
                return 8;

            // Scared of other types
            if (GetType() != c.GetType())
            {
                return -5;
            }

            if (Gender == Gender.Female)
            {
                if (c.Gender == Gender.Male)
                    return 2;
                else
                    return 2;
            }
            else
            {
                if (c.Gender == Gender.Male)
                    return -4;
                else
                    if (Loves(c))
                        return 4 * c.Fitness;
            }
            return 0;
        }

        private bool IsChild()
        {
            return State == CreatureState.Child;
        }

        private bool IsDead()
        {
            return State == CreatureState.Dead;
        }
        private bool IsReproductive()
        {
            if (LastBabyTime + IncubationDuration > Age)
                return false;

            return State == CreatureState.Adult;
        }

        public bool IsAlive()
        {
            return State == CreatureState.Adult || State == CreatureState.Child;
        }
    }
}