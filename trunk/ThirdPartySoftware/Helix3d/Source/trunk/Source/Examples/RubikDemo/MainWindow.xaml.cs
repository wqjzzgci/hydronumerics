using System;
using System.Windows;
using System.Windows.Input;

namespace RubikDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            view1.ZoomToFit();
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            info.Text = "";
            if (e.Key == Key.Back && cube1.CanUnscramble())
                cube1.Unscramble();
            if (e.Key == Key.Space)
                cube1.Scramble();
            if (e.Key == Key.Add)
                cube1.Size++;
            if (e.Key == Key.Subtract && cube1.Size>2)
                cube1.Size--;
            cube1.Rotate(e.Key);
        }

    }
}