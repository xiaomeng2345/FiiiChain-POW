using EdjCase.JsonRpc.Router;
using EdjCase.JsonRpc.Router.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiiiChain.DTO;
using FiiiChain.Framework;
using FiiiChain.Business;
using FiiiChain.Messages;
using FiiiChain.Consensus;
using FiiiChain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace FiiiChain.Wallet.API
{
    public class WalletController : BaseRpcController
    {
        private IMemoryCache _cache;

        public WalletController(IMemoryCache memoryCache) { _cache = memoryCache; }

        public IRpcMethodResult BackupWallet(string targetAddress)
        {
            try
            {
                WalletComponent component = new WalletComponent();
                component.BackupWallet(targetAddress, _cache.Get<string>("WalletPassphrase"));
                return Ok();
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult RestoreWalletBackup(string backupFilePaths, string passphrase = null)
        {
            try
            {
                var result = false;
                WalletComponent component = new WalletComponent();
                result = component.RestoreWalletBackup(backupFilePaths, passphrase);
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult ImportPrivKey(string privateKey)
        {
            try
            {
                var result = false;

                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult DumpPrivKey(string address)
        {
            try
            {
                var result = false;

                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult EncryptWallet(string passphrase)
        {
            try
            {
                var result = false;
                var settting = new SettingComponent().GetSetting();

                if(settting.Encrypt)
                {
                    throw new CommonException(ErrorCode.Service.Wallet.CAN_NOT_ENCRYPT_AN_ENCRYPTED_WALLET);
                }

                result = new WalletComponent().EncryptWallet(passphrase);
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult WalletLock()
        {
            try
            {
                var settting = new SettingComponent().GetSetting();

                if (!settting.Encrypt)
                {
                    throw new CommonException(ErrorCode.Service.Wallet.CAN_NOT_ENCRYPT_AN_ENCRYPTED_WALLET);
                }

                _cache.Remove("WalletPassphrase");
                return Ok();
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult WalletPassphrase(string passphrase)
        {
            try
            {
                var settting = new SettingComponent().GetSetting();

                if (!settting.Encrypt)
                {
                    throw new CommonException(ErrorCode.Service.Wallet.CAN_NOT_ENCRYPT_AN_ENCRYPTED_WALLET);
                }

                var result = false;
                if(new WalletComponent().CheckPassword(passphrase))
                {
                    _cache.Set<string>("WalletPassphrase", passphrase,new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = new TimeSpan(0,5,0)
                    });

                    result = true;
                }
                return Ok(result);
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }

        public IRpcMethodResult WalletPassphraseChange(string currentPassphrase, string newPassphrase)
        {
            try
            {
                var settting = new SettingComponent().GetSetting();

                if (!settting.Encrypt)
                {
                    throw new CommonException(ErrorCode.Service.Wallet.CAN_NOT_CHANGE_PASSWORD_IN_AN_UNENCRYPTED_WALLET);
                }

                WalletComponent wc = new WalletComponent();
                wc.ChangePassword(currentPassphrase, newPassphrase);
                _cache.Remove("WalletPassphrase");
                return Ok();
            }
            catch (CommonException ce)
            {
                return Error(ce.ErrorCode, ce.Message, ce);
            }
            catch (Exception ex)
            {
                return Error(ErrorCode.UNKNOWN_ERROR, ex.Message, ex);
            }
        }
    }
}
