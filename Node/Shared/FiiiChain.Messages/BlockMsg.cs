// Copyright (c) 2018 FiiiLab Technology Ltd
// Distributed under the MIT software license, see the accompanying
// file LICENSE or or http://www.opensource.org/licenses/mit-license.php.
using FiiiChain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.Messages
{
    public class BlockMsg : BasePayload
    {
        public BlockMsg()
        {
            this.Transactions = new List<TransactionMsg>();
        }

        public BlockHeaderMsg Header { get; set; }
        public List<TransactionMsg> Transactions { get; set; }

        public override void Deserialize(byte[] bytes, ref int index)
        {
            this.Header = new BlockHeaderMsg();
            this.Header.Deserialize(bytes, ref index);

            var txIndex = 0;
            while (txIndex < this.Header.TotalTransaction)
            {
                var transactionMsg = new TransactionMsg();
                transactionMsg.Deserialize(bytes, ref index);
                this.Transactions.Add(transactionMsg);

                txIndex++;
            }
        }

        public override byte[] Serialize()
        {
            var bytes = new List<byte>();
            bytes.AddRange(Header.Serialize());

            foreach (var tx in Transactions)
            {
                bytes.AddRange(tx.Serialize());
            }

            return bytes.ToArray();
        }
    }
}
