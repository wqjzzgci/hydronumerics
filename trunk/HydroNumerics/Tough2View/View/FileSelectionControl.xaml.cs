using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HydroNumerics.Tough2.View
{
	/// <summary>
	/// Interaction logic for FileSelectionControl.xaml
	/// </summary>
	public partial class FileSelectionControl : UserControl
	{
		public FileSelectionControl()
		{
			this.InitializeComponent();
		}

    public static DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileSelectionControl), new PropertyMetadata(null));

    public string FileName
    {
      get { return GetValue(FileNameProperty) as string; }
      set { SetValue(FileNameProperty, value); }
    }


    public string LabelName
    {
      get
      {
        return Label1.Content.ToString();
      }
      set
      {
        Label1.Content = value;
      }
    }

    public string FilterString { get; set; }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
            
      Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
      openFileDialog.Multiselect = false;
      openFileDialog.Filter = FilterString;

      if (openFileDialog.ShowDialog().Value)
      {
        FileName = openFileDialog.FileName;
      }
    }
	}
}