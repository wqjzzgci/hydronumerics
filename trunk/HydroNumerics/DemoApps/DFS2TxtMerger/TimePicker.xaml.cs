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

namespace Dfs2TxtMerger
{
  /// <summary>
  /// Interaction logic for TimePicker.xaml
  /// </summary>
  public partial class TimePicker : UserControl
  {
    public TimePicker()
    {
      InitializeComponent();
    }

    public TimeSpan Value
    {
      get { return (TimeSpan)GetValue(ValueProperty); }
      set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
    DependencyProperty.Register("Value", typeof(TimeSpan), typeof(TimePicker),
    new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

    private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      TimePicker control = obj as TimePicker;
      TimeSpan ts = (e.NewValue as TimeSpan?).Value;
      control.Seconds = ts.Seconds;
      control.Minutes = ts.Minutes;
      control.Hours = ts.Hours;
      control.Days = ts.Days;
    }

    public int Days
    {
      get { return (int)GetValue(DaysProperty); }
      set { SetValue(DaysProperty, value); }
    }
    public static readonly DependencyProperty DaysProperty =
    DependencyProperty.Register("Days", typeof(int), typeof(TimePicker),
    new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));


    public int Hours
    {
      get { return (int)GetValue(HoursProperty); }
      set { SetValue(HoursProperty, value); }
    }
    public static readonly DependencyProperty HoursProperty =
    DependencyProperty.Register("Hours", typeof(int), typeof(TimePicker),
    new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

    public int Minutes
    {
      get { return (int)GetValue(MinutesProperty); }
      set { SetValue(MinutesProperty, value); }
    }
    public static readonly DependencyProperty MinutesProperty =
    DependencyProperty.Register("Minutes", typeof(int), typeof(TimePicker),
    new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

    public int Seconds
    {
      get { return (int)GetValue(SecondsProperty); }
      set { SetValue(SecondsProperty, value); }
    }

    public static readonly DependencyProperty SecondsProperty =
    DependencyProperty.Register("Seconds", typeof(int), typeof(TimePicker),
    new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));


    private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
      TimePicker control = obj as TimePicker;
   
      control.Value = TimeSpan.FromDays(control.Days) + TimeSpan.FromHours(control.Hours) + TimeSpan.FromMinutes(control.Minutes) + TimeSpan.FromSeconds(control.Seconds);
    }

    private void Down(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Up || args.Key == Key.Down)
      {
        args.Handled = true;

        switch (((TextBox)sender).Name)
        {
          case "sec":
            if (args.Key == Key.Up)
              this.Seconds++;
            if (args.Key == Key.Down)
              this.Seconds--;
            break;

          case "min":
            if (args.Key == Key.Up)
              this.Minutes++;
            if (args.Key == Key.Down)
              this.Minutes--;
            break;

          case "hour":
            if (args.Key == Key.Up)
              this.Hours++;
            if (args.Key == Key.Down)
              this.Hours--;
            break;
          case "day":
            if (args.Key == Key.Up)
              this.Days++;
            if (args.Key == Key.Down)
              this.Days--;
            break;
        }
      }
    }

  }
}
