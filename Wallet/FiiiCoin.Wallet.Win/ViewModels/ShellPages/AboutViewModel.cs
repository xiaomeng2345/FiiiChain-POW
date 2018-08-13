using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class AboutViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.AboutPage;
        }
    }
}
