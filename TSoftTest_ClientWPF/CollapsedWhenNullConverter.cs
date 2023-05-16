using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TSoftTest_ClientWPF;

public class CollapsedWhenNullConverter: MarkupExtension, IValueConverter
{
    public override object ProvideValue(IServiceProvider serviceProvider) => this;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Equals(value, null) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}