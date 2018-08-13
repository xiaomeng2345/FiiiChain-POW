using FiiiCoin.Wallet.Win.Common;
using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Interop;

namespace FiiiCoin.Wallet.Win.Views.ShellPages
{
    /// <summary>
    /// AboutView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IPage))]
    public partial class AboutPage : Page ,IPage
    {
        public AboutPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                webBrowser.ObjectForScripting = new WpfSender();
            };
        }

        public Page GetCurrentPage()
        {
            return this;
        }

        public string GetPageName()
        {
            return Pages.AboutPage;
        }
    }

    [ComVisible(true)]
    public class WpfSender
    {
        public void Jump(string url)
        {
            System.Diagnostics.Process.Start(url);
        }
    }
}
