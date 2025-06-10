using System;
using Microsoft.UI.Xaml.Data;

namespace XCVTransformer.AuxClasses
{
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => value is bool b ? !b : value;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => value is bool b ? !b : value;
    }
}
