using FiiiCoin.Models;
using FiiiCoin.Wallet.Win.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace FiiiCoin.Wallet.Win.Converters
{
    public class PaymentToTradeAmountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Payment))
                return null;

            var payment = value as Payment;
            var categoryType = (PaymentCategoryType)Enum.Parse(typeof(PaymentCategoryType), payment.Category);
            var amount = payment.Amount;
            switch (categoryType)
            {
                case PaymentCategoryType.self:
                    amount = -payment.Fee;
                    break;
                case PaymentCategoryType.generate:
                case PaymentCategoryType.receive:
                default:
                    break;
                case PaymentCategoryType.send:
                    amount = -payment.Amount;
                    break;
            }

            var result = amount / Math.Pow(10, 8);
            if (result > 0)
                return "+" + result.ToString("0.00000000");
            else if (result < 0)
                return result.ToString("0.00000000");
            else
                return "0.00000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }



    public class PaymentToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Payment))
                return null;

            var payment = value as Payment;
            var categoryType = (PaymentCategoryType)Enum.Parse(typeof(PaymentCategoryType), payment.Category);
            var result = 0;
            switch (categoryType)
            {
                case PaymentCategoryType.self:
                case PaymentCategoryType.send:
                    result = -1;
                    break;
                case PaymentCategoryType.generate:
                case PaymentCategoryType.receive:
                    result = 1;
                    break;
                default:
                    result = 0;
                    break;
            }
            
            if (result >= 0)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#333333"));
            else
                return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
