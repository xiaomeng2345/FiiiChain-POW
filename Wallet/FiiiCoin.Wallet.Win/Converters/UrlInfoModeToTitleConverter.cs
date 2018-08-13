using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models.UiModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class UrlInfoModeToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is UrlMode))
                return null;
            var val = (UrlMode)value;
            var result = "";
            switch (val)
            {
                case UrlMode.CreatePay:
                    result = LanguageService.Default.GetLanguageValue("CreatePayUrl");
                    break;
                case UrlMode.Edit:
                    result = LanguageService.Default.GetLanguageValue("EditPayUrl");
                    break;
                case UrlMode.CreateByReceive:
                    result = LanguageService.Default.GetLanguageValue("CreateByReceive");
                    break;
                case UrlMode.EditByReceive:
                    result = LanguageService.Default.GetLanguageValue("EditByReceive");
                    break;
                default:
                    break;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
