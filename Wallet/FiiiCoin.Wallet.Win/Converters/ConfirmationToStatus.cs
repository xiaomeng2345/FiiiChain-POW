using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class ConfirmationToStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            var i = 0;
            if (int.TryParse(value.ToString(),out i))
            {
                if (i <= 0)
                    return LanguageService.Default.GetLanguageValue("converter_unconfirmed");
                if (i >= 6)
                    return LanguageService.Default.GetLanguageValue("converter_confirmed");

                return string.Format(LanguageService.Default.GetLanguageValue("converter_confirming"), i);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
