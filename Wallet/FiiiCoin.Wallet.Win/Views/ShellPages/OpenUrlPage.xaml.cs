using FiiiCoin.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// OpenUrlView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class OpenUrlPage : Page, IPage
    {
        public OpenUrlPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.OpenUrlPage;
        }
    }
}
