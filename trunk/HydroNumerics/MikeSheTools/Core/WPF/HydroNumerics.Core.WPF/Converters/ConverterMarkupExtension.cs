using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace HydroNumerics.Core.WPF
{
  /// <summary>
  /// Base class for value converters.
  /// This class provides the following features:
  /// - Allow the derived value converter to be used as a markup extension,
  ///   thus making the XAML code much more simpler:
  ///   Instead of: 
  ///      Container.Resources
  ///         src:MyConverter x:Key="MyConverter" 
  ///      Container.Resources
  ///      
  ///      Converter={StaticResource MyConverter}
  ///      
  ///   Use: Converter={src:MyConverter} 
  ///   
  /// - The derived converter has only one instance (singleton).
  /// - Default implementation for unsupported value conversions (throws NotImplementedException);
  /// </summary>
  /// <typeparam name="T">Your value converter</typeparam>
  public abstract class ConverterMarkupExtension<T> : MarkupExtension, IValueConverter, IMultiValueConverter
      where T : class, new()
  {
    /// <summary>
    /// Holds reference to the converter single instance
    /// </summary>
    private static T _converter = null;

    /// <summary>
    /// Returns an object that is set as the value of the target property for this markup extension.
    /// </summary>
    /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
    /// <returns>The converter to set on the property where the extension is applied.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (_converter == null)
      {
        _converter = new T();
      }

      return _converter;
    }

    #region IValueConverter Members

    /// <summary>
    /// Converts a value.
    /// Default implementation is to throw NotImplementedException.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Converts a value back.
    /// Default implementation is to throw NotImplementedException.
    /// </summary>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region IMultiValueConverter Members

    /// <summary>
    /// Converts source values to a value for the binding target. The data binding
    /// engine calls this method when it propagates the values from source bindings
    /// to the binding target.
    /// Default implementation is to throw NotImplementedException.
    /// </summary>
    /// <param name="value">The array of values that the source bindings in the System.Windows.Data.MultiBinding
    /// produces. The value System.Windows.DependencyProperty.UnsetValue indicates
    /// that the source binding has no value to provide for conversion.</param>
    /// <param name="targetTypes">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A converted value.If the method returns null, the valid null value is used.
    /// A return value of System.Windows.DependencyProperty.System.Windows.DependencyProperty.UnsetValue
    /// indicates that the converter did not produce a value, and that the binding
    /// will use the System.Windows.Data.BindingBase.FallbackValue if it is available,
    /// or else will use the default value.A return value of System.Windows.Data.Binding.System.Windows.Data.Binding.DoNothing
    /// indicates that the binding does not transfer the value or use the System.Windows.Data.BindingBase.FallbackValue
    /// or the default value.</returns>
    public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Converts a binding target value to the source binding values.
    /// Default implementation is to throw NotImplementedException.
    /// </summary>
    /// <param name="value">The value that the binding target produces.</param>
    /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and
    /// types of values that are suggested for the method to return.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>An array of values that have been converted from the target value back to
    /// the source values.</returns>
    public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}