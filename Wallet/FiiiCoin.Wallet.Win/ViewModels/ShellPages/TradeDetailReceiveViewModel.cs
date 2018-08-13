using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class TradeDetailReceiveViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.TradeDetailReceivePage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<TradeRecordInfo>(GetRequest);
        }

        void GetRequest(TradeRecordInfo tradeRecordInfo)
        {
            TradeRecordInfo = tradeRecordInfo;
        }


        private TradeRecordInfo _tradeRecordInfo;

        public TradeRecordInfo TradeRecordInfo
        {
            get { return _tradeRecordInfo; }
            set
            {
                _tradeRecordInfo = value;
                RaisePropertyChanged("TradeRecordInfo");
            }
        }
    }
}
