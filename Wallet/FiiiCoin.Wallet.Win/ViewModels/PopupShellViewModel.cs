using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FiiiCoin.Wallet.Win.ViewModels
{
    public class PopupShellViewModel : VmBase
    {
        public PopupShellViewModel()
        {
            Messenger.Default.Register<string>(this, MessageTopic.UpdatePopupView, UpdateView);
            Messenger.Default.Register<string>(this, MessageTopic.ClosePopUpWindow, OnClosePopupView);
        }

        private Page _popupShellView;

        public Page PopupShellView
        {
            get { return _popupShellView; }
            set
            {
                _popupShellView = value;
                RaisePropertyChanged("PopupShellView");
            }
        }


        public void UpdateView(string newPageName)
        {
            var page = BootStrapService.Default.GetPage(newPageName);
            if (page != null && page != _popupShellView)
                PopupShellView = page;
        }

        void OnClosePopupView(string pageName)
        {
            if (PopupShellView != null && PopupShellView.ToString().Contains(pageName))
            {
                Messenger.Default.Send(false, MessageTopic.ChangedPopupViewState);
            }
        }

        protected override string GetPageName()
        {
            return Pages.PopupShell;
        }
    }
}
