using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is DateTime))
                return null;
            var date = (DateTime)value;
            if (parameter == null)
                return date.ToString("yyyy-MM-dd hh-mm-ss");
            else
                return date.ToString(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is string))
                return value;

            DateTime date = DateTime.Now;
            DateTime.TryParse(value.ToString(), out date);

            return date;
        }
    }
}
