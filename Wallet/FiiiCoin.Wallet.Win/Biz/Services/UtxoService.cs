﻿using FiiiCoin.Business;
using FiiiCoin.DTO;
using FiiiCoin.ServiceAgent;
using FiiiCoin.Utility.Api;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Biz.Services
{
    public class UtxoService : ServiceBase<UtxoService>
    {
        public Result<TxOutSetOM> GetTxOutSetOM()
        {
            var response = UtxoApi.GetTxOutSetInfo().Result;
            return GetResult<TxOutSetOM>(response);
        }

        public Result<long> GetTradingMoney()
        {
            ApiResponse response =  UtxoApi.GetUnconfirmedBalance().Result;
            return base.GetResult<long>(response);
        }
    }
}
