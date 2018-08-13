using FiiiCoin.Wallet.Win.Common;
using GFramework.BlankWindow;
using System.ComponentModel.Composition;
using System.Windows;

namespace FiiiCoin.Wallet.Win
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IShell))]
    public partial class Shell : BlankWindow, IShell
    {
        public Shell()
        {
            InitializeComponent();
        }

        public Window GetWindow()
        {
            return this;
        }

        private void Menu_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is System.Windows.Controls.MenuItem)
            {
                e.Handled = true;
            }
        }
    }
}
