﻿using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FiiiCoin.Wallet.Win.ViewModels.ShellPages
{
    public class InputWalletPwdViewModel : PopupShellBase
    {
        protected override string GetPageName()
        {
            return Pages.InputWalletPwdPage;
        }

        public InputWalletPwdViewModel()
        {
            RegeistMessenger<SendMsgData<InputWalletPwdPageTopic>>(OnGetResponse);
            IsEnableOk = true;
        }
        
        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value;
                if (string.IsNullOrEmpty(_password))
                    IsEnableOk = false;
                else
                    IsEnableOk = true;
                RaisePropertyChanged("Password"); }
        }

        private bool _isEnableOk;

        public bool IsEnableOk
        {
            get { return _isEnableOk; }
            set { _isEnableOk = value; RaisePropertyChanged("IsEnableOk"); }
        }


        protected override void Refresh()
        {
            base.Refresh();
            Password = string.Empty;
        }

        public override void OnOkClick()
        {
            switch (_msgData.Token)
            {
                case InputWalletPwdPageTopic.Normal:
                    _msgData.CallBack();
                    break;
                case InputWalletPwdPageTopic.UnLockWallet:
                    var result = UnLockWallet();
                    if (!result)
                        return;
                    _msgData.CallBack();
                    break;
                case InputWalletPwdPageTopic.RequestPassword:
                    var unlockResult = UnLockWallet();
                    if (!unlockResult)
                    {
                        ShowMessage(LanguageService.Default.GetLanguageValue("enterPwd_fail"));
                        return;
                    }
                    _msgData.CallBackParams = Password;
                    _msgData.CallBack();
                    break;
                default:
                    break;
            }
        }

        public override void OnClosePopup()
        {
            base.OnClosePopup();
            Password = string.Empty;
        }


        bool UnLockWallet()
        {
            var result =  WalletService.Default.UnLockWallet(Password);
            if (result.IsFail || !result.Value)
            {
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.EnterPwdFail));
                return false;
            }
            return true;
        }

        private SendMsgData<InputWalletPwdPageTopic> _msgData;
        void OnGetResponse(SendMsgData<InputWalletPwdPageTopic> msgData)
        {
            this.Password = string.Empty;
            _msgData = msgData;
        }
    }
}
