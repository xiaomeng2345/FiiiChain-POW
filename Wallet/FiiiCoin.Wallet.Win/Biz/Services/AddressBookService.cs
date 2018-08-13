using FiiiCoin.Business;
using FiiiCoin.Models;
using FiiiCoin.Utility;
using FiiiCoin.Utility.Api;
using FiiiCoin.Wallet.Win.Common;
using FiiiCoin.Wallet.Win.Models;
using System.Collections.Generic;

namespace FiiiCoin.Wallet.Win.Biz.Services
{
    public class AddressBookService : ServiceBase<AddressBookService>
    {
        public Result<bool> AddNewAddressBookItem(string account, string tag)
        {
            var status = GetBlockChainStatus();
            if (status.IsFail)
                return new Result<bool> { IsFail = true };

            var check = VerfyAddress(status.Value.ChainNetwork, account);
            if (!check)
            {
                return new Result<bool>() { IsFail = true ,ErrorCode= 70000001 };
            }
            ApiResponse response = AddressBookApi.AddNewAddressBookItem(account, tag).Result;
            return GetResult<bool>(response);
        }

        public  Result<List<AddressBookInfo>> GetAddressBook()
        {
            ApiResponse response = AddressBookApi.GetAddressBook().Result;
            return GetResult<List<AddressBookInfo>>(response);
        }


        public Result<AddressBookInfo> GetAddressBookItemByAddress(string address)
        {
            ApiResponse response = AddressBookApi.GetAddressBookItemByAddress(address).Result;
            return GetResult<AddressBookInfo>(response);
        }


        public Result<List<AddressBookInfo>> GetAddressBookByTag(string tag)
        {
            ApiResponse response = AddressBookApi.GetAddressBookByTag(tag).Result;
            return GetResult<List<AddressBookInfo>>(response);
        }

        public Result<bool> DeleteAddressBookByIds(params long[] ids)
        {
            ApiResponse response = AddressBookApi.DeleteAddressBookByIds(ids).Result;
            return GetResult<bool>(response);
        }


        public bool VerfyAddress(string netType,string account)
        {
            bool result = AddressTools.AddressVerfy(netType, account);
            return result;
        }



        private BlockChainStatus chainStatus = null;
        public Result<BlockChainStatus> GetBlockChainStatus()
        {
            if (chainStatus == null)
            {
                ApiResponse apiResponse = BlockChainEngineApi.GetBlockChainStatus().Result;
                return GetResult<BlockChainStatus>(apiResponse);
            }
            else
                return new Result<BlockChainStatus>() { Value = chainStatus, IsFail = false };
        }
    }
}