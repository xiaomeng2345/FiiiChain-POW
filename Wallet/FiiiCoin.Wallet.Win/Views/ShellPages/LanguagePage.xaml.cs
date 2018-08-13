using FiiiCoin.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// LanguageView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class LanguagePage : Page , IPage
    {
        public LanguagePage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.LanguagePage;
        }
    }
}
