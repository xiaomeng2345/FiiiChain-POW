using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.ValidationRules
{
    public abstract class RuleBase : ValidationRule
    {
        public string ErrorKey { get; set; }

        public string GetErrorMsg()
        {
            return LanguageService.Default.GetLanguageValue(ErrorKey);
        }
    }
}
