// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiiiChain.Consensus
{
    public class AccountIdHelper
    {
        private const int testnetPrefix = 0x4ABB4165;
        private const int mainnetPrefix = 0x4ABB3300;
        private const int prefixLen = 4;

        public static string CreateAccountAddress(byte[] publicKey)
        {
            var pubkHash = HashHelper.Hash160(publicKey);

            return CreateAccountAddressByPublicKeyHash(pubkHash);
        }

        public static string CreateAccountAddressByPublicKeyHash(byte[] pubkHash)
        {
            //byte[] prefix = new byte[] { 0x00 };   //1
            //byte[] prefix = new byte[] { 0x40, 0xE7, 0xE9, 0x26 };   //fiiit
            byte[] fullPrefix;

            if(GlobalParameters.IsTestnet)
            {
                fullPrefix = BitConverter.GetBytes(testnetPrefix);
            }
            else
            {
                fullPrefix = BitConverter.GetBytes(mainnetPrefix);
            }

            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(fullPrefix);
            }

            var payload = new List<byte>();
            payload.AddRange(fullPrefix);
            payload.AddRange(pubkHash);

            var checksum = HashHelper.DoubleHash(payload.ToArray()).Take(4);
            payload.AddRange(checksum);

            return Base58.Encode(payload.ToArray());


        }

        public static byte[] GetPublicKeyHash(string accountAddress)
        {
            var bytes = Base58.Decode(accountAddress);
            var publicKeyHash = new byte[bytes.Length - (prefixLen + 4)];
            Array.Copy(bytes, prefixLen, publicKeyHash, 0, publicKeyHash.Length);

            return publicKeyHash;
        }

        public static bool AddressVerify(string accountAddress)
        {
            var bytes = Base58.Decode(accountAddress);
            //var prefix = bytes[prefixLen];
            var checksum = new byte[4];
            var data = new byte[bytes.Length - 4];
            Array.Copy(bytes, 0, data, 0, bytes.Length - 4);
            Array.Copy(bytes, bytes.Length - checksum.Length, checksum, 0, checksum.Length);

            var newChecksum = HashHelper.DoubleHash(data).Take(4);
            return BitConverter.ToInt32(checksum, 0) == BitConverter.ToInt32(newChecksum.ToArray(), 0);
        }
    }
}
