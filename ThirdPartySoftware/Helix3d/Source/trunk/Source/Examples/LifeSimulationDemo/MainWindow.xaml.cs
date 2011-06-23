using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LifeSimulationDemo
{
    // http://en.wikipedia.org/wiki/Life_simulation_game

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Creature> Creatures = new List<Creature>();
        List<Creature> newCreatures = new List<Creature>();
        private Stopwatch watch;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
            watch = new Stopwatch();
            watch.Start();
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            AddCreature(-5, -5, Gender.Female);
            AddCreature(-5, 5, Gender.Female);
            AddCreature(5, -5, Gender.Male);
            AddCreature(5, 5, Gender.Male);
            AddPredator(10, -2, Gender.Male);
            AddPredator(10, 2, Gender.Female);
        }

        private void AddCreature(double x, double y, Gender g)
        {
            var c = new Creature(new Point3D(x, y, 0), null, null);
            c.Gender = g;
            Add(c);
        }
        private void AddPredator(double x, double y, Gender g)
        {
            var c = new Predator(new Point3D(x, y, 0), null, null);
            c.Gender = g;
            Add(c);
        }

        private void Add(Creature c)
        {
            c.ReproduceAction = reproduceAction;

            Creatures.Add(c);
            view1.Children.Add(c);
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            double dt = 1.0 * watch.ElapsedTicks / Stopwatch.Frequency;
            foreach (var c in Creatures)
                c.Update(dt, Creatures);
            foreach (var c in newCreatures)
                Creatures.Add(c);
            newCreatures.Clear();

            var toDelete = new List<Creature>();
            foreach (var c in Creatures)
                if (c.Age > c.LifeTime + 30)
                    toDelete.Add(c);
            foreach (var c in toDelete)
            {
                Creatures.Remove(c);
                view1.Children.Remove(c);
            }

            watch.Restart();
        }

        private void reproduceAction(Creature mother, Creature father)
        {
            Creature c;
            if (mother.GetType() == typeof(Creature))
                c = new Creature(mother.Position, mother, father);
            else
                c = new Predator(mother.Position, mother, father);
            c.ReproduceAction = reproduceAction;

            newCreatures.Add(c);
            view1.Children.Add(c);
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //     view1.ZoomToFit();
        }

        private void view1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var pt = view1.FindNearestPoint(e.GetPosition(view1));
            if (!pt.HasValue)
                return;
            Creature c;
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                c = new Predator(new Point3D(pt.Value.X, pt.Value.Y, 0), null, null);
            else
                c = new Creature(new Point3D(pt.Value.X, pt.Value.Y, 0), null, null);
            Add(c);
        }

    }

    public enum Gender
    {
        Male,
        Female
    } ;

    public enum CreatureState
    {
        Egg,
        Child,
        Adult,
        Dead
    }
}
