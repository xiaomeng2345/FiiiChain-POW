﻿using FiiiCoin.Wallet.Win.Biz.Services;

namespace FiiiCoin.Wallet.Win.Biz.Monitor
{
    public class AmountMonitor : ServiceMonitorBase<WalletAmountData>
    {
        private static AmountMonitor _default;

        public static AmountMonitor Default
        {
            get
            {
                if (_default == null)
                    _default = new AmountMonitor();
                return _default;
            }
        }

        protected override WalletAmountData ExecTaskAndGetResult()
        {
            WalletAmountData walletAmountData = new WalletAmountData();
            var totalAmountResult = UtxoService.Default.GetTxOutSetOM();

            if (!totalAmountResult.IsFail)
                walletAmountData.CanUseAmount = totalAmountResult.Value.Total_amount;
            else
                return null;
            
            var waitMoneyResult = UtxoService.Default.GetTradingMoney();
            if (!totalAmountResult.IsFail)
                walletAmountData.WaitAmount = waitMoneyResult.Value;
            else
                return null;

            walletAmountData.TotalAmount = walletAmountData.CanUseAmount + walletAmountData.WaitAmount;
            return walletAmountData;
        }
    }

    public class WalletAmountData
    {
        public long CanUseAmount { get; set; }
        public long WaitAmount { get; set; }
        public long TotalAmount { get; set; }
    }

}
