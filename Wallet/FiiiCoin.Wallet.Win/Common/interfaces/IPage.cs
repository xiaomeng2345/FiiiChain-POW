using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Common
{
    public interface IPage
    {
        string GetPageName();

        Page GetCurrentPage();
    }
}
