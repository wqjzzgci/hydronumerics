using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit;

namespace LifeSimulationDemo
{
    public class Predator : Creature
    {
        private static MeshGeometry3D eggGeometry;
        private static MeshGeometry3D childGeometry;
        private static MeshGeometry3D creatureGeometry;
        private static MeshGeometry3D deadGeometry;
        private static Material eggMaterial;
        private static Material maleMaterial;
        private static Material femaleMaterial;
        private static Material deadMaterial;

        static Predator()
        {
            var egg = new MeshBuilder();
            egg.AddPyramid(new Point3D(0, 0, 0), 0.2, 0.4);
            eggGeometry = egg.ToMesh();

            var child = new MeshBuilder();
            child.AddBox(new Point3D(0, 0, 0.5), 1, 1, 1);
            child.AddSphere(new Point3D(0.2, 0.2, 1), 0.2, 12, 24);
            child.AddSphere(new Point3D(0.2, -0.2, 1), 0.2, 12, 24);
            childGeometry = child.ToMesh();

            var creature = new MeshBuilder();
            creature.AddBox(new Point3D(0, 0, 0.5), 1, 1, 1);
            creature.AddCone(new Point3D(0.2, 0.2, 1), new Vector3D(0, 0, 1), 0.16, 0.04, 0.3, false, true, 12);
            creature.AddCone(new Point3D(0.2, -0.2, 1), new Vector3D(0, 0, 1), 0.16, 0.04, 0.3, false, true, 12);
            creatureGeometry = creature.ToMesh();

            var coffin = new MeshBuilder();
            coffin.AddCylinder(new Point3D(0, 0, 0), new Point3D(0, 0, 0.25), 1, 12);
            deadGeometry = coffin.ToMesh();

            maleMaterial = MaterialHelper.CreateMaterial(Brushes.Navy);
            femaleMaterial = MaterialHelper.CreateMaterial(Brushes.Violet);
            eggMaterial = MaterialHelper.CreateMaterial(Brushes.AntiqueWhite);
            deadMaterial = MaterialHelper.CreateMaterial(Brushes.Black);

        }

        public Predator(Point3D position, Creature mother, Creature father)
            : base(position, mother, father)
        {

        }

        protected override void ProximityCheck(Creature creature, double distance)
        {
            base.ProximityCheck(creature, distance);
            if (State == CreatureState.Adult && creature.GetType() == typeof(Creature) && distance < creature.PrivacyDist * 0.7)
                creature.Kill();
        }

        public override bool Attack(Creature c)
        {
            if (State == CreatureState.Adult && c.GetType() == typeof(Creature) && c.IsAlive())
                return true;
            return base.Attack(c);
        }
        
        protected override double GetAttraction(Creature c)
        {
            // Adults try to kill Creatures
            if (Attack(c))
                return 10;
            return base.GetAttraction(c);
        }

        protected override void UpdateGeometry()
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
    }
}