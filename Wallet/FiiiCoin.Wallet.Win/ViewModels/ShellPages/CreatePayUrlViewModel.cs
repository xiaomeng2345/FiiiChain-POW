using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models.UiModels;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class CreatePayUrlViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.CreatePayUrlPage;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            RegeistMessenger<UrlInfo>(OnGetRequest);
        }

        public override void OnClosePopup()
        {
            switch (UrlInfo.Mode)
            {
                case UrlMode.CreatePay:
                case UrlMode.Edit:
                    UpdatePage(Pages.PayUrlsPage);
                    break;
                case UrlMode.CreateByReceive:
                case UrlMode.EditByReceive:
                    UpdatePage(Pages.ReceiveAddressPage);
                    break;
                default:
                    break;
            }
        }

        bool isRunning = false;

        public override void OnOkClick()
        {
            if (isRunning)
                return;
            isRunning = true;
            switch (UrlInfo.Mode)
            {
                case UrlMode.CreatePay:
                case UrlMode.Edit:
                    SendMessenger(Pages.PayUrlsPage, UrlInfo);
                    break;
                case UrlMode.CreateByReceive:
                    var adddressInfo = AccountsService.Default.GetNewAddress(UrlInfo.Tag);
                    UrlInfo.Address = adddressInfo.Value.Address;
                    SendMessenger(Pages.ReceiveAddressPage, UrlInfo);
                    break;
                case UrlMode.EditByReceive:
                    SendMessenger(Pages.ReceiveAddressPage, UrlInfo);
                    break;
                default:
                    break;
            }
            base.OnOkClick();
            isRunning = false;
        }

        void OnGetRequest(UrlInfo info)
        {
            if (info != null)
            {
                UrlInfo = new UrlInfo() { Address = info.Address, Tag = info.Tag, Mode = info.Mode, Id = info.Id, Timestamp = info.Timestamp };
            }
            if (UrlInfo.Mode == UrlMode.EditByReceive || UrlInfo.Mode == UrlMode.CreateByReceive)
                IsEditAddress = false;
            else
                IsEditAddress = true;
        }

        private UrlInfo _urlInfo;

        public UrlInfo UrlInfo
        {
            get { return _urlInfo; }
            set
            {
                _urlInfo = value;
                RaisePropertyChanged("UrlInfo");
            }
        }

        private bool _isEditAddress;

        public bool IsEditAddress
        {
            get { return _isEditAddress; }
            set { _isEditAddress = value; RaisePropertyChanged("IsEditAddress"); }
        }
    }
}