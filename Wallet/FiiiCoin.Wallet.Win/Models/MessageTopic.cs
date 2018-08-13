using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Models
{
    public enum MessageTopic
    {
        UpdatePopupView,
        UpdateMainView,
        ChangedPopupViewState,
        ShowMessageAutoClose,
        ClosePopUpWindow,
    }

    public enum SendMessageTopic
    {
        Refresh
    }

    public enum CommonTopic
    {
        UpdateWalletStatus,
        ExportBackUp,
    }


    public enum InputWalletPwdPageTopic
    {
        Normal,
        UnLockWallet,
        RequestPassword,
    }
}
