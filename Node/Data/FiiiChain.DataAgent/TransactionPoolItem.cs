using FiiiChain.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace FiiiChain.DataAgent
{
    [Serializable]
    public class TransactionPoolItem
    {
        public TransactionPoolItem(long feeRate, TransactionMsg transaction)
        {
            this.FeeRate = feeRate;
            this.Transaction = transaction;
        }

        public TransactionMsg Transaction { get; set; }
        public long FeeRate { get; set; }
        public bool Isolate { get; set; }
    }
}
