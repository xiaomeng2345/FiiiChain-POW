// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
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
    public class TransactionController : BaseRpcController
    {
        private IMemoryCache _cache;

        public TransactionController(IMemoryCache memoryCache) { _cache = memoryCache; }
        public IRpcMethodResult SendMany(string fromAccount, SendManyOutputIM[] receivers, string[] feeDeductAddresses)
        {
            try
            {
                string result = null;
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var transactionCommentComponent = new TransactionCommentComponent();
                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs();
                var tx = new TransactionMsg();
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;

                if(receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }

                    var output = new OutputMsg();
                    output.Amount = receiver.amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.amount;
                }

                foreach(var address in feeDeductAddresses)
                {
                    if(receivers.Where(r=>r.address == address).Count() == 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.FEE_DEDUCT_ADDRESS_INVALID);
                    }
                }

                var totalInput = 0L;
                var index = 0;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);
                double totalAmount = totalOutput;

                while (index < utxos.Count)
                {
                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            index++;
                            continue;
                        }

                        if(!utxoBlock.IsVerified)
                        {
                            index++;
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            index++;
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                index++;
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(decryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        index++;
                        continue;
                    }

                    if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                    {
                        totalAmount = totalOutput + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = tx.Outputs[0].Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);

                            if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                            {
                                totalAmount = totalOutput + totalFee;
                            }


                            var newAccount = accountComponent.GenerateNewAccount();

                            if (setting.Encrypt)
                            {
                                if (!string.IsNullOrWhiteSpace(_cache.Get<string>("WalletPassphrase")))
                                {
                                    newAccount.PrivateKey = AES128.Encrypt(newAccount.PrivateKey, _cache.Get<string>("WalletPassphrase"));
                                    accountComponent.UpdatePrivateKeyAr(newAccount);
                                }
                                else
                                {
                                    throw new CommonException(ErrorCode.Service.Wallet.WALLET_HAS_BEEN_LOCKED);
                                }
                            }

                            var newOutput = new OutputMsg();
                            newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                            newOutput.Index = tx.Outputs.Count;
                            newOutput.LockScript = Script.BuildLockScipt(newAccount.Id);
                            newOutput.Size = newOutput.LockScript.Length;
                            tx.Outputs.Add(newOutput);
                        }

                        break;
                    }

                    index++;
                }

                var totalAmountLong = Convert.ToInt64(totalAmount);
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (feeDeductAddresses != null && feeDeductAddresses.Length > 0)
                {
                    var averageFee = totalFee / feeDeductAddresses.Length;

                    for(int i = 0; i < receivers.Length; i ++)
                    {
                        if(feeDeductAddresses.Contains(receivers[i].address))
                        {
                            var fee = Convert.ToInt64(averageFee);
                            tx.Outputs[i].Amount -= fee;

                            if (tx.Outputs[i].Amount <= 0)
                            {
                                throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                            }
                        }
                    }
                }

                tx.Timestamp = Time.EpochTime;
                tx.Hash = tx.GetHash();
                txComponent.AddTransactionToPool(tx);
                Startup.P2PBroadcastTransactionAction(tx.Hash);
                result = tx.Hash;

                for(int i = 0; i < receivers.Length; i ++)
                {
                    var receiver = receivers[i];

                    if (!string.IsNullOrWhiteSpace(receiver.tag))
                    {
                        addressBookComponent.SetTag(receiver.address, receiver.tag);
                    }

                    if (!string.IsNullOrWhiteSpace(receiver.comment))
                    {
                        transactionCommentComponent.Add(tx.Hash, i, receiver.comment);
                    }
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

        public IRpcMethodResult SendToAddress(string toAddress, long amount, string comment, string tag, bool deductFeeFromAmount)
        {
            try
            {
                string result = null;
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                if (!AccountIdHelper.AddressVerify(toAddress))
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs();
                var tx = new TransactionMsg();
                var totalSize = tx.Serialize().Length;

                var output = new OutputMsg();
                output.Amount = amount;
                output.Index = 0;
                output.LockScript = Script.BuildLockScipt(toAddress);
                output.Size = output.LockScript.Length;
                tx.Outputs.Add(output);
                totalSize += output.Serialize().Length;

                var totalInput = 0L;
                var index = 0;
                double totalAmount = amount;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);

                while (index < utxos.Count)
                {
                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            index++;
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            index++;
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            index++;
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                index++;
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(decryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        index++;
                        continue;
                    }

                    if(!deductFeeFromAmount)
                    {
                        totalAmount = amount + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = output.Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);

                            if (!deductFeeFromAmount)
                            {
                                totalAmount = amount + totalFee;
                            }

                            var newAccount = accountComponent.GenerateNewAccount();

                            if (setting.Encrypt)
                            {
                                if (!string.IsNullOrWhiteSpace(_cache.Get<string>("WalletPassphrase")))
                                {
                                    newAccount.PrivateKey = AES128.Encrypt(newAccount.PrivateKey, _cache.Get<string>("WalletPassphrase"));
                                    accountComponent.UpdatePrivateKeyAr(newAccount);
                                }
                                else
                                {
                                    throw new CommonException(ErrorCode.Service.Wallet.WALLET_HAS_BEEN_LOCKED);
                                }
                            }

                            var newOutput = new OutputMsg();
                            newOutput.Amount = totalInput - (long)Math.Ceiling(totalAmount);
                            newOutput.Index = 1;
                            newOutput.LockScript = Script.BuildLockScipt(newAccount.Id);
                            newOutput.Size = newOutput.LockScript.Length;
                            tx.Outputs.Add(newOutput);
                        }

                        break;
                    }

                    index++;
                }

                if (totalInput < totalAmount)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if(deductFeeFromAmount)
                {
                    output.Amount -= (long)Math.Ceiling(totalFee);

                    if(output.Amount <= 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                    }
                }

                tx.Timestamp = Time.EpochTime;
                tx.Hash = tx.GetHash();
                txComponent.AddTransactionToPool(tx);
                Startup.P2PBroadcastTransactionAction(tx.Hash);
                result = tx.Hash;

                if (!string.IsNullOrWhiteSpace(tag))
                {
                    addressBookComponent.SetTag(toAddress, tag);
                }

                if (!string.IsNullOrWhiteSpace(comment))
                {
                    transactionCommentComponent.Add(tx.Hash, 0, comment);
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

        public IRpcMethodResult EstimateTxFeeForSendMany(string fromAccount, SendManyOutputIM[] receivers, string[] feeDeductAddresses)
        {
            try
            {
                EstimateTxFeeOM result = new EstimateTxFeeOM();
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();
                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs();
                var tx = new TransactionMsg();
                double totalOutput = 0;
                var totalSize = tx.Serialize().Length;

                if (receivers == null || receivers.Length == 0)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                foreach (var receiver in receivers)
                {
                    if (!AccountIdHelper.AddressVerify(receiver.address))
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                    }

                    var output = new OutputMsg();
                    output.Amount = receiver.amount;
                    output.Index = tx.Outputs.Count;
                    output.LockScript = Script.BuildLockScipt(receiver.address);
                    output.Size = output.LockScript.Length;
                    tx.Outputs.Add(output);

                    totalSize += output.Serialize().Length;
                    totalOutput += receiver.amount;
                }

                foreach (var address in feeDeductAddresses)
                {
                    if (receivers.Where(r => r.address == address).Count() == 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.FEE_DEDUCT_ADDRESS_INVALID);
                    }
                }

                var totalInput = 0L;
                var index = 0;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);
                double totalAmount = totalOutput;

                while (index < utxos.Count)
                {
                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            index++;
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            index++;
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            index++;
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                index++;
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(decryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        index++;
                        continue;
                    }

                    if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                    {
                        totalAmount = totalOutput + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = tx.Outputs[0].Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);

                            if (feeDeductAddresses == null || feeDeductAddresses.Length == 0)
                            {
                                totalAmount = totalOutput + totalFee;
                            }
                        }

                        break;
                    }

                    index++;
                }
                var totalAmountLong = Convert.ToInt64(totalAmount);
                if (totalInput < totalAmountLong)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (feeDeductAddresses != null && feeDeductAddresses.Length > 0)
                {
                    var averageFee = totalFee / feeDeductAddresses.Length;

                    for (int i = 0; i < receivers.Length; i++)
                    {
                        if (feeDeductAddresses.Contains(receivers[i].address))
                        {
                            tx.Outputs[i].Amount -= (long)Math.Ceiling(averageFee);

                            if (tx.Outputs[i].Amount <= 0)
                            {
                                throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                            }
                        }
                    }
                }

                result.totalFee = Convert.ToInt64(totalFee);
                result.totalSize = Convert.ToInt32(totalSize);

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

        public IRpcMethodResult EstimateTxFeeForSendToAddress(string toAddress, long amount, string comment, string commentTo, bool deductFeeFromAmount)
        {
            try
            {
                EstimateTxFeeOM result = new EstimateTxFeeOM();
                var utxoComponent = new UtxoComponent();
                var txComponent = new TransactionComponent();
                var settingComponent = new SettingComponent();
                var addressBookComponent = new AddressBookComponent();
                var accountComponent = new AccountComponent();

                if (!AccountIdHelper.AddressVerify(toAddress))
                {
                    throw new CommonException(ErrorCode.Service.Transaction.TO_ADDRESS_INVALID);
                }

                var setting = settingComponent.GetSetting();
                var utxos = utxoComponent.GetAllConfirmedOutputs();
                var tx = new TransactionMsg();
                var totalSize = tx.Serialize().Length;

                var output = new OutputMsg();
                output.Amount = amount;
                output.Index = 0;
                output.LockScript = Script.BuildLockScipt(toAddress);
                output.Size = output.LockScript.Length;
                tx.Outputs.Add(output);
                totalSize += output.Serialize().Length;

                var blockComponent = new BlockComponent();
                var lastBlockHeight = blockComponent.GetLatestHeight();

                var totalInput = 0L;
                var index = 0;
                double totalAmount = amount;
                double totalFee = setting.FeePerKB * ((double)totalSize / 1024.0);

                while (index < utxos.Count)
                {
                    var account = accountComponent.GetAccountById(utxos[index].AccountId);

                    if (account != null && !string.IsNullOrWhiteSpace(account.PrivateKey))
                    {
                        var utxoTX = txComponent.GetTransactionMsgByHash(utxos[index].TransactionHash);
                        Block utxoBlock = blockComponent.GetBlockEntiytByHash(utxos[index].BlockHash);

                        if (utxoTX == null || utxoBlock == null)
                        {
                            index++;
                            continue;
                        }

                        if (!utxoBlock.IsVerified)
                        {
                            index++;
                            continue;
                        }

                        if (Time.EpochTime < utxoTX.Locktime)
                        {
                            index++;
                            continue;
                        }

                        if (utxoTX.InputCount == 1 && utxoTX.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                        {
                            var blockHeight = utxoBlock.Height;

                            if (lastBlockHeight - blockHeight < 100L)
                            {
                                index++;
                                continue;
                            }
                        }

                        var input = new InputMsg();
                        input.OutputTransactionHash = utxos[index].TransactionHash;
                        input.OutputIndex = utxos[index].OutputIndex;
                        input.UnlockScript = Script.BuildUnlockScript(input.OutputTransactionHash, input.OutputIndex, Base16.Decode(decryptPrivateKey(account.PrivateKey)), Base16.Decode(account.PublicKey));
                        input.Size = input.UnlockScript.Length;
                        tx.Inputs.Add(input);

                        var size = input.Serialize().Length;
                        totalSize += size;
                        totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        totalInput += utxos[index].Amount;
                    }
                    else
                    {
                        index++;
                        continue;
                    }

                    if (!deductFeeFromAmount)
                    {
                        totalAmount = amount + totalFee;
                    }

                    if (totalInput >= (long)Math.Ceiling(totalAmount))
                    {
                        var size = output.Serialize().Length;

                        if ((totalInput - (long)Math.Ceiling(totalAmount)) > (setting.FeePerKB * (double)size / 1024.0))
                        {
                            totalSize += size;
                            totalFee += setting.FeePerKB * ((double)size / 1024.0);
                        }

                        break;
                    }

                    index++;
                }

                if (totalInput < totalAmount)
                {
                    throw new CommonException(ErrorCode.Service.Transaction.BALANCE_NOT_ENOUGH);
                }

                if (deductFeeFromAmount)
                {
                    output.Amount -= (long)Math.Ceiling(totalFee);

                    if (output.Amount <= 0)
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.SEND_AMOUNT_LESS_THAN_FEE);
                    }
                }

                result.totalFee = Convert.ToInt64(totalFee);
                result.totalSize = Convert.ToInt32(totalSize);

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

        public IRpcMethodResult SetTxFee(long feePerKB)
        {
            try
            {
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();

                setting.FeePerKB = feePerKB;
                settingComponent.SaveSetting(setting);

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

        public IRpcMethodResult SetConfirmations(long confirmations)
        {
            try
            {
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();

                setting.Confirmations = confirmations;
                settingComponent.SaveSetting(setting);

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

        public IRpcMethodResult GetTxSettings()
        {
            try
            {
                var settingComponent = new SettingComponent();
                var setting = settingComponent.GetSetting();

                var result = new GetTxSettingsOM();
                result.Confirmations = setting.Confirmations;
                result.FeePerKB = setting.FeePerKB;
                result.Encrypt = setting.Encrypt;

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

        public IRpcMethodResult GeTransaction(string txId)
        {
            try
            {
                var result = new TransactionComponent().GetTransactionEntityByHash(txId);
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

        public IRpcMethodResult ListTransactions(string account, int count, int skip = 0, bool includeWatchOnly = true)
        {
            try
            {
                var txComponent = new TransactionComponent();
                var accountComponent = new AccountComponent();
                var addressBookComponent = new AddressBookComponent();
                var utxoComponent = new UtxoComponent();
                var blockComponent = new BlockComponent();
                var transactionCommentComponent = new TransactionCommentComponent();

                List<PaymentOM> result = new List<PaymentOM>();
                var accounts = accountComponent.GetAllAccounts();
                var paymentAccountIds = accounts.Where(a => !string.IsNullOrWhiteSpace(a.PrivateKey)).Select(a => a.Id).ToList();
                var allAccountIds = accounts.Select(a => a.Id).ToList();
                var addressBook = addressBookComponent.GetWholeAddressBook();
                var latestHeight = blockComponent.GetLatestHeight();
                var data = txComponent.SearchTransactionEntities(account, count, skip, includeWatchOnly);

                foreach(var tx in data)
                {
                    long totalInput = 0;
                    long selfTotalOutput = 0;
                    long otherUserTotalOutput = 0;
                    bool coibase = false;

                    if(tx.Inputs.Count == 1 && tx.Outputs.Count == 1 && tx.Inputs[0].OutputTransactionHash == Base16.Encode(HashHelper.EmptyHash()))
                    {
                        coibase = true;
                    }

                    if(!coibase)
                    {
                        foreach (var input in tx.Inputs)
                        {
                            var oldOutput = txComponent.GetOutputEntiyByIndexAndTxHash(input.OutputTransactionHash, input.OutputIndex);

                            if(oldOutput != null && paymentAccountIds.Contains(oldOutput.ReceiverId))
                            {
                                totalInput += input.Amount;
                            }
                            else
                            {
                                totalInput = 0;
                                break;
                            }
                        }
                    }

                    foreach (var output in tx.Outputs)
                    {
                        if(allAccountIds.Contains(output.ReceiverId))
                        {
                            selfTotalOutput += output.Amount;
                        }
                        else
                        {
                            otherUserTotalOutput += output.Amount;
                        }
                    }

                    BlockMsg block = null;

                    if(tx.BlockHash != null)
                    {
                        block = blockComponent.GetBlockMsgByHash(tx.BlockHash);
                    }

                    if(coibase)
                    {
                        var payment = new PaymentOM();
                        payment.address = tx.Outputs[0].ReceiverId;
                        payment.account = accounts.Where(a => a.Id == payment.address).Select(a => a.Tag).FirstOrDefault();
                        payment.category = "generate";
                        payment.totalInput = totalInput;
                        payment.totalOutput = selfTotalOutput;
                        payment.amount = selfTotalOutput;
                        payment.fee = 0;
                        payment.txId = tx.Hash;
                        payment.vout = 0;
                        payment.time = tx.Timestamp;
                        payment.size = tx.Size;

                        var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, 0);
                        if(txComment != null)
                        {
                            payment.comment = txComment.Comment;
                        }

                        if(block != null)
                        {
                            payment.blockHash = tx.BlockHash;
                            payment.blockIndex = 0;// block.Transactions.FindIndex(t=>t.Hash == tx.Hash);
                            payment.blockTime = block.Header.Timestamp;
                            payment.confirmations = latestHeight - block.Header.Height + 1;
                        }
                        else
                        {
                            payment.confirmations = 0;
                        }

                        result.Add(payment);
                    }
                    else if(totalInput > 0 && otherUserTotalOutput == 0)
                    {
                        var payment = new PaymentOM();
                        payment.address = null;
                        payment.account = null;
                        payment.category = "self";
                        payment.totalInput = totalInput;
                        payment.totalOutput = tx.Outputs[0].Amount;// selfTotalOutput;
                        payment.fee = totalInput - selfTotalOutput;
                        payment.amount = payment.fee;
                        payment.txId = tx.Hash;
                        payment.vout = 0;
                        payment.time = tx.Timestamp;
                        payment.size = tx.Size;

                        var txComments = transactionCommentComponent.GetByTransactionHash(tx.Hash);
                        if (txComments.Count > 0)
                        {
                            payment.comment = "";
                            foreach(var item in txComments)
                            {
                                if(!string.IsNullOrWhiteSpace(item.Comment))
                                {
                                    payment.comment += item.Comment + ";";
                                }
                            }
                        }

                        if (block != null)
                        {
                            payment.blockHash = tx.BlockHash;
                            payment.blockIndex = block.Transactions.FindIndex(t=>t.Hash == tx.Hash);
                            payment.blockTime = block.Header.Timestamp;
                            payment.confirmations = latestHeight - block.Header.Height + 1;
                        }
                        else
                        {
                            payment.confirmations = 0;
                        }

                        result.Add(payment);
                    }
                    else if(totalInput > 0)
                    {
                        for(int i = 0; i < tx.Outputs.Count; i ++)
                        {
                            if (!allAccountIds.Contains(tx.Outputs[i].ReceiverId))
                            {
                                var payment = new PaymentOM();
                                payment.address = tx.Outputs[i].ReceiverId;
                                payment.account = addressBook.Where(a=>a.Address == payment.address && !string.IsNullOrWhiteSpace(a.Tag)).Select(a=>a.Tag).FirstOrDefault();
                                payment.category = "send";
                                payment.totalInput = totalInput;
                                payment.totalOutput = tx.Outputs[i].Amount;
                                payment.fee = totalInput - (selfTotalOutput + otherUserTotalOutput);
                                payment.amount = (i == 0 ? tx.Outputs[i].Amount + payment.fee : tx.Outputs[i].Amount);
                                payment.txId = tx.Hash;
                                payment.vout = i;
                                payment.time = tx.Timestamp;
                                payment.size = tx.Size;

                                var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, i);
                                if (txComment != null)
                                {
                                    payment.comment = txComment.Comment;
                                }

                                if (block != null)
                                {
                                    payment.blockHash = tx.BlockHash;
                                    payment.blockIndex = block.Transactions.FindIndex(t => t.Hash == tx.Hash);
                                    payment.blockTime = block.Header.Timestamp;
                                    payment.confirmations = latestHeight - block.Header.Height + 1;
                                }
                                else
                                {
                                    payment.confirmations = 0;
                                }

                                result.Add(payment);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tx.Outputs.Count; i++)
                        {
                            if (allAccountIds.Contains(tx.Outputs[i].ReceiverId))
                            {
                                var payment = new PaymentOM();
                                payment.address = tx.Outputs[i].ReceiverId;
                                payment.account = accounts.Where(a => a.Id == payment.address).Select(a => a.Tag).FirstOrDefault(); ;
                                payment.category = "receive";
                                payment.totalInput = totalInput;
                                payment.totalOutput = tx.Outputs[i].Amount;
                                payment.fee = totalInput - (selfTotalOutput + otherUserTotalOutput);
                                payment.amount = tx.Outputs[i].Amount;
                                payment.txId = tx.Hash;
                                payment.vout = i;
                                payment.time = tx.Timestamp;
                                payment.size = tx.Size;

                                var txComment = transactionCommentComponent.GetByTransactionHashAndIndex(tx.Hash, i);
                                if (txComment != null)
                                {
                                    payment.comment = txComment.Comment;
                                }

                                if (block != null)
                                {
                                    payment.blockHash = tx.BlockHash;
                                    payment.blockIndex = block.Transactions.FindIndex(t => t.Hash == tx.Hash);
                                    payment.blockTime = block.Header.Timestamp;
                                    payment.confirmations = latestHeight - block.Header.Height + 1;
                                }
                                else
                                {
                                    payment.confirmations = 0;
                                }

                                result.Add(payment);
                            }
                        }
                    }
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

        private string decryptPrivateKey(string privateKey)
        {
            var setting = new SettingComponent().GetSetting();

            if(setting.Encrypt)
            {
                if(!string.IsNullOrWhiteSpace(_cache.Get<string>("WalletPassphrase")))
                {
                    try
                    {
                        return AES128.Decrypt(privateKey, _cache.Get<string>("WalletPassphrase"));
                    }
                    catch
                    {
                        throw new CommonException(ErrorCode.Service.Transaction.WALLET_DECRYPT_FAIL);
                    }
                }
                else
                {
                    throw new CommonException(ErrorCode.Service.Transaction.WALLET_DECRYPT_FAIL);
                }
            }
            else
            {
                return privateKey;
            }
        }
    }
}
