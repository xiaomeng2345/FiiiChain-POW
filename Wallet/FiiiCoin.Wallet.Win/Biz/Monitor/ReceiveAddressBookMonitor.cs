using FiiiCoin.Models;
using FiiiCoin.Wallet.Win.Biz.Services;
using System.Collections.Generic;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class ReceiveAddressBookMonitor : ServiceMonitorBase<List<AccountInfo>>
    {
        private static ReceiveAddressBookMonitor _default;

        public static ReceiveAddressBookMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new ReceiveAddressBookMonitor();
                return _default;
            }
        }

        protected override List<AccountInfo> ExecTaskAndGetResult()
        {
            var result = AccountsService.Default.GetAddressesByTag();
            if (result.IsFail)
                return null;
            else
                return result.Value;
        }
    }
}
