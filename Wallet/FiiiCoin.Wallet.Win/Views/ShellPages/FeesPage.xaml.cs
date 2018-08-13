using FiiiCoin.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// FeesPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class FeesPage : Page , IPage
    {
        public FeesPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.FeesPage;
        }
    }
}
