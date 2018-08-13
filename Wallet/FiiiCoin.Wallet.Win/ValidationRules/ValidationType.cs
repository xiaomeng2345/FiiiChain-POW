using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.ValidationRules
{
    public enum ValidationType
    {
        Error_NotNull,
        Error_Password,
        Error_PasswordDifferent,
        Error_Address,
        Error_OutofRange,
        Error_Amount
    }
}
