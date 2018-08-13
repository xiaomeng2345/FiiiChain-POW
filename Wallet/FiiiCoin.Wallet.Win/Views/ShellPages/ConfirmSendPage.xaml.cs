using FiiiCoin.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// ConfirmSendPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class ConfirmSendPage : Page,IPage
    {
        public ConfirmSendPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.ConfirmSendPage;
        }
    }
}
