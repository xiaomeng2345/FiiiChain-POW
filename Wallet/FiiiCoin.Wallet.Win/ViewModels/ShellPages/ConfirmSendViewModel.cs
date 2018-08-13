using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using FiiiCoin.Wallet.Win.Models.UiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class ConfirmSendViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.ConfirmSendPage;
        }
        
        public override void OnOkClick()
        {
            msgData.CallBack();
            base.OnOkClick();
        }

        private ConfirmSendData confirmSendData;

        public ConfirmSendData ConfirmSendData
        {
            get { return confirmSendData; }
            set { confirmSendData = value; RaisePropertyChanged("ConfirmSendData"); }
        }


        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<SendMsgData<ConfirmSendData>>(OnGetResponse);
        }
        private SendMsgData<ConfirmSendData> msgData;
        void OnGetResponse(SendMsgData<ConfirmSendData> data)
        {
            msgData = data;
            ConfirmSendData = data.Token;
        }
    }
}