using System;
using System.Collections.Generic;
using System.Linq;
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

using HydroNumerics.MikeSheTools.ViewModel;

namespace HydroNumerics.MikeSheTools.WellViewer
{
  /// <summary>
  /// Interaction logic for ChangeMetaData.xaml
  /// </summary>
  public partial class ChangeMetaData : UserControl
  {
    public ChangeMetaData()
    {
      InitializeComponent();
      DataContextChanged += new DependencyPropertyChangedEventHandler(ChangeMetaData_DataContextChanged);
    }

    void ChangeMetaData_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ChangeDescriptionViewModel cdvm = DataContext as ChangeDescriptionViewModel;

      if (cdvm != null)
      {
        if (cdvm.ValidComments != null)
        {
          if (cdvm.ValidComments.Count > 0)
          {
            FixedCommentsSection.Visibility = System.Windows.Visibility.Visible;
            FirstComment.ItemsSource = cdvm.ValidComments[0];
            Binding b = new Binding("FirstFixedComment");
            b.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(FirstComment, ComboBox.SelectedItemProperty,b);
          }
          if (cdvm.ValidComments.Count > 1)
          {
            SecondComment.Visibility = System.Windows.Visibility.Visible;
            SecondComment.ItemsSource = cdvm.ValidComments[1];
            Binding b = new Binding("SecondFixedComment");
            b.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(SecondComment, ComboBox.SelectedItemProperty,b);
          }
        }
      }
    }

  }
}
