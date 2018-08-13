// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FiiiChain.Messages
{
    public class AddrMsg : BasePayload
    {
        public int Count
        {
            get { return this.AddressList.Count; }
        }
        public List<KeyValuePair<string, int>> AddressList { get; set; }

        public AddrMsg()
        {
            this.AddressList = new List<KeyValuePair<string, int>>();
        }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            var buffer = new byte[bytes.Length - index];
            Array.Copy(bytes, 0, buffer, 0, buffer.Length);

            var textData = Encoding.UTF8.GetString(buffer);
            var addressArray = textData.Split(";");

            foreach(var address in addressArray)
            {
                var ipPort = address.Split(",");

                if(ipPort.Length == 2)
                {
                    int port;
                    
                    if(int.TryParse(ipPort[1], out port))
                    {
                        this.AddressList.Add(new KeyValuePair<string, int>(ipPort[0], int.Parse(ipPort[1])));
                    }
                }
            }
        }

        public override byte[] Serialize()
        {
            var data = new List<byte>();

            for (int i = 0; i < this.AddressList.Count; i++)
            {
                var item = this.AddressList[i];
                data.AddRange(Encoding.UTF8.GetBytes(item.Key + "," + item.Value));

                if(i < this.AddressList.Count - 1)
                {
                    data.Add(Convert.ToByte(';'));
                }
            }

            return data.ToArray();
        }
    }
}
