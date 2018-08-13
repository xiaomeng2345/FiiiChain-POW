using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Models;
using System.Collections.ObjectModel;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class TradeRecodesMonitor : ServiceMonitorBase<ObservableCollection<TradeRecordInfo>>
    {
        private static TradeRecodesMonitor _default;

        public static TradeRecodesMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new TradeRecodesMonitor();
                return _default;
            }
        }



        protected override ObservableCollection<TradeRecordInfo> ExecTaskAndGetResult()
        {
            var result = new ObservableCollection<TradeRecordInfo>();
            var tradeRecordsResult = FiiiCoinService.Default.GetListTransactions("*", 0, true, 1000);
            if (!tradeRecordsResult.IsFail)
            {
                return tradeRecordsResult.Value;
            }
            else
                return null;
        }
    }
}
