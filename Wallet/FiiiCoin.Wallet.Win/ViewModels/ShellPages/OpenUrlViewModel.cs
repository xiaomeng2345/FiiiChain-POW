using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using FiiiCoin.Wallet.Win.ValidationRules;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class OpenUrlViewModel : PopupShellBase
    {
        public OpenUrlViewModel()
        {

        }

        private string _url;
        private bool _isUrlError = false;

        public string Url
        {
            get { return _url; }
            set { _url = value; RaisePropertyChanged("Url"); }
        }

        public bool IsUrlError
        {
            get { return _isUrlError; }
            set { _isUrlError = value; RaisePropertyChanged("IsUrlError"); }
        }

        protected override string GetPageName()
        {
            return Pages.OpenUrlPage;
        }

        public override void OnOkClick()
        {
            if (string.IsNullOrEmpty(Url))
                return;

            var @params = Url.Replace("fiiicoin:", "").Replace("amount=", "").Replace("label=", "").Replace("message=", "").Replace("?", "&").Split('&');

            if (@params.Length != 4)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Error_Uri));
                return;
            }
            long longAmount;
            if (!long.TryParse(@params[1], out longAmount))
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(ValidationType.Error_Amount.ToString()));
                return;
            }

            SendMsgData<SendItemInfo> data = new SendMsgData<SendItemInfo>();
            data.Token = new SendItemInfo();
            data.Token.Address = @params[0];
            data.Token.Tag = @params[2];
            data.Token.PayAmountStr = (longAmount / Math.Pow(10, 8)).ToString("0.00000000");
            SendMessenger(Pages.SendPage, data);
            UpdatePage(Pages.SendPage, PageModel.TabPage);
            base.OnOkClick();
            Url = string.Empty;
        }
    }
}
