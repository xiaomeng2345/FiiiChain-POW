using FiiiCoin.Wallet.Win.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FiiiCoin.Wallet.Win.Views
{
    /// <summary>
    /// SendPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class SendPage : Page , IPage
    {
        public SendPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.SendPage;
        }
    }
}
