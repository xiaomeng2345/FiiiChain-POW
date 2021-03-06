﻿using FiiiCoin.Wallet.Win.Common;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// TradeDetailMiningPage.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class TradeDetailMiningPage : Page ,IPage
    {
        public TradeDetailMiningPage()
        {
            InitializeComponent();
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.TradeDetailMiningPage;
        }
    }
}
