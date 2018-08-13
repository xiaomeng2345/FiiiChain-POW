using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class SubStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int len = 0;
            if (value == null || parameter == null || !int.TryParse(parameter.ToString(), out len))
                return value;

            var text = value.ToString();
            if (text.Length <= len)
                return text;
            else
                return text.Substring(0, len);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
