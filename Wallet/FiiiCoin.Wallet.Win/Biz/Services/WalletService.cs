﻿using FiiiCoin.Business;
using FiiiCoin.Utility.Api;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using System.Threading.Tasks;

namespace FiiiCoin.Wallet.Win.Biz.Services
{
    public class WalletService : ServiceBase<WalletService>
    {
        public Result LockWallet()
        {
            ApiResponse response =  WalletManagementApi.WalletLock().Result;
            var result = GetResult(response);

            return result;
        }

        public Result<bool> UnLockWallet(string passphrase, long seconds = 50000)
        {
            ApiResponse response =  WalletManagementApi.WalletPassphrase(passphrase).Result;
            var result = GetResult<bool>(response);

            return result;
        }

        public Result ChangeWalletPassword(string oldPwd, string newPwd)
        {
            ApiResponse response =  WalletManagementApi.WalletPassphraseChange(oldPwd, newPwd).Result;
            var result = GetResult(response);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">*.dat</param>
        /// <returns></returns>
        public Result ExportBackupWallet(string path)
        {
            ApiResponse response =  WalletManagementApi.BackupWallet(path).Result;
            var result = GetResult(response);

            return result;
        }

        public Result ImportBackupWallet(string path, string password = null)
        {
            ApiResponse response =  WalletManagementApi.RestoreWalletBackup(path, password).Result;
            var result = GetResult(response);

            return result;
        }

        public Result EncryptWallet(string pwd)
        {
            ApiResponse response =  WalletManagementApi.EncryptWallet(pwd).Result;
            var result = GetResult(response);
            return result;
        }
    }
}