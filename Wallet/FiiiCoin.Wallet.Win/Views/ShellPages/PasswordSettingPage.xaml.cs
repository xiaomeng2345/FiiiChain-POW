using FiiiCoin.Wallet.Win.Common;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// PasswordSettingView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class PasswordSettingPage : Page, IPage
    {
        public PasswordSettingPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.PasswordSettingPage;
        }
    }
}
