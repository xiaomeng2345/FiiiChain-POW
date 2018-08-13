using FiiiCoin.Wallet.Win.Common;
using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views
{
    /// <summary>
    /// PopupShell.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class PopupShell : Page,IPage
    {
        public PopupShell()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.PopupShell;
        }
    }
}
