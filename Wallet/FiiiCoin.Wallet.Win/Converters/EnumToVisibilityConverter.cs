using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(value is Enum) || !(parameter is Enum))
                return Visibility.Visible;

            if (!ReferenceEquals(value, parameter))
                return Visibility.Visible;

            var valueEnum = value as Enum;
            var parameterEnum = parameter as Enum;

            if (valueEnum.HasFlag(parameterEnum))
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
