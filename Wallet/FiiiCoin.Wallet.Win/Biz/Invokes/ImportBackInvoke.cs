using FiiiCoin.Wallet.Win.Biz.Services;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Common.interfaces;
using FiiiCoin.Wallet.Win.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Biz.Invokes
{
    [Export(typeof(IInvoke))]
    public class ImportBackInvoke : VmBase, IInvoke
    {
        public string GetInvokeName()
        {
            return InvokeKeys.Restore;
        }

        public void Invoke<T>(T obj)
        {
            StartImport();
        }

        public FileInfo GetFile()
        {
            FileInfo fileResult = null;
            var settingResult = FiiiCoinService.Default.GetTxSettings();
            if (settingResult.IsFail)
            {
                ShowMessage(settingResult.GetErrorMsg());
                return fileResult;
            }
            
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = settingResult.Value.Encrypt? "BackUp Data|*.fcdatx": "BackUp Data| *.fcdat";
            fileDialog.RestoreDirectory = true;
            var result = fileDialog.ShowDialog(BootStrapService.Default.Shell.GetWindow());
            if (result.HasValue && result.Value)
            {
                var file = fileDialog.FileName;
                fileResult = new FileInfo(file);
            }
            return fileResult;
        }

        void StartImport()
        {
            FileInfo fileInfo = GetFile();
            if (fileInfo == null)
                return;
            if (fileInfo.Extension == ".fcdatx")
            {
                ImportWithPassword(fileInfo.FullName);
            }
            else
            {
                Import(fileInfo.FullName);
            }
        }

        void ImportWithPassword(string filePath)
        {
            SendMsgData<InputWalletPwdPageTopic> data = new SendMsgData<InputWalletPwdPageTopic>();
            data.Token = InputWalletPwdPageTopic.RequestPassword;
            data.SetCallBack(() =>
            {
                var password = "";
                if (data.CallBackParams != null)
                {
                    password = data.CallBackParams.ToString();
                    Import(filePath, password);
                    OnClosePopup();
                }
                
            });
            SendMessenger(Pages.InputWalletPwdPage, SendMessageTopic.Refresh);
            SendMessenger(Pages.InputWalletPwdPage, data);
            UpdatePage(Pages.InputWalletPwdPage);
        }

        void Import(string file, string password = null)
        {
            var exportResult = WalletService.Default.ImportBackupWallet(file,password);
            if (!exportResult.IsFail)
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Import_sucesses));
            else
                ShowMessage(LanguageService.Default.GetLanguageValue(MessageKeys.Import_Fail));
        }
    }
}
