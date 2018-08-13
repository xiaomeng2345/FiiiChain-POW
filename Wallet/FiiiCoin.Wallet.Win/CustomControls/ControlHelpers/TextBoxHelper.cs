using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FiiiCoin.Wallet.Win.CustomControls.ControlHelpers
{
    public class TextBoxHelper
    {
        public static readonly DependencyProperty NullMsgProperty =
            DependencyProperty.RegisterAttached("NullMsg", typeof(string), typeof(TextBoxHelper), new PropertyMetadata(""));
        
        public static string GetNullMsg(DependencyObject obj)
        {
            return (string)obj.GetValue(NullMsgProperty);
        }

        public static void SetNullMsg(DependencyObject obj, string value)
        {
            obj.SetValue(NullMsgProperty, value);
        }
    }
}
