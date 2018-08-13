using FiiiCoin.Models;
using FiiiCoin.Wallet.Win.Biz.Services;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class TxSettingMonitor : ServiceMonitorBase<TransactionFeeSetting>
    {
        private static TxSettingMonitor _default;

        public static TxSettingMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new TxSettingMonitor();
                return _default;
            }
        }

        protected override TransactionFeeSetting ExecTaskAndGetResult()
        {
            var feeResult = FiiiCoinService.Default.GetTxSettings();

            if (!feeResult.IsFail)
            {
                return feeResult.Value;
            }
            else
                return null;
        }
    }
}
